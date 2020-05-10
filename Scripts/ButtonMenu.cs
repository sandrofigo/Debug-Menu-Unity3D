//
// Copyright (c) Sandro Figo
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DebugMenu
{
    public class ButtonMenu : MonoBehaviour
    {
        public static ButtonMenu Instance { get; private set; } //TODO: use generic singleton class

        [SerializeField]
        private GameObject menuButtonPrefab = null;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public List<DebugMenuButton> buttons = new List<DebugMenuButton>();
        public List<RectTransform> openPanels = new List<RectTransform>();

        public void DestroyAllOpenPanels()
        {
            for (int i = openPanels.Count - 1; i >= 0; i--)
            {
                Destroy(openPanels[i].gameObject);
                openPanels.Remove(openPanels[i]);
            }
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
            var nodeList = nodes.ToList();
            nodeList.Sort((node1, node2) => string.Compare(node1.name, node2.name, StringComparison.Ordinal));

            foreach (Node node in nodeList)
            {
                GameObject menuButton = Instantiate(menuButtonPrefab, transform, true);

                var debugMenuButton = menuButton.GetComponent<DebugMenuButton>();
                debugMenuButton.node = node;
                debugMenuButton.text.text = node.name;
                debugMenuButton.text.color = (Color)Settings.TextColor.Get();
                debugMenuButton.image.color = (Color)Settings.BackgroundColor.Get();

                buttons.Add(debugMenuButton);
            }
        }
    }
}