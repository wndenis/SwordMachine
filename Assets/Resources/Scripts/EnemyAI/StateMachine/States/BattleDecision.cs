using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace StateMachineTools
{
    public class BattleDecision : EnemyState
    {
        private static BattleDecision instance;

        private BattleDecision()
        {
            if (instance != null)
                return;
            instance = this;
            type = "Battle decision";
        }

        public static BattleDecision Instance
        {
            get
            {
                if (instance == null)
                    new BattleDecision();
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
        // }

        protected bool CourageChance(SmartEnemyDriver owner)
        {
            return Random.Range(0f, 1f) < 1 - owner.courage;
        }

        public override IEnumerator StateLogic(SmartEnemyDriver owner)
        {
            var armedDuration = 6f;
            // if we make decisions too frequently - wait
            while (!owner.CheckForCooldown(owner.BattleActionCooldown))
            {
                yield return null;
                // owner.stateMachine.ChangeState(Await.Instance);
                // yield break;
            }

            // update cooldown
            owner.lastActionTime = Time.time;
            // check for target
            owner.UpdateTargets();

            if (!owner.HasTargets)
                for (var t = 0f; t < armedDuration; t += Time.fixedDeltaTime)
                {
                    owner.UpdateTargets();
                    if (owner.HasTargets)
                        break;
                    yield return new WaitForFixedUpdate();
                }


            if (owner.HasTargets)
            {
                
                var fromBodyToEnemySword = 100f;
                if (owner.HasWeaponNearby)
                    fromBodyToEnemySword = (owner.headRigidbody.position - owner.nearestEnemyWeapon.TipTransform.position).magnitude;
                var fromBodyToEnemyBody = 100f;
                if (owner.HasEnemyNearby)
                    fromBodyToEnemyBody = (owner.headRigidbody.position - owner.nearestEnemy.HeadTransform.position).magnitude;
                
                var maxFromBodyToEnemySword = owner.BattleDistance * 2f; //insta attack


                if (owner.HasEnemyNearby && fromBodyToEnemyBody > owner.BattleDistance + 3f && fromBodyToEnemySword > owner.BattleDistance)
                {
                    owner.stateMachine.ChangeState(Chase.Instance);
                    yield break;
                }

                if (!(fromBodyToEnemyBody < maxFromBodyToEnemySword || fromBodyToEnemySword < maxFromBodyToEnemySword))
                {
                    owner.stateMachine.ChangeState(Disarm.Instance);
                    yield break;
                }
                if (fromBodyToEnemySword < fromBodyToEnemyBody)
                {
                    owner.stateMachine.ChangeState(Defend.Instance);
                    yield break;
                }
                else
                {
                    owner.stateMachine.ChangeState(Attack.Instance);
                    yield break;
                }
                yield break;
                
                // // insta attack
                // if ((!owner.HasWeaponNearby || fromBodyToEnemySword > maxFromBodyToEnemySword) 
                //     && owner.HasEnemyNearby)
                // {
                //     owner.stateMachine.ChangeState(Attack.Instance);
                //     yield break;
                // }
                //
                // if (owner.HasWeaponNearby && !owner.HasEnemyNearby)
                //
                //
                // if (fromBodyToEnemyBody <= owner.DefendDistance)
                //     owner.stateMachine.ChangeState(Defend.Instance);
                //
                // else if (fromBodyToEnemySword < owner.DefendDistance)
                // {
                //     if (owner.nearestEnemyWeapon.Rigidbody.velocity.magnitude > 1f || CourageChance(owner))
                //         owner.stateMachine.ChangeState(Defend.Instance);
                //     else if (owner.HasEnemyNearby)
                //         owner.stateMachine.ChangeState(Attack.Instance);
                // }
                // else
                // {
                //     if (owner.nearestEnemyWeapon.Rigidbody.velocity.magnitude > 25f || CourageChance(owner))
                //         owner.stateMachine.ChangeState(Defend.Instance);
                //     else if (owner.HasEnemyNearby)
                //         owner.stateMachine.ChangeState(Attack.Instance);
                // }
                //
                // owner.stateMachine.ChangeState(Await.Instance);
//                
//                
//                if (fromBodyToEnemySword < owner.DefendDistance)
//                {
//                    if (Random.Range(0, 1) <= owner.Courage &&
//                        fromBodyToEnemyBody <= owner.BattleDistance)
//                    {
//                        owner.stateMachine.ChangeState(Attack.Instance);
//                    }
//                    else if (unitTarget)
//                    {
//                        owner.stateMachine.ChangeState(Defend.Instance);
//                    }
//                }
//                else if (fromSwordToEnemyBody < owner.BattleDistance &&
//                         fromSwordToEnemyBody > owner.MinimalAttackDistance)
//                {
//                    if (Random.Range(0, 1) <= Mathf.Clamp(Mathf.Clamp(Mathf.Abs(owner.Courage - 1), 1, 2), 0, 1) && unitTarget)
//                        owner.stateMachine.ChangeState(Defend.Instance);
//                    else
//                        owner.stateMachine.ChangeState(Attack.Instance);
//                }
//                else if (fromBodyToEnemySword < owner.BattleDistance &&
//                         fromSwordToEnemyBody > owner.DefendDistance)
//                {
//                    if (Random.Range(-1, 1) < owner.Courage)
//                        owner.stateMachine.ChangeState(Attack.Instance);
//                    else if (unitTarget)
//                        owner.stateMachine.ChangeState(Defend.Instance);
//                }
//                else
//                {
//                    owner.stateMachine.ChangeState(Await.Instance);
//                }
//
//                yield break; 
            }
            else
            {
                owner.stateMachine.ChangeState(Disarm.Instance);
            }
        }
    }
}
