﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    private InteractionImplementer interactions;
    private Transform feedback;

    void Awake()
    {
        interactions = new InteractionImplementer();

        foreach (Transform child in transform)
        {
            if (child.CompareTag("FeedbackTransform"))
            {
                feedback = child;
                break;
            }
        }
    }

    void Start()
    {
        if (feedback == null)
        {
            // Normal pour les tiles triggers de ne pas avoir de feedback puisqu'il sera sur le perso ? 
            if (GetComponent<TileTrigger>() != null) return;
            string msgDebug = "No feedback transform for interactions ";

            if (GetComponent<PawnInstance>() != null)
                msgDebug += GetComponent<PawnInstance>().Data.PawnName;
            else if (GetComponent<LootInstance>() != null)
                msgDebug += GetComponent<LootInstance>().name;
            else if (GetComponent<ItemInstance>() != null)
                msgDebug += GetComponent<ItemInstance>().name;
            else
                msgDebug += transform.name;
            Debug.LogWarning(msgDebug);
        }
    }
    
    public void InitUI()
    {
        if (feedback.childCount > 0 && feedback.GetChild(0).GetComponent<WorldspaceCanvasCameraAdapter>() != null)
            feedback.GetChild(0).gameObject.SetActive(true);
    }

    #region Accessors
    public InteractionImplementer Interactions
    {
        get
        {
            return interactions;
        }

        set
        {
            interactions = value;
        }
    }

    public Transform Feedback
    {
        get
        {
            return feedback;
        }

        set
        {
            feedback = value;
        }
    }
    #endregion
}
