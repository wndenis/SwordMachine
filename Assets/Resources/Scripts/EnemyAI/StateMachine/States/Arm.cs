using System.Collections;
using Resources.Scripts;
using UnityEngine;

namespace StateMachineTools
{
    public class Arm : EnemyState
    {
        private static Arm instance;

        private Arm()
        {
            if (instance != null)
                return;
            instance = this;
            type = "Arm";
        }

        public static Arm Instance
        {
            get
            {
                if (instance == null)
                    new Arm();
                return instance;
            }
        }
        
        public override IEnumerator StateLogic(SmartEnemyDriver owner)
        {
            
            Vector3 fromHandToPoint;
            Vector3 force;
            float speed = owner.swordAcceleration - 100f;
            owner.CustomHandControl(true);
            yield return null;
            //look at sword
            var rotationTarget = new MoveTarget(owner.HasEnemyNearby ? 
                owner.nearestEnemy.transform : owner.nearestEnemyWeapon.transform);
            owner.BodyDriver.LockRotationToTarget(rotationTarget);
            owner.targetToLook.SetTarget(owner.Hand2Rigidbody.transform);
            do
            {
                fromHandToPoint = owner.armingHandTransform.position - owner.Hand2Rigidbody.position;
                force = fromHandToPoint.normalized * speed;
                owner.Hand2Rigidbody.AddForce(force, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            } while (fromHandToPoint.magnitude > 0.1f);
            //TODO: animate sword to OPEN 
            
            yield return new WaitForSeconds(0.2f);
            do
            {
                fromHandToPoint = owner.awaitHandTransform.position - owner.Hand2Rigidbody.position;
                force = fromHandToPoint.normalized * speed;
                owner.Hand2Rigidbody.AddForce(force, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            } while (fromHandToPoint.magnitude > 0.15f);
            
            owner.targetToLook.Reset();
            owner.stateMachine.ChangeState(BattleDecision.Instance);
        }
    }

}
