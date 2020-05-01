using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

public class SwordUseWithSoundNoVr : MonoBehaviour
{
    public SciFiSwordAnimation sword;
    public SteamVR_Action_Boolean pressed;

    private void Awake()
    {
        pressed[SteamVR_Input_Sources.Any].onStateDown += PlayAnimation;
    }
    
    private void PlayAnimation(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        sword.Switch();
    }
}

