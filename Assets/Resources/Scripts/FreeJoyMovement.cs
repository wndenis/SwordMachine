using System;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class FreeJoyMovement : MonoBehaviour
{
    private SteamVR_Action_Vector2 movementAction = SteamVR_Actions.default_Movement;
    private float _mMoveSpeed = 2.5f;
    private float _mHorizontalTurnSpeed = 180f;
    private float _mVerticalTurnSpeed = 2.5f;
    private bool _mInverted = false;
    private const float VERTICAL_LIMIT = 60f;
    private Player player;

    float GetAngle(float input)
    {
        if (input < 0f)
        {
            return -Mathf.LerpAngle(0, VERTICAL_LIMIT, -input);
        }
        else if (input > 0f)
        {
            return Mathf.LerpAngle(0, VERTICAL_LIMIT, input);
        }
        return 0f;
    }

    private void Start()
    {
        player = Player.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            return;
        }
        var dir = movementAction.GetAxis(SteamVR_Input_Sources.LeftHand);
        var orientation = player.hmdTransform.rotation;
        var moveDirection = orientation * Vector3.forward * dir.y + orientation * Vector3.right * dir.x;
        var playerTransform = player.transform;
        var pos = playerTransform.position;
        pos.x += moveDirection.x * _mMoveSpeed * Time.deltaTime;
        pos.z += moveDirection.z * _mMoveSpeed * Time.deltaTime;
        playerTransform.position = pos;
    }
}
