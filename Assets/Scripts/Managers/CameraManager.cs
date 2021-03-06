﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initial -0.03, 1.08, -3.07
/// </summary>
public class CameraManager : MonoBehaviour {

    enum eZoomState
    {
        idle = 0,
        forward = 1,
        backward = -1
    }

    public enum CameraState
    {
        Close,
        Far
    }

    struct Transformation
    {
        public Quaternion rotation;
        public Vector3 position;
    }

    // *********************************
    // Camera parameters
    [Header("Camera Controls")]

    // Camera Zoom
    eZoomState zoomState = eZoomState.idle;
    [SerializeField] float fZoomSpeed = 1;
    [SerializeField] float fLerpNotch = 0.1f;
    Transformation tFar, tClose;
    float fZoomLerp = 1;
    float fZoomLerpOrigin = 1;
    float fLerpTarget = 1;
    [SerializeField]
    float farSpeedMultiplier = 1.5f;
    Vector3 newPosition;
    public CameraState state = CameraState.Close;

    // Camera Drag
    [SerializeField] float fDragFactor = 0.1f;
    [SerializeField]
    float fKeySpeed = 5.0f;
    bool bIsDraging = false;
    Vector3 v3DragOrigin;
    Vector3 closeToFar;
    bool needOffset;

    bool isSpecialUpdate = false;

    // Camera adapters
    public List<GreyTileCameraAdapter> greyTileCameraAdapters = new List<GreyTileCameraAdapter>();
    public List<SelectionPointerCameraAdapter> selectionPointerCameraAdapters = new List<SelectionPointerCameraAdapter>();
    public List<WorldspaceCanvasCameraAdapter> worldspaceCanvasCameraAdapters = new List<WorldspaceCanvasCameraAdapter>();

    bool isFirstCallToUpdatePosition = false;
    bool hasMainQuestBeenShown = false;


    // Camera behaviour
    bool isFollowingKeeper = true;

    // *********************************
    enum CameraBound
    {
        North,
        East,
        South,
        West
    }


    [SerializeField]
    Transform cameraBounds;

    Tile activeTile;
    Vector3 positionFromATileClose;
    bool isUpdateNeeded = false;
    Vector3 oldPosition;
    float lerpParameter = 0.0f;

    Vector3 referenceTClosePosition;

    #region Accessors
    public float FZoomLerp
    {
        get
        {
            return fZoomLerp;
        }

        private set
        {
            fZoomLerp = value;
            foreach (GreyTileCameraAdapter ca in greyTileCameraAdapters)
                ca.RecalculateOrientation(Camera.main);

            foreach (SelectionPointerCameraAdapter ca in selectionPointerCameraAdapters)
                ca.RecalculateOrientationAndScale();

            foreach (WorldspaceCanvasCameraAdapter ca in worldspaceCanvasCameraAdapters)
                ca.RecalculateActionCanvas(Camera.main);
        }
    }

    public Tile ActiveTile
    {
        get
        {
            return activeTile;
        }

        set
        {
            activeTile = value;
        }
    }

    public bool IsFollowingKeeper
    {
        get
        {
            return isFollowingKeeper;
        }

        set
        {
            isFollowingKeeper = value;
            GameManager.Instance.GameScreens.optionsMenu.GetComponent<PanelOptionsReference>().FollowingCharacterToggleButton.isOn = value;
        }
    }
    #endregion

    public void Start()
    {
        positionFromATileClose = transform.position;

        GameObject goCameraCloseRef = GameObject.Find("CameraCloseRef");
        tClose.rotation = goCameraCloseRef.transform.rotation;
        tClose.position = goCameraCloseRef.transform.position;
        referenceTClosePosition = tClose.position;
        Destroy(goCameraCloseRef);

        GameObject goCameraFarRef = GameObject.Find("CameraFarRef");
        tFar.rotation = goCameraFarRef.transform.rotation;
        tFar.position = goCameraFarRef.transform.position;
        Destroy(goCameraFarRef);

        closeToFar = tFar.position - tClose.position;

        zoomState = eZoomState.forward;
        state = CameraState.Close;
        FZoomLerp = .85f;
        fLerpTarget = .85f;
        fZoomLerpOrigin = .85f;
        isUpdateNeeded = true;
        isFirstCallToUpdatePosition = false;
        hasMainQuestBeenShown = false;

        GameManager.Instance.RegisterCameraManager(this);
    }
    public void UpdateCameraPosition(PawnInstance pi)
    {
        isUpdateNeeded = true;
        oldPosition = tClose.position;

        activeTile = pi.CurrentTile;
    }

