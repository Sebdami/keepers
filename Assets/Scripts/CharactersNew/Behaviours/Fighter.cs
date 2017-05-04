﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Behaviour;
using System;

namespace Behaviour
{
    public class Fighter : MonoBehaviour
    {
        // Balance variables
        private int effectiveAttackValue = 5;
        private int effectiveDefenseValue = 5;

        // Warning, UI is not think for values above 9, so ask before changing this setting
        public static int StockMaxValue = 9;

        PawnInstance instance;
        private InteractionImplementer battleInteractions;
        private Transform interactionsPosition;

        // TODO: externalize this in Monster
        bool isAMonster;

        // Non monster variables
        bool isTargetableByMonster = true;

        // TODO: externalize this in Monster
        // Monster variables
        bool hasRecentlyBattled = false;

        // Battle stats
        [SerializeField]
        private List<SkillBattle> battleSkills;

        // Fix, is there a way to set directly a sub skillbattle?
        [SerializeField]
        private List<SkillBattle> depressedSkills;
        [SerializeField]
        int nbrOfDice;
        [SerializeField]
        Die[] dice;

        // Instance variables
        [SerializeField]
        int physicalSymbolStored = 0;
        [SerializeField]
        int magicalSymbolStored = 0;
        [SerializeField]
        int defensiveSymbolStored = 0;

        bool hasPlayedThisTurn = false;

        bool hasClickedOnAttack = false;

        Face[] lastThrowResult;
        List<GameObject> lastThrowDiceInstance;

        // Pending variables
        bool isWaitingForDmgFeedback = false;
        int pendingDamage = 0;
        bool isWaitingForSkillPanelToClose = false;
        float showSkillPanelTimer = 2.2f;
        float showFeedbackDmgTimer = 1.7f;

        SkillDecisionAlgo skillDecisionAlgo;
        List<BattleBoeuf> effectiveBoeufs = new List<BattleBoeuf>();

        void Awake()
        {
            instance = GetComponent<PawnInstance>();
            battleInteractions = new InteractionImplementer();
            foreach (Transform child in transform)
            {
                if (child.CompareTag("FeedbackTransform"))
                {
                    interactionsPosition = child;
                    break;
                }
            }
        }

        void Start()
        {
            if (GetComponent<Monster>() != null) IsAMonster = true;
            else IsAMonster = false;

            battleInteractions.Add(new Interaction(Attack), 0, "Attack", GameManager.Instance.SpriteUtils.spriteAttack);
            battleInteractions.Add(new Interaction(OpenSkillPanel), 0, "Skills", GameManager.Instance.SpriteUtils.spriteUseSkill);

            showSkillPanelTimer = 2.2f;
            showFeedbackDmgTimer = 1.7f;

            if (!IsAMonster)
            {
                SkillBattle defaultAtk = new SkillBattle("default");
                battleSkills.Insert(0, defaultAtk);
                depressedSkills.Insert(0, defaultAtk);
            }

            foreach (SkillBattle sb in battleSkills)
            {
                if (sb.SkillUser == null)
                    sb.SkillUser = this;
            }

            foreach (SkillBattle sb in depressedSkills)
            {
                if (sb.SkillUser == null)
                    sb.SkillUser = this;
            }

            MonstersBattleSkillsSelection mbss = new MonstersBattleSkillsSelection();
            skillDecisionAlgo = mbss.GetDecisionAlgorithm(GetComponent<PawnInstance>().Data.PawnId);
        }

        private void Update()
        {
            if (isWaitingForSkillPanelToClose)
            {
                if (showSkillPanelTimer < 0.0f)
                {
                    if (GameManager.Instance.CurrentState != GameState.InTuto
                        || (GameManager.Instance.CurrentState == GameState.InTuto && (GetComponent<Keeper>() != null || GetComponent<Escortable>() != null)))
                    {
                        EndSkillProcess();
                    }                
                }
                else
                {
                    showSkillPanelTimer -= Time.deltaTime;
                }

                if (isWaitingForDmgFeedback)
                {
                    if (showFeedbackDmgTimer < 0.0f)
                    {
                        GetComponent<PawnInstance>().AddFeedBackToQueue(-pendingDamage);
                        GetComponent<Mortal>().CurrentHp -= pendingDamage;
                        isWaitingForDmgFeedback = false;
                    }
                    else
                    {
                        showFeedbackDmgTimer -= Time.deltaTime;
                    }
                }
            }
        }

        void OnDestroy()
        {
            if (BattleHandler.IsABattleAlreadyInProcess() && BattleHandler.IsWaitingForSkillEnd)
            {
                EndSkillProcess();
            }
        }

