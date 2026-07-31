using Survivor.Player;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Collections;
using UnityEngine;

namespace Survivor.Inventory
{
    public class InventorySystem : MonoBehaviour, ISaveable
    {
        //
        // Unity Hooks
        //

        [Header("Dependencies")]
        [SerializeField] private InputProvider m_InputProvider;

        private void Awake()
        {
            //
            // Setup the inventory load state
            //

            if(SaveSystem.TryFindSaveData(SaveKey, out InventorySavedData data))
            {
                m_ItemSlots = new List<ItemSlot>(data.Items);
            }
            SaveSystem.RegisterSaveable(this);
        }

        private void Update()
        {
            if(GameController.IsGameMode(GameController.GameMode.Inventory))
            {
                bool isClosingInventory = m_InputProvider.Always.IsInventoryToggled;
                if(isClosingInventory)
                {
                    GameController.PopGameMode();
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
                    m_ItemSlots.Add(new ItemSlot()
                    {
                        Amount = 1,
                        Data   = itemData,
                    });
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

        //
        // Saving Interface
        //

        [System.Serializable]
        private struct InventorySavedData
        {
            public List<ItemSlot> Items;
        }

        public int SaveKey => SaveSystem.StringKeyToIntKey("Inventory");

        public string SaveState()
        {
            string result = SaveSystem.AsSaveData(new InventorySavedData()
            {
                Items = m_ItemSlots,
            });

            return result;
        }
    }

    [System.Serializable]
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