using Survivor.Event;
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

        private ItemSlot m_ItemSlot;

        public void OnPointerEnter(PointerEventData eventData)
        {
            InventorySystemUI.Instance.SetHoveredItem(m_ItemSlot.Data);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventorySystemUI.Instance.SetHoveredItem(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_ItemSlot.Data != null)
            {
                EventManager.Instance.PushEvent(new EventItemConsumed
                {
                    HealthDelta  = m_ItemSlot.Data.HealthDelta,
                    StaminaDelta = m_ItemSlot.Data.StaminaDelta,
                    HungerDelta  = m_ItemSlot.Data.HungerDelta,
                });

                m_ItemSlot.Amount -= 1;
                BindData(m_ItemSlot);
            }
        }


        public void BindData(ItemSlot itemSlot)
        {
            if(itemSlot.Data != null && itemSlot.Amount > 0)
            {
                m_ItemContainer.sprite = itemSlot.Data.Icon;
                m_QuantityText.text    = itemSlot.Amount.ToString();

                m_ItemSlot = itemSlot;
            }
            else
            {
                InventorySystemUI.Instance.SetHoveredItem(null);
                Destroy(gameObject);
            }
        }
    }

    public struct EventItemConsumed
    {
        public int HealthDelta;
        public int StaminaDelta;
        public int HungerDelta;
    }
}