        public void EndSkillProcess()
        {
            GameManager.Instance.GetBattleUI.GetComponent<UIBattleHandler>().SkillName.SetActive(false);
            showSkillPanelTimer = 2.2f;
            showFeedbackDmgTimer = 1.7f;
            isWaitingForSkillPanelToClose = false;
            BattleHandler.IsWaitingForSkillEnd = false;
            if (GetComponent<Mortal>().CurrentHp <= 0)
            {
                gameObject.SetActive(false);
            }
        }

        public void ResetValuesAfterBattle()
        {
            physicalSymbolStored = 0;
            magicalSymbolStored = 0;
            defensiveSymbolStored = 0;
            hasPlayedThisTurn = false;
        }

        public void UpdateSkillButton(bool _hasAUsableSkill)
        {
            if (_hasAUsableSkill)
            {
                // TODO: change to proper sprite
                battleInteractions.SwapSprite("Skills", GameManager.Instance.SpriteUtils.spriteUseSkill);
            }
            else
            {
                battleInteractions.SwapSprite("Skills", GameManager.Instance.SpriteUtils.spriteUseSkill);
            }
        }

        #region Interactions
        public void Attack(int _i = 0)
        {
            Debug.Log("attack");
            HasClickedOnAttack = true;
        }

        public void AttackProcess(Fighter _attackTarget)
        {
            Debug.Log("attackProcess lunched");

            int attackDamage = 0;

            for (int i = 0; i < lastThrowResult.Length; i++)
            {
                // Apply attack calculation
                if (lastThrowResult[i].Type == FaceType.Physical)
                {
                    attackDamage += (effectiveAttackValue * lastThrowResult[i].Value);
                }
                else
                {
                    attackDamage += 1;
                }

            }
            if (_attackTarget.GetComponent<Keeper>() != null || _attackTarget.GetComponent<Escortable>() != null)
            {
                int effectiveDamage = (int)((attackDamage / (float)effectiveAttackValue));
                _attackTarget.GetComponent<Mortal>().CurrentHp -= effectiveDamage;
            }
            else if (_attackTarget.GetComponent<Monster>() != null)
            {
                // max defense => 10% dmg taken
                // 0 defense => 100% dmg taken
                
                int effectiveDamage = attackDamage;
                foreach (BattleBoeuf boeuf in effectiveBoeufs)
                {
                    if (boeuf.BoeufType == BoeufType.Damage)
                        effectiveDamage += boeuf.EffectValue;
                }

                effectiveDamage = ComputeEffectiveDamage(_attackTarget, effectiveDamage);
                _attackTarget.GetComponent<Mortal>().CurrentHp -= effectiveDamage;
                _attackTarget.GetComponent<PawnInstance>().AddFeedBackToQueue(-effectiveDamage);
            }

            if (_attackTarget.GetComponent<Mortal>().CurrentHp <= 0)
                _attackTarget.gameObject.SetActive(false);

            hasClickedOnAttack = false;
            HasPlayedThisTurn = true;
        }

        public void OpenSkillPanel(int _i = 0)
        {
            GameManager.Instance.GetBattleUI.GetComponent<UIBattleHandler>().GetSkillsPanelIndex(GetComponent<PawnInstance>()).gameObject.SetActive(true);
        }
        #endregion

        #region Accessors
        public List<SkillBattle> BattleSkills
        {
            get
            {
                return battleSkills;
            }

            set
            {
                battleSkills = value;
            }
        }

        public bool IsTargetableByMonster
        {
            get
            {
                return isTargetableByMonster;
            }

            set
            {
                isTargetableByMonster = value;
            }
        }

        // TODO: externalize this in Monster
        public bool HasRecentlyBattled
        {
            get
            {
                return hasRecentlyBattled;
            }

            set
            {
                hasRecentlyBattled = value;
                if (GetComponent<Monster>() != null)
                    GetComponent<Monster>().HasRecentlyBattled = value;
            }
        }

        // TODO: externalize this in Monster
        public bool IsAMonster
        {
            get
            {
                return isAMonster;
            }

            set
            {
                isAMonster = value;
            }
        }

        public Die[] Dice
        {
            get
            {
                return dice;
            }

            set
            {
                dice = value;
            }
        }

        public int PhysicalSymbolStored
        {
            get
            {
                return physicalSymbolStored;
            }

            set
            {
                physicalSymbolStored = value;
                if (physicalSymbolStored > StockMaxValue)
                    physicalSymbolStored = StockMaxValue;
                GameManager.Instance.GetBattleUI.GetComponent<UIBattleHandler>().UpdateAttributesStocks(this);
            }
        }

        public int MagicalSymbolStored
        {
            get
            {
                return magicalSymbolStored;
            }

            set
            {
                magicalSymbolStored = value;
                if (magicalSymbolStored > StockMaxValue)
                    magicalSymbolStored = StockMaxValue;
                GameManager.Instance.GetBattleUI.GetComponent<UIBattleHandler>().UpdateAttributesStocks(this);
            }
        }

