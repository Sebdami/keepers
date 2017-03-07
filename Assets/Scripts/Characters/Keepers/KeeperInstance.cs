﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KeeperInstance : MonoBehaviour, ITradable {

    [Header("Keeper Info")]
    [SerializeField]
    private Keeper keeper = null;
    private bool isSelected = false;

    private bool isStarving = false;
    private bool isMentalHealthLow = false;

    [SerializeField]
    private short currentHunger = 0;

    [SerializeField]
    private short currentMentalHealth;

    [SerializeField]
    private int currentHp;

    [SerializeField]
    private int currentMp;

    [SerializeField]
    private short actionPoints = 3;

    [SerializeField]
    private GameObject goSelectionAura;

    // Used only in menu. Handles selection in main menu.
    [SerializeField]
    private bool isSelectedInMenu = false;
    public MeshRenderer meshToHighlight;

    private bool isAlive = true;
    // Inventory
    private Item[] inventory;
    private Item[] equipment;

    // Update variables
    NavMeshAgent agent;

    Vector3 v3AgentDirectionTemp;

    private InteractionImplementer interactionImplementer;
    public bool isEscortAvailable = true;

    // Rotations
    float fLerpRotation = 0.666f;
    Quaternion quatTargetRotation;
    Quaternion quatPreviousRotation;
    bool bIsRotating = false;
    [SerializeField]
    float fRotateSpeed = 1.0f;


    public short CurrentHunger
    {
        get { return currentHunger; }
        set
        {
            currentHunger = value;
            if (currentHunger > keeper.MaxHunger)
            {
                currentHunger = keeper.MaxHunger;
                isStarving = true;
            }
            else if (currentHunger < 0)
            {
                currentHunger = 0;
                isStarving = false;
            }
            else
            {
                isStarving = false;
            }

        }
    }

    public int CurrentHp
    {
        get { return currentHp; }
        set
        {
            currentHp = value;
            if (currentHp > keeper.MaxHp)
            {
                currentHp = keeper.MaxHp;
                isAlive = true;
            }
            else if (currentHp < 0)
            {
                currentHp = 0;
                
                isAlive = false;
                Die();
            }
            else
            {
                isAlive = true;
            }

        }
    }

    public int CurrentMp
    {
        get { return currentMp; }
        set
        {
            currentMp = value;
            if (currentMp > keeper.MaxMp)
            {
                currentMp = keeper.MaxMp;
            }
            else if (currentMp < 0)
            {
                currentMp = 0;
            }

        }
    }

    void Die()
    {
        Debug.Log("Blaeuurgh... *dead*");
        GameManager.Instance.CheckGameOver();
    }

    public short CurrentMentalHealth
    {
        get { return currentMentalHealth; }
        set
        {
            currentMentalHealth = value;
            if (currentMentalHealth < 0)
            {
                currentMentalHealth = 0;
                isMentalHealthLow = true;
            }
            else if (currentMentalHealth > keeper.MaxMentalHealth)
            {
                currentMentalHealth = keeper.MaxMentalHealth;
                isMentalHealthLow = false;
            }
            else
            {
                isMentalHealthLow = false;
            }
        }
    }

    public short ActionPoints
    {
        get
        {
            return actionPoints;
        }

        set
        {

            if (value < actionPoints) GameManager.Instance.Ui.DecreaseActionTextAnimation(actionPoints - value);
            actionPoints = value;
            GameManager.Instance.Ui.UpdateActionText();
            if (actionPoints > keeper.MaxActionPoints)
                actionPoints = keeper.MaxActionPoints;
            if (actionPoints < 0)
                actionPoints = 0;
        }
    }

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }

        set
        {
            isAlive = value;
        }
    }


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fRotateSpeed = 5.0f;
        inventory = new Item[4];
        equipment = new Item[3];
        isEscortAvailable = true;
        InteractionImplementer = new InteractionImplementer();
        InteractionImplementer.Add(new Interaction(Trade), "Trade", GameManager.Instance.Ui.spriteTrade);
        currentHp = keeper.MaxHp;
        currentHunger = keeper.MaxHunger;
        currentMentalHealth = keeper.MaxMentalHealth;
        actionPoints = keeper.MaxActionPoints;
        currentMp = keeper.MaxMp;
    }

    private void Update()
    {
        GameObject goDestinationTemp = gameObject;
        for (int i = 0; i < keeper.GoListCharacterFollowing.Count; i++)
        {
            keeper.GoListCharacterFollowing[i].GetComponent<NavMeshAgent>().destination = goDestinationTemp.transform.position;
            goDestinationTemp = keeper.GoListCharacterFollowing[i];
        }

        if (bIsRotating)
        {
            Rotate();
        }
    }

    private void ToggleHighlightOnMesh(bool isSelected)
    {
        if (meshToHighlight != null)
        {
            if (isSelected)
            {
                meshToHighlight.material.shader = Shader.Find("Outlined/Silhouetted Diffuse");
                meshToHighlight.material.SetColor("_OutlineColor", Color.blue);
            }
            else
            {
                meshToHighlight.material.shader = Shader.Find("Diffuse");
            }
        }
    }

    public KeeperInstance(KeeperInstance from)
    {
        keeper = from.keeper;
        goSelectionAura = from.goSelectionAura;

    }

    /* ------------------------------------------------------------------------------------ */
    /* ------------------------------------- Accessors ------------------------------------ */
    /* ------------------------------------------------------------------------------------ */

    public bool IsSelected
    {
        get
        {
            return isSelected;
        }

        set
        {
            isSelected = value;
            GoSelectionAura.SetActive(value);
        }
    }



    public bool IsSelectedInMenu
    {
        get
        {
            return isSelectedInMenu;
        }

        set
        {
            isSelectedInMenu = value;
            ToggleHighlightOnMesh(isSelectedInMenu);
        }
    }

    public Keeper Keeper
    {
        get
        {
            return keeper;
        }

        set
        {
            keeper = value;
        }
    }

    public GameObject GoSelectionAura
    {
        get
        {
            return goSelectionAura;
        }

        set
        {
            goSelectionAura = value;
        }
    }

    public Item[] Inventory
    {
        get
        {
            return inventory;
        }

        set
        {
            inventory = value;
        }
    }

    public Item[] Equipment
    {
        get
        {
            return equipment;
        }

        set
        {
            equipment = value;
        }
    }

    public InteractionImplementer InteractionImplementer
    {
        get
        {
            return interactionImplementer;
        }

        set
        {
            interactionImplementer = value;
        }
    }

    public void TriggerRotation(Vector3 v3Direction)
    {
        agent.angularSpeed = 0.0f;

        quatPreviousRotation = transform.rotation;

        Vector3 v3PosTemp = transform.position;
        v3PosTemp.y = 0;
        v3Direction.y = 0.0f;

        quatTargetRotation.SetLookRotation(v3Direction - v3PosTemp);

        bIsRotating = true;

        agent.enabled = false;

        v3AgentDirectionTemp = v3Direction;

        fLerpRotation = 0.0f;
    }

    void Rotate()
    {
        if(fLerpRotation >= 1.0f)
        {
            transform.rotation = quatTargetRotation;
            bIsRotating = false;
            agent.enabled = true;
            fLerpRotation = 0.0f;

            agent.destination = v3AgentDirectionTemp;
            agent.angularSpeed = 100.0f;
        }
        else
        {
            fLerpRotation += fRotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(quatPreviousRotation, quatTargetRotation, fLerpRotation);
        }
    }


    public void Trade(int _i = 0)
    {
        GameManager.Instance.Ui.ShowInventoryPanels();
    }
}
