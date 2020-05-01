using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Resources.Scripts;
using RootMotion.Demos;
using RootMotion.Dynamics;
using UnityEngine;
using StateMachineTools;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class SmartEnemyDriver : MonoBehaviour
{
    public UnitAI unitAi;
    public LayerMask EnemyLayers;
    // public SmartEnemyHeadDriver HeadDriver;
    // public SmartEnemyHandDriver HandDriver;

    [Space(25)]
    public PuppetMaster puppetMaster;
    public BehaviourPuppet puppetBehaviour;
    public Animator animator;
    //HEAD
    [Header("Head")]
    public float sightDistance = 40f;
    public float sightSharpness = 0.1f;
    public Rigidbody headRigidbody;
    // [Header("IK settings")] 
    [Range(0, 1)] 
    public float sightWeightIK = 0.65f;
    //===
    
    //BODY
    [Header("Body")]
    public SmartEnemyBodyDriver BodyDriver;
    //===

    //HAND
    [Header("Hands")] 
    public SciFiSwordPropMelee swordProp;

    public Rigidbody Hand0Rigidbody;
    public Rigidbody Hand1Rigidbody;
    public Rigidbody Hand2Rigidbody;
    [Tooltip("Auxiliary transform")]
    public Transform awaitHandTransform;
    [Tooltip("Auxiliary transform")]
    public Transform armingHandTransform;
    // [Tooltip("Auxiliary transform")]
    // public Transform armedSwordTransform;
    
    public float swordAcceleration = 25f;
    
    //===
    [Header("AI settings")]
    [Range(0, 1)]
    [Tooltip("Bigger is more brave\n1 is offensive behaviour\n0 is only defensive")]
    public float courage = 0.5f;
    [Range(0, 1)]
    [Tooltip("Bigger is better\n1 is perfect skill\n0 is no skill")]
    public float skill = 1f;

    
    public float DefendDistance = 0.475f;  // from our body to sword
    // public float MinimalAttackDistance = 0.85f;
    public float BattleDistance = 1.85f + 0.35f;
    public float BattleDistanceThreshold = 0.8f;
    public float BattleActionCooldown = 0.15f;


    [HideInInspector]
    public Sword nearestEnemyWeapon;
    [HideInInspector]
    public PlayableUnit nearestEnemy;
    
    // [HideInInspector]
    // public Transform targetToAttack;
    // [HideInInspector]
    // public Vector3 targetToAttackStatic;
    [HideInInspector]
    [NotNullAttribute]
    public MoveTarget targetToLook { get; private set; }
    // [HideInInspector]
    // public Transform targetToMove;
    [HideInInspector]
    public float lastActionTime;
    public bool HasTargets => HasWeaponNearby || HasEnemyNearby;

    public bool HasWeaponNearby { get; private set; }

    public bool HasEnemyNearby { get; private set; }
    //=======PRIVATE=======

    [HideInInspector]
    public StateMachine<SmartEnemyDriver> stateMachine;
    [HideInInspector] public Coroutine currentCoroutine;

    void Start()
    {
        // unitAi = GetComponent<UnitAI>();
        stateMachine = new StateMachine<SmartEnemyDriver>(this);
        lastActionTime = -BattleActionCooldown;
        targetToLook = new MoveTarget();
        BodyDriver.character.propRoot.currentProp = swordProp;
        unitAi.PickupSword(swordProp.sword);

        // HeadDriver.SmartEnemyDriver = this;
        // HandDriver.smartEnemyDriver = this;
        // BodyDriver.SmartEnemyDriver = this;
        
        stateMachine.ChangeState(Idle.Instance);
    }

    public bool CheckForCooldown(float cooldown)
    {
        return Time.time - lastActionTime > cooldown;
    }

    public void CustomHandControl(bool active)
    {
        var pinWeight = 0.1f;
        var muscleWeight = 0.2f;
        // var pinWeight = 0f;
        // var muscleWeight = 0f;
        if (!active)
        {
            pinWeight = 1f;
            muscleWeight = 1f;
        }
        for (int i = 13; i < puppetMaster.muscles.Length; i++)
        {
            puppetMaster.muscles[i].props.pinWeight = pinWeight;
            puppetMaster.muscles[i].props.muscleWeight = muscleWeight;
        }
        
    }

    public TextMesh text;
    private void Update()
    {
        text.text = ((EnemyState)(stateMachine.currentState)).type;
    }

    private struct WeaponAndUnit
    {
        public Sword sword;
        public PlayableUnit unit;
    }
    public void UpdateTargets()
    {
        var res = new WeaponAndUnit();

        var ourTransform = unitAi.BodyTransform;
        var nearestUnitDistance = sightDistance * sightDistance;
        var smallestWeaponAngle = 180f;

        var handPos = Hand2Rigidbody.position;

        var targetColliders = new Collider[90];
        Physics.OverlapSphereNonAlloc(ourTransform.position + ourTransform.forward * 5f, sightDistance, targetColliders, EnemyLayers);
        
        
        foreach (var elem in targetColliders)
        {
            //check tags and null
            if (elem == null)
                continue;

            if (elem.CompareTag("Unit"))
            {
                //save as unit
                var unit = elem.GetComponent<PlayableUnit>();
                if (unit != null)
                    if (!unit.IsAlive)
                        continue;
                var dist = (handPos - elem.transform.position).sqrMagnitude;
                if (dist < nearestUnitDistance)
                {
                    res.unit = unit;
                    nearestUnitDistance = dist;
                }
            }
            else if (elem.CompareTag("Target") || elem.CompareTag("Weapon"))
            {
                //save as weapon
                print(elem.name);
                var weapon = elem.GetComponent<Sword>();
                if (weapon == null)
                    continue;
                
                if (weapon.Rigidbody.velocity.magnitude < 0.2f) // ignore slow objects
                    continue;

                if (Time.time - weapon.trajectoryApproximator.buffer.bornTime < 0.055f) // ignore new spawned objects
                    continue;
                
                

                var weaponPosition = weapon.Rigidbody.position;
                var ourPosition = ourTransform.position;
                
                var currentDist = (ourPosition - weaponPosition).sqrMagnitude;
                var maxDistance = BattleDistance + BattleDistanceThreshold;
                if (currentDist > maxDistance * maxDistance) // ignore if it really far (sqr distance)
                    continue;
                
                var futurePoint = weaponPosition + weapon.Rigidbody.velocity * 0.1f;

                var futureDist = (ourPosition - futurePoint).sqrMagnitude;

                if (futureDist > currentDist) // ignore objects moving away from us
                    continue;

                var fromCurrentToFuture = futurePoint - weaponPosition;
                var fromCurrentToBody = ourPosition - weaponPosition;
                var angle = Vector3.Angle(fromCurrentToFuture, fromCurrentToBody);
                    
                // var dist = (handPos - elem.transform.position).sqrMagnitude;
                if (angle < smallestWeaponAngle)
                {
                    res.sword = weapon;
                    smallestWeaponAngle = angle;
                }
            }
        }
        nearestEnemy = res.unit;
        nearestEnemyWeapon = res.sword;
        HasWeaponNearby = nearestEnemyWeapon != null;
        HasEnemyNearby = nearestEnemy != null;
        
        var headPos = headRigidbody.position;
        
        if (HasEnemyNearby)
            Debug.DrawRay(headPos, res.unit.transform.position - headPos, new Color(0.16f, 0.36f, 0.63f), 1f);
        if (HasWeaponNearby)
            Debug.DrawRay(headPos, res.sword.transform.position - headPos, new Color(0.75f, 0.45f, 0.8f), 1f);
        
        // TogglePhysics(HasTargets); todo: enable it later
    }

    public bool ForceUsePhysics = false;

    private void TogglePhysics(bool physicsEnabled)
    {
        if (ForceUsePhysics)
            return;

        puppetMaster.mode = physicsEnabled ? PuppetMaster.Mode.Active : PuppetMaster.Mode.Kinematic;
    }


    private float lookWeightMult = 1f;
    private bool lookFading;
    private IEnumerator WeightFade()
    {
        if (lookFading)
            yield break;
        lookFading = true;
        const float dur = 0.3f;
        for (var t = 0f; t < dur; t += Time.deltaTime)
        {
            lookWeightMult = Mathf.Lerp(lookWeightMult, 0, t / dur);
            yield return null;
        }

        lookFading = false;

    }
    public void UpdateIK(int layerIndex)
    {
        if (!targetToLook.isEmpty)
        {
            lookWeightMult = 1f;
        }
        else
        {
            if (!lookFading)
                StartCoroutine(WeightFade());
        }
        
        if (puppetMaster.state != PuppetMaster.State.Alive)
            if (puppetBehaviour.state != BehaviourPuppet.State.Puppet)
                lookWeightMult = 0f;
        
        if (lookWeightMult < 0.01f) return;
        //todo: debug lookweightMult
        //todo: tune battle distance
        
        animator.SetLookAtPosition(targetToLook.position);
        animator.SetLookAtWeight(sightWeightIK * lookWeightMult);
    }

    //TODO: on puppet master lose balance set state STUN
}