        public int DefensiveSymbolStored
        {
            get
            {
                return defensiveSymbolStored;
            }

            set
            {
                defensiveSymbolStored = value;
                if (defensiveSymbolStored > StockMaxValue)
                    defensiveSymbolStored = StockMaxValue;
                GameManager.Instance.GetBattleUI.GetComponent<UIBattleHandler>().UpdateAttributesStocks(this);
            }
        }

        public bool HasPlayedThisTurn
        {
            get
            {
                return hasPlayedThisTurn;
            }

            set
            {
                hasPlayedThisTurn = value;
                if (hasPlayedThisTurn == true)
                {
                    GameManager.Instance.ClearListKeeperSelected();
                    BattleHandler.CheckTurnStatus();
                }
            }
        }

        public Transform InteractionsPosition
        {
            get
            {
                return interactionsPosition;
            }

            set
            {
                interactionsPosition = value;
            }
        }

        public InteractionImplementer BattleInteractions
        {
            get
            {
                return battleInteractions;
            }

            set
            {
                battleInteractions = value;
            }
        }

        public bool HasClickedOnAttack
        {
            get
            {
                return hasClickedOnAttack;
            }

            set
            {
                hasClickedOnAttack = value;
                if (hasClickedOnAttack == true)
                {
                    GameManager.Instance.Ui.mouseFollower.SetActive(true);
                    GameManager.Instance.Ui.mouseFollower.GetComponent<MouseFollower>().ExpectedTarget(TargetType.FoeSingle);
                    BattleHandler.ActivateFeedbackSelection(false, true);
                }
            }
        }

        public Face[] LastThrowResult
        {
            get
            {
                return lastThrowResult;
            }

            set
            {
                lastThrowResult = value;
            }
        }

        public List<GameObject> LastThrowDiceInstance
        {
            get
            {
                return lastThrowDiceInstance;
            }

            set
            {
                lastThrowDiceInstance = value;
            }
        }

        public bool IsWaitingForDmgFeedback
        {
            get
            {
                return isWaitingForDmgFeedback;
            }

            set
            {
                isWaitingForDmgFeedback = value;
            }
        }

        public bool IsWaitingForSkillPanelToClose
        {
            get
            {
                return isWaitingForSkillPanelToClose;
            }

            set
            {
                isWaitingForSkillPanelToClose = value;
            }
        }

        public int PendingDamage
        {
            get
            {
                return pendingDamage;
            }

            set
            {
                pendingDamage = ComputeEffectiveDamage(this, value);
            }
        }

        private int ComputeEffectiveDamage(Fighter _fighter, int value)
        {
            foreach (BattleBoeuf boeuf in _fighter.effectiveBoeufs)
            {
                if (boeuf.BoeufType == BoeufType.Defense)
                    value -= boeuf.EffectValue;
            }
            return Mathf.Max(0, value);
        }

        public SkillDecisionAlgo SkillDecisionAlgo
        {
            get
            {
                return skillDecisionAlgo;
            }

            set
            {
                skillDecisionAlgo = value;
            }
        }

        public List<BattleBoeuf> EffectiveBoeufs
        {
            get
            {
                return effectiveBoeufs;
            }

            set
            {
                effectiveBoeufs = value;
            }
        }

        public List<SkillBattle> DepressedSkills
        {
            get
            {
                return depressedSkills;
            }

            set
            {
                depressedSkills = value;
            }
        }

        #endregion

        public void UseSkill(PawnInstance _target)
        {
            skillDecisionAlgo.Invoke(this).UseSkill(_target);
        }

        public void UpdateActiveBoeufs()
        {
            foreach (BattleBoeuf boeuf in effectiveBoeufs)
            {
                boeuf.Duration--;
                Debug.Log(boeuf.Duration);
                if (boeuf.Duration == 0)
                {
                    Debug.Log("??");
                    effectiveBoeufs.Remove(boeuf);
                }
            }
        }

        // TODO: externalize this in Monster
        #region Monster functions
        public void RestAfterBattle()
        {
            foreach (BoxCollider bc in GetComponentsInChildren<BoxCollider>())
                bc.enabled = false;

            HasRecentlyBattled = true;

            // TODO fix this
            GetComponent<Monster>().HasRecentlyBattled = true;

            Invoke("ReactivateTrigger", 3.0f);
        }

        private void ReactivateTrigger()
        {
            foreach (BoxCollider bc in GetComponentsInChildren<BoxCollider>())
                bc.enabled = true;
            HasRecentlyBattled = false;

            // TODO fix this
            GetComponent<Monster>().HasRecentlyBattled = false;

        }
        #endregion
    }
}