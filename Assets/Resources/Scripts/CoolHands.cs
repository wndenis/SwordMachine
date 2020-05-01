using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolHands : MonoBehaviour
{
    public Transform RightHand;
    public Transform LeftHand;
    public Transform Head;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void OnAnimatorIK()
    {
        if (animator)
        {
            if(Head != null) {
                animator.SetLookAtWeight(1);
                animator.SetLookAtPosition(Head.position);
            }    

            // Set the right hand target position and rotation, if one has been assigned
            if(RightHand != null) {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
                animator.SetIKPosition(AvatarIKGoal.RightHand, RightHand.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, RightHand.rotation * Quaternion.AngleAxis(90, RightHand.forward));
            }        
            if(LeftHand != null) {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1);  
                animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHand.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHand.rotation * Quaternion.AngleAxis(90, LeftHand.forward));
            }        
        }
    }
}
