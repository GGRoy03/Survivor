using Survivor.Player;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Collections;
using UnityEngine;

namespace Survivor.Inventory
{
    public class InventorySystem : MonoBehaviour
    {
        //
        // Unity Hooks
        //

        [Header("Dependencies")]
        [SerializeField] private PlayerInputProvider m_InputProvider;

        private void Update()
        {
            if(GameController.Current == GameController.GameMode.Inventory)
            {
                bool isClosingInventory = m_InputProvider.Always.IsInventoryToggled;
                if(isClosingInventory)
                {
                    GameController.SetGameMode(GameController.GameMode.Gameplay);
                }
            }
        }

        //
        // Adding/Removing Items
        //

        private List<ItemSlot> m_ItemSlots = new();
        public int InventoryVersion {get; private set; }
        public ReadOnlyCollection<ItemSlot> Items => m_ItemSlots.AsReadOnly();

        private int FindItemSlot(ItemData itemData)
        {
            Debug.Assert(itemData != null);

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

            return itemSlotIdx;
        }

        public void PushItem(ItemData itemData)
        {
            if(itemData != null)
            {
                int itemSlotIdx = FindItemSlot(itemData);
                if(itemSlotIdx != -1)
                {     
                    m_ItemSlots[itemSlotIdx] = new ItemSlot()
                    {
                        Amount = m_ItemSlots[itemSlotIdx].Amount + 1,
                        Data   = itemData,
                    };
                }
                else
                {
                    m_ItemSlots[itemSlotIdx]= new ItemSlot()
                    {
                        Amount = 1,
                        Data   = itemData,
                    };
                }

                ++InventoryVersion;
            }
        }

        public void PopItem(ItemData itemData)
        {
            if(itemData != null)
            {
                int itemSlotIdx = FindItemSlot(itemData);
                if(itemSlotIdx != -1)
                {
                    var itemSlot = m_ItemSlots[itemSlotIdx];
                    if(itemSlot.Amount > 1)
                    {
                        m_ItemSlots[itemSlotIdx] = new ItemSlot()
                        {
                            Amount = itemSlot.Amount - 1,
                            Data   = itemSlot.Data,
                        };
                    }
                    else
                    {
                        m_ItemSlots.RemoveAtSwapBack(itemSlotIdx);
                    }
                }

                ++InventoryVersion;
            }
        }
    }

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
}