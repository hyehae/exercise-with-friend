using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombinerUsage : MonoBehaviour
{
    public GameObject myRig;
    public Material material;
    public Mesh[] meshes;

    void Start()
    {
        // Instantiate Rig (Often from an FBX)
        GameObject rig = Instantiate(myRig);

        // Find the "packed" renderer, which has bone info
        SkinnedMeshRenderer rigRenderer = rig.GetComponentInChildren<SkinnedMeshRenderer>();
        Transform[] bones = rigRenderer.bones;
        Transform rootBone = rigRenderer.rootBone;

        // Pack! (In this case, by overwriting the already existing renderer found on the rig)
        SpellcastStudios.MeshCombiner.CombineFast(rigRenderer, rootBone, material, bones, meshes);
    }
}
