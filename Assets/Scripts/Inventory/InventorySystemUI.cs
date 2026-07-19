using System.Collections.Generic;
using TMPro;

using UnityEngine;

namespace Survivor.Inventory
{
    public class InventorySystemUI : MonoBehaviour
    {
        [Header("Inventory Container")]
        [SerializeField] private RectTransform m_InventoryContainer;
        [SerializeField] private GameObject    m_ItemSlotPrefab;

        [Header("Item Description Window")]
        [SerializeField] private float           m_ItemInformationWindowOffsetInY;
        [SerializeField] private RectTransform   m_ItemInformationWindow;
        [SerializeField] private TextMeshProUGUI m_ItemNameText;
        [SerializeField] private TextMeshProUGUI m_ItemHealthText;
        [SerializeField] private TextMeshProUGUI m_ItemHungerText;
        [SerializeField] private TextMeshProUGUI m_ItemStaminaText;

        private static InventorySystemUI m_Instance;
        public static InventorySystemUI Instance => m_Instance;

        //
        // Unity Hooks
        //

        private void Awake()
        {
            if(m_Instance == null)
            {
                m_Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            m_InventoryContainer.gameObject.SetActive(false);

            SetHoveredItem(null);
        }

        private void Update()
        {
            if(m_ItemInformationWindow != null && m_ItemInformationWindow.gameObject.activeSelf)
            {
                m_ItemInformationWindow.position = Input.mousePosition + (m_ItemInformationWindowOffsetInY * Vector3.up);
            }
        }

        //
        // UI Hooks
        //

        public void OnUpdate(List<ItemSlot> itemSlots)
        {
            //
            // NOTE:
            // This whole thing is written as if inventory changes were very rare and
            // the inventory was relatively small.
            //

            if(itemSlots != null)
            {
                //
                // Destroy every single item in the UI.
                //
                // NOTE:
                // Extemely lazy way to update the UI and probably terrible for big inventories,
                // but it's probably fine for this case.
                //

                foreach (Transform transform in m_InventoryContainer.transform)
                {
                    Destroy(transform.gameObject);
                }

                //
                // Completely recontruct the UI from scracth using the updated list.
                //

                foreach (var item in itemSlots)
                {
                    if(item.Data != null)
                    {
                        var itemInstance = Instantiate(m_ItemSlotPrefab, m_InventoryContainer);
                        if(itemInstance && itemInstance.TryGetComponent<InventoryItem>(out var itemSlot))
                        {
                            itemSlot.BindData(item);
                        }
                    }
                }
            }
        }

        public void SetVisibility(bool value)
        {
            if(m_InventoryContainer != null)
            {
                m_InventoryContainer.gameObject.SetActive(value);
            }
        }

        public void SetHoveredItem(ItemData itemData)
        {
            if(m_ItemInformationWindow != null)
            {
                if (itemData != null)
                {
                    m_ItemInformationWindow.gameObject.SetActive(true);

                    m_ItemNameText.text    = itemData.Name;
                    m_ItemHealthText.text  = "Health: "  + itemData.HealthDelta.ToString();
                    m_ItemHungerText.text  = "Hunger: "  + itemData.HungerDelta.ToString();
                    m_ItemStaminaText.text = "Stamina: " + itemData.StaminaDelta.ToString();
                }
                else
                {
                    m_ItemInformationWindow.gameObject.SetActive(false);
                }
            }
        }
    }
}