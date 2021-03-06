﻿using UnityEngine;
using UnityEngine.AI;

using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour {

    [SerializeField]
    private MenuManager menuManager;
    private MenuUI menuUI;
    private BoxOpener boxOpener;

    private bool oncePressR;

    public Transform trLevelCardTarget;

    //public Opener LevelDeck;

    [SerializeField]
    public LayerMask layerToCheck;

    public LayerMask layerControls;


    private LevelDataBase leveldb;

    public bool OncePressR
    {
        get
        {
            return oncePressR;
        }

        set
        {
            oncePressR = value;
        }
    }

    public LevelDataBase Leveldb
    {
        get
        {
            if (leveldb == null)
                leveldb = menuManager.Leveldb;
            return leveldb;
        }

        set
        {
            leveldb = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        menuManager = GetComponent<MenuManager>();
        menuUI = GetComponent<MenuUI>();
        boxOpener = GetComponent<BoxOpener>();
        oncePressR = false;
    }

    // Update is called once per frame
    void Update()
    {
        MenuControls();
    }

    

    public void MenuControls()
    {
        // TMP
        //if (!menuManager.DuckhavebringThebox && !oncePressR)
        //{
        //    oncePressR = true;
        //    menuUI.pressR.gameObject.SetActive(true);
        //}

        //if (oncePressR)
        //{
        if (!menuManager.DuckhavebringThebox)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameObject boxNavBox = GameObject.Find("Box Nav box");
                boxNavBox.GetComponent<Animator>().enabled = false;
                boxNavBox.GetComponent<NavMeshAgent>().baseOffset = 0.1f;
                boxNavBox.transform.localPosition = new Vector3(0.0f, 0.025f, -0.22f);
                boxNavBox.GetComponent<boxMove>().enabled = false;

                GameObject.Find("DuckNukeThem").SetActive(false);
                FindObjectOfType<MenuManager>().DuckhavebringThebox = true;
                menuUI.pressR.gameObject.SetActive(false);
            }
        } else
        {
            if ( !oncePressR)
            {
                boxOpener.boxLock.GetComponent<GlowObjectCmd>().ActivateBlinkBehaviour(true);
                boxOpener.boxLock.GetComponent<GlowObjectCmd>().enabled = true;
                oncePressR = true;
            }

        }
            //        GameManager.Instance.PersistenceLoader.SetPawnUnlocked("grekhan", false);
            //        GameManager.Instance.PersistenceLoader.SetPawnUnlocked("lupus", false);
            //        GameManager.Instance.PersistenceLoader.SetPawnUnlocked("swag", false);
            //        GameManager.Instance.PersistenceLoader.SetPawnUnlocked("lucky", false);
            //        GameManager.Instance.PersistenceLoader.SetPawnUnlocked("emo", false);
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistencePawns["grekhan"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistencePawns["lupus"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistencePawns["swag"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistencePawns["lucky"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistencePawns["emo"] = false;

            //        GameManager.Instance.PersistenceLoader.SetLevelUnlocked("4", false);
            //        GameManager.Instance.PersistenceLoader.SetLevelUnlocked("2", false);
            //        GameManager.Instance.PersistenceLoader.SetLevelUnlocked("1", true);
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceLevels["4"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceLevels["2"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceLevels["1"] = true;

            //        GameManager.Instance.PersistenceLoader.SetEventUnlocked("1", false);
            //        GameManager.Instance.PersistenceLoader.SetEventUnlocked("2", false);
            //        GameManager.Instance.PersistenceLoader.SetEventUnlocked("3", false);
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceEvents["1"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceEvents["2"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceEvents["3"] = false;

            //        GameManager.Instance.PersistenceLoader.SetDeckUnlocked("deck_04", false);
            //        GameManager.Instance.PersistenceLoader.SetDeckUnlocked("deck_01", true);
            //        GameManager.Instance.PersistenceLoader.SetDeckUnlocked("deck_02", false);
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceDecks["deck_04"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceDecks["deck_01"] = true;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceDecks["deck_02"] = false;

            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceSequences["seqfirstmove"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceSequences["seqtutocombat"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceSequences["seqmulticharacters"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceSequences["seqmoraleexplained"] = true;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceSequences["seqlowhunger"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceSequences["seqlowmorale"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceSequences["seqashleylowhunger"] = false;
            //        GameManager.Instance.PersistenceLoader.Pd.dicPersistenceSequences["seqashleyescort"] = false;
            //        GameManager.Instance.PersistenceLoader.SetSequenceUnlocked("seqfirstmove", false);
            //        GameManager.Instance.PersistenceLoader.SetSequenceUnlocked("seqtutocombat", false);
            //        GameManager.Instance.PersistenceLoader.SetSequenceUnlocked("seqmulticharacters", false);
            //        GameManager.Instance.PersistenceLoader.SetSequenceUnlocked("seqmoraleexplained", false);
            //        GameManager.Instance.PersistenceLoader.SetSequenceUnlocked("seqlowhunger", false);
            //        GameManager.Instance.PersistenceLoader.SetSequenceUnlocked("seqlowmorale", false);
            //        GameManager.Instance.PersistenceLoader.SetSequenceUnlocked("seqashleylowhunger", false);
            //        GameManager.Instance.PersistenceLoader.SetSequenceUnlocked("seqashleyescort", false);
            //        GameManager.Instance.CurrentState = GameState.Normal;
            //        AudioManager.Instance.Fade(AudioManager.Instance.menuMusic);
            //        //GameManager.Instance.Ui.GoActionPanelQ.transform.parent.SetParent(GameManager.Instance.Ui.transform);
            //        SceneManager.LoadScene(0);
            //    }
            //}

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (boxOpener.IsBoxOpen)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) == true)
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("CardLevel"))
                    {
                        LevelSelectionControls(hit.transform.gameObject);
                    }
                    else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("KeeperInstance"))
                    {
                        KeeperSelectionControls(hit.transform.gameObject);
                        //FoldEverything();
                    }
                    else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("DeckOfCards"))
                    {
                        DeckSelectionControls(hit.transform.gameObject);
                    }
                    else
                    {
                        //FoldEverything();
                    }
                }
            }

            LayerMask mask = 1 << LayerMask.NameToLayer("BoxLock");
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask);
            if (hit.transform != null)
            {
                boxOpener.BoxControls();
            }
        }
    }


    public void LevelSelectionControls(GameObject hit)
    {

        CardLevel card = hit.transform.gameObject.GetComponent<CardLevel>();
        if (card != null && !menuUI.ACardInfoIsShown && !menuUI.IsACardMoving && !menuUI.IsACardInfoMoving && !menuUI.IsACardInfoMovingForShowing)
        {
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.paperSelectSound, 0.5f);


            if  (card.gameObject == menuUI.LevelCardSelected)
            {
                menuUI.LevelCardSelected.GetComponent<CardLevel>().IsSelected = false;
                menuManager.CardLevelSelected = -1;
                menuManager.DeckOfCardsSelected = string.Empty;

                if(menuManager.currentMiniature != null)
                {
                    GameObject.Destroy(menuManager.currentMiniature.gameObject);
                }

                GameManager.Instance.ListEventSelected.Clear();
                menuManager.SetActiveChatBoxes(false);
                menuUI.previousCardSelected = null;
            }
            else
            {
                menuUI.previousCardSelected = menuUI.LevelCardSelected;
                menuUI.LevelCardSelected = card.gameObject;

                menuUI.LevelCardSelected.GetComponent<CardLevel>().IsSelected = true;

                menuManager.CardLevelSelected = card.levelIndex;
                menuManager.DeckOfCardsSelected = Leveldb.GetLevelById(card.levelIndex).deckId;

                menuManager.currentMiniature = Instantiate(menuManager.prefabsMiniatures[card.levelIndex - 1], menuManager.trVortex.position + Vector3.up * 0.2f, Quaternion.identity).GetComponent<Miniature>();
                menuManager.currentMiniature.bStartHidden = true;

                GameManager.Instance.ListEventSelected.Clear();

                for (int i = 0; i < Leveldb.GetLevelById(card.levelIndex).listEventsId.Count; i++)
                {
                    if (GameManager.Instance.PersistenceLoader.Pd.dicPersistenceEvents[Leveldb.GetLevelById(card.levelIndex).listEventsId[i]] == true)
                    {
                        GameManager.Instance.ListEventSelected.Add(Leveldb.GetLevelById(card.levelIndex).listEventsId[i]);
                    }
                }
                menuManager.SetActiveChatBoxes(true);
                menuUI.IsACardMoving = true;
            }


            menuUI.UpdateDeckSelected();
            menuUI.UpdateStartButton();
            boxOpener.UpdateLockAspect();
        }
    }

    public void DeckSelectionControls(GameObject hit)
    {
        if (!menuUI.ACardInfoIsShown && !menuUI.IsACardMoving && !menuUI.IsACardInfoMoving && !menuUI.IsACardInfoMovingForShowing)
        {
            menuUI.UpdateDeckDisplayed();
            foreach (GameObject go in menuManager.GoCardsLevels)
            {
                if (go.transform.parent == menuManager.GoDeck.transform)
                {
                    go.transform.SetParent(null);
                    go.SetActive(true);
                }
            }

            foreach (List<GameObject> goChildren in menuManager.GoCardChildren)
            {

                    foreach (GameObject go in goChildren)

                    {
                        if (menuUI.LevelCardSelected != go.GetComponentInParent<CardLevel>().gameObject)
                        {

                            go.SetActive(false);

                            go.transform.localPosition = new Vector3(0.0f, 0f, 0.0f);
                        }
                    }
       
            }
        } 
 

 
    }

    public void KeeperSelectionControls(GameObject hit)
    {
        PawnInstance pi = hit.transform.gameObject.GetComponent<PawnInstance>();
        if (menuManager.CardLevelSelected == -1 && menuManager.ListeSelectedKeepers.Count == 0 && !menuManager.GoDeck.GetComponent<Deck>().IsOpen)
        {
            menuManager.GoDeck.GetComponent<GlowObjectCmd>().ActivateBlinkBehaviour(true);

            menuManager.GoDeck.GetComponent<GlowObjectCmd>().enabled = true;
        }
        else if (pi != null &&  menuUI.cardsInfoAreReady && !menuManager.GoDeck.GetComponent<Deck>().IsOpen && !menuUI.IsAPawnMoving && !menuUI.ACardInfoIsShown && !menuUI.ACardInfoIsShown)
        {
            if (menuManager.ContainsSelectedKeepers(pi.Data.PawnId)) // REMOVE
            {
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.deselectSound, 0.25f);
                //pi.GetComponent<OpenerContent>().Hide();
                menuManager.RemoveFromSelectedKeepers(pi.Data.PawnId);
                if (menuManager.GoDeck.GetComponent<Deck>() != null && !menuManager.GoDeck.GetComponent<Deck>().IsOpen)
                {
                    menuManager.DicPawnChatBox[pi.gameObject].SetMode(ChatBox.ChatMode.awaiting);
                    menuManager.DicPawnChatBox[pi.gameObject].Say(ChatBox.ChatMode.unchosen);
                }


                menuUI.UpdateKeepers(pi, hit.transform.parent);
                pi.transform.SetParent(null);

                boxOpener.UpdateLockAspect();

                menuUI.UpdateStartButton();
            }
            else    // ADD
            {
                if(menuManager.CardLevelSelected != -1)
                {
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.selectSound, 0.25f);
                    //pi.GetComponent<OpenerContent>().Show();

                    menuManager.AddToSelectedKeepers(pi.Data.PawnId);
                    if (menuManager.GoDeck.GetComponent<Deck>() != null && !menuManager.GoDeck.GetComponent<Deck>().IsOpen)
                    {
                        menuManager.DicPawnChatBox[pi.gameObject].SetMode(ChatBox.ChatMode.picked);
                        menuManager.DicPawnChatBox[pi.gameObject].Say(ChatBox.ChatMode.chosen);
                    }


                    menuUI.UpdateKeepers(pi, hit.transform.parent);
                    pi.transform.SetParent(null);

                    boxOpener.UpdateLockAspect();

                    menuUI.UpdateStartButton();
                }
     

            }

        }
    } 

    //void FoldEverything()
    //{
    //    for (int i = 0; i < LevelDeck.listOpenerSiblings.Count; i++)
    //    {
    //        LevelDeck.listOpenerSiblings[i].Fold();
    //    }
    //    LevelDeck.Fold();
    //}
}
