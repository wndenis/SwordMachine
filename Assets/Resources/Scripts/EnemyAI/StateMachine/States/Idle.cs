using System.Collections;
using Resources.Scripts;
using UnityEngine;

namespace StateMachineTools
{
    public class Idle : EnemyState
    {
        private static Idle instance;

         private Idle()
        {
            if (instance != null)
                return;
            instance = this;
            type = "Idle";
        }

        public static Idle Instance
        {
            get
            {
                if (instance == null)
                    new Idle();
                return instance;
            }
        }
        
        public override IEnumerator StateLogic(SmartEnemyDriver owner)
        {
            // owner.targetToLook = owner.HeadDriver.defaultTargetToLookAt;
            owner.CustomHandControl(false);
            owner.targetToLook.Reset();
            owner.BodyDriver.ReleaseRotation();
            owner.BodyDriver.CompleteTask();
            do
            {
                yield return new WaitForSeconds(0.035f);
                owner.UpdateTargets();
            } while (!owner.HasTargets);
            owner.stateMachine.ChangeState(Arm.Instance);
        }
    }

}
