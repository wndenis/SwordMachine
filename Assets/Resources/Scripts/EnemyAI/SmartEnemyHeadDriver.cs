using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class SmartEnemyHeadDriver : MonoBehaviour
{
    public Transform Head;
    [HideInInspector] public SmartEnemyDriver SmartEnemyDriver;
    [HideInInspector] public Transform TargetToLookAt;
    public Rigidbody HeadRigidbody { get; private set; }

    public float SightSharpness = 0.1f;
    public Transform defaultTargetToLookAt;
    
    [Header("IK settings")]
    [Range(0, 1)]
    public float weight;
    public Animator animator;

    void Start()
    {
        HeadRigidbody = Head.GetComponent<Rigidbody>();
    }
    
    public Transform ScanForTargett()
    {
        Transform target = null;
        var nearestDistance = SmartEnemyDriver.sightDistance;
        var targetColliders = new Collider[90];
        bool correctTarget;
        Physics.OverlapSphereNonAlloc(transform.position, SmartEnemyDriver.sightDistance, targetColliders, SmartEnemyDriver.EnemyLayers);

        foreach (var elem in targetColliders)
        {
            if (elem == null || SmartEnemyDriver == null)
                continue;
            var unit = elem.GetComponent<PlayableUnit>();
            if (unit != null)
                if (!unit.IsAlive)
                    continue;
            
            //var pointOnTarget = GetVisiblePointOnTarget(elem.transform);
            var pointOnTarget = elem.transform.position;
            var distanceToPointOnTarget = (pointOnTarget - Head.position).magnitude;

            if (distanceToPointOnTarget <= 0)
            {
                Debug.DrawRay(Head.position, pointOnTarget - Head.position, Color.red, 0.1f);
                continue;
            }

            if (distanceToPointOnTarget < nearestDistance)
            {
                Debug.DrawRay(Head.position, pointOnTarget - Head.position, Color.green, 0.1f);
                nearestDistance = distanceToPointOnTarget;
                target = elem.transform;
            }
            else
            {
                Debug.DrawRay(Head.position, pointOnTarget - Head.position, Color.cyan, 0.1f);
            }
        }
        return target;
    }
    private Vector3 GetVisiblePointOnTarget(Transform targetObject)
    {
        RaycastHit raycastHit;
        var hit = false;
        var offset = new Vector3(0, 0, 0);
        var subDirection = new Vector3();

        const float step = 0.5f;
        const float maxOffset = 1f;

        //scanning near center of target
        for (var xOffset = -maxOffset; xOffset <= maxOffset && !hit; xOffset += step)
        for (var yOffset = -maxOffset; yOffset <= maxOffset && !hit; yOffset += step)
        for (var zOffset = -maxOffset; zOffset <= maxOffset && !hit; zOffset += step)
        {
            offset.Set(xOffset, yOffset, zOffset);
            //todo: rewrite with vector rotation
            subDirection = targetObject.position - Head.position;
            Debug.DrawRay(Head.position, subDirection, Color.grey, 0.1f);
            hit = Physics.Raycast(Head.position, subDirection, out raycastHit,
                SmartEnemyDriver.sightDistance);

            if (hit && raycastHit.transform != targetObject && raycastHit.transform.CompareTag("Sword"))
            {
                hit = Physics.Raycast(raycastHit.point + subDirection.normalized * 0.05f, subDirection, out raycastHit,
                    SmartEnemyDriver.sightDistance - 0.05f);
            }

            if (hit && raycastHit.transform == targetObject)
            {
                return raycastHit.point;
            }
        }

        return Vector3.zero;
    }
}
