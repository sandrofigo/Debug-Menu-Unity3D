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
            {
                ButtonMenu.Instance.ResetAllMenuButtons();
                ButtonMenu.Instance.DestroyAllOpenPanels();

                RectTransform rectTransform = GetComponent<RectTransform>();

                RectTransform panel = Instantiate(ButtonMenu.Instance.panelPrefab).GetComponent<RectTransform>();
                panel.transform.SetParent(DebugMenuManager.Instance.transform);

                ButtonMenu.Instance.openPanels.Add(panel);

                panel.anchoredPosition = rectTransform.anchoredPosition - new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);

                DebugMenuItemPanel p = panel.GetComponent<DebugMenuItemPanel>();
                p.button = this;
                p.image.color = Settings.BackgroundColor;

                foreach (Node childNode in node.children)
                {
                    p.CreateItem(childNode);
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