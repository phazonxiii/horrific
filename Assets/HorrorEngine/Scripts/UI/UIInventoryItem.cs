using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HorrorEngine
{
    public class UIInventoryItem : MonoBehaviour
    {
        [FormerlySerializedAs("Icon")]
        [SerializeField] private Image m_Icon;
        [FormerlySerializedAs("Combining")]
        [SerializeField] private GameObject m_SelectionLocked;
        [FormerlySerializedAs("Name")]
        [SerializeField] private TMPro.TextMeshProUGUI m_ItemName;
        [FormerlySerializedAs("Count")]
        [SerializeField] private TMPro.TextMeshProUGUI m_Count;
        [SerializeField] private LocalizableText m_NoItemName;
        [SerializeField] private Color m_NormalAmountColor = Color.green;
        [SerializeField] private Color m_EmptyAmountColor = Color.red;
        [SerializeField] private Color m_MaxStackAmountColor = Color.blue;
        [SerializeField] private GameObject m_Status;
        [SerializeField] private Image m_StatusFill;
        [SerializeField] private Gradient m_StatusFillColorOverValue;

        public InventoryEntry InventoryEntry { get; private set; }

        public ItemData Data => InventoryEntry.Item;

        public void Fill(InventoryEntry entry)
        {
            InventoryEntry = entry;
           
            float amount = 0;
            if (entry != null && entry.Item)
            {
                amount = entry.Item.Flags.HasFlag(ItemFlags.Stackable) ? entry.Count : entry.SecondaryCount;
            }

            FillItem(entry != null ? entry.Item : null, amount, entry != null ? entry.Status : 0f);
           
            if (m_SelectionLocked)
                m_SelectionLocked.gameObject.SetActive(false);
        }

        public void FillItem(ItemData data, float amount = 0, float status = 0)
        {
            if (data)
            {
                m_Icon.sprite = data.Image;
                m_Icon.gameObject.SetActive(true);

                m_Count.text = Convert.ToInt32(amount).ToString();
                bool isReloadable = data as ReloadableWeaponData;
                m_Count.gameObject.SetActive(data.Flags.HasFlag(ItemFlags.Stackable) || amount > 0 || isReloadable);
                if (amount == 0)
                    m_Count.color = m_EmptyAmountColor;
                else if (data.MaxStackSize > 0 && amount >= data.MaxStackSize)
                    m_Count.color = m_MaxStackAmountColor;
                else
                    m_Count.color = m_NormalAmountColor;

                m_Status.gameObject.SetActive(data.Flags.HasFlag(ItemFlags.Depletable));
                m_StatusFill.fillAmount = status;
                m_StatusFill.color = m_StatusFillColorOverValue.Evaluate(status);

                if (m_ItemName)
                {
                    m_ItemName.text = data.Name;
                }
            }
            else
            {
                m_Icon.gameObject.SetActive(false);
                m_Count.gameObject.SetActive(false);
                m_Status.SetActive(false);

                if (m_ItemName)
                {
                    m_ItemName.text = m_NoItemName;
                }

                if (m_SelectionLocked)
                    m_SelectionLocked.gameObject.SetActive(false);
            }
        }


        public void SetSelectionLocked(bool islocked)
        {
            if (m_SelectionLocked)
                m_SelectionLocked.gameObject.SetActive(islocked);
        }
    }
}