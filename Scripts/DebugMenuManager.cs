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
    public class DebugMenuManager : Singleton<DebugMenuManager>
    {
        [SerializeField]
        private RectTransform consolePanel = null;

        [SerializeField]
        private RectTransform buttonMenu = null;

        [SerializeField]
        private InputField inputField = null;

        [SerializeField]
        private Text suggestionsText = null;

        private static Text outputText;

        private readonly List<Node> nodes = new List<Node>();

        private string lastMethod;

        public Node lastInvokedNode;

        private bool Visible { get; set; }

        public delegate void VisibilityHandler(bool visible);

        public event VisibilityHandler VisibilityChanged;

        private void OnVisibilityChanged()
        {
            VisibilityChanged?.Invoke(Visible);
        }

        protected override void Awake()
        {
            base.Awake();
            
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

            ClearSuggestions();
            ClearOutput();

            inputField.onValueChanged.AddListener(delegate { OnInputFieldChanged(); });

            ConstructNodeTree(Helper.GetMethodData());

            // Add menu buttons
            ButtonMenu.Instance.CreateMenuButtons(nodes);

            // Prevent layout glitch
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonMenu);
            buttonMenu.gameObject.SetActive(false);
        }

        private void Update()
        {
            //TODO: use EditorPrefs
            if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.F3))
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

                OnVisibilityChanged();
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

                    var suggestions = GetSuggestions(input);

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

                Node node = FindNodeByPath(split[0]);

                if (node?.method != null)
                {
                    // Valid method
                    var parameters = new List<object>();

                    var parameterTypes = node.method.GetParameters();

                    for (int i = 1; i < split.Length; i++)
                    {
                        try
                        {
                            object x = Convert.ChangeType(split[i], parameterTypes[i - 1].ParameterType);
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
                    Log($"Return value: {obj ?? "null"}");
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

            Node lastCompleteNode = FindNodeByPath(parentPath);

            List<Node> currentNodes = new List<Node>();

            if (lastCompleteNode == null)
            {
                // Use the whole node tree if there is no separator ('.') present
                if (!path.Contains("."))
                    currentNodes = nodes;
            }
            else
            {
                currentNodes = lastCompleteNode.children;
            }

            foreach (Node node in currentNodes)
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
                                typeText += Helper.TypeAliases[parameters[i].ParameterType];
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
        private Node FindNodeByPath(string path)
        {
            string[] split = path.Split('.');

            var currentNodes = nodes;

            for (int splitIndex = 0; splitIndex < split.Length; splitIndex++)
            {
                foreach (Node node in currentNodes)
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

        private void ConstructNodeTree(IEnumerable<MethodData> methodData)
        {
            foreach (MethodData data in methodData)
            {
                foreach (MethodInfo info in data.methods)
                {
                    DebugMethod method = Helper.GetDebugMethod(info);

                    if (method == null)
                        continue;

                    if (method.parameters != null)
                    {
                        foreach (object parameter in method.parameters)
                        {
                            Node childNode = new Node
                            {
                                name = info.Name + parameter,
                                method = info,
                                monoBehaviour = data.monoBehaviour,
                                debugMethod = method
                            };

                            string baseNodeName = info.DeclaringType.ToString();

                            Node baseNode = Helper.GetNodeByName(nodes, baseNodeName);

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

                    // Custom path
                    if (method.customPath != string.Empty)
                    {
                        string[] split = method.customPath.Split('/');

                        List<Node> currentNodeList = nodes;

                        for (int splitIndex = 0; splitIndex < split.Length; splitIndex++)
                        {
                            Node n = Helper.GetNodeByName(currentNodeList, split[splitIndex]);

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
                                    monoBehaviour = data.monoBehaviour,
                                    debugMethod = method
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
                            monoBehaviour = data.monoBehaviour,
                            debugMethod = method
                        };

                        string baseNodeName = info.DeclaringType.ToString();

                        Node baseNode = Helper.GetNodeByName(nodes, baseNodeName);

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
    }
}