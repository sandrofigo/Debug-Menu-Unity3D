//
// Copyright (c) Sandro Figo
//

using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugMenuItemPanel : MonoBehaviour
    {
        public Image image;

        private void Update()
        {
#if !ENABLE_INPUT_SYSTEM
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
#else
            if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
#endif
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