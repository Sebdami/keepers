﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Behaviour
{
    public class Mortal : MonoBehaviour
    {
        [System.Serializable]
        public class MortalData : ComponentData
        {
            [SerializeField]
            int maxHp;

            public MortalData(int _maxHp = 0)
            {
                maxHp = _maxHp;
            }

            public int MaxHp
            {
                get
                {
                    return maxHp;
                }

                set
                {
                    maxHp = value;
                }
            }
        }

        PawnInstance instance;

        [SerializeField]
        private MortalData data;

        int currentHp;
        bool isAlive;

        [SerializeField]
        private ParticleSystem deathParticles;

        // UI
        private GameObject selectedHPUI;
        private GameObject shortcutHPUI;

        void Start()
        {
            instance = GetComponent<PawnInstance>();
            InitUI();

            CurrentHp = data.MaxHp;
            isAlive = true;
        }

        public void Die()
        {
            if (GetComponent<Keeper>() != null)
            {
                // TODO refacto TileManager needed
                //Debug.Log("Blaeuurgh... *dead*");
                //Tile currentTile = TileManager.Instance.GetTileFromKeeperOld[this];

                //// Drop items
                //ItemManager.AddItemOnTheGround(currentTile, transform, GetComponent<Behaviour.Inventory>().Items);

                //// Remove reference from tiles
                //TileManager.Instance.RemoveKilledKeeperOld(this);

                //// Death operations
                //GameManager.Instance.ShortcutPanel_NeedUpdate = true;

                //GlowController.UnregisterObject(GetComponent<GlowObjectCmd>());
                //anim.SetTrigger("triggerDeath");

                //// Try to fix glow bug
                //Destroy(GetComponent<GlowObjectCmd>());

                //GameManager.Instance.Ui.HideSelectedKeeperPanel();
                //GameManager.Instance.CheckGameState();

                //// Deactivate pawn
                //DeactivatePawn();
            }
            else if (GetComponent<Monster>() != null)
            {

            }
            else
            {
                Debug.Log("Ashley is dead");

            }
            GameManager.Instance.CheckGameState();
        }

        private void DeactivatePawn()
        {
            foreach (Collider c in GetComponentsInChildren<Collider>())
                c.enabled = false;
            enabled = false;

            // Deactivate gameobject after a few seconds
            StartCoroutine(DeactivateGameObject());
        }

        IEnumerator DeactivateGameObject()
        {
            yield return new WaitForSeconds(5.0f);
            gameObject.SetActive(false);
            foreach (Collider c in GetComponentsInChildren<Collider>())
                c.enabled = true;
            enabled = true;
        }

        #region UI
        public void InitUI()
        {
            CreateShortcutHPPanel();
            ShortcutHPUI.name = "Mortal";

            if (instance.GetComponent<Escortable>() != null)
            {
                ShortcutHPUI.transform.SetParent(instance.GetComponent<Escortable>().ShorcutUI.transform);
                ShortcutHPUI.transform.localScale = Vector3.one;
                ShortcutHPUI.transform.localPosition = Vector3.zero;
            }
            else if (instance.GetComponent<Keeper>() != null)
            {

                CreateSelectedHPPanel();
                SelectedHPUI.name = "Mortal";
                SelectedHPUI.transform.SetParent(instance.GetComponent<Keeper>().SelectedStatPanelUI.transform);
                SelectedHPUI.transform.localScale = Vector3.one;
                SelectedHPUI.transform.localPosition = new Vector3(200, 200, 0);

                ShortcutHPUI.transform.SetParent(instance.GetComponent<Keeper>().ShorcutUI.transform);
                ShortcutHPUI.transform.localScale = Vector3.one;
                ShortcutHPUI.transform.localPosition = Vector3.zero;
            }
        }

        public void CreateSelectedHPPanel()
        {
            SelectedHPUI = Instantiate(GameManager.Instance.PrefabUtils.PrefabHPUI);
        }

        public void CreateShortcutHPPanel()
        {
            ShortcutHPUI = Instantiate(GameManager.Instance.PrefabUtils.PrefabHPUI);
        }

        public void UpdateHPPanel(int hunger)
        {
            if (instance.GetComponent<Escortable>() != null)
            {
                ShortcutHPUI.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = (float)hunger / (float)Data.MaxHp;
            }
            else if (instance.GetComponent<Keeper>() != null)
            {
                SelectedHPUI.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = (float)hunger / (float)Data.MaxHp;
                ShortcutHPUI.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = (float)hunger / (float)Data.MaxHp;
            }

        }
        #endregion


        #region Accessors
        public int MaxHp
        {
            get
            {
                return Data.MaxHp;
            }
        }

        public int CurrentHp
        {
            get { return currentHp; }
            set
            {
                currentHp = value;
                if (currentHp > Data.MaxHp)
                {
                    currentHp = Data.MaxHp;
                    IsAlive = true;
                }
                else if (currentHp <= 0)
                {
                    currentHp = 0;

                    IsAlive = false;
                    Die();
                }
                else
                {
                    IsAlive = true;
                    UpdateHPPanel(currentHp);
                }
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

        public MortalData Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public ParticleSystem DeathParticles
        {
            get
            {
                return deathParticles;
            }

            set
            {
                deathParticles = value;
            }
        }

        public GameObject SelectedHPUI
        {
            get
            {
                return selectedHPUI;
            }

            set
            {
                selectedHPUI = value;
            }
        }

        public GameObject ShortcutHPUI
        {
            get
            {
                return shortcutHPUI;
            }

            set
            {
                shortcutHPUI = value;
            }
        }

        #endregion
    }
}
