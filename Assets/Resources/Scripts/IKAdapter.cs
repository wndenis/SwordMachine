using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKAdapter : MonoBehaviour
{
    public SmartEnemyDriver smartEnemyDriver;
    // Start is called before the first frame update
    private void OnAnimatorIK(int layerIndex)
    {
        smartEnemyDriver.UpdateIK(layerIndex);
    }
}
