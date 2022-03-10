using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKFootPlacement : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] LayerMask layersToHit;

    [Range(0, 1f)]
    [SerializeField] float distanceToGround;

    [SerializeField] float distanceToGroundOffset = 1f;

    private void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, animator.GetFloat("IKLeftFootWeight"));
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, animator.GetFloat("IKLeftFootWeight"));

            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, animator.GetFloat("IKRightFootWeight"));
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, animator.GetFloat("IKRightFootWeight"));

            // Left Foot
            RaycastHit hit;
            Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, distanceToGround + distanceToGroundOffset, layersToHit))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += distanceToGround;

                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forward, hit.normal));
                }
            }

            // Right Foot
            ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, distanceToGround + distanceToGroundOffset, layersToHit))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += distanceToGround;

                    animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(forward, hit.normal));
                }
            }
        }
    }
}
