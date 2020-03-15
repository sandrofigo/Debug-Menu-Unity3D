//
// Copyright (c) Sandro Figo
//

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugMenuManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private RectTransform consolePanel;

        [SerializeField]
        private RectTransform buttonMenu;

        [SerializeField]
        private GameObject menuButtonPrefab;

        [SerializeField]
        private InputField inputField;

        [SerializeField]
        private Text suggestionsText;
#pragma warning restore 0649

        private static Text outputText;

        private List<Node> nodes = new List<Node>();

        private MethodData[] methodData;

        private string lastMethod;

        public static DebugMenuManager Instance { get; set; }

        public Node lastInvokedNode;

        public bool Visible { get; set; }

        public delegate void VisibilityHandler(bool visible);

        public event VisibilityHandler OnVisibilityChanged;

        private void VisibilityChanged()
        {
            OnVisibilityChanged?.Invoke(Visible);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            outputText = transform.Find("Console Panel/Output Text").GetComponent<Text>();

            consolePanel.gameObject.SetActive(false);
        }

        private void Start()
        {
            var eventSystem = FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                Debug.LogError("No UI EventSystem is present in the current scene!");
                return;
            }

            // Initialization
            inputField.Select();
            inputField.ActivateInputField();

            ClearSuggestions();
            ClearOutput();

            inputField.onValueChanged.AddListener(delegate { OnInputFieldChanged(); });

            methodData = GetMethodData();

            // Construct node tree
            ConstructNodeTree();

            // Add menu buttons

            nodes.Sort((node1, node2) => string.Compare(node1.name, node2.name, StringComparison.Ordinal));

            foreach (var node in nodes)
            {
                GameObject menuButton = Instantiate(menuButtonPrefab, buttonMenu, true);
                menuButton.GetComponentInChildren<Text>().text = node.name;

                DebugMenuButton m = menuButton.GetComponent<DebugMenuButton>();
                m.isRoot = true;
                m.node = node;

                ButtonMenu.Instance.buttons.Add(m);
            }

            // Prevent layout glitch
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonMenu);
            buttonMenu.gameObject.SetActive(false);
        }

        private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.F3)) && (Debug.isDebugBuild || Application.isEditor))
            {
                Visible = !Visible;

                consolePanel.gameObject.SetActive(!consolePanel.gameObject.activeInHierarchy);

                buttonMenu.gameObject.SetActive(!buttonMenu.gameObject.activeInHierarchy);

                if (Visible)
                {
                    inputField.Select();
                    inputField.ActivateInputField();
                }
                else
                {
                    ButtonMenu.Instance.DestroyAllOpenPanels();
                    ButtonMenu.Instance.ResetAllMenuButtons();
                }

                VisibilityChanged();
            }

            if (Input.GetKeyDown(KeyCode.F4) && lastInvokedNode != null)
            {
                if (lastInvokedNode.method.GetParameters().Length == 0)
                {
                    object obj = lastInvokedNode.method.Invoke(lastInvokedNode.monoBehaviour, null);
                }
                else
                {
                    Debug.LogWarning("Re-invoking a method with parameters is not supported yet.");
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Visible)
                {
                    string input = inputField.text;

                    Suggestion[] suggestions = GetSuggestions(input);

                    if (suggestions.Length > 0)
                    {
                        inputField.text = suggestions[0].path;
                        inputField.MoveTextEnd(false);
                    }

                    if (!inputField.isFocused)
                    {
                        inputField.Select();
                        inputField.ActivateInputField();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                inputField.text = lastMethod;
                inputField.caretPosition = inputField.text.Length;
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                lastMethod = inputField.text;

                string[] split = inputField.text.Split(' ');

                Node node = FindNode(split[0]);

                if (node?.method != null)
                {
                    // Valid method

                    List<object> parameters = new List<object>();

                    var parameterTypes = node.method.GetParameters();

                    for (int i = 1; i < split.Length; i++)
                    {
                        try
                        {
                            var x = Convert.ChangeType(split[i], parameterTypes[i - 1].ParameterType);
                            parameters.Add(x);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }

                    Log(inputField.text);
                    lastInvokedNode = node;
                    object obj = node.method.Invoke(node.monoBehaviour, parameters.ToArray());
                    if (obj == null)
                        Log("Return value: null\n");
                    else
                        Log($"Return value: {obj}\n");
                }
                else
                {
                    Log($"'{inputField.text}' is not a valid method!\n");
                }

                inputField.text = string.Empty;
                inputField.ActivateInputField();
            }
        }

        private void OnInputFieldChanged()
        {
            string input = inputField.text;

            Suggestion[] suggestions = GetSuggestions(input);
            suggestionsText.text = string.Empty;

            string s = string.Empty;
            foreach (Suggestion suggestion in suggestions)
            {
                s += $"{suggestion.path}{(suggestion.node.children.Count > 0 ? "..." : string.Empty)} {suggestion.typeText}\n";
            }

            suggestionsText.text = s;
        }

        private Suggestion[] GetSuggestions(string path)
        {
            List<Suggestion> suggestions = new List<Suggestion>();

            if (path == string.Empty) return suggestions.ToArray();

            string[] split = path.Split('.');

            int lastSeparatorIndex = path.LastIndexOf('.');

            if (lastSeparatorIndex < 0) lastSeparatorIndex = 0;

            string parentPath = path.Substring(0, lastSeparatorIndex);

            Node lastCompleteNode = FindNode(parentPath);

            List<Node> currentNodes = new List<Node>();

            if (lastCompleteNode == null)
            {
                // Only use the whole node tree if there is no separator ('.') present
                if (!path.Contains("."))
                    currentNodes = nodes;
            }
            else
            {
                currentNodes = lastCompleteNode.children;
            }

            foreach (var node in currentNodes)
            {
                if (node.name.StartsWith(split[split.Length - 1], StringComparison.OrdinalIgnoreCase))
                {
                    string suggestion = parentPath + (parentPath == string.Empty ? string.Empty : ".") + node.name;
                    string typeText = string.Empty;

                    if (node.method != null)
                    {
                        var parameters = node.method.GetParameters();

                        if (parameters.Length > 0)
                        {
                            typeText = "(";

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                typeText += TypeAliases[parameters[i].ParameterType];
                                if (i < parameters.Length - 1)
                                    typeText += ", ";
                            }

                            typeText += ")";
                        }
                    }

                    suggestions.Add(new Suggestion(suggestion, typeText, node));
                }
            }

            return suggestions.ToArray();
        }

        public static void Log(string s)
        {
            outputText.text += s + "\n";
        }

        private void ClearSuggestions()
        {
            suggestionsText.text = string.Empty;
        }

        private void ClearOutput()
        {
            outputText.text = string.Empty;
        }

        /// <summary>
        /// Returns a node with the given path (node names separated with '.')
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Node FindNode(string path)
        {
            string[] split = path.Split('.');

            List<Node> currentNodes = nodes;

            for (int splitIndex = 0; splitIndex < split.Length; splitIndex++)
            {
                foreach (var node in currentNodes)
                {
                    if (node.name == split[splitIndex])
                    {
                        // Found a matching node

                        if (splitIndex == split.Length - 1) return node;

                        if (node.children.Count > 0)
                        {
                            currentNodes = node.children;
                        }
                        else
                        {
                            return node;
                        }
                    }
                }
            }

            return null;
        }

        private Node GetNode(List<Node> nodeCollection, string nodeName)
        {
            foreach (Node node in nodeCollection)
            {
                if (node.name == nodeName)
                    return node;
            }

            return null;
        }

        private bool NodeExists(string nodeName)
        {
            bool exists = false;
            foreach (Node node in nodes)
            {
                exists = node.name == nodeName;
            }

            return exists;
        }

        private void ConstructNodeTree()
        {
            foreach (MethodData data in methodData)
            {
                foreach (MethodInfo info in data.methods)
                {
                    DebugMethod method = GetDebugMethod(info);

                    if (method == null)
                        continue;

                    // Custom path
                    if (method.customPath != string.Empty)
                    {
                        string[] split = method.customPath.Split('/');

                        List<Node> currentNodeList = nodes;

                        for (int splitIndex = 0; splitIndex < split.Length; splitIndex++)
                        {
                            Node n = GetNode(currentNodeList, split[splitIndex]);

                            if (n == null)
                            {
                                n = new Node
                                {
                                    name = split[splitIndex]
                                };
                                currentNodeList.Add(n);
                            }

                            currentNodeList = n.children;

                            if (splitIndex == split.Length - 1)
                            {
                                Node finalNode = new Node
                                {
                                    name = info.Name,
                                    method = info,
                                    monoBehaviour = data.monoBehaviour
                                };
                                n.children.Add(finalNode);
                            }
                        }
                    }
                    else // Type path
                    {
                        Node childNode = new Node
                        {
                            name = info.Name,
                            method = info,
                            monoBehaviour = data.monoBehaviour
                        };

                        string baseNodeName = info.DeclaringType.ToString();

                        Node baseNode = GetNode(nodes, baseNodeName);

                        if (baseNode != null)
                        {
                            // Base node exists
                            baseNode.children.Add(childNode);
                        }
                        else
                        {
                            // Base node doesn't exist
                            baseNode = new Node
                            {
                                name = info.DeclaringType.ToString()
                            };
                            baseNode.children.Add(childNode);

                            nodes.Add(baseNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of <see cref="MethodData"/> from active <see cref="MonoBehaviour"/>s in the scene
        /// </summary>
        /// <returns></returns>
        private MethodData[] GetMethodData()
        {
            MonoBehaviour[] active = FindObjectsOfType<MonoBehaviour>();

            List<MethodData> methods = new List<MethodData>();

            List<Type> usedTypes = new List<Type>();

            foreach (MonoBehaviour mono in active)
            {
                MethodData data = new MethodData();
                data.methods.AddRange(mono.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public));
                data.monoBehaviour = mono;

                if (data.methods.Count > 0 && !usedTypes.Contains(mono.GetType()))
                {
                    methods.Add(data);
                    usedTypes.Add(mono.GetType());
                }
            }

            return methods.ToArray();
        }

        /// <summary>
        /// Returns a <see cref="DebugMethod"/>, if there is no attribute assigned it will return null
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private DebugMethod GetDebugMethod(MethodInfo method)
        {
            return Attribute.GetCustomAttribute(method, typeof(DebugMethod)) as DebugMethod;
        }

        /// <summary>
        /// Returns an array of <see cref="DebugMethod"/>s
        /// </summary>
        /// <param name="methods"></param>
        /// <returns></returns>
        private DebugMethod[] GetDebugMethods(MethodInfo[] methods)
        {
            List<DebugMethod> methodList = new List<DebugMethod>();

            foreach (MethodInfo methodInfo in methods)
            {
                DebugMethod m = Attribute.GetCustomAttribute(methodInfo, typeof(DebugMethod)) as DebugMethod;

                methodList.Add(m);
            }

            return methodList.ToArray();
        }

        /// <summary>
        /// Returns true if a method is a debug method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool IsDebugMethod(MethodInfo method)
        {
            return Attribute.GetCustomAttribute(method, typeof(DebugMethod)) is DebugMethod;
        }

        private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string>()
        {
            {typeof(byte), "byte"},
            {typeof(sbyte), "sbyte"},
            {typeof(short), "short"},
            {typeof(ushort), "ushort"},
            {typeof(int), "int"},
            {typeof(uint), "uint"},
            {typeof(long), "long"},
            {typeof(ulong), "ulong"},
            {typeof(float), "float"},
            {typeof(double), "double"},
            {typeof(decimal), "decimal"},
            {typeof(object), "object"},
            {typeof(bool), "bool"},
            {typeof(char), "char"},
            {typeof(string), "string"},
            {typeof(void), "void"},
            {typeof(byte?), "byte?"},
            {typeof(sbyte?), "sbyte?"},
            {typeof(short?), "short?"},
            {typeof(ushort?), "ushort?"},
            {typeof(int?), "int?"},
            {typeof(uint?), "uint?"},
            {typeof(long?), "long?"},
            {typeof(ulong?), "ulong?"},
            {typeof(float?), "float?"},
            {typeof(double?), "double?"},
            {typeof(decimal?), "decimal?"},
            {typeof(bool?), "bool?"},
            {typeof(char?), "char?"}
        };
    }
}