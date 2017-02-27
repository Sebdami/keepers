﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KeeperInstance : MonoBehaviour {

    [Header("Keeper Info")]
    [SerializeField]
    private Keeper keeper = null;
    private bool isSelected = false;

    [SerializeField]
    private GameObject goSelectionAura;

    // Used only in menu. Handles selection in main menu.
    [SerializeField]
    private bool isSelectedInMenu = false;
    public MeshRenderer meshToHighlight;


    // Update variables
    NavMeshAgent agent;

    Vector3 v3AgentDirectionTemp;

    public bool isEscortAvailable;

    // Rotations
    float fLerpRotation = 0.666f;
    Quaternion quatTargetRotation;
    Quaternion quatPreviousRotation;
    bool bIsRotating = false;
    [SerializeField]
    float fRotateSpeed = 1.0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fRotateSpeed = 5.0f;
        isEscortAvailable = true;
    }

    private void Update()
    {
        Vector3 currentPosition;
        if (agent.isOnOffMeshLink)
        {
            currentPosition = transform.position;
            if (Input.GetKeyDown(KeyCode.A))
            {
                agent.CompleteOffMeshLink();
                TileManager.Instance.MoveKeeper(this, TileManager.Instance.GetTileFromKeeper[this], Direction.North_East);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {

                agent.CompleteOffMeshLink();
                agent.Warp(currentPosition);
            }
        }

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

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<MonsterInstance>())
        {
            agent.Stop();
            agent.ResetPath();
            BattleHandler.LaunchBattle(TileManager.Instance.GetTileFromKeeper[this]);
            agent.Resume();
        }

        InteractionImplementer ii = new InteractionImplementer();

        Direction eTrigger = Direction.None;

        string strTag = col.gameObject.tag;

        switch (strTag)
        {
            case "NorthTrigger":
                eTrigger = Direction.North;
                break;
            case "NorthEastTrigger":
                eTrigger = Direction.North_East;
                break;
            case "SouthEastTrigger":
                eTrigger = Direction.South_East;
                break;
            case "SouthTrigger":
                eTrigger = Direction.South;
                break;
            case "SouthWestTrigger":
                eTrigger = Direction.South_West;
                break;
            case "NorthWestTrigger":
                eTrigger = Direction.North_West;
                break;
            default:
                eTrigger = Direction.None;
                break;
        }

        
        if (eTrigger != Direction.None && col.gameObject.GetComponentInParent<Tile>().Neighbors[(int)eTrigger] != null)
        {
            ii.Add(new Interaction(Move), "Move", null, true, (int)eTrigger);
            IngameUI ui = GameObject.Find("IngameUI").GetComponent<IngameUI>();
            ui.UpdateActionPanelUIQ(ii);

            if(col.gameObject.GetComponentInParent<Tile>().Neighbors[(int)eTrigger].State == TileState.Greyed)
            {
                ii.Add(new Interaction(Explore), "Explore", null, true, (int)eTrigger);
                ui.UpdateActionPanelUIQ(ii);
            }
        }

        /*   TileManager.Instance.MoveKeeper(this, TileManager.Instance.GetTileFromKeeper[this], Direction.North);
        }
        else if (col.gameObject.CompareTag("SouthTrigger"))
        {
            iIdTrigger = 0;
            TileManager.Instance.MoveKeeper(this, TileManager.Instance.GetTileFromKeeper[this], Direction.South);
        }
        else if (col.gameObject.CompareTag("NorthEastTrigger"))
        {
            iIdTrigger = 0;
            TileManager.Instance.MoveKeeper(this, TileManager.Instance.GetTileFromKeeper[this], Direction.North_East);
        }
        else if (col.gameObject.CompareTag("NorthWestTrigger"))
        {
            iIdTrigger = 0;
            TileManager.Instance.MoveKeeper(this, TileManager.Instance.GetTileFromKeeper[this], Direction.North_West);
        }
        else if (col.gameObject.CompareTag("SouthEastTrigger"))
        {
            TileManager.Instance.MoveKeeper(this, TileManager.Instance.GetTileFromKeeper[this], Direction.South_East);
        }
        else if (col.gameObject.CompareTag("SouthWestTrigger"))
        {
            TileManager.Instance.MoveKeeper(this, TileManager.Instance.GetTileFromKeeper[this], Direction.South_West);
        }*/
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

    public void TriggerRotation(Vector3 v3Direction)
    {
        agent.angularSpeed = 0.0f;

        quatPreviousRotation = transform.rotation;

        quatTargetRotation.SetLookRotation(v3Direction - transform.position);

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

    void Move(int _i)
    {
        
        TileManager.Instance.MoveKeeper(this, TileManager.Instance.GetTileFromKeeper[this], (Direction)_i);
        for(int i =0; i< keeper.GoListCharacterFollowing.Count; i++)
        {

        }
    }

    void Explore(int _i)
    {
        TileManager.Instance.GetTileFromKeeper[this].Neighbors[_i].State = TileState.Discovered;
    }
}
