using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HorrorEngine
{
    [Serializable]
    public class ShopEntryCostFormat
    {
        public GameAttribute Value;
        public LocalizableText Format;
    }

    public enum ShopCantPurchaseReason
    {
        None,
        CantPay,
        InsufficientSpace
    }

    public class UIShopEntryItem : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_CountText;
        [SerializeField] private TextMeshProUGUI m_PriceText;
        [SerializeField] private TextMeshProUGUI m_ExtraMsgText;
        [SerializeField] private ShopEntryCostFormat[] m_CostFormats;
        [SerializeField] private LocalizableText m_CantPayMessage;
        [SerializeField] private LocalizableText m_InsufficientSpaceMessage;

        public UnityEvent OnPurchaseable;
        public UnityEvent OnNonPurchaseable;

        public ShopContentEntry Entry { get; private set; }

        // --------------------------------------------------------------------

        public void Fill(ShopContentEntry entry)
        {
            Entry = entry;
            m_Icon.sprite = entry.Item.Item.Image;
            m_NameText.text = entry.Item.Item.Name;
            m_PriceText.text = GetFormattedCost();
            m_CountText.text = entry.Item.Count.ToString();
            m_CountText.gameObject.SetActive(entry.Item.Count > 1);

            if (CanBePurchased(out ShopCantPurchaseReason reason))
                OnPurchaseable?.Invoke();
            else
                OnNonPurchaseable?.Invoke();

            switch (reason)
            {
                case ShopCantPurchaseReason.CantPay:
                    m_ExtraMsgText.text = m_CantPayMessage;
                    break;
                case ShopCantPurchaseReason.InsufficientSpace:
                    m_ExtraMsgText.text = m_InsufficientSpaceMessage;
                    break;
                case ShopCantPurchaseReason.None:
                    m_ExtraMsgText.text = "";
                    break;
            }
        }

        // --------------------------------------------------------------------

        public string GetFormattedCost()
        {
            string price = "";
            foreach (var cost in Entry.Cost)
            {
                price += GetFormattedCost(cost.Attribute, cost.Amount) + " ";
            }
            
            return price;
        }

        // --------------------------------------------------------------------

        public string GetFormattedCost(GameAttribute val, int amount)
        {
            if (m_CostFormats == null || m_CostFormats.Length == 0)
                return amount.ToString();

            foreach (var entry in m_CostFormats)
            {
                if (entry.Value == val)
                {
                    return string.Format(entry.Format, amount);
                }
            }
            
            return amount.ToString();
        }

        // --------------------------------------------------------------------

        public bool CanBePurchased(out ShopCantPurchaseReason reason)
        {
            reason = ShopCantPurchaseReason.None;

            if (!GameManager.Instance.Inventory.CanAdd(Entry.Item))
            {
                reason = ShopCantPurchaseReason.InsufficientSpace;
                return false;
            }

            foreach (var cost in Entry.Cost)
            {
                if (GameAttributeUtils.GetAsFloat(cost.Attribute) < cost.Amount)
                {
                    reason = ShopCantPurchaseReason.CantPay;
                    return false;
                }
            }


            return true;
        }
    }
}