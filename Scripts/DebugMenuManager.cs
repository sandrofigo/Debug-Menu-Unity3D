//
// Copyright (c) Sandro Figo
//

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
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

        public readonly List<Node> nodes = new List<Node>();

        private string lastMethod;

        public Node lastInvokedNode;

        public object lastReturnValue;

        private KeyCode enableKeyCode;

        private bool visible;

        private CursorState lastCursorState;

        private void Start()
        {
            outputText = transform.Find("Console Panel/Output Text").GetComponent<Text>();

            consolePanel.gameObject.SetActive(false);

            ClearSuggestions();
            ClearOutput();

            inputField.onValueChanged.AddListener(delegate { OnInputFieldChanged(); });

            ConstructNodeTree(Helper.GetMethodData());

            // Add menu buttons
            ButtonMenu.Instance.CreateMenuButtons(nodes);

            // Prevent layout glitch
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonMenu);
            buttonMenu.gameObject.SetActive(false);

            enableKeyCode = Helper.GetKeyCodeFromString(Settings.EnableKey);
        }

        private void Update()
        {
#if (!ENABLE_INPUT_SYSTEM)
            if (Input.GetKeyDown(enableKeyCode))
#else
            if (Keyboard.current.f3Key.wasPressedThisFrame) //TODO: read key from settings
#endif
            {
                visible = !visible;

                if (!Settings.HideConsole)
                    consolePanel.gameObject.SetActive(!consolePanel.gameObject.activeInHierarchy);

                buttonMenu.gameObject.SetActive(!buttonMenu.gameObject.activeInHierarchy);

                if (visible)
                {
                    inputField.Select();
                    inputField.ActivateInputField();

                    lastCursorState = GetCursorState();
                    EnableCursor();
                }
                else
                {
                    ButtonMenu.Instance.DestroyAllOpenPanels();
                    ButtonMenu.Instance.ResetAllMenuButtons();

                    RestorePreviousCursorState();
                }
            }

#if (!ENABLE_INPUT_SYSTEM)
            if (Input.GetKeyDown(KeyCode.F4) && lastInvokedNode != null)
#else
            if (Keyboard.current.f4Key.wasPressedThisFrame && lastInvokedNode != null)
#endif
            {
                if (lastInvokedNode.method.GetParameters().Length == 0)
                {
                    object returnValue = lastInvokedNode.method.Invoke(lastInvokedNode.monoBehaviour, null);
                }
                else
                {
                    Debug.LogWarning("Re-invoking a method with parameters is not supported yet.");
                }
            }

#if (!ENABLE_INPUT_SYSTEM)
            if (Input.GetKeyDown(KeyCode.Tab))
#else
            if (Keyboard.current.tabKey.wasPressedThisFrame)
#endif
            {
                if (visible)
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

#if (!ENABLE_INPUT_SYSTEM)
            if (Input.GetKeyDown(KeyCode.UpArrow))
#else
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
#endif
            {
                inputField.text = lastMethod;
                inputField.caretPosition = inputField.text.Length;
            }

#if (!ENABLE_INPUT_SYSTEM)
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
#else
            if (Keyboard.current.backspaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
#endif
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
                    object obj = node.method.Invoke(node.monoBehaviour, parameters.ToArray()); //TODO: unify invocation (DebugMenuItem)
                    Log($"Return value: {obj ?? "null"}\n");
                }
                else
                {
                    Log($"'{inputField.text}' is not a valid method!\n");
                }

                inputField.text = string.Empty;
                inputField.ActivateInputField();
            }
        }

        private void RestorePreviousCursorState()
        {
            Cursor.visible = lastCursorState.isVisible;
            Cursor.lockState = lastCursorState.lockMode;
        }

        private static void EnableCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private static CursorState GetCursorState()
        {
            return new CursorState { isVisible = Cursor.visible, lockMode = Cursor.lockState };
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
                if (node.debugMethod?.HasParameters ?? false)
                    continue;

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
        public Node FindNodeByPath(string path)
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

                    var context = new MethodContext(data, info, method, nodes);

                    context.CreateNodes();
                }
            }

            // Sort nodes by name
            nodes.Sort((node1, node2) => string.Compare(node1.name, node2.name, StringComparison.Ordinal));

            // Sort base nodes based on priority
            nodes.Sort((node1, node2) => node2.priority.CompareTo(node1.priority));
        }
    }
}