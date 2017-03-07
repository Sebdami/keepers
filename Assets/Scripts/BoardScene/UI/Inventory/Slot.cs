﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {

    public GameObject currentItem
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    public bool hasAlreadyAnItem
    {
        get
        {
            if (transform.childCount > 0) return true;
            return false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<DragHandler>() != null)
        {
            // Get the original parent
            Transform aux = eventData.pointerDrag.GetComponent<DragHandler>().startParent;

            //Ou on est ?
            GameObject inventaireDequi = aux.parent.parent.gameObject;
            GameObject inventaireversqui = transform.parent.parent.gameObject;

            KeeperInstance dequi = null;
            KeeperInstance versqui = null;
            // Recuperation du bon inventaire
            if (inventaireDequi == GameManager.Instance.Ui.goInventory.transform.parent.gameObject)
            {
                dequi = GameManager.Instance.ListOfSelectedKeepers[0];
            }
            else
            {
                for (int i = 0; i < GameManager.Instance.AllKeepersList.Count; i++)
                {
                    if (i == inventaireDequi.transform.GetSiblingIndex())
                    {
                        dequi = GameManager.Instance.AllKeepersList[i];
                    }
                }
            }
            if (inventaireversqui == GameManager.Instance.Ui.goInventory.transform.parent.gameObject)
            {
                versqui = GameManager.Instance.ListOfSelectedKeepers[0];
            } else
            {
                for (int i = 0; i< GameManager.Instance.AllKeepersList.Count; i++)
                {
                    if( i == inventaireversqui.transform.GetSiblingIndex())
                    {
                        versqui = GameManager.Instance.AllKeepersList[i];
                    }
                }
            }

            if (dequi == null || versqui == null)
                Debug.Log("gros bug");

            //Si les inventaires sont differents
            if (dequi != versqui)
            {
                if (hasAlreadyAnItem)
                {
                    Item itemDragged = eventData.pointerDrag.GetComponent<ItemInstance>().item;
                    Item itemOn = currentItem.GetComponent<ItemInstance>().item;

                    if ((itemOn.GetType() == itemDragged.GetType()) && itemOn.GetType() == typeof(Consummable) && itemOn.sprite.name == itemDragged.sprite.name)
                    {
                        int quantityLeft = ItemManager.MergeStackables2(((Consummable)currentItem.GetComponent<ItemInstance>().item), ((Consummable)eventData.pointerDrag.GetComponent<ItemInstance>().item));
                        if (quantityLeft > 0)
                        {
                            ((Consummable)eventData.pointerDrag.GetComponent<ItemInstance>().item).quantite = quantityLeft;
                        }
                        else
                        {
                            ItemManager.RemoveItem(dequi, eventData.pointerDrag.GetComponent<ItemInstance>().item);
                        }
                    }
                    else
                    {
                        // Swap
                        ItemManager.SwapItemBeetweenInventories(dequi, aux.GetSiblingIndex(), versqui, transform.GetSiblingIndex());

                    }
                }
                else
                {
                    //Move the item to the slot
                    //eventData.pointerDrag.transform.SetParent(transform);

                    ItemManager.RemoveItem(dequi, eventData.pointerDrag.GetComponent<ItemInstance>().item);
                    ItemManager.AddItem(versqui, eventData.pointerDrag.GetComponent<ItemInstance>().item, false);
                   
                    ItemManager.MoveItemToSlot(
                         versqui,
                         eventData.pointerDrag.GetComponent<ItemInstance>().item,
                         transform.GetSiblingIndex()
                     );
                }
            }
            // Si l'inventaire est le même
            else
            {
                if (hasAlreadyAnItem)
                {
                    Item itemDragged = eventData.pointerDrag.GetComponent<ItemInstance>().item;
                    Item itemOn = currentItem.GetComponent<ItemInstance>().item;

                    if ((itemOn.GetType() == itemDragged.GetType()) && itemOn.GetType() == typeof(Consummable) && itemOn.sprite.name == itemDragged.sprite.name)
                    {
                        Consummable consummableDragged = (Consummable)itemDragged;
                        Consummable consummableOn = (Consummable)itemDragged;
                        int quantityLeft = ItemManager.MergeStackables2(consummableOn, consummableDragged);
                        if (quantityLeft > 0)
                        {
                            ((Consummable)eventData.pointerDrag.GetComponent<ItemInstance>().item).quantite = quantityLeft;
                        }
                        else
                        {
                            ItemManager.RemoveItem(dequi, eventData.pointerDrag.GetComponent<ItemInstance>().item);

                        }
                    }
                    else
                    {
                        // swap dequi = versqui
                        ItemManager.SwapItemInSameInventory(dequi, aux.GetSiblingIndex(), transform.GetSiblingIndex());
                    }
                }
                else
                {
                    //Move the item to the slot
                    ItemManager.MoveItemToSlot(
                          versqui,
                          eventData.pointerDrag.GetComponent<ItemInstance>().item,
                          transform.GetSiblingIndex()
                    );

                }
            }

            Destroy(eventData.pointerDrag.gameObject);
            GameManager.Instance.Ui.UpdateKeeperInventoryPanel();
            GameManager.Instance.SelectedKeeperNeedUpdate = true;

        }
        else if (eventData.pointerDrag.GetComponent<DragHandlerInventoryPanel>() != null)
        {
            Debug.Log("A panel was drop in a slot");
        }
    }

}
