using System.Collections.Generic;
using TMPro;

using UnityEngine;

namespace Survivor.Inventory
{
    public class InventorySystemUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private InventorySystem m_InventorySystem;

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

        //
        // Unity Hooks
        //

        private int m_LastInventoryVersion = -1;

        private void Awake()
        {
            SetVisibility(false);
            SetHoveredItem(null);
        }

        private void Update()
        {
            if(GameController.IsGameMode(GameController.GameMode.Inventory))
            {
                m_InventoryContainer.gameObject.SetActive(true);

                //
                // Update the tooltip position
                //

                if(m_ItemInformationWindow != null && m_ItemInformationWindow.gameObject.activeSelf)
                {
                    m_ItemInformationWindow.position = Input.mousePosition + (m_ItemInformationWindowOffsetInY * Vector3.up);
                }

                //
                // Check if the inventory system has changed its internal state.
                //

                if(m_InventorySystem != null)
                {
                    int currentVersion = m_InventorySystem.InventoryVersion;
                    if(m_LastInventoryVersion != currentVersion)
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
                        // Completely recontruct the UI from scratch using the updated list.
                        //

                        var items = m_InventorySystem.Items;
                        foreach(var item in items)
                        {
                            //
                            // NOTE:
                            // Is this check really needed?
                            //

                            if(item.Data != null)
                            {
                                var itemInstance = Instantiate(m_ItemSlotPrefab, m_InventoryContainer);
                                if(itemInstance && itemInstance.TryGetComponent<InventoryItem>(out var itemSlot))
                                {
                                    itemSlot.BindData(item, m_InventorySystem, this);
                                }
                            }
                        }

                        m_LastInventoryVersion = currentVersion;
                    }
                }
            }
            else
            {
                m_InventoryContainer.gameObject.SetActive(false);
            }
        }

        //
        // UI Hooks
        //

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