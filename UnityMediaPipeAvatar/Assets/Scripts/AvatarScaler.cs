using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarScaler : MonoBehaviour
{
    public float headProportion = 1f / 8f;
    public float neckProportion = 1f / 16f;
    public float spineProportion = 3f / 8f;
    public float legProportion = 1f / 2f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        AdjustProportions();
    }

    void Update()
    {
        //AdjustProportions();
    }

    void AdjustProportions()
    {
        float totalHeight = CalculateTotalHeight();
        if (totalHeight <= 0) return;

        float headHeight = totalHeight * headProportion;
        float neckHeight = totalHeight * neckProportion;
        float spineHeight = totalHeight * spineProportion;
        float legHeight = totalHeight * legProportion;

        Debug.Log("headHeight: " + headHeight);
        Debug.Log("neckHeight: " + neckHeight);
        Debug.Log("spineHeight: " + spineHeight);
        Debug.Log("legHeight: " + legHeight);

        AdjustBoneLength(HumanBodyBones.Head, headHeight);
        AdjustBoneLength(HumanBodyBones.Neck, neckHeight);
        AdjustBoneLength(HumanBodyBones.Spine, spineHeight);
        AdjustLegLength(legHeight);
    }

    float CalculateTotalHeight()
    {
        float headHeight = GetBoneHeight(HumanBodyBones.Head, HumanBodyBones.Neck);
        float neckHeight = GetBoneHeight(HumanBodyBones.Neck, HumanBodyBones.Spine);
        float spineHeight = GetBoneHeight(HumanBodyBones.Spine, HumanBodyBones.Hips);
        float leftLegHeight = GetBoneHeight(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg) + GetBoneHeight(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot);
        float rightLegHeight = GetBoneHeight(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg) + GetBoneHeight(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot);

        float legHeight = (leftLegHeight + rightLegHeight) / 2f;

        return headHeight + neckHeight + spineHeight + legHeight;
    }

    float GetBoneHeight(HumanBodyBones startBone, HumanBodyBones endBone)
    {
        Transform startBoneTransform = animator.GetBoneTransform(startBone);
        Transform endBoneTransform = animator.GetBoneTransform(endBone);

        if (startBoneTransform != null && endBoneTransform != null)
        {
            return Vector3.Distance(startBoneTransform.position, endBoneTransform.position);
        }
        return 0;
    }

    void AdjustBoneLength(HumanBodyBones bone, float targetHeight)
    {
        Transform boneTransform = animator.GetBoneTransform(bone);
        HumanBodyBones parentBone = GetParentBone(bone);

        if (boneTransform != null && parentBone != HumanBodyBones.LastBone)
        {
            Transform parentBoneTransform = animator.GetBoneTransform(parentBone);
            if (parentBoneTransform != null)
            {
                float currentHeight = Vector3.Distance(boneTransform.position, parentBoneTransform.position);
                if (currentHeight > 0)
                {
                    Vector3 scale = boneTransform.localScale;
                    scale.y *= targetHeight / currentHeight;
                    boneTransform.localScale = scale;
                }
            }
        }
    }

    void AdjustLegLength(float targetLegHeight)
    {
        float upperLegHeight = targetLegHeight;
        float lowerLegHeight = targetLegHeight;

        AdjustBoneLength(HumanBodyBones.LeftUpperLeg, upperLegHeight);
        AdjustBoneLength(HumanBodyBones.RightUpperLeg, upperLegHeight);
        AdjustBoneLength(HumanBodyBones.LeftLowerLeg, lowerLegHeight);
        AdjustBoneLength(HumanBodyBones.RightLowerLeg, lowerLegHeight);
    }

    HumanBodyBones GetParentBone(HumanBodyBones bone)
    {
        switch (bone)
        {
            case HumanBodyBones.Head: return HumanBodyBones.Neck;
            case HumanBodyBones.Neck: return HumanBodyBones.Spine;
            case HumanBodyBones.Spine: return HumanBodyBones.Hips;
            case HumanBodyBones.LeftUpperLeg: return HumanBodyBones.Hips;
            case HumanBodyBones.RightUpperLeg: return HumanBodyBones.Hips;
            case HumanBodyBones.LeftLowerLeg: return HumanBodyBones.LeftUpperLeg;
            case HumanBodyBones.RightLowerLeg: return HumanBodyBones.RightUpperLeg;
            // Add other bones as necessary
            default: return HumanBodyBones.LastBone; // Indicates no valid parent bone
        }
    }
}
