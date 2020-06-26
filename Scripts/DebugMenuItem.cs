//
// Copyright (c) Sandro Figo
//

using UnityEngine;
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugMenuItem : MonoBehaviour
    {
        [HideInInspector]
        public Node node;

        public RectTransform arrow;

        private RectTransform rectTransform;

        [HideInInspector]
        public bool panelOpen;

        public Text arrowText;

        public Text text;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(Node n, Transform parent)
        {
            node = n;
            text.text = n.name;
            text.color = Settings.TextColor;
            arrowText.color = Settings.TextColor;
            
            transform.SetParent(parent);
            
            if (node.HasChildren())
                arrow.gameObject.SetActive(true);
        }

        public void OnClick()
        {
            if (node.HasChildren())
            {
                if (!panelOpen)
                {
                    RectTransform parentPanel = transform.parent.GetComponent<RectTransform>();
                    
                    RectTransform panel = ButtonMenu.Instance.CreateMenuPanel(node);
                    panel.anchoredPosition = parentPanel.anchoredPosition + new Vector2(parentPanel.rect.width, rectTransform.anchoredPosition.y);
                    
                    panelOpen = true;
                    
                    LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
                }
            }
            else
            {
                InvokeMethod();
            }
        }

        private void InvokeMethod()
        {
            DebugMenuManager.Log(node.name);
            DebugMenuManager.Instance.lastInvokedNode = node;

            object returnValue;
            
            if (node.method.GetParameters().Length == 0)
            {
                returnValue = node.method.Invoke(node.monoBehaviour, null);
            }
            else
            {
                returnValue = node.method.Invoke(node.monoBehaviour, new[] {node.debugMethod.parameters[node.parameterIndex]});
            }
            
            DebugMenuManager.Log($"Return value: {returnValue ?? "null"}\n");

            if (Settings.AutoClosePanels)
            {
                ButtonMenu.Instance.ResetAllMenuButtons();
                ButtonMenu.Instance.DestroyAllOpenPanels();
            }
        }
    }
}