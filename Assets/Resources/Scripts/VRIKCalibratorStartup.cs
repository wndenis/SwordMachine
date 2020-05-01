﻿using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class VRIKCalibratorStartup : MonoBehaviour
{
    [Tooltip("Reference to the VRIK component on the avatar.")] public VRIK ik;
    [Tooltip("The settings for VRIK calibration.")] public VRIKCalibrator.Settings settings;
    [Tooltip("The HMD.")] public Transform headTracker;
    [Tooltip("(Optional) A tracker placed anywhere on the body of the player, preferrably close to the pelvis, on the belt area.")] public Transform bodyTracker;
    [Tooltip("(Optional) A tracker or hand controller device placed anywhere on or in the player's left hand.")] public Transform leftHandTracker;
    [Tooltip("(Optional) A tracker or hand controller device placed anywhere on or in the player's right hand.")] public Transform rightHandTracker;
    [Tooltip("(Optional) A tracker placed anywhere on the ankle or toes of the player's left leg.")] public Transform leftFootTracker;
    [Tooltip("(Optional) A tracker placed anywhere on the ankle or toes of the player's right leg.")] public Transform rightFootTracker;

    [Header("Data stored by Calibration")]
    public VRIKCalibrator.CalibrationData data = new VRIKCalibrator.CalibrationData();

    private IEnumerator Start()
    {
        headTracker = ik.solver.spine.headTarget;
        bodyTracker = ik.solver.spine.pelvisTarget;
        leftHandTracker = ik.solver.leftArm.target;
        rightHandTracker = ik.solver.rightArm.target;
        rightFootTracker = ik.solver.rightLeg.target;
        leftFootTracker = ik.solver.leftLeg.target;
        yield break;
        while (true)
        {
            yield return new WaitForSeconds(2f);
            data = VRIKCalibrator.Calibrate(ik, settings, headTracker, bodyTracker, leftHandTracker, rightHandTracker, leftFootTracker, rightFootTracker);
            yield break;
        }
    }
}
