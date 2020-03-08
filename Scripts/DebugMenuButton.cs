//
// Copyright (c) Sandro Figo
//
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
        public bool isRoot;

        [HideInInspector]
        public Node node;

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

                foreach (var node in node.children)
                {
                    RectTransform menuItem = Instantiate(menuItemPrefab).GetComponent<RectTransform>();
                    menuItem.SetParent(panel);
                    menuItem.GetComponentInChildren<Text>().text = node.name;
                    DebugMenuItem m = menuItem.GetComponent<DebugMenuItem>();
                    m.node = node;
                    if (node.children.Count > 0)
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