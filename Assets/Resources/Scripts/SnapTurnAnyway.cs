using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SnapTurnAnyway : MonoBehaviour
{
    public float snapAngle = 90.0f;

    public SteamVR_Action_Boolean snapLeftAction = SteamVR_Input.GetBooleanAction("SnapTurnLeft");
    public SteamVR_Action_Boolean snapRightAction = SteamVR_Input.GetBooleanAction("SnapTurnRight");

    public bool fadeScreen = true;
    public float fadeTime = 0.1f;
    public Color screenFadeColor = Color.black;

    public static float teleportLastActiveTime;

    private bool canRotate = true;

    public float canTurnEverySeconds = 0.4f;
    
    private void Update()
    {
        Player player = Player.instance;

        if (canRotate && snapLeftAction != null && snapRightAction != null && snapLeftAction.activeBinding && snapRightAction.activeBinding)
        {
            //only allow snap turning after a quarter second after the last teleport
            if (Time.time < (teleportLastActiveTime + canTurnEverySeconds))
                return;

            // only allow snap turning when not holding something

            bool rightHandValid = player.rightHand.currentAttachedObject == null ||
                (player.rightHand.currentAttachedObject != null
                && player.rightHand.currentAttachedTeleportManager != null
                && player.rightHand.currentAttachedTeleportManager.teleportAllowed);

            bool leftHandValid = player.leftHand.currentAttachedObject == null ||
                (player.leftHand.currentAttachedObject != null
                && player.leftHand.currentAttachedTeleportManager != null
                && player.leftHand.currentAttachedTeleportManager.teleportAllowed);

            
            bool rightHandTurnLeft = snapLeftAction.GetStateDown(SteamVR_Input_Sources.RightHand);
            bool rightHandTurnRight = snapRightAction.GetStateDown(SteamVR_Input_Sources.RightHand);

            if (rightHandTurnLeft)
            {
                RotatePlayer(-snapAngle);
            }
            else if (rightHandTurnRight)
            {
                RotatePlayer(snapAngle);
            }
        }
    }


    private Coroutine rotateCoroutine;
    public void RotatePlayer(float angle)
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }

        rotateCoroutine = StartCoroutine(DoRotatePlayer(angle));
    }

    //-----------------------------------------------------
    private IEnumerator DoRotatePlayer(float angle)
    {
        Player player = Player.instance;

        canRotate = false;

        if (fadeScreen)
        {
            SteamVR_Fade.Start(Color.clear, 0);

            Color tColor = screenFadeColor;
            tColor = tColor.linear * 0.6f;
            SteamVR_Fade.Start(tColor, fadeTime);
        }

        yield return new WaitForSeconds(fadeTime);

        Vector3 playerFeetOffset = player.trackingOriginTransform.position - player.feetPositionGuess;
        player.trackingOriginTransform.position -= playerFeetOffset;
        player.transform.Rotate(Vector3.up, angle);
        playerFeetOffset = Quaternion.Euler(0.0f, angle, 0.0f) * playerFeetOffset;
        player.trackingOriginTransform.position += playerFeetOffset;
        

        if (fadeScreen)
        {
            SteamVR_Fade.Start(Color.clear, fadeTime);
        }
        yield return new WaitForSeconds(canTurnEverySeconds);
        canRotate = true;
    }
}
