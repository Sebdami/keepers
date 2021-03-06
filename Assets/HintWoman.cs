﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintWoman : MonoBehaviour {

    PawnInstance instance;
    private GameObject goHint;
    public string[] commeSurLePanneau;
    private int indiceMsg;

    void Awake()
    {
        instance = GetComponent<PawnInstance>();
        indiceMsg = 0;
    }

    // Use this for initialization
    void Start () {
        GetComponent<Interactable>().Interactions.Add(new Interaction(Hint), 0, "Talk", GameManager.Instance.SpriteUtils.spriteMoral);

        goHint = Instantiate(GameManager.Instance.PrefabUIUtils.PrefabContentQuestUI, GameManager.Instance.Ui.goContentQuestParent.transform);
        goHint.transform.localPosition = Vector3.zero;
        goHint.transform.localScale = Vector3.one;
        goHint.transform.GetChild(1).GetComponent<Image>().sprite = instance.Data.AssociatedSprite;
        if(commeSurLePanneau.Length > 0)
        {
            goHint.transform.GetChild(3).GetComponentInChildren<Text>().text = commeSurLePanneau[indiceMsg];
        }
        else
        {
            goHint.transform.GetChild(3).GetComponentInChildren<Text>().text = "Debug Hint Message";
        }

        goHint.transform.GetChild(4).GetComponent<Text>().text = Translater.PnjName(transform.name);
        Button close = goHint.transform.GetChild(0).GetComponent<Button>();
        close.onClick.RemoveAllListeners();
        close.onClick.AddListener(CloseBox);
        Button validate = goHint.transform.GetChild(goHint.transform.childCount - 3).GetComponent<Button>();
        if (validate != null)
        {
            validate.onClick.RemoveAllListeners();
            validate.onClick.AddListener(CloseBox);
        }
    }

    public void Hint(int _i)
    {
        if (GameManager.Instance.ListOfSelectedKeepers.Count > 0)
        {
            if (!GameManager.Instance.GetFirstSelectedKeeper().Data.Behaviours[(int)BehavioursEnum.CanSpeak])
            {
                if (GameManager.Instance.GetFirstSelectedKeeper().Data.PawnId == "lucky")
                {
                    int i = Random.Range(0, 2);
                    if( i == 1)
                    {
                        goHint.transform.GetChild(3).GetComponentInChildren<Text>().text = Translater.PnjText(transform.name, 0, CharacterRace.Cat);
                    } else
                    {
                        goHint.transform.GetChild(3).GetComponentInChildren<Text>().text = Translater.PnjText(transform.name, 1, CharacterRace.Cat);
                    }
                } else
                {
                    goHint.transform.GetChild(3).GetComponentInChildren<Text>().text = Translater.PnjText(transform.name, 0, CharacterRace.Dog);
                }


            }

            GameManager.Instance.Ui.goContentQuestParent.SetActive(true);
            OpenBox();
        }
    }

    void AcceptQuest()
    {
        GameManager.Instance.Ui.goContentQuestParent.SetActive(false);
        CloseBox();
    }
    void OpenBox()
    {
        goHint.SetActive(true);
        GameManager.Instance.CurrentState = GameState.InPause;
    }
    void CloseBox()
    {
        GameManager.Instance.CurrentState = GameState.Normal;
        if (GameManager.Instance.GetFirstSelectedKeeper().Data.Behaviours[(int)BehavioursEnum.CanSpeak])
            indiceMsg++;
        if ( indiceMsg < commeSurLePanneau.Length)
        {
            goHint.transform.GetChild(3).GetComponentInChildren<Text>().text = Translater.PnjText(transform.name, indiceMsg, CharacterRace.Human);
        } else
        {
            indiceMsg = 0;
            goHint.transform.GetChild(3).GetComponentInChildren<Text>().text = Translater.PnjText(transform.name, indiceMsg, CharacterRace.Human);
        }
 
        goHint.SetActive(false);
    }
}
