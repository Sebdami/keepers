﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DieFeedback : MonoBehaviour {

    PawnInstance owner;
    Face faceInfo;

    GameObject feedback;
    bool isRising = false;
    bool isMovingToPanel = false;
    float timer = 0.0f;
    Vector3 lerpStartPosition;
    Vector3 lerpEndPosition;
    float lerpParam = 0.0f;

    public void PopFeedback(Face _faceInfo, PawnInstance _owner)
    {
        if (feedback == null)
            feedback = Instantiate(GameManager.Instance.PrefabUIUtils.diceFeedback, GameManager.Instance.Ui.transform.GetChild(0));

        owner = _owner;
        faceInfo = _faceInfo;
        feedback.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        feedback.transform.localScale = Vector3.one;
        feedback.GetComponentInChildren<Text>().text = "+ " + faceInfo.Value;

        Transform attributesPanel = GameManager.Instance.GetBattleUI.GetComponent<UIBattleHandler>().GetCharacterPanelIndex(owner).GetChild((int)CharactersPanelChildren.Attributes);
        if (faceInfo.Type == FaceType.Physical)
        {
            feedback.GetComponentInChildren<Image>().sprite = GameManager.Instance.SpriteUtils.spriteAttackSymbol;
            feedback.GetComponentInChildren<Image>().color = new Color(1, 0.5f, 0, 1);
            lerpEndPosition = attributesPanel.GetChild((int)AttributesChildren.Attack).position;
        }
        else if (faceInfo.Type == FaceType.Defensive)
        {
            feedback.GetComponentInChildren<Image>().sprite = GameManager.Instance.SpriteUtils.spriteDefenseSymbol;
            feedback.GetComponentInChildren<Image>().color = new Color(0, 0.5f, 1.0f, 1);
            lerpEndPosition = attributesPanel.GetChild((int)AttributesChildren.Defense).position;
        }
        else if (faceInfo.Type == FaceType.Magical)
        {
            feedback.GetComponentInChildren<Image>().sprite = GameManager.Instance.SpriteUtils.spriteMagicSymbol;
            feedback.GetComponentInChildren<Image>().color = new Color(0.6f, 0, 0.6f, 1);
            lerpEndPosition = attributesPanel.GetChild((int)AttributesChildren.Magic).position;
        }

        isRising = true;
    }

    private void Update()
    {
        if (feedback != null)
        {
            if (isRising)
            {
                if (timer < 0.5f)
                {
                    timer += Time.deltaTime;
                    feedback.transform.localPosition += (Time.deltaTime * 200) * Vector3.up;
                }
                else
                {
                    timer = 0.0f;
                    isRising = false;
                    isMovingToPanel = true;
                    lerpStartPosition = feedback.transform.position;
                    lerpParam = 0.0f;
                }
            }
            else
            {
                if (isMovingToPanel)
                {
                    feedback.transform.position = Vector3.Lerp(lerpStartPosition, lerpEndPosition, lerpParam*2);
                    lerpParam += Time.deltaTime;
                    if (lerpParam >= 0.5f)
                    {
                        isMovingToPanel = false;
                        Destroy(feedback);
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        Destroy(feedback);
    }

    private void OnDestroy()
    {
        Destroy(feedback);
    }
}
