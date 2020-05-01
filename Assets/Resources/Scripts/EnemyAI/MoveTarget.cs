using UnityEngine;

namespace Resources.Scripts
{
    public class MoveTarget
    {
        private bool transformMode;
        private Transform targetTransform;
        private Vector3 cachedVector;
        public bool isEmpty { get; private set; }

        public MoveTarget()
        {
            isEmpty = true;
        }
        public MoveTarget(Vector3 target)
        {
            SetTarget(target);
        }

        public MoveTarget(Transform target)
        {
            SetTarget(target);
        }

        public void SetTarget(Vector3 target)
        {
            isEmpty = false;
            transformMode = false;
            cachedVector = target;
        }

        public void SetTarget(Transform target)
        {
            isEmpty = false;
            transformMode = true;
            targetTransform = target;
            cachedVector = target.position;
        }

        public void Reset()
        {
            isEmpty = true;
            targetTransform = null;
            cachedVector = Vector3.zero;
        }

        public Vector3 position
        {
            get
            {
                if (transformMode)
                {
                    if (targetTransform != null)
                        cachedVector = targetTransform.position;
                    else
                        isEmpty = true;
                }
                    
                return cachedVector;
            }
        }
    }
}