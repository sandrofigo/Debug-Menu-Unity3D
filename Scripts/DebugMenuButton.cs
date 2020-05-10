//
// Copyright (c) Sandro Figo
//

using UnityEngine;
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugMenuButton : MonoBehaviour
    {
        [HideInInspector]
        public bool panelOpen;

        [HideInInspector]
        public Node node;

        public Image image;
        
        public Text text;

        public void OnClick()
        {
            if (!panelOpen)
                ShowPanel();
            else
                HidePanel();
        }

        private void ShowPanel()
        {
            ButtonMenu.Instance.ResetAllMenuButtons();
            ButtonMenu.Instance.DestroyAllOpenPanels();

            RectTransform rectTransform = GetComponent<RectTransform>();
                
            RectTransform panel = ButtonMenu.Instance.CreateMenuPanel(node);
            panel.anchoredPosition = rectTransform.anchoredPosition - new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);
                
            panelOpen = true;

            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
        }

        private void HidePanel()
        {
            panelOpen = false;
            
            ButtonMenu.Instance.DestroyAllOpenPanels();
        }
    }
}