//
// Copyright (c) Sandro Figo
//
using UnityEngine;
using UnityEngine.EventSystems;

namespace DebugMenu
{
    public class DebugMenuItemPanel : MonoBehaviour
    {
        [HideInInspector]
        public DebugMenuButton button;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                ButtonMenu.Instance.DestroyAllOpenPanels();
                ButtonMenu.Instance.ResetAllMenuButtons();
            }
        }
    }
}