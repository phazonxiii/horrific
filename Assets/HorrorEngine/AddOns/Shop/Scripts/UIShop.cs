using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

namespace HorrorEngine
{
    public class UIShop : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI m_ItemName;
        [SerializeField] private TMPro.TextMeshProUGUI m_ItemDesc;

        [SerializeField] private Transform m_ShopCategoriesParent;
        [SerializeField] private GameObject m_ShopCategoryButton;

        [SerializeField] private Transform m_ShopItemsParent;
        [SerializeField] private GameObject m_ShopItemEntry;

        [SerializeField] private UIShopConfirmation m_ConfirmDialog;
        [SerializeField] private LocalizableText m_ConfirmText;
        [SerializeField] private string m_ItemNameReplacementTag = "[ITEMNAME]";

        [Header("Audio")]
        [SerializeField] private AudioClip m_ShowClip;
        [SerializeField] private AudioClip m_CloseClip;
        [SerializeField] private AudioClip m_PurchaseConfirmedClip;
        [SerializeField] private AudioClip m_PurchaseCanceledClip;
        [SerializeField] private AudioClip m_FailedPurchase;

        private IUIInput m_Input;
        private ShopContent m_Content;
        private List<UIShopCategoryButton> m_Categories = new();

        private int m_SelectedCategory;
        private UIShopEntryItem m_SelectedEntry;
        
        // --------------------------------------------------------------------

        private void Awake()
        {
            m_Input = GetComponentInParent<IUIInput>();
        }

        // --------------------------------------------------------------------

        private void Start()
        {
            gameObject.SetActive(false);
        }

        // --------------------------------------------------------------------

        public void Show(ShopContent content)
        {
            m_Content = content;

            PauseController.Instance.Pause(this);

            CursorController.Instance.SetInUI(true);

            UIManager.Get<UIAudio>().Play(m_ShowClip);

            gameObject.SetActive(true);

            FillCategories();

            if (m_Content.ProductSelling.Count > 0)
            {
                SetSelectedCategory(0);
            }

            m_ConfirmDialog.Hide();

        }

        // --------------------------------------------------------------------

        private void SetSelectedCategory(int index)
        {
            m_SelectedCategory = index;

            for (int i = 0; i < m_Categories.Count; ++i)
            {
                m_Categories[i].SetSelected(index == i);
            }

            FillCategory(index);
        }

        // --------------------------------------------------------------------

        private void FillCategories() 
        {
            m_Categories.Clear();

            for (int i = m_ShopCategoriesParent.childCount - 1; i >= 0; i--)
            {
                // TODO - Use pooling
                Destroy(m_ShopCategoriesParent.GetChild(i).gameObject);
            }

            int index = 0;
            foreach (var category in m_Content.ProductSelling)
            {
                // TODO - Use pooling
                var button = Instantiate(m_ShopCategoryButton);
                button.transform.SetParent(m_ShopCategoriesParent);
                button.transform.localScale = Vector3.one;

                UIShopCategoryButton catButton = button.GetComponent<UIShopCategoryButton>();
                catButton.Fill(category.Name, category.Icon);

                int catIndex = index;
                button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SetSelectedCategory(catIndex);
                });

                m_Categories.Add(catButton);

