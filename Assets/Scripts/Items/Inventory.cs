﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public Item[] inventory;

    [SerializeField]
    public int nbSlot = 6;

    public void Awake()
    {
        inventory = new Item[nbSlot];
    }
}