    // WTF fonction spaghetti
    public void UpdateCameraPositionWithOffset(Vector3 _newTileCenter, Tile _newTile, Direction directionDuDeplacement)
    {
        isSpecialUpdate = true;
        oldPosition = tClose.position;
        activeTile = _newTile.Neighbors[(int)Utils.GetOppositeDirection(directionDuDeplacement)];
        if (directionDuDeplacement == Direction.South || directionDuDeplacement == Direction.South_East || directionDuDeplacement == Direction.South_West)
        {
            newPosition = _newTileCenter + Vector3.back;
        } else
        {
            if(activeTile != null)
                newPosition = positionFromATileClose + activeTile.transform.position ;
        }

        fZoomLerpOrigin = fZoomLerp;

        fLerpTarget = 1 - (2* fLerpNotch);
        zoomState = eZoomState.backward;
    }


    public void UpdateCameraPosition(Tile targetTile)
    {
        isUpdateNeeded = true;
        oldPosition = tClose.position;
        activeTile = targetTile;
        if (!isFirstCallToUpdatePosition && !hasMainQuestBeenShown)
            isFirstCallToUpdatePosition = true;
    }

    public void UpdateCameraPosition(Vector3 _newPosition)
    {
        isUpdateNeeded = true;
        oldPosition = tClose.position;
        newPosition = _newPosition;
        zoomState = eZoomState.forward;
        fZoomLerpOrigin = fZoomLerp;
        fLerpTarget = 1.0f;
    }

    public void UpdateCameraPositionExitBattle()
    {
        isUpdateNeeded = true;
        oldPosition = tClose.position;
        newPosition = referenceTClosePosition;
        tClose.position = referenceTClosePosition;
    }

    void Update()
    {
        if (GameManager.Instance != null && 
            ((GameManager.Instance.CurrentState == GameState.Normal || GameManager.Instance.CurrentState == GameState.InPause) 
            || (GameManager.Instance.CurrentState == GameState.InTuto && TutoManager.s_instance.PlayingSequence != null && (TutoManager.s_instance.PlayingSequence.GetType() == typeof(SeqMoraleExplained) || TutoManager.s_instance.PlayingSequence.GetType() == typeof(seqTeamCrocket)))))
        {
            if (isUpdateNeeded)
            {
                lerpParameter += Time.deltaTime;

                Vector3 v3NewPos = Vector3.Lerp(oldPosition, positionFromATileClose + activeTile.transform.position + Vector3.back, Mathf.Min(lerpParameter, 1.0f));

                v3NewPos.y = tClose.position.y;
                tClose.position = v3NewPos;

                tFar.position = tClose.position + closeToFar;

                transform.position = Vector3.Lerp(tFar.position, tClose.position, fZoomLerp);
                transform.rotation = Quaternion.Lerp(tFar.rotation, tClose.rotation, fZoomLerp);

                if (lerpParameter >= 1.0f)
                {
                    isUpdateNeeded = false;
                    oldPosition = Vector3.zero;
                    lerpParameter -= 1.0f;
                    if (isFirstCallToUpdatePosition && !hasMainQuestBeenShown)
                    {
                        // show main quest
                        GameManager.Instance.ShowMainQuest();
                        hasMainQuestBeenShown = true;
                    }

                    // Ajout par rémi
                    if ((zoomState == eZoomState.forward && fZoomLerp > fLerpTarget) || (zoomState == eZoomState.backward && fZoomLerp < fLerpTarget) || (fLerpTarget == fZoomLerp))
                    {
                        FZoomLerp = fLerpTarget;
                        zoomState = eZoomState.idle;
                    }
                }

                //Vector3 pos = transform.position;
                //pos.x = Mathf.Clamp(pos.x, cameraBounds.GetChild((int)CameraBound.West).position.x, cameraBounds.GetChild((int)CameraBound.East).position.x);
                //pos.z = Mathf.Clamp(pos.z, cameraBounds.GetChild((int)CameraBound.South).position.z, cameraBounds.GetChild((int)CameraBound.North).position.z);
                //transform.position = pos;
            } else if (isSpecialUpdate == true)
            {
                lerpParameter += Time.deltaTime;

                Vector3 v3NewPos = Vector3.Lerp(oldPosition, newPosition, Mathf.Min(lerpParameter, 1.0f));

                v3NewPos.y = tClose.position.y;
                tClose.position = v3NewPos;

                tFar.position = tClose.position + closeToFar;

                transform.position = Vector3.Lerp(tFar.position, tClose.position, fZoomLerp);
                transform.rotation = Quaternion.Lerp(tFar.rotation, tClose.rotation, fZoomLerp);

                if (lerpParameter >= 1.0f)
                {
                    oldPosition = Vector3.zero;
                    lerpParameter = 0.0f;

                    isSpecialUpdate = false;
                    newPosition = Vector3.zero;
                }


            }

            if (GameManager.Instance.CurrentState != GameState.InPause)
                Controls();

            if (zoomState != eZoomState.idle)
            {
                UpdateCamZoom();
            } 

        }
        else if (GameManager.Instance.CurrentState == GameState.InBattle
            || (GameManager.Instance.CurrentState == GameState.InTuto && TutoManager.s_instance.PlayingSequence != null && TutoManager.s_instance.PlayingSequence.GetType() == typeof(SeqTutoCombat)))
        {
            if (isUpdateNeeded)
            {
                lerpParameter += Time.deltaTime;

                Vector3 v3NewPos = Vector3.Lerp(oldPosition, newPosition + activeTile.transform.position, Mathf.Min(lerpParameter, 1.0f));

                v3NewPos.y = tClose.position.y;
                tClose.position = v3NewPos;

                tFar.position = tClose.position + closeToFar;

                transform.position = Vector3.Lerp(tFar.position, tClose.position, fZoomLerp);
                transform.rotation = Quaternion.Lerp(tFar.rotation, tClose.rotation, fZoomLerp);

                if (lerpParameter >= 1.0f)
                {
                    isUpdateNeeded = false;
                    oldPosition = Vector3.zero;
                    lerpParameter = 0.0f;
                }
            }

            if (zoomState != eZoomState.idle)
            {
                UpdateCamZoom();
            }
        }
        foreach (GreyTileCameraAdapter ca in greyTileCameraAdapters)
            ca.RecalculateOrientation(Camera.main);

        foreach (SelectionPointerCameraAdapter ca in selectionPointerCameraAdapters)
            ca.RecalculateOrientationAndScale();

        foreach (WorldspaceCanvasCameraAdapter ca in worldspaceCanvasCameraAdapters)
            ca.RecalculateActionCanvas(Camera.main);

        if (fZoomLerp < 0.6f)
        {
            state = CameraState.Far;
        }
        else
        {
            state = CameraState.Close;
        }
    }
    private void Controls()
    {
        ControlZoom();

        ControlDrag();

        ControlKeys();
    }

