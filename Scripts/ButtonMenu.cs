//
// Copyright (c) Sandro Figo
//
using System.Collections.Generic;
using UnityEngine;

namespace DebugMenu
{
    public class ButtonMenu : MonoBehaviour
    {
        public static ButtonMenu Instance { get; set; }

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
    }
}