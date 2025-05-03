using UnityEngine;
using UnityEngine.EventSystems;

namespace HorrorEngine
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private bool m_PauseGame = true;
        [SerializeField] private GameObject m_Content;
        [SerializeField] private UIInventoryContextMenu m_ContextMenu;
        [SerializeField] private UIInventoryItem[] m_ItemSlots;
        [SerializeField] private TMPro.TextMeshProUGUI m_ItemName;
        [SerializeField] private TMPro.TextMeshProUGUI m_ItemDesc;
        [SerializeField] private UIInventoryItem m_Equipped;
        [SerializeField] private UIInventoryItem m_EquippedSecondary;
        [SerializeField] private float m_ExpandingInteractionDelay = 1f;

        [Header("Audio")]
        [SerializeField] private AudioClip m_ShowClip;
        [SerializeField] private AudioClip m_UseClip;
        [SerializeField] private AudioClip m_CantUseClip;
        [SerializeField] private AudioClip m_NavigateClip;
        [SerializeField] private AudioClip m_CloseClip;
        [SerializeField] private AudioClip m_ExpandClip;

        private UIInventoryItem m_SelectedSlot;
        private UIInventoryItem m_CombiningSlot;

        private IUIInput m_Input;
        private bool m_Expanding;

        // --------------------------------------------------------------------

        protected void Awake()
        {
            m_Input = GetComponentInParent<IUIInput>();

            foreach (var slot in m_ItemSlots)
            {
                RegisterItemCallbacks(slot);
            }

            RegisterItemCallbacks(m_EquippedSecondary);

            m_ContextMenu.UseButton.onClick.AddListener(OnUse);
            m_ContextMenu.EquipButton.onClick.AddListener(OnEquip);
            m_ContextMenu.CombineButton.onClick.AddListener(OnCombine);
            m_ContextMenu.ExamineButton.onClick.AddListener(OnExamine);
            m_ContextMenu.DropButton.onClick.AddListener(OnDrop);
        }

        // --------------------------------------------------------------------

        void RegisterItemCallbacks(UIInventoryItem slot)
        {
            UISelectableCallbacks selectable = slot.GetComponent<UISelectableCallbacks>();
            selectable.OnSelected.AddListener(OnSlotSelected);

            UIPointerClickEvents pointerEvents = slot.GetComponent<UIPointerClickEvents>();
            pointerEvents.OnDoubleClick.AddListener(OnSubmit);
        }

        // --------------------------------------------------------------------

        void OnSlotSelected(GameObject obj)
        {
            if (m_SelectedSlot) // Cancel previous selection
            {
                if (m_ContextMenu.isActiveAndEnabled)
                {
                    m_ContextMenu.gameObject.SetActive(false);
                }
                
                if (m_SelectedSlot != m_CombiningSlot)
                    m_SelectedSlot.SetSelectionLocked(false);

                m_SelectedSlot = null;
            }

            UIInventoryItem slot = obj.GetComponent<UIInventoryItem>();
            m_SelectedSlot = slot;
            if (slot.InventoryEntry != null && slot.InventoryEntry.Item)
            {
                m_ItemName.text = slot.InventoryEntry.Item.Name;
                m_ItemDesc.text = slot.InventoryEntry.Item.Description;
                UIManager.Get<UIAudio>().Play(m_NavigateClip);
            }
            else
            {
                m_ItemName.text = "";
                m_ItemDesc.text = "";
            }
        }

        // --------------------------------------------------------------------

        void Start()
        {
            gameObject.SetActive(false);
        }

        // --------------------------------------------------------------------

        void Update()
        {
            if (m_Expanding)
                return;

            if (m_Input.IsCancelDown() || m_Input.IsToggleInventoryDown())
            {
                OnCancel();
            }

            if (m_SelectedSlot && m_SelectedSlot.InventoryEntry != null)
            {
                if (m_Input.IsConfirmDown())
                {
                    OnSubmit();
                }
            }

            EventSystemUtils.SelectDefaultOnLostFocus(m_SelectedSlot ? m_SelectedSlot.gameObject : m_ItemSlots[0].gameObject);
        }

        // --------------------------------------------------------------------

        public void Show()
        {
            if (m_PauseGame)
                PauseController.Instance.Pause(this);

            CursorController.Instance.SetInUI(true);
            
            m_SelectedSlot = null;
            m_CombiningSlot = null;
            m_Expanding = false;

            gameObject.SetActive(true); // Needs to happen before fill or animations wont play

            Fill();
            FillEquipped();
            
            m_ContextMenu.gameObject.SetActive(false);

            UIManager.Get<UIAudio>().Play(m_ShowClip);

            if (m_Expanding)
            {
                UIManager.Get<UIAudio>().Play(m_ExpandClip);
                this.InvokeActionUnscaled(EndExpansion, m_ExpandingInteractionDelay);
            }
        }

        // --------------------------------------------------------------------

        void EndExpansion()
        {
            m_Expanding = false;
        }

        // --------------------------------------------------------------------

        private void Hide()
        {
            if (m_PauseGame)
                PauseController.Instance.Resume(this);

            CursorController.Instance.SetInUI(false);
            m_ContextMenu.gameObject.SetActive(false);
            gameObject.SetActive(false);

            UIManager.PopAction();
        }

        // --------------------------------------------------------------------

        private void OnCancel()
        {
            UIManager.Get<UIAudio>().Play(m_CloseClip);
            if (m_CombiningSlot)
            {
                CancelCombine();
            }
            else if (m_ContextMenu.isActiveAndEnabled)
            {
                m_ContextMenu.gameObject.SetActive(false);
                m_SelectedSlot.SetSelectionLocked(false);
            }
            else
            {
                Hide();
            }
        }

        // --------------------------------------------------------------------

        private void CancelCombine()
        {
            m_CombiningSlot.SetSelectionLocked(false);
            m_CombiningSlot = null;
        }

        // --------------------------------------------------------------------

        private void OnSubmit()
        {
            m_SelectedSlot = EventSystem.current.currentSelectedGameObject.GetComponent<UIInventoryItem>();
            if (!m_SelectedSlot) // Selected obj is not an item
            {
                if (m_CombiningSlot)
                    CancelCombine();
                return;
            }

            ItemData item = m_SelectedSlot.InventoryEntry.Item;
            if (item == null)
                return;

            
            if (m_CombiningSlot && m_SelectedSlot != m_CombiningSlot)
            {
                Combine();
            }
            else
            {
                if (!m_ContextMenu.Show(item))
                {
                    OnUse();
                }
                else
                {
                    m_SelectedSlot.SetSelectionLocked(true);
                }
            }
        }

        // --------------------------------------------------------------------

        private void OnUse()
        {
            if (GameManager.Instance.Inventory.Use(m_SelectedSlot.InventoryEntry))
            {
                UIManager.Get<UIAudio>().Play(m_UseClip);

                ItemData item = m_SelectedSlot.InventoryEntry.Item;
                if (item && item.Flags.HasFlag(ItemFlags.UseOnInteractive))
                {
                    Hide();
                }
                else
                {
                    Fill();
                    FillEquipped();
                }
            }
            else
            {
                UIManager.Get<UIAudio>().Play(m_CantUseClip);
            }

            m_Input.Flush(); // Flush so an object is not immediately selected after this
        }

        // --------------------------------------------------------------------

        private void OnEquip()
        {
            EquipableItemData item = m_SelectedSlot.InventoryEntry.Item as EquipableItemData;
            item.Equip(m_SelectedSlot.InventoryEntry);
            UIManager.Get<UIAudio>().Play(m_UseClip);

            Fill();
            FillEquipped();

            m_Input.Flush(); // Flush so an object is not immediately selected after this
        }

        // --------------------------------------------------------------------

        private void OnCombine()
        {
            if (m_CombiningSlot)
            {
                if (m_SelectedSlot.InventoryEntry.Item)
                    Combine();
                else
                    CancelCombine();
            }
            else if (m_SelectedSlot.InventoryEntry.Item)
            {
                // First item of the combination picked
                m_SelectedSlot.SetSelectionLocked(true);
                m_CombiningSlot = m_SelectedSlot;
                EventSystem.current.SetSelectedGameObject(m_SelectedSlot.gameObject);
            }
        }


        // --------------------------------------------------------------------

        private void OnExamine()
        {
            ItemData item = m_SelectedSlot.InventoryEntry.Item;
            if (item == null)
                return;

            Hide();

            UIManager.Get<UIExamineItem>().Show(item);
        }

        // --------------------------------------------------------------------


        private void OnDrop()
        {
            ItemData item = m_SelectedSlot.InventoryEntry.Item;
            if (item == null)
                return;

            var gameMgr = GameManager.Instance;
            
            if (item.Flags.HasFlag(ItemFlags.CreatePickupOnDrop))
            {
                gameMgr.Player.GetComponentInChildren<PickupDropper>().Drop(m_SelectedSlot.InventoryEntry);
            }

            gameMgr.Inventory.Drop(m_SelectedSlot.InventoryEntry); 

            Hide();
        }

        // --------------------------------------------------------------------

        public void OnFilesCategory()
        {
            Hide();
            UIManager.Get<UIDocs>().Show();
        }

        // --------------------------------------------------------------------

        public void OnMapCategory()
        {
            Hide();
            UIManager.Get<UIMap>().Show();
        }


        // --------------------------------------------------------------------

        private void Combine()
        {
            InventoryEntry highlightedEntry = GameManager.Instance.Inventory.Combine(m_SelectedSlot.InventoryEntry, m_CombiningSlot.InventoryEntry);
            
            Fill();
            FillEquipped();

            if (highlightedEntry != null)
            {
                for (int i = 0; i < m_ItemSlots.Length; ++i)
                {
                    if (m_ItemSlots[i].InventoryEntry == highlightedEntry)
                    {
                        EventSystem.current.SetSelectedGameObject(m_ItemSlots[i].gameObject);
                    }
                }
            }

            m_CombiningSlot = null;
        }

        // --------------------------------------------------------------------

        private void Fill()
        {
            var inventory = GameManager.Instance.Inventory;
            var items = inventory.Items;
            int itemsCount = items.Length;

            m_Expanding = inventory.Expanded;
            inventory.Expanded = false;

            for (int i = 0; i < items.Length; ++i)
            {
                m_ItemSlots[i].gameObject.SetActive(i < itemsCount);

                if (i >= inventory.PreExpansionSize && m_Expanding)
                {
                    m_ItemSlots[i].GetComponent<Animator>().Play("Expand");
                }

                m_ItemSlots[i].Fill(items[i]);
            }

            for (int i = items.Length; i < m_ItemSlots.Length; ++i)
            {
                m_ItemSlots[i].gameObject.SetActive(false);
            }
        }

        // --------------------------------------------------------------------

        private void FillEquipped()
        {
            FillEquipped(EquipmentSlot.Primary, m_Equipped);
            FillEquipped(EquipmentSlot.Secondary, m_EquippedSecondary);
        }

        // --------------------------------------------------------------------

        private void FillEquipped(EquipmentSlot slot, UIInventoryItem uiItem)
        {
            InventoryEntry equippedPrim = GameManager.Instance.Inventory.GetEquipped(slot);
            if (equippedPrim != null)
                uiItem.Fill(equippedPrim);
            else
                uiItem.Fill(null);
        }
    }
}