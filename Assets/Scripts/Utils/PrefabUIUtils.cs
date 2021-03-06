﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabUIUtils : MonoBehaviour {

    // KeeperPanel
    [Header("Character")]
    //public GameObject goSelectedKeeperPanel;
    // Inventory
    public GameObject PrefabInventaireUI;

    public GameObject PrefabSlotUI;
    public GameObject PrefabSlotAshleyUI;
    public GameObject PrefabItemUI;

    // ActionPanel
    [Header("Selected Keeper UI")]
    public GameObject PrefabSelectedKeeper;
    public GameObject PrefabSelectedStatsUIPanel;
    public GameObject PrefabSelectedInventoryUIPanel;
    public GameObject PrefabSelectedEquipementUIPanel;

    public GameObject PrefabMentalHealthUI;
    public GameObject PrefabHungerUI;
    public GameObject PrefabHPUI;
    public GameObject PrefabActionPAUI;

    // ActionPanel
    [Header("Action UI")]
    public GameObject WorldSpaceUIprefab;
    public GameObject PrefabActionUI;
    public GameObject PrefabActionPoint;

    // ShortcutPanel
    [Header("Shortcut UI")]
    public GameObject PrefabShorcutCharacter;

    public GameObject PrefabShortcutMentalHealthUI;
    public GameObject PrefabShortcutHungerUI;
    public GameObject PrefabShortcutHPUI;

    public GameObject PrefabShortcutActionPAUI;

    // Utils 
    [Header("Utils UI")]
    public GameObject PrefabImageUI;
    public GameObject PrefabConfimationButtonUI;

    [Header("Quest")]
    public GameObject PrefabContentQuestUI;

    [Header("Panneau")]
    public GameObject PrefabContentPanneauUI;

    [Header("Menu")]
    public GameObject prefabMenuSelectedKeeperUI;

    [Header("Battle")]
    public GameObject prefabLifeBar;
    public GameObject diceFeedback;
    public GameObject diceFeedbackOnPointerEnter;
    public GameObject skillDescriptionPanel;
    public GameObject attributesAscFeedback;

}