                ++index;
            }
        }

        // --------------------------------------------------------------------

        private void FillCategory(int index)
        {
            for (int i = m_ShopItemsParent.childCount - 1; i >= 0; i--)
            {
                // TODO - Use pooling
                Destroy(m_ShopItemsParent.GetChild(i).gameObject);
            }

            int itemIndex = 0;
            foreach (var itemEntry in m_Content.ProductSelling[index].Items)
            {
                // TODO - Use pooling
                var button = Instantiate(m_ShopItemEntry);
                button.transform.SetParent(m_ShopItemsParent);
                button.transform.localScale = Vector3.one;

                var uiEntry = button.GetComponent<UIShopEntryItem>();
                uiEntry.Fill(itemEntry);

                button.GetComponent<UISelectableCallbacks>().OnSelected.AddListener((selectable) =>
                {
                    SetSelectedEntry(uiEntry);
                });

                button.GetComponent<UIPointerClickEvents>().OnDoubleClick.AddListener(OnSubmit);

                ++itemIndex;
            }

            SetSelectedEntry(null);
            EventSystemUtils.ForceSelection(null); // Focusable will be set in Update
        }

        // --------------------------------------------------------------------

        private void OnSubmit()
        {
            if (!m_SelectedEntry)
                return;

            if (!m_SelectedEntry.CanBePurchased(out ShopCantPurchaseReason reason))
            {
                UIManager.Get<UIAudio>().Play(m_FailedPurchase);
                return;
            }

            if (m_SelectedEntry != null)
            {
                string text = m_ConfirmText;
                text = text.Replace(m_ItemNameReplacementTag, m_SelectedEntry.Entry.Item.Item.Name);
                m_ConfirmDialog.Show(text, m_SelectedEntry.Entry.Item.Item, m_SelectedEntry.Entry.Cost);
            }
        }

        // --------------------------------------------------------------------

        private void SetSelectedEntry(UIShopEntryItem uiEntry)
        {
            m_SelectedEntry = uiEntry;
            
            ItemData itemData = uiEntry ? uiEntry.Entry.Item.Item : null;
            if (itemData)
            {
                m_ItemName.text = itemData.Name;
                m_ItemDesc.text = itemData.Description;
            }
            else
            {
                m_ItemName.text = "";
                m_ItemDesc.text = "";
            }
        }

        // --------------------------------------------------------------------

        private void Update()
        {
            if (m_Input.IsCancelDown())
            {
                OnCancel();
            }
            else if (m_SelectedEntry != null)
            {
                if (m_Input.IsConfirmDown())
                {
                    OnSubmit();
                }
            }

            EventSystemUtils.SelectDefaultOnLostFocus(GetDesiredFocusable());
        }
        // --------------------------------------------------------------------

        private GameObject GetDesiredFocusable()
        {
            if (m_SelectedEntry)
            {
                return m_SelectedEntry.gameObject;
            }
            else if (m_ShopItemsParent.childCount > 0)
            {
                return m_ShopItemsParent.GetChild(0).gameObject;
            }

            return null;
        }

        // --------------------------------------------------------------------

        private void OnCancel()
        {
            if (m_ConfirmDialog.isActiveAndEnabled)
            {
                OnDialogCancel();
                return;
            }

            UIManager.Get<UIAudio>().Play(m_CloseClip);
            Hide();
        }

        // --------------------------------------------------------------------

        private void Hide()
        {
            PauseController.Instance.Resume(this);

            CursorController.Instance.SetInUI(false);

            gameObject.SetActive(false);
        }

        // --------------------------------------------------------------------

        public void OnDialogConfirm()
        {
            if (m_SelectedEntry)
            {
                PayCost(m_SelectedEntry.Entry.Cost);
                GameManager.Instance.Inventory.Add(m_SelectedEntry.Entry.Item);
                UIManager.Get<UIAudio>().Play(m_PurchaseConfirmedClip);

                FillCategory(m_SelectedCategory);
            }

            m_ConfirmDialog.Hide();

            m_Input.Flush(); // Flush so an object is not immediately selected after this
        }

        // --------------------------------------------------------------------

        public void OnDialogCancel()
        {
            UIManager.Get<UIAudio>().Play(m_PurchaseCanceledClip);
            m_ConfirmDialog.Hide();

            m_Input.Flush(); // Flush so an object is not immediately selected after this
        }

        // --------------------------------------------------------------------

        private void PayCost(ShopContentValueEntry[] totalCost)
        {
            foreach (var cost in totalCost)
            {
                float currentVal = GameAttributeUtils.GetAsFloat(cost.Attribute);
                GameAttributeUtils.SetAsFloat(cost.Attribute, currentVal - cost.Amount);
            }
        }

        // --------------------------------------------------------------------

        public void PrevCategory(CallbackContext context)
        {
            if (context.performed)
            {
                --m_SelectedCategory;
                if (m_SelectedCategory < 0)
                {
                    m_SelectedCategory = m_Categories.Count - 1;
                }

                SetSelectedCategory(m_SelectedCategory);
            }
        }

        // --------------------------------------------------------------------

        public void NextCategory(CallbackContext context)
        {
            if (context.performed)
            {
                ++m_SelectedCategory;
                if (m_SelectedCategory >= m_Categories.Count)
                {
                    m_SelectedCategory = 0;
                }

                SetSelectedCategory(m_SelectedCategory);
            }
        }
    }
}