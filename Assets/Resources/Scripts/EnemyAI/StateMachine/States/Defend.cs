using System.Collections;
using Resources.Scripts;
using UnityEngine;

namespace StateMachineTools
{
    public class Defend : EnemyState
    {
        private static Defend instance;

        private Defend()
        {
            if (instance != null)
                return;
            instance = this;
            type = "Defend";
        }

        public static Defend Instance
        {
            get
            {
                if (instance == null)
                    new Defend();
                return instance;
            }
        }
        
        // private PlayableUnit FindPlayableUnit(Transform target)
        // {
        //     var unit = target.GetComponent<PlayableUnit>();
        //     if (unit)
        //         return unit;
        //     unit = target.GetComponentInParent<PlayableUnit>();
        //     return unit;
        // } //TODO: remove duplicates

        public override IEnumerator StateLogic(SmartEnemyDriver owner)
        {
            var currentSpeed = 0f;
            var maxSpeed = 0f;
            var targetSword = owner.nearestEnemyWeapon.trajectoryApproximator.PredictIntersection(0.85f,
                owner.unitAi.Sword.TipTransform.position, owner.swordAcceleration);

            Debug.DrawLine(owner.unitAi.Sword.TipTransform.position, targetSword, Color.red, 0.25f);
            owner.lastActionTime = Time.time;

            var fromBodyToSword = owner.unitAi.BodyTransform.position - targetSword;
            var backDirection = fromBodyToSword.normalized;
            var rndAngle = Random.Range(-50f, 50f);
            var backPoint = owner.unitAi.BodyTransform.position + (Quaternion.Euler(0f, rndAngle, 0f) * backDirection).normalized * 70.9f; //was 0.65

            //todo: do rushes
            owner.targetToLook.SetTarget(owner.nearestEnemyWeapon.TipTransform);
            owner.CustomHandControl(true);
            var enemySwordRotationTarget = new MoveTarget(owner.nearestEnemyWeapon.TipTransform);
            owner.BodyDriver.LockRotationToTarget(enemySwordRotationTarget);
            if (Random.Range(0f, 1f) < (1 - owner.courage) || fromBodyToSword.magnitude < owner.DefendDistance)
            {
                var stepBackMoveTarget = new MoveTarget(backPoint);
                owner.BodyDriver.AddTask(new SmartEnemyBodyDriver.MovementTask(stepBackMoveTarget, 1f, 
                    0.05f, 0.05f, SmartEnemyBodyDriver.BotMovementType.Strafe));
            }
            
            var skillOffset = Random.insideUnitSphere;
            skillOffset.y *= 2f;
            skillOffset *= 1.05f - owner.skill;

            while (currentSpeed >= maxSpeed * 0.6f && owner.unitAi.Sword)// && owner.HasWeaponNearby)
            {
                maxSpeed = Mathf.Max(maxSpeed, currentSpeed);
                var position = owner.unitAi.Sword.TipTransform.position;
                currentSpeed = owner.unitAi.Sword.Rigidbody.GetPointVelocity(position).sqrMagnitude;
                var direction = (targetSword - owner.Hand2Rigidbody.position + skillOffset).normalized;
                owner.Hand2Rigidbody.AddForce(direction * owner.swordAcceleration, ForceMode.Acceleration);
                
                direction = (targetSword - position).normalized;
                owner.unitAi.Sword.Rigidbody.AddForceAtPosition(direction * owner.swordAcceleration, 
                    position, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            }
            // owner.targetToLook.Reset();
            // owner.BodyDriver.ReleaseRotation();
            owner.lastActionTime = Time.time;
            owner.stateMachine.ChangeState(BattleDecision.Instance);
        }
    }

}
