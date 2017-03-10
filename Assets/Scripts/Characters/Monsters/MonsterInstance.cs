﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInstance : MonoBehaviour {
    [Header("Monster Info")]
    [SerializeField]
    private Monster monster = null;
    public ParticleSystem deathParticles = null;
    [SerializeField]
    private int currentHp;

    [SerializeField]
    private int currentMp;

    public Monster Monster
    {
        get
        {
            return monster;
        }

        set
        {
            monster = value;
        }
    }

    public Sprite spriteToCopy;

    [SerializeField]
    private List<Item> possibleDrops;

    public List<Item> PossibleDrops
    {
        get
        {
            return possibleDrops;
        }

        set
        {
            possibleDrops = value;
        }
    }
    
    public void Awake()
    {
        possibleDrops = new List<Item>();
        Consummable item1 = new Consummable();
        item1.value = 1;
        item1.sprite = spriteToCopy;
        item1.quantite = 1;
        item1.rarity = 1;
        possibleDrops.Add(item1);
    }

    public int CurrentHp
    {
        get { return currentHp; }
        set
        {
            currentHp = value;
            if (currentHp > monster.MaxHp)
            {
                currentHp = monster.MaxHp;
            }
            else if (currentHp < 0)
            {
                currentHp = 0;
            }
        }
    }

    public int CurrentMp
    {
        get { return currentMp; }
        set
        {
            currentMp = value;
            if (currentMp > monster.MaxMp)
            {
                currentMp = monster.MaxMp;
            }
            else if (currentMp < 0)
            {
                currentMp = 0;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<BoxCollider>().enabled = false;
        Invoke("ReactivateTrigger", 1.0f);
    }

    void Update () {
		
	}

    private void ReactivateTrigger()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
}
