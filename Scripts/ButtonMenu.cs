//
// Copyright (c) Sandro Figo
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DebugMenu
{
    public class ButtonMenu : Singleton<ButtonMenu>
    {
        [SerializeField]
        private GameObject menuButtonPrefab = null;

        public DebugMenuItemPanel panelPrefab;
        public DebugMenuItem itemPrefab;
        
        public List<DebugMenuButton> buttons = new List<DebugMenuButton>();
        public List<RectTransform> openPanels = new List<RectTransform>();

        public void DestroyAllOpenPanels()
        {
            for (int i = openPanels.Count - 1; i >= 0; i--)
            {
                Destroy(openPanels[i].gameObject);
            }
            
            openPanels.Clear();
        }

        public void ResetAllMenuButtons()
        {
            foreach (DebugMenuButton button in buttons)
            {
                button.panelOpen = false;
            }
        }

        public void CreateMenuButtons(IEnumerable<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                GameObject menuButton = Instantiate(menuButtonPrefab, transform, true);

                var debugMenuButton = menuButton.GetComponent<DebugMenuButton>();
                debugMenuButton.node = node;
                debugMenuButton.text.text = node.name;
                debugMenuButton.text.color = Settings.TextColor;
                debugMenuButton.image.color = Settings.BackgroundColor;

                buttons.Add(debugMenuButton);
            }
        }

        public RectTransform CreateMenuPanel(Node node)
        {
            var panel = Instantiate(panelPrefab, DebugMenuManager.Instance.transform).GetComponent<RectTransform>();
            panel.SetParent(DebugMenuManager.Instance.transform);

            openPanels.Add(panel);
                    
            var itemPanel = panel.GetComponent<DebugMenuItemPanel>();
            itemPanel.image.color = Settings.BackgroundColor;

            foreach (Node childNode in node.children)
            {
                itemPanel.CreateItem(childNode);
            }

            return panel;
        }
    }
}