using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Survivor.Inventory
{
    public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image           m_ItemContainer;
        [SerializeField] private TextMeshProUGUI m_QuantityText;

        private ItemData          m_ItemData;
        private InventorySystemUI m_InventoryUI;
        private InventorySystem   m_Inventory;

        //
        // UI Event Hooks
        //

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(m_InventoryUI != null)
            {
                m_InventoryUI.SetHoveredItem(m_ItemData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(m_InventoryUI != null)
            {
                m_InventoryUI.SetHoveredItem(null);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_ItemData != null)
            {
                m_Inventory.PopItem(m_ItemData);
            }
        }

        public void BindData(ItemSlot itemSlot, InventorySystem inventory, InventorySystemUI inventoryUI)
        {
            m_ItemContainer.sprite = itemSlot.Data.Icon;
            m_QuantityText.text    = itemSlot.Amount.ToString();
            m_ItemData             = itemSlot.Data;
            m_Inventory            = inventory;
            m_InventoryUI          = inventoryUI;
        }
    }
}