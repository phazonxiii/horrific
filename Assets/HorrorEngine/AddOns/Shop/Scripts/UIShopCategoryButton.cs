using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HorrorEngine
{
    public class UIShopCategoryButton : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;
        
        public UnityEvent OnSelected;
        public UnityEvent OnDeselected;

        public void Fill(string name, Sprite icon)
        {
            if (m_Icon)
                m_Icon.sprite = icon;

            if (m_NameText)
                m_NameText.text = name;

            SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            if (selected)
                OnSelected?.Invoke();
            else
                OnDeselected?.Invoke();
        }
    }
}