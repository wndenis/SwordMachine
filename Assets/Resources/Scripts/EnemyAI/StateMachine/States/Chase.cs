using System.Collections;
using Resources.Scripts;
using UnityEngine;

namespace StateMachineTools
{
    public class Chase : EnemyState
    {
        private static Chase instance;

         private Chase()
        {
            if (instance != null)
                return;
            instance = this;
            type = "Chase";
        }

        public static Chase Instance
        {
            get
            {
                if (instance == null)
                    new Chase();
                return instance;
            }
        }
        
        public override IEnumerator StateLogic(SmartEnemyDriver owner)
        {
            var currentTarget = owner.nearestEnemy.transform;
            owner.targetToLook.SetTarget(currentTarget);
            
            // move to enemy
            owner.CustomHandControl(false);
            owner.BodyDriver.ReleaseRotation();
            owner.BodyDriver.AddTask(new SmartEnemyBodyDriver.MovementTask(new MoveTarget(currentTarget),
                1f, owner.BattleDistance - owner.BattleDistanceThreshold));
            
            var dist = 99f;
            var reachDist = owner.BattleDistance * owner.BattleDistance;
            do
            {
                yield return new WaitForSeconds(0.31f);
                owner.UpdateTargets();
                

                if (!owner.HasEnemyNearby || owner.nearestEnemy.transform != currentTarget ||
                    !owner.BodyDriver.hasMovementTask)
                {
                    owner.targetToLook.Reset();
                    owner.BodyDriver.CompleteTask();
                    owner.stateMachine.ChangeState(BattleDecision.Instance);
                    yield break;
                }

                dist = (owner.headRigidbody.position - currentTarget.position).sqrMagnitude;
            } while (dist > reachDist);
            owner.BodyDriver.CompleteTask();
            owner.stateMachine.ChangeState(BattleDecision.Instance);
        }
    }

}
