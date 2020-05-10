﻿//
// Copyright (c) Sandro Figo
//
using UnityEngine;
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugMenuItem : MonoBehaviour
    {
        public RectTransform panelPrefab;
        public RectTransform menuItemPrefab;

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

        public void OnClick()
        {
            if (node.children.Count > 0)
            {
                if (!panelOpen)
                {
                    RectTransform panel = Instantiate(panelPrefab, DebugMenuManager.Instance.transform).GetComponent<RectTransform>();
                    panel.SetParent(DebugMenuManager.Instance.transform);

                    ButtonMenu.Instance.openPanels.Add(panel);

                    RectTransform parentPanel = transform.parent.GetComponent<RectTransform>();

                    panel.anchoredPosition = parentPanel.anchoredPosition + new Vector2(parentPanel.rect.width, rectTransform.anchoredPosition.y);

                    foreach (var n in node.children)
                    {
                        RectTransform menuItem = Instantiate(menuItemPrefab).GetComponent<RectTransform>();
                        menuItem.SetParent(panel);
                        menuItem.GetComponentInChildren<Text>().text = n.name;
                        DebugMenuItem m = menuItem.GetComponent<DebugMenuItem>();
                        m.node = n;
                        if (n.children.Count > 0)
                        {
                            m.arrow.gameObject.SetActive(true);
                        }
                        else
                        {
                            m.arrow.gameObject.SetActive(false);
                        }
                    }

                    panelOpen = true;

                    LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
                }
            }
            else
            {
                DebugMenuManager.Log(node.name);
                DebugMenuManager.Instance.lastInvokedNode = node;
                
                if (node.method.GetParameters().Length == 0)
                {
                    object obj = node.method.Invoke(node.monoBehaviour, null);
                    if (obj != null)
                        DebugMenuManager.Log($"Return value: {obj}\n");
                }
                else
                {
                    object obj = node.method.Invoke(node.monoBehaviour, new []{node.debugMethod.parameters[0]});
                    if (obj != null)
                        DebugMenuManager.Log($"Return value: {obj}\n");
                }
                
                ButtonMenu.Instance.ResetAllMenuButtons();
                ButtonMenu.Instance.DestroyAllOpenPanels();
            }
        }
    }
}