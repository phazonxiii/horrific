using UnityEngine;
using UnityEngine.UI;


namespace HorrorEngine
{
    public class UIShopConfirmation : MonoBehaviour
    {
        [SerializeField] private Image m_ItemImage;
        [SerializeField] private TMPro.TextMeshProUGUI m_ConfirmText;

        [SerializeField] public GameObject m_ConfirmButton;
        [SerializeField] public GameObject m_CancelButton;

        public void Show(string message, ItemData item, ShopContentValueEntry[] cost)
        {
            m_ConfirmText.text = message;
            m_ItemImage.sprite = item.Image;

            EventSystemUtils.ForceSelection(m_CancelButton);

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}