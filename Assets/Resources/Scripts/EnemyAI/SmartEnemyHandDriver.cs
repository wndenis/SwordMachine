using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SmartEnemyHandDriver : MonoBehaviour
{
    public Sword heldSword;
    public Transform hand0;
    public Transform hand1;
    public Transform hand2;
    [Header("Auxiliary transforms")]
    public Transform awaitHandTransform;
    public Transform armingHandTransform;
    public Transform armedSwordTransform;
    
    [HideInInspector] public SmartEnemyDriver smartEnemyDriver;
    
    
    
    //================================================================================================================
    public Rigidbody Hand0Rigidbody { get; private set; }
    public Rigidbody Hand1Rigidbody { get; private set; }
    public Rigidbody Hand2Rigidbody { get; private set; }
    public ConstantForce swordConstantForce { get; private set; }
    
    private void Start()
    {
        // Hand0Rigidbody = hand0.GetComponent<Rigidbody>();
        // Hand1Rigidbody = hand1.GetComponent<Rigidbody>();
        // Hand2Rigidbody = hand2.GetComponent<Rigidbody>();
        // swordConstantForce = heldSword.GetComponent<ConstantForce>();
    }
}
