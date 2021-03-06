﻿using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Boomlagoon.JSON;
using QuestSystem;

namespace QuestDeckLoader
{
    public class QuestAssociationDealer
    {
        public string idQuest;
        public string nameQuest;
        public string idQuestDealer;
        public string cardModelname;

        public QuestAssociationDealer()
        {
            // For tests
            idQuest = "defaultIdQuest";
            idQuestDealer = "defaultIdQuestDealer";
        }
    }

    public class QuestDeckData
    {
        public string idQuestDeck;
        public string nameQuestDeck;
        public string deckModelName;
        public string idMainQuest;
        public string mainQuestCardModel;
        public List<QuestAssociationDealer> secondaryQuests;

        public QuestDeckData()
        {
            // For tests
            idQuestDeck = "defaultId";
            nameQuestDeck = "defaultQuestDeckName";
            deckModelName = "";
            idMainQuest = "defaultIdMainQuest";

            // Not for tests
            secondaryQuests = new List<QuestAssociationDealer>();
        }
    }

    public class QuestDeckDatabase
    {
       
        List<QuestDeckData> listeQuestDeck;
        List<QuestSystem.QuestDeck> questDeckList;

        // Use this for initialization
        public QuestDeckDatabase()
        {
            listeQuestDeck = new List<QuestDeckData>();
            questDeckList = new List<QuestSystem.QuestDeck>();
        }

        public void Init()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "questDecks.json");
            string fileContents;

            if (path.Contains("://"))
            {
                WWW www = new WWW(path);
                fileContents = www.text;
            }
            else
                fileContents = File.ReadAllText(path);

            JSONObject json = JSONObject.Parse(fileContents);

            JSONArray questDeckArray = json["QuestDecks"].Array;

            foreach (JSONValue questDeck in questDeckArray)
            {
                QuestDeckData newQuestDeckDataContainer = new QuestDeckData();
                QuestSystem.QuestDeck newDeck = new QuestSystem.QuestDeck();
                foreach (KeyValuePair<string, JSONValue> deckQuestEntry in questDeck.Obj)
                {
                    switch (deckQuestEntry.Key)
                    {
                        // ROOT DATA
                        case "id":
                            newQuestDeckDataContainer.idQuestDeck = deckQuestEntry.Value.Str;
                            newDeck.Id = deckQuestEntry.Value.Str;
                            break;
                        case "name":
                            newQuestDeckDataContainer.nameQuestDeck = deckQuestEntry.Value.Str;
                            newDeck.DeckName = deckQuestEntry.Value.Str;
                            break;
                        case "level":
                            newDeck.LevelId = deckQuestEntry.Value.Str;
                            break;
                        case "deckModelName":
                            // if null -> didn't the corresponding material
                            newQuestDeckDataContainer.deckModelName = deckQuestEntry.Value.Str;
                            newDeck.DeckModelName = deckQuestEntry.Value.Str;
                            break;
                        case "main":
                            newQuestDeckDataContainer.idMainQuest = deckQuestEntry.Value.Str;
                            newDeck.MainQuest = deckQuestEntry.Value.Str;
                            break;
                        case "mainQuestCardModel":
                            newQuestDeckDataContainer.mainQuestCardModel = deckQuestEntry.Value.Str;
                            break;
                        case "secondary":
                            JSONArray secondaryQuestArray = deckQuestEntry.Value.Array;
                            foreach (JSONValue secondaryQuest in secondaryQuestArray)
                            {
                                QuestAssociationDealer association = new QuestAssociationDealer();
                                foreach (KeyValuePair<string, JSONValue> secondaryQuestId in secondaryQuest.Obj)
                                {
                                    switch (secondaryQuestId.Key)
                                    {
                                        // SECONDARY QUEST DATA
                                        case "idQuest":
                                            association.idQuest = secondaryQuestId.Value.Str;
                                            break;
                                        case "nameQuest" :
                                            association.nameQuest = secondaryQuestId.Value.Str;
                                            break;
                                        case "idQuestDealer":
                                            association.idQuestDealer = secondaryQuestId.Value.Str;
                                            break;
                                        case "cardModelname":
                                            association.cardModelname = secondaryQuestId.Value.Str;
                                            break;
                                    }
                                }
                                newDeck.SideQuests.Add(association.idQuest); 
                                newQuestDeckDataContainer.secondaryQuests.Add(association);
                            }
                            break;
                    }
                }
                listeQuestDeck.Add(newQuestDeckDataContainer);
                questDeckList.Add(newDeck);
            }
        }
        #region Accessors
        public List<QuestDeckData> ListQuestDeck
        {
            get
            {
                return listeQuestDeck;
            }
        }

        public List<QuestDeck> QuestDeckList
        {
            get
            {
                return questDeckList;
            }
        }

        public QuestDeck GetDeckByID(string id)
        {
            return questDeckList.Find(x => x.Id == id);
        } 

        public QuestDeckData GetQuestDeckDataByID(string id)
        {
            return listeQuestDeck.Find(x => x.idQuestDeck == id);
        }

        #endregion
    }
}