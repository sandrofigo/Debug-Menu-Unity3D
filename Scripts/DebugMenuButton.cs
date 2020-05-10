//
// Copyright (c) Sandro Figo
//

using System;
using UnityEngine;
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugMenuButton : MonoBehaviour
    {
        public GameObject menuDropdownPrefab;
        public GameObject menuItemPrefab;

        [HideInInspector]
        public bool panelOpen;

        [HideInInspector]
        public Node node;

        public Image image;
        
        public Text text;

        public void OnClick()
        {
            if (!panelOpen)
            {
                ButtonMenu.Instance.ResetAllMenuButtons();
                ButtonMenu.Instance.DestroyAllOpenPanels();

                RectTransform rectTransform = GetComponent<RectTransform>();

                RectTransform panel = Instantiate(menuDropdownPrefab).GetComponent<RectTransform>();
                panel.transform.SetParent(DebugMenuManager.Instance.transform);

                ButtonMenu.Instance.openPanels.Add(panel);

                panel.anchoredPosition = rectTransform.anchoredPosition - new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);

                DebugMenuItemPanel p = panel.GetComponent<DebugMenuItemPanel>();
                p.button = this;
                p.image.color = (Color)Settings.BackgroundColor.Get();

                foreach (var childNode in node.children)
                {
                    RectTransform menuItem = Instantiate(menuItemPrefab).GetComponent<RectTransform>();
                    menuItem.SetParent(panel);
                    DebugMenuItem m = menuItem.GetComponent<DebugMenuItem>();
                    m.node = childNode;
                    m.text.text = childNode.name;
                    m.text.color = (Color)Settings.TextColor.Get();
                    m.arrowText.color = (Color)Settings.TextColor.Get();
                    if (childNode.children.Count > 0)
                        m.arrow.gameObject.SetActive(true);
                }

                panelOpen = true;

                LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
            }
            else
            {
                ButtonMenu.Instance.DestroyAllOpenPanels();
                ButtonMenu.Instance.ResetAllMenuButtons();
            }
        }
    }
}