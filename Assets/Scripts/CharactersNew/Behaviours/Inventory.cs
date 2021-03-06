﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Behaviour
{
    [RequireComponent(typeof(PawnInstance))]
    public class Inventory : MonoBehaviour
    {
        [System.Serializable]
        public class InventoryData : ComponentData
        {
            [SerializeField]
            int nbSlot;

            public InventoryData(int _nbSlot = 0)
            {
                nbSlot = _nbSlot;
            }

            public int NbSlot
            {
                get
                {
                    return nbSlot;
                }

                set
                {
                    nbSlot = value;
                }
            }
        }

        PawnInstance instance;

        [SerializeField]
        InventoryData data;

        private ItemContainer[] items;

        [SerializeField]
        List<string> possibleItems;

        // UI
        private GameObject inventoryPanel;
        private GameObject selectedInventoryPanel;

        void Awake()
        {
            instance = GetComponent<PawnInstance>();
        }

        void OnDestroy()
        {
            Destroy(inventoryPanel);
            Destroy(selectedInventoryPanel);
        }

        public void Add(ItemContainer item)
        {
            ItemContainer[] temp = items;
            items = new ItemContainer[data.NbSlot];
            for (int i = 0; i < data.NbSlot; i++)
            {
                items[i] = temp[i];
            }
            items[data.NbSlot] = item;
            data.NbSlot++;
        }

        public void ComputeItems()
        {
            List<ItemContainer> tmpItems = new List<ItemContainer>();
            if (GetComponent<Prisoner>() == null)
                GetComponent<Interactable>().Interactions.Add(new Interaction(Trade), 0, "Trade", GameManager.Instance.SpriteUtils.spriteTrade);

            Item it = null;
            int computedNbSlots = 0;
            foreach (string _IdItem in possibleItems)
            {
                it = GameManager.Instance.ItemDataBase.getItemById(_IdItem);
                if (GetComponent<Monster>() != null && GetComponent<Monster>().GetMType != MonsterType.Common)
                {
                    tmpItems.Add(new ItemContainer(it, 1));
                    computedNbSlots++;
                }
                else if (Random.Range(1, 10) > it.Rarity)
                {
                    tmpItems.Add(new ItemContainer(it, 1));
                    computedNbSlots++;
                }
            }
            items = tmpItems.ToArray();
            data.NbSlot = computedNbSlots;
        }

        public void Trade(int _i = 0)
        {
            ShowInventoryPanel(true);
        }

        #region UI
        public void InitUI()
        {
            if (items == null)
                items = new ItemContainer[data.NbSlot];

            CreateInventoryUI();
            InitInventoryPanel();

            ShowInventoryPanel(false);

            if (instance != null)
            {
                if (!GetComponent<Interactable>().Interactions.listActionContainers.Exists(x => x.strName == "Trade") && GetComponent<Prisoner>() == null)
                    GetComponent<Interactable>().Interactions.Add(new Interaction(Trade), 0, "Trade", GameManager.Instance.SpriteUtils.spriteTrade);

                if (instance.GetComponent<Keeper>() != null || instance.GetComponent<Prisoner>() != null)
                {
                    CreateSelectedInventoryPanel();
                    InitSelectedInventoryPanel();
                }
            }

            UpdateInventories();
        }

        public void CreateInventoryUI()
        {
            inventoryPanel = Instantiate(GameManager.Instance.PrefabUIUtils.PrefabInventaireUI, GameManager.Instance.PrefabUIUtils.PrefabInventaireUI.transform.position, GameManager.Instance.PrefabUIUtils.PrefabInventaireUI.transform.rotation);
            inventoryPanel.transform.SetParent(GameManager.Instance.Ui.Panel_Inventories.transform, false);
            inventoryPanel.name = "Inventory_" + (GetComponent<LootInstance>() != null ? "Loot" : instance.Data.PawnName);
        }

        public void CreateSelectedInventoryPanel()
        {
            selectedInventoryPanel = Instantiate(GameManager.Instance.PrefabUIUtils.PrefabSelectedInventoryUIPanel, GameManager.Instance.PrefabUIUtils.PrefabSelectedInventoryUIPanel.transform.position, GameManager.Instance.PrefabUIUtils.PrefabSelectedInventoryUIPanel.transform.rotation);
            selectedInventoryPanel.GetComponent<InventoryOwner>().Owner = instance.gameObject;
            if (instance.GetComponent<Keeper>() != null)
            {
                selectedInventoryPanel.transform.SetParent(instance.GetComponent<Keeper>().SelectedPanelUI.transform, false);
                selectedInventoryPanel.name = "Inventory";
            }
            else
            {
                selectedInventoryPanel.transform.SetParent(GameManager.Instance.Ui.goSelectedKeeperPanel.transform, false);
                selectedInventoryPanel.gameObject.SetActive(false);
                Destroy(selectedInventoryPanel.transform.GetComponent<ContentSizeFitter>());
                selectedInventoryPanel.name = "Inventory_Ashley";
            }
            selectedInventoryPanel.transform.localScale = Vector3.one;
        }

        public void ShowInventoryPanel(bool isShow)
        {
            inventoryPanel.SetActive(isShow);
        }

        public void InitInventoryPanel()
        {
            GameObject owner = null;
            Sprite associatedSprite = null;
            int nbSlot = 0;

            if ( instance!= null && instance.GetComponent<PawnInstance>() != null)
            {
                PawnInstance pawnInstance = instance.GetComponent<PawnInstance>();
                associatedSprite = pawnInstance.Data.AssociatedSprite;
                name = pawnInstance.Data.PawnName;
                owner = pawnInstance.gameObject;
                nbSlot = data.NbSlot;
            }
            else if (GetComponent<LootInstance>() != null)
            {
                LootInstance lootInstance = GetComponent<LootInstance>();

                associatedSprite = GameManager.Instance.SpriteUtils.spriteLoot;
                inventoryPanel.transform.GetChild(1).gameObject.SetActive(false);
                owner = lootInstance.gameObject;
                nbSlot = data.NbSlot;
            }
            else
            {
                return;
            }

            inventoryPanel.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = associatedSprite;
            inventoryPanel.transform.GetChild(0).GetComponent<InventoryOwner>().Owner = owner;

            for (int i = 0; i < nbSlot; i++)
            {
                //Create Slots
                GameObject currentgoSlotPanel = Instantiate(GameManager.Instance.PrefabUIUtils.PrefabSlotUI, Vector3.zero, Quaternion.identity) as GameObject;
                currentgoSlotPanel.transform.SetParent(inventoryPanel.transform.GetChild(0).transform);

                currentgoSlotPanel.transform.localPosition = Vector3.zero;
                currentgoSlotPanel.transform.localScale = Vector3.one;
                currentgoSlotPanel.name = "Slot" + i;
            }
        }

        public void InitSelectedInventoryPanel()
        {
            PawnInstance pawnInstance = instance.GetComponent<PawnInstance>();
            Sprite associatedSprite = pawnInstance.Data.AssociatedSprite;
            GameObject owner = pawnInstance.gameObject;
            selectedInventoryPanel.GetComponent<InventoryOwner>().Owner = instance.gameObject;

            for (int i = 0; i < data.NbSlot; i++)
            {
                //Create Slots
                GameObject currentgoSlotPanel;
                if (GetComponent<Keeper>() != null)
                    currentgoSlotPanel = Instantiate(GameManager.Instance.PrefabUIUtils.PrefabSlotUI, Vector3.zero, Quaternion.identity) as GameObject;
                else
                {
                    currentgoSlotPanel = Instantiate(GameManager.Instance.PrefabUIUtils.PrefabSlotAshleyUI, Vector3.zero, Quaternion.identity) as GameObject;
                    Button butt = currentgoSlotPanel.GetComponentInChildren<Button>();
                    butt.onClick.AddListener(instance.GetComponent<Prisoner>().ProcessFeeding);
                }

                currentgoSlotPanel.transform.SetParent(selectedInventoryPanel.transform);

                currentgoSlotPanel.transform.localPosition = Vector3.zero;
                currentgoSlotPanel.transform.localScale = Vector3.one;
                currentgoSlotPanel.name = "Slot" + i;
            }

            selectedInventoryPanel.GetComponent<GridLayoutGroup>().constraintCount = data.NbSlot;
        }

        public void UpdateInventories()
        {
            UpdateInventoryPanel();
            if (instance != null && (instance.GetComponent<Keeper>() != null || GetComponent<Prisoner>() != null))
            {
                UpdateSelectedPanel();
            }
        }

        public void UpdateInventoryPanel()
        {
            GameObject owner = null;
            Sprite associatedSprite = null;
            string name = "";

            if (instance != null && instance.GetComponent<PawnInstance>() != null)
            {
                PawnInstance pawnInstance = instance.GetComponent<PawnInstance>();
                associatedSprite = pawnInstance.Data.AssociatedSprite;
                name = pawnInstance.Data.PawnName;
                owner = pawnInstance.gameObject;
            }
            else if (GetComponent<LootInstance>() != null)
            {
                LootInstance lootInstance = GetComponent<LootInstance>();

                associatedSprite = GameManager.Instance.SpriteUtils.spriteLoot;
                owner = lootInstance.gameObject;
                name = "Loot";
            }
            else
            {
                return;
            }

            for (int i = 0; i < items.Length; i++)
            {
                GameObject currentSlot = inventoryPanel.transform.GetChild(0).GetChild(i).gameObject;
                if (currentSlot.GetComponentInChildren<ItemInstance>() != null)
                {
                    Destroy(currentSlot.GetComponentInChildren<ItemInstance>().gameObject);
                }
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null && items[i].Item != null && items[i].Item.Id != null)
                {
        
                    GameObject currentSlot = inventoryPanel.transform.GetChild(0).GetChild(i).gameObject;
                    GameObject go = Instantiate(GameManager.Instance.PrefabUIUtils.PrefabItemUI);
                    go.transform.SetParent(currentSlot.transform);
                    go.GetComponent<ItemInstance>().ItemContainer = items[i];
                    go.name = items[i].ToString();

                    go.GetComponent<Image>().sprite = items[i].Item.InventorySprite;
                    go.transform.localScale = Vector3.one;

                    go.transform.position = currentSlot.transform.position;
                    go.transform.SetAsFirstSibling();

                    if (go.GetComponent<ItemInstance>().ItemContainer.Item.GetType() == typeof(Ressource))
                    {
                        go.transform.GetComponentInChildren<Text>().text = items[i].Quantity.ToString();
                    }
                    else
                    {
                        go.transform.GetComponentInChildren<Text>().text = "";
                    }
                }
            }
        }

        public void UpdateSelectedPanel()
        {
            int nbSlot = data.NbSlot;

            for (int i = 0; i < items.Length; i++)
            {
                GameObject currentSlot = (GetComponent<Prisoner>() == null) ? selectedInventoryPanel.transform.GetChild(i).gameObject : selectedInventoryPanel.transform.GetComponentInChildren<Slot>().gameObject;
                if (currentSlot.GetComponentInChildren<ItemInstance>() != null)
                {
                    Destroy(currentSlot.GetComponentInChildren<ItemInstance>().gameObject);
                }
            }

            for (int i = 0; i < nbSlot; i++)
            {
                GameObject currentSlot = (GetComponent<Prisoner>() == null) ? selectedInventoryPanel.transform.GetChild(i).gameObject : selectedInventoryPanel.transform.GetComponentInChildren<Slot>().gameObject;
                if (items != null && items.Length > 0 && i < items.Length && items[i] != null && items[i].Item != null && items[i].Item.Id != null)
                {
                    GameObject go = Instantiate(GameManager.Instance.PrefabUIUtils.PrefabItemUI);
                    go.transform.SetParent(currentSlot.transform);
                    go.GetComponent<ItemInstance>().ItemContainer = items[i];
                    go.name = items[i].Item.ItemName;

                    go.GetComponent<Image>().sprite = items[i].Item.InventorySprite;
                    go.transform.localScale = Vector3.one;

                    go.transform.position = currentSlot.transform.position;
                    go.transform.SetAsFirstSibling();

                    if (go.GetComponent<ItemInstance>().ItemContainer.Item.GetType() == typeof(Ressource))
                    {
                        go.transform.GetComponentInChildren<Text>().text = items[i].Quantity.ToString();
                    }
                    else
                    {
                        go.transform.GetComponentInChildren<Text>().text = "";
                    }
                }
            }

            if (GetComponent<Prisoner>() != null)
            {
                Button feedButt = GetComponent<Inventory>().SelectedInventoryPanel.GetComponentInChildren<Button>();
                feedButt.GetComponent<Image>().enabled = (GetComponent<HungerHandler>() != null && GetComponent<HungerHandler>().CurrentHunger != GetComponent<HungerHandler>().Data.MaxHunger && !IsEmpty() && GetComponent<HungerHandler>().CurrentHunger != 0);
            }

        }
        
        #endregion

        public bool IsEmpty()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] != null)
                {
                    return false;
                }
            }
            return true;
        }

        #region Accessors
        public ItemContainer[] Items
        {
            get
            {
                return items;
            }

            set
            {
                if (value != null)
                {
                    data.NbSlot = value.Length;
                }
                else
                {
                    data.NbSlot = 0;
                }

                items = value;
            }
        }

        public GameObject InventoryPanel
        {
            get
            {
                return inventoryPanel;
            }

            set
            {
                inventoryPanel = value;
            }
        }

        public InventoryData Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        public GameObject SelectedInventoryPanel
        {
            get
            {
                return selectedInventoryPanel;
            }

            set
            {
                selectedInventoryPanel = value;
            }
        }

        public List<string> PossibleItems
        {
            get
            {
                return possibleItems;
            }

            set
            {
                possibleItems = value;
            }
        }
        #endregion
    }
}