    private void ControlZoom()
    {
        float fScrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (fScrollValue != 0)
        {
            if ((fScrollValue > 0 && fZoomLerp < 1) || (fScrollValue < 0 && fZoomLerp > 0))
            {
                switch (zoomState)
                {
                    case eZoomState.idle:
                        fZoomLerpOrigin = fZoomLerp;
                        fLerpTarget = fZoomLerp;
                        break;

                    case eZoomState.forward:
                        if (fScrollValue < 0)
                        {
                            fZoomLerpOrigin = fZoomLerp;
                            fLerpTarget = fZoomLerp;
                        }
                        break;
                    case eZoomState.backward:
                        if (fScrollValue > 0)
                        {
                            fZoomLerpOrigin = fZoomLerp;
                            fLerpTarget = fZoomLerp;
                        }
                        break;
                    default:
                        break;
                }

                fLerpTarget += fLerpNotch * (fScrollValue * 10);

                if (fLerpTarget > 1)
                {
                    fLerpTarget = 1;
                }
                else if (fLerpTarget < 0)
                {
                    fLerpTarget = 0;
                }

                if (fScrollValue > 0)
                {
                    zoomState = eZoomState.forward;
                }
                else
                {
                    zoomState = eZoomState.backward;
                }
            }
        }
        ControlZoomKeyboard();
    }

