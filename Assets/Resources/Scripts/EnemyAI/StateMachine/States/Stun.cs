using System.Collections;
using UnityEditor;
using UnityEngine;

namespace StateMachineTools
{
    public class Stun : EnemyState
    {
        private static Stun instance;

        private Stun()
        {
            if (instance != null)
                return;
            instance = this;
            type = "Stun";
        }

        public static Stun Instance
        {
            get
            {
                if (instance == null)
                    new Stun();
                return instance;
            }
        }

        public override IEnumerator StateLogic(SmartEnemyDriver owner)
        {
            owner.BodyDriver.CompleteTask();
            owner.targetToLook.Reset();
            yield return new WaitForSeconds(0.45f);
            owner.stateMachine.ChangeState(BattleDecision.Instance);
        }
    }

}
