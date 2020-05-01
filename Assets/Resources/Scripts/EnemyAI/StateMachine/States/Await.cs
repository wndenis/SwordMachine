using System.Collections;
using UnityEditor;
using UnityEngine;

namespace StateMachineTools
{
    public class Await : EnemyState
    {
        private static Await instance;

        private Await()
        {
            if (instance != null)
                return;
            instance = this;
            type = "Await";
        }

        public static Await Instance
        {
            get
            {
                if (instance == null)
                    new Await();
                return instance;
            }
        }

        public override IEnumerator StateLogic(SmartEnemyDriver owner)
        {
            var currentAwaitingPosition = owner.awaitHandTransform.position;
            var duration = 0.05f;
            var startTime = Time.time;
            var timeToAnimate = 0f;
            var speed = owner.swordAcceleration * 2f;
            // var forceDelay = 0.5f;
            Vector3 offset;

            while (Time.time - startTime < duration)
            {

                currentAwaitingPosition += Random.insideUnitSphere * 0.01f;
                
                timeToAnimate = Random.Range(0.075f, 0.15f);
                owner.Hand2Rigidbody.AddForce((currentAwaitingPosition - owner.Hand2Rigidbody.position) * (speed * timeToAnimate));
                yield return new WaitForSeconds(timeToAnimate);
                
                // if (Time.time - startTime > forceDelay)
                //     owner.swordConstantForce.enabled = true;
                //todo: check if it smooth
                yield return new WaitForFixedUpdate();
            }
            owner.stateMachine.ChangeState(BattleDecision.Instance);
        }
    }

}