    // TODO: find a better way
    private void ControlZoomKeyboard()
    {
        if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.PageUp))
        {
            if (fZoomLerp < 1 && fZoomLerp > 0)
            {
                fZoomLerpOrigin = fZoomLerp;
                fLerpTarget = fZoomLerp; 
                fLerpTarget += fLerpNotch;

                if (fLerpTarget > 1)
                {
                    fLerpTarget = 1;
                }
                else if (fLerpTarget < 0)
                {
                    fLerpTarget = 0;
                }

                zoomState = eZoomState.forward;
            }
        }

        if (Input.GetKey(KeyCode.PageDown) || Input.GetKey(KeyCode.KeypadMinus))
        {
            if (fZoomLerp < 1 && fZoomLerp > 0)
            {
                fZoomLerpOrigin = fZoomLerp;
                fLerpTarget = fZoomLerp;
                fLerpTarget -= fLerpNotch;

                if (fLerpTarget > 1)
                {
                    fLerpTarget = 1;
                }
                else if (fLerpTarget < 0)
                {
                    fLerpTarget = 0;
                }

                zoomState = eZoomState.backward;
            }
        }
    }

    void OnDrawGizmos()
    {
        float north = cameraBounds.GetChild((int)CameraBound.North).position.z;
        float south = cameraBounds.GetChild((int)CameraBound.South).position.z;
        float east = cameraBounds.GetChild((int)CameraBound.East).position.x;
        float west = cameraBounds.GetChild((int)CameraBound.West).position.x;

        Gizmos.color = Color.magenta;

        Gizmos.DrawLine(new Vector3(west, 0, north), new Vector3(east, 0, north));
        Gizmos.DrawLine(new Vector3(east, 0, north), new Vector3(east, 0, south));
        Gizmos.DrawLine(new Vector3(east, 0, south), new Vector3(west, 0, south));
        Gizmos.DrawLine(new Vector3(west, 0, south), new Vector3(west, 0, north));
    }

    private void ControlKeys()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            isUpdateNeeded = false;
            oldPosition = Vector3.zero;
            lerpParameter = 0.0f;

            Vector3 v3IncrementPos = new Vector3( Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            float speed = fKeySpeed * Time.deltaTime;
            if (state == CameraState.Far)
                speed *= farSpeedMultiplier;
            v3IncrementPos = v3IncrementPos.normalized * speed; 
            if (!((tClose.position + v3IncrementPos).z > cameraBounds.GetChild((int)CameraBound.South).position.z &&
               (tClose.position + v3IncrementPos).z < cameraBounds.GetChild((int)CameraBound.North).position.z))
            {
                v3IncrementPos.z = 0.0f;
            }

            transform.position += v3IncrementPos;
            tClose.position += v3IncrementPos;
            tFar.position += v3IncrementPos;

            //v3DragOrigin = Input.mousePosition;
        }
       
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, cameraBounds.GetChild((int)CameraBound.West).position.x, cameraBounds.GetChild((int)CameraBound.East).position.x);
        //pos.z = Mathf.Clamp(pos.z, cameraBounds.GetChild((int)CameraBound.South).position.z, cameraBounds.GetChild((int)CameraBound.North).position.z);
        transform.position = pos;

        pos = tClose.position;
        pos.x = Mathf.Clamp(pos.x, cameraBounds.GetChild((int)CameraBound.West).position.x, cameraBounds.GetChild((int)CameraBound.East).position.x);
        tClose.position = pos;

        pos = tFar.position;
        pos.x = Mathf.Clamp(pos.x, cameraBounds.GetChild((int)CameraBound.West).position.x, cameraBounds.GetChild((int)CameraBound.East).position.x);
        tFar.position = pos;
    }

    private void ControlDrag()
    {
        if (Input.GetMouseButtonDown(2) && !bIsDraging)
        {
            isUpdateNeeded = false;
            oldPosition = Vector3.zero;
            lerpParameter = 0.0f;

            v3DragOrigin = Input.mousePosition;

            bIsDraging = true;
        }
        else if (Input.GetMouseButton(2) && bIsDraging)
        {

            Vector3 v3DragDiff = v3DragOrigin - Input.mousePosition;

            Vector3 v3IncrementPos = new Vector3(v3DragDiff.x * fDragFactor, 0, v3DragDiff.y * fDragFactor);
            if(!((tClose.position + v3IncrementPos).z > cameraBounds.GetChild((int)CameraBound.South).position.z &&
               (tClose.position + v3IncrementPos).z < cameraBounds.GetChild((int)CameraBound.North).position.z))
            {
                v3IncrementPos.z = 0.0f;
            }

            transform.position += v3IncrementPos;
            tClose.position += v3IncrementPos;
            tFar.position += v3IncrementPos;

            v3DragOrigin = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(2) && bIsDraging)
        {
            bIsDraging = false;
        }
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, cameraBounds.GetChild((int)CameraBound.West).position.x, cameraBounds.GetChild((int)CameraBound.East).position.x);
        //pos.z = Mathf.Clamp(pos.z, cameraBounds.GetChild((int)CameraBound.South).position.z, cameraBounds.GetChild((int)CameraBound.North).position.z);
        transform.position = pos;

        pos = tClose.position;
        pos.x = Mathf.Clamp(pos.x, cameraBounds.GetChild((int)CameraBound.West).position.x, cameraBounds.GetChild((int)CameraBound.East).position.x);
        tClose.position = pos;

        pos = tFar.position;
        pos.x = Mathf.Clamp(pos.x, cameraBounds.GetChild((int)CameraBound.West).position.x, cameraBounds.GetChild((int)CameraBound.East).position.x);
        tFar.position = pos;
    }

    private void UpdateCamZoom()
    {
        FZoomLerp = fZoomLerp + (fLerpTarget - fZoomLerpOrigin) * fZoomSpeed * Time.unscaledDeltaTime;

        if((zoomState == eZoomState.forward && fZoomLerp > fLerpTarget) || (zoomState == eZoomState.backward && fZoomLerp < fLerpTarget) || (fLerpTarget == fZoomLerp))
        {
            FZoomLerp = fLerpTarget;
            zoomState = eZoomState.idle;
        }

        transform.position = Vector3.Lerp(tFar.position, tClose.position, fZoomLerp);
        transform.rotation = Quaternion.Lerp(tFar.rotation, tClose.rotation, fZoomLerp);
        
    }
}
