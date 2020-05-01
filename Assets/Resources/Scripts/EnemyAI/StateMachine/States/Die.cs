using System.Collections;
using UnityEditor;
using UnityEngine;

namespace StateMachineTools
{
    public class Die : EnemyState
    {
        private static Die instance;

        private Die()
        {
            if (instance != null)
                return;
            instance = this;
            type = "Die";
        }

        public static Die Instance
        {
            get
            {
                if (instance == null)
                    new Die();
                return instance;
            }
        }

        public override IEnumerator StateLogic(SmartEnemyDriver owner)
        {
            //todo: stop moving
            owner.targetToLook.Reset();
            owner.BodyDriver.CompleteTask();
            yield return null;
        }
    }

}
