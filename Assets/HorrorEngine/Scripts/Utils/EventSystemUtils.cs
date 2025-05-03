using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HorrorEngine
{
    public class EventSystemUtils : MonoBehaviour
    {
        public static void SelectDefaultOnLostFocus(GameObject defaultObj)
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected == null || !selected.activeInHierarchy)
            {
                ForceSelection(defaultObj);
            }
        }

        public static void ForceSelection(GameObject selection)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selection);
        }
    }
}