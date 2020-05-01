using System.Collections;
using UnityEngine;

namespace StateMachineTools
{
    public class Disarm : EnemyState
    {
        private static Disarm instance;

        private Disarm()
        {
            if (instance != null)
                return;
            instance = this;
            type = "Disarm";
        }

        public static Disarm Instance
        {
            get
            {
                if (instance == null)
                    new Disarm();
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
            do
            {
                fromHandToPoint = owner.armingHandTransform.position - owner.Hand2Rigidbody.position;
                force = fromHandToPoint.normalized * speed;
                owner.Hand2Rigidbody.AddForce(force, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            } while (fromHandToPoint.magnitude > 0.1f);
            //TODO: animate sword to CLOSE
            
            yield return new WaitForSeconds(0.2f);

            do
            {
                fromHandToPoint = owner.awaitHandTransform.position - owner.Hand2Rigidbody.position;
                force = fromHandToPoint.normalized * speed;
                owner.Hand2Rigidbody.AddForce(force, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            } while (fromHandToPoint.magnitude > 0.2f);
            owner.stateMachine.ChangeState(Idle.Instance);
        }
    }

}
