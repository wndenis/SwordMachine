using System.Collections;
using Resources.Scripts;
using UnityEngine;

namespace StateMachineTools
{
    public class Attack : EnemyState
    {
        private static Attack instance;

        private Attack()
        {
            if (instance != null)
                return;
            instance = this;
            type = "Attack";
        }

        public static Attack Instance
        {
            get
            {
                if (instance == null)
                    new Attack();
                return instance;
            }
        }
        
        public override IEnumerator StateLogic(SmartEnemyDriver owner)
        {
            var currentSpeed = 0f;
            var maxSpeed = 0f;
            var minDuration = 0.125f;
            var startTime = Time.time;
            var speed = owner.swordAcceleration * Random.Range(0.7f, 1f);
            owner.lastActionTime = Time.time;

            var tipTransform = owner.unitAi.Sword.TipTransform;
            
            owner.targetToLook.SetTarget(owner.nearestEnemy.transform);
            owner.CustomHandControl(true);
            
            var fromBodyToEnemyBodyVector = owner.BodyDriver.Body.transform.position - owner.nearestEnemy.BodyTransform.position;
            var fromBodyToEnemyBodyLen = fromBodyToEnemyBodyVector.magnitude;
            var enemyBodyMoveTarget = new MoveTarget(owner.nearestEnemy.BodyTransform);
            
            owner.BodyDriver.LockRotationToTarget(enemyBodyMoveTarget);
            // if we far, move to target
            if (fromBodyToEnemyBodyLen > owner.BattleDistance + owner.BattleDistanceThreshold)
            {
                owner.BodyDriver.CompleteTask();
                
                owner.BodyDriver.AddTask(new SmartEnemyBodyDriver.MovementTask(enemyBodyMoveTarget, 0.8f, 
                    owner.BattleDistance - owner.BattleDistanceThreshold, owner.BattleDistanceThreshold, SmartEnemyBodyDriver.BotMovementType.Strafe));
            }
            // if we too close, step back
            else if (fromBodyToEnemyBodyLen < owner.BattleDistance - owner.BattleDistanceThreshold)
            {
                owner.BodyDriver.CompleteTask();
                var distanceToReachBattleDistance = owner.BattleDistance - fromBodyToEnemyBodyLen;
                var bodyPos = owner.BodyDriver.Body.transform.position;
                var point = bodyPos + (bodyPos - owner.nearestEnemy.BodyTransform.position)
                    .normalized * distanceToReachBattleDistance;
                owner.BodyDriver.AddTask(new SmartEnemyBodyDriver.MovementTask(new MoveTarget(point), 0.8f, //todo: v3 target
                    0f, owner.BattleDistanceThreshold, SmartEnemyBodyDriver.BotMovementType.Strafe));
            }
            //if distance is good and our skill is high enough, move around target
            else if (Random.Range(0f, 1f) < owner.skill / 1.5f)
            {
                owner.BodyDriver.CompleteTask();

                var horizontalOffset = (owner.BattleDistance + 
                                        Random.Range(-1f, 1f) * owner.BattleDistanceThreshold)
                                       * Random.insideUnitCircle.normalized;
                if (horizontalOffset.magnitude < owner.BattleDistance - owner.BattleDistanceThreshold)
                    horizontalOffset = horizontalOffset.normalized * owner.BattleDistance;
                var pointAround = new Vector3(horizontalOffset.x, 0, horizontalOffset.y) + owner.nearestEnemy.BodyTransform.position;
                
                var spd = 0.5f * Random.Range(0.8f, 1.2f);
                
                owner.BodyDriver.AddTask(new SmartEnemyBodyDriver.MovementTask(new MoveTarget(pointAround), spd, 
                    0, 0.5f, SmartEnemyBodyDriver.BotMovementType.Strafe));
            }
            // else if (fromBodyToEnemyBodyLen < owner.BattleDistance - 0.5f)
            //     owner.animator.SetBool(BACKWARD, true);
            
            
            
            // //замах 1
            // if ((owner.nearestEnemy.BodyTransform.position - owner.unitAi.Sword.TipTransform.position).magnitude < 0.35f)
            // {
            //     //todo: check best config
            //     owner.Hand2Rigidbody.AddForce(owner.unitAi.Sword.Rigidbody.velocity.normalized * speed);
            //     owner.unitAi.Sword.Rigidbody.AddForceAtPosition(Vector3.up * speed, tipTransform.position);
            //     // owner.unitAi.Sword.Rigidbody.AddForceAtPosition(Vector3.up * speed, handleTransform.position);
            // }
            //
            //
            // //замах 2
            // //todo: direction must lead to back of our body, not completely random
            // var swayPoint = Random.insideUnitSphere.normalized * Random.Range(1f, 2f) + owner.unitAi.Sword.HandleTransform.position;
            //
            // while ((currentSpeed >= maxSpeed * 0.9f  || Time.time - startTime < minDuration) && owner.nearestEnemy)
            // {
            //     maxSpeed = Mathf.Max(maxSpeed, currentSpeed);
            //     var position = tipTransform.position;
            //     currentSpeed = owner.unitAi.Sword.Rigidbody.GetPointVelocity(position).sqrMagnitude;
            //
            //     var vectorToSwayPoint = swayPoint - position;
            //     if (vectorToSwayPoint.sqrMagnitude < 0.0625f)
            //         break;
            //     var direction = vectorToSwayPoint.normalized;
            //     owner.unitAi.Sword.Rigidbody.AddForceAtPosition(speed * 0.55f * direction, tipTransform.position, ForceMode.Acceleration);
            //     yield return new WaitForFixedUpdate();
            // }
            
            yield return new WaitForSeconds(0.1f);
            
            //удар
            currentSpeed = 0f;
            maxSpeed = 0f;
            var skillOffset = Random.insideUnitSphere;
            skillOffset.y *= 2f;
            skillOffset *= 1.05f - owner.skill;
            
            //choose where to hit - head or body
            var target = owner.nearestEnemy.BodyTransform;
            if (Random.Range(0f, 1f) < 0.3f)
                target = owner.nearestEnemy.HeadTransform;

            while ((currentSpeed >= maxSpeed * 0.9f  || Time.time - startTime < minDuration) && owner.nearestEnemy)
            {
                var tipPosition = tipTransform.position;
                currentSpeed = owner.unitAi.Sword.Rigidbody.GetPointVelocity(tipPosition).sqrMagnitude;
                maxSpeed = Mathf.Max(maxSpeed, currentSpeed);

                var targetPos = target.position;
                var direction = (targetPos - owner.Hand2Rigidbody.position).normalized;

                owner.Hand2Rigidbody.AddForce(speed * direction, ForceMode.Acceleration);

                
                direction = (targetPos - tipPosition + skillOffset).normalized;
                owner.unitAi.Sword.Rigidbody.AddForceAtPosition(direction * speed, tipPosition, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            }
            // owner.BodyDriver.ReleaseRotation();
            owner.lastActionTime = Time.time;
            // owner.nearestEnemy = null;
            // owner.HasEnemyNearby = false;
            owner.stateMachine.ChangeState(BattleDecision.Instance);
        }
    }

}
