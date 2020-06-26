//
// Copyright (c) Sandro Figo
//
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugMenuItemPanel : MonoBehaviour
    {
        public Image image;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                ButtonMenu.Instance.DestroyAllOpenPanels();
                ButtonMenu.Instance.ResetAllMenuButtons();
            }
        }

        public void CreateItem(Node node)
        {
            DebugMenuItem item = Instantiate(ButtonMenu.Instance.itemPrefab);
            item.Initialize(node, transform);
        }
    }
}