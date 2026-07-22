using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Inventory
{
    public struct ItemSlot
    {
        public ItemData Data;
        public int Amount;

        public ItemSlot(ItemData itemData)
        {
            Data   = itemData;
            Amount = 1;
        }
    }

    public class InventorySystem
    {
        private static InventorySystem m_Instance;
        private List<ItemSlot> m_ItemSlots = new();

        public static InventorySystem Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new InventorySystem();
                }

                return m_Instance;
            }
        }

        private void PushItemToSlot(ItemData itemData)
        {
            Debug.Assert(itemData);

            int itemSlotIdx = -1;
            for (int slotIdx = 0; slotIdx < m_ItemSlots.Count; ++slotIdx)
            {
                var itemSlot = m_ItemSlots[slotIdx];
                if (itemSlot.Data == itemData)
                {
                    itemSlotIdx = slotIdx;
                    break;
                }
            }

            if (itemSlotIdx == -1)
            {
                m_ItemSlots.Add(new ItemSlot(itemData));
            }
            else
            {
                var itemAtSlot = m_ItemSlots[itemSlotIdx];
                itemAtSlot.Amount += 1;

                m_ItemSlots[itemSlotIdx] = itemAtSlot;
            }  
        }

        public void AddItem(ItemData itemData)
        {
            if (itemData != null)
            {
                PushItemToSlot(itemData);
                InventorySystemUI.Instance.OnUpdate(m_ItemSlots);
            }
        }
    }
}