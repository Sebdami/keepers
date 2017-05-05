﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuestSystem;
using UnityEngine.UI;

namespace Behaviour
{
    public class QuestDealer : MonoBehaviour
    {
        PawnInstance instance;
        public GameObject goQuest;
        //public GameObject prefabContentQuest;

        // Array of arrays. use: QuestIDs[CurrentQuestDeck] -> Array of Quests IDs for this component in the current quest deck
        // QuestIDs[CurrentQuestDeck].QuestIDs[0] -> The first Quest ID that correspond to this Quest dealer and the current quest deck
        //public QuestIDArray[] PossibleQuests;

        //public int currentQuestIndex;
        public Quest questToGive;

        void Awake()
        {
            instance = GetComponent<PawnInstance>();
 
        }

        public void Init()
        {
            goQuest = Instantiate(GameManager.Instance.PrefabUIUtils.PrefabContentQuestUI, GameManager.Instance.Ui.goContentQuestParent.transform);
            goQuest.transform.localPosition = Vector3.zero;
            goQuest.transform.localScale = Vector3.one;
            goQuest.transform.GetChild(1).GetComponent<Image>().sprite = instance.Data.AssociatedSprite;
            GetComponent<Interactable>().Interactions.Add(new Interaction(Quest), 0, "Quest", GameManager.Instance.SpriteUtils.spriteQuest);

            if (questToGive == null)
            {
                Debug.Log("Quest Not Found");
            }
        }

        void BuildQuestPanel()
        {

            goQuest.transform.GetChild(goQuest.transform.childCount - 1).GetComponent<Text>().text = questToGive.Information.Title;
            goQuest.transform.GetChild(goQuest.transform.childCount - 2).GetComponentInChildren<Text>().text = questToGive.Information.Dialog;
            Button validate = goQuest.transform.GetChild(goQuest.transform.childCount - 3).GetComponent<Button>();
            if (validate != null)
            {
                validate.onClick.RemoveAllListeners();
                validate.onClick.AddListener(AcceptQuest);
            }
            else
            {
                Debug.Log("Didn't find the button. You probably fucked up with the children selection.");
            }
        }

        void BuildAlreadyActivePanel()
        {
            goQuest.transform.GetChild(goQuest.transform.childCount - 1).GetComponent<Text>().text = questToGive.Information.Title;
            goQuest.transform.GetChild(goQuest.transform.childCount - 2).GetComponentInChildren<Text>().text = questToGive.Information.HintDialog;
            Button validate = goQuest.transform.GetChild(goQuest.transform.childCount - 3).GetComponent<Button>();
            if (validate != null)
            {
                validate.onClick.RemoveAllListeners();
                validate.onClick.AddListener(CloseBox);
            }
        }

        void BuildEndQuestPanel()
        {
            goQuest.transform.GetChild(goQuest.transform.childCount - 1).GetComponent<Text>().text = questToGive.Information.Title;
            goQuest.transform.GetChild(goQuest.transform.childCount - 2).GetComponentInChildren<Text>().text = questToGive.Information.EndDialog;
            Button validate = goQuest.transform.GetChild(goQuest.transform.childCount - 3).GetComponent<Button>();
            if (validate != null)
            {
                validate.onClick.RemoveAllListeners();
                validate.onClick.AddListener(EndQuest);
            }
        }

        public void Quest(int _i = 0)
        {
            if (GameManager.Instance.ListOfSelectedKeepers.Count > 0 && questToGive != null)
            {
                int costAction = GetComponent<Interactable>().Interactions.Get("Quest").costAction;
                if (GameManager.Instance.ListOfSelectedKeepers[0].GetComponent<Keeper>().ActionPoints >= costAction)
                {
                    GameManager.Instance.ListOfSelectedKeepers[0].GetComponent<Keeper>().ActionPoints -= (short)costAction;
                    if(GameManager.Instance.QuestManager.ActiveQuests.Contains(questToGive))
                    {
                        if(questToGive.CheckIfComplete())
                        {
                            //Si la quête a été complétée
                            BuildEndQuestPanel();
                            OpenBox();
                        }
                        else
                        {
                            //Si la quête a déjà été acceptée
                            BuildAlreadyActivePanel();
                            OpenBox();
                        }
                    }
                    else
                    {
                        BuildQuestPanel();
                        OpenBox();
                    }
                    
                    GameManager.Instance.Ui.goContentQuestParent.SetActive(true);
                }
                else
                {
                    GameManager.Instance.Ui.ZeroActionTextAnimation(GameManager.Instance.GetFirstSelectedKeeper().GetComponent<Behaviour.Keeper>());
                }
            }
        }

        void AcceptQuest()
        {
            QuestUtility.AcceptQuest(questToGive);
            GameManager.Instance.Ui.goContentQuestParent.SetActive(false);
            CloseBox();
        }
        void OpenBox()
        {
            goQuest.SetActive(true);
            GameManager.Instance.CurrentState = GameState.InPause;
        }
        void CloseBox()
        {
            GameManager.Instance.CurrentState = GameState.Normal;
            goQuest.SetActive(false);
        }

        void EndQuest()
        {

            QuestUtility.CompleteQuest(questToGive);
            CloseBox();
            // Do things?
        }
    }
}