﻿using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class SpriteUIUtils : MonoBehaviour {

    // Actions
    [Header("Actions")]
    public Sprite spriteMove;
    public Sprite spriteExplore;
    public Sprite spriteHarvest;
    public Sprite spritePick;
    public Sprite spriteTrade;
    public Sprite spriteEscort;
    public Sprite spriteUnescort;
    public Sprite spriteExamine;
    public Sprite spriteHeal;

    public Sprite spriteLoot;
    public Sprite spriteQuest;
    public Sprite spriteMoral;
    public Sprite spriteMoralBuff;
    public Sprite spriteMoralDebuff;
    public Sprite spriteEndAction;
    public Sprite spriteEndActionFR;

    public Sprite spriteAttack;

    public Sprite spriteTokenAction;
    public Sprite spriteNoAction;
    public Sprite spriteTokenActionFR;
    public Sprite spriteNoActionFR;

    // for death
    [Header("Death")]
    public Sprite spriteDeath;

    // Feedback ui
    [Header("UI")]
    public Sprite spriteHunger;
    public Sprite spriteLove; // HP 
    public Sprite spriteMentalDown;
    public Sprite spriteMentalNormal;
    public Sprite spriteMentalUp;

    // Objectif
    [Header("Objectif")]
    public Sprite spriteOrientation;

    // Tuto
    [Header("Tuto")]
    public Sprite spriteTutoHighlightCercle;
    public Sprite spriteTutoHighlightSquare;
    public Sprite spriteMouse;
    public Sprite spriteMouseRightClicked;
    public Sprite spriteMouseLeftClicked;
    public Sprite spriteMouseMiddleClicked;
    public Sprite spriteCookie;
    public Sprite spriteTutoCircleFeedback;

    // Life bars
    [Header("Battle")]
    public Sprite spritePlayerGreenLifeBar;
    public Sprite spriteMonsterGreenLifeBar;

    // Stocks
    public Sprite spriteFillAtk;
    public Sprite spriteFillAtkFull;
    public Sprite spriteFillDef;
    public Sprite spriteFillDefFull;
    public Sprite spriteFillMagic;
    public Sprite spriteFillMagicFull;
    public Sprite spriteAttackSymbol;
    public Sprite spriteDefenseSymbol;
    public Sprite spriteMagicSymbol;
    public Sprite spriteAtkBuff;
    public Sprite spriteDefBuff;
    public Sprite spriteAtkDesBoeufs;
    public Sprite spriteDefDesBoeufs;
    public Sprite spriteAggroDesBoeufs;
    public Sprite VictoryEN;
    public Sprite DefeatEN;
    public Sprite VictoryFR;
    public Sprite DefeatFR;

    // Menu
    [Header("Menu")]
    public Sprite spriteTrivial;
    public Sprite spriteEasy;
    public Sprite spriteMedium;
    public Sprite spriteHard;
    public Sprite spriteEasyFR;
    public Sprite spriteMediumFR;
    public Sprite spriteHardFR;

    [Header("Win Screens")]
    public Sprite level1;
    public Sprite level2;
    public Sprite level3;
    public Sprite level1FR;
    public Sprite level2FR;
    public Sprite level3FR;

    [Header("Defeat Screens")]
    public Sprite loseScreen;
    public Sprite loseScreenFR;

    [Header("Feedbacks")]
    public Sprite hungerFeedback;
    public Sprite lowMoodFeedback;
    public Sprite luckat;
    public Sprite badLuckat;

}

