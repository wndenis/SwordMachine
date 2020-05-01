using System.Collections;
using UnityEngine;

namespace StateMachineTools
{
    public abstract class EnemyState : State<SmartEnemyDriver>
    {
        public string type;
        public sealed override void EnterState(SmartEnemyDriver owner)
        {
            //MonoBehaviour.print($"Entering {type}");
            owner.currentCoroutine = owner.StartCoroutine(StateLogic(owner));
        }

        public sealed override void ExitState(SmartEnemyDriver owner)
        {
            //MonoBehaviour.print($"Exiting {type}");
            if (owner.currentCoroutine != null)
                owner.StopCoroutine(owner.currentCoroutine);
        }
    }
}