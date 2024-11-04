using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformationBasedHeight : MonoBehaviour
{

    public bool keepChangingProportions;
    public SkinnedMeshRenderer skin;
    Transform[] bones;
    Vector3[] bonesOriginalPosition; // store original positions of bones to compute relative enlargement. Important: store in Vector3[], which stores values themselves unlike Transform[], which is referential
    Quaternion[] TPose; // record angles of this character when in T-Pose (not always Quaternion.idendity)

    [Range(0.7f, 1.3f)]
    public float bodySize = 1;
    public float baseHeight = 1.67f; // meter

    public float upperbody = 0.3f; // from neck to hips
    public float LegSize = 0.5f; // from leg to toebase
    public float ArmSize = 0.38f;// from forearm to hand
    public float headSize1 = 0.13f;
    public float headSize2 = 1f;

    float w; // 가중치
    float upperbodyValue; 
    float LegSizeValue;
    float ArmSizeValue;
    float headSize1Value;
    float headSize2Value;

    public float shoulderWidth = 1;
    public float hipWidth = 1;
    public float waist_shape = 0;
    public float hips_shape = 0;

    [Range(0.8f, 1.2f)]
    public float bodyCorpulent = 1f;
    [Range(0.8f, 1.2f)]
    public float ArmCorpulence = 1;
    [Range(0.8f, 1.2f)]
    public float LegCorpulence = 1;

    float rightArmCorpulence;
    float leftArmCorpulence;

    float rightLegCorpulence;
    float leftLegCorpulence;

    // for none-bone based deformation
    Mesh mesh;
    Vector3[] OriginalVertices;
    Vector3[] DisplacedVertices;
    Vector3 centroid_Hips; // will store middle of body at hips' height
    Vector3 centroid_Waist; // will store middle of body at waist's height


    // Use this for initialization
    void Start()
    { 

        bones = new Transform[skin.bones.Length];
        TPose = new Quaternion[skin.bones.Length];

        bonesOriginalPosition = new Vector3[skin.bones.Length];
        for (int i = 0; i < skin.bones.Length; i++)
        {
            bonesOriginalPosition[i] = skin.bones[i].position;
            bones[i] = skin.bones[i];
            TPose[i] = bones[i].rotation;
        }


        // none-bone-based
        mesh = (Mesh)Instantiate(skin.sharedMesh);
        skin.sharedMesh = mesh;
        OriginalVertices = mesh.vertices;

        centroid_Hips = Centroid_Mesh_HeightY(OriginalVertices, bones[8].position.y, 0.02f);
        centroid_Waist = Centroid_Mesh_HeightY(OriginalVertices, bones[9].position.y, 0.02f);

    }

    void Update()
    {

        for (int i = 0; i < bones.Length; i++) // bring character into T-Pose, so that deformation is applied correctly. Otherwise character can still be in the pose
                                               // created by IK in LateUpdate()
        {
            bones[i].rotation = TPose[i];
        }

        if (keepChangingProportions)
        {
            SetIndividualLimbsToCommonValue();
            SetProportions();
            ApplyBoneBasedDistortion();
            ApplyNotBoneBasedBumps();
            mesh.RecalculateBounds();
        }
    }

    void SetProportions()
    {
        w = baseHeight * bodySize;

        upperbodyValue = upperbody * w;
        LegSizeValue = LegSize * w;
        ArmSizeValue = ArmSize * w;
        headSize1Value = headSize1 * w;
    }


    void SetIndividualLimbsToCommonValue()
    {
        rightArmCorpulence = ArmCorpulence;
        leftArmCorpulence = ArmCorpulence;
        rightLegCorpulence = LegCorpulence;
        leftLegCorpulence = LegCorpulence;
    }

    void ApplyBoneBasedDistortion()
    {
        // set original position
        for (int i = 0; i < skin.bones.Length; i++)
        {
            skin.bones[i].position = bonesOriginalPosition[i];
        }


        // body size
        skin.bones[9].position = skin.bones[9].parent.position + ((skin.bones[9].position - skin.bones[9].parent.position) * w);
        skin.bones[7].position = skin.bones[7].parent.position + ((skin.bones[7].position - skin.bones[7].parent.position) * w);
        for (int i = 0; i < 52; i++)
        {
            if (i != 8)
            {
                skin.bones[i].position = skin.bones[i].parent.position + ((skin.bones[i].position - skin.bones[i].parent.position) * w);
            }
        }
        skin.bones[8].localScale = new Vector3(bodyCorpulent * w, bodyCorpulent * w, bodyCorpulent * w);

        // upper Body (spine2 - spine1 - spine - hips)
        skin.bones[9].position = skin.bones[9].parent.position + (skin.bones[9].position - skin.bones[9].parent.position) * upperbodyValue;
        skin.bones[7].position = skin.bones[7].parent.position + (skin.bones[7].position - skin.bones[7].parent.position) * upperbodyValue;
        skin.bones[0].position = skin.bones[0].parent.position + (skin.bones[0].position - skin.bones[0].parent.position) * upperbodyValue;

        // shoulder width
        Vector3 rightShoulderPos = skin.bones[3].position;
        Vector3 leftShoulderPos = skin.bones[1].position;
        skin.bones[3].position = leftShoulderPos + (skin.bones[3].position - leftShoulderPos) * shoulderWidth; // clavicles are not offsetted relative to parent, but to each other
        skin.bones[1].position = rightShoulderPos + (skin.bones[1].position - rightShoulderPos) * shoulderWidth;
        skin.bones[4].position = skin.bones[4].parent.position + (skin.bones[4].position - skin.bones[4].parent.position) * shoulderWidth;
        skin.bones[2].position = skin.bones[2].parent.position + (skin.bones[2].position - skin.bones[2].parent.position) * shoulderWidth; // shoulders are again offsetted relative to parents

        // hips width
        Vector3 rightHipPos = skin.bones[13].position;
        Vector3 leftHipPos = skin.bones[28].position;
        skin.bones[13].position = leftHipPos + (skin.bones[13].position - leftHipPos) * hipWidth;
        skin.bones[28].position = rightHipPos + (skin.bones[28].position - rightHipPos) * hipWidth;

        for (int i = 10; i <= 27; i++) // right Arm
        {
            if (i != 13)
            {
                skin.bones[i].position = skin.bones[i].parent.position + (skin.bones[i].position - skin.bones[i].parent.position) * ArmSizeValue;
            }
        }
        skin.bones[4].localScale = new Vector3(rightArmCorpulence * ArmSizeValue, rightArmCorpulence * ArmSizeValue, rightArmCorpulence * ArmSizeValue);


        for (int i = 32; i <= 48; i++) // left Arm
        {
            skin.bones[i].position = skin.bones[i].parent.position + (skin.bones[i].position - skin.bones[i].parent.position) * ArmSizeValue;
        }
        skin.bones[2].localScale = new Vector3(leftArmCorpulence * ArmSizeValue, leftArmCorpulence * ArmSizeValue, leftArmCorpulence * ArmSizeValue);


        for (int i = 29; i <= 31; i++) // right Leg
        {
            skin.bones[i].position = skin.bones[i].parent.position + (skin.bones[i].position - skin.bones[i].parent.position) * LegSizeValue;
        }
        skin.bones[13].localScale = new Vector3(rightLegCorpulence * LegSizeValue, rightLegCorpulence * LegSizeValue, rightLegCorpulence * LegSizeValue);


        for (int i = 49; i <= 51; i++) // left Leg
        {
            skin.bones[i].position = skin.bones[i].parent.position + (skin.bones[i].position - skin.bones[i].parent.position) * LegSizeValue;
        }
        skin.bones[28].localScale = new Vector3(leftLegCorpulence * LegSizeValue, leftLegCorpulence * LegSizeValue, leftLegCorpulence * LegSizeValue);


        // head
        skin.bones[6].position = skin.bones[6].parent.position + (skin.bones[6].position - skin.bones[6].parent.position) * headSize1Value;
        skin.bones[5].localScale = new Vector3(headSize2 * headSize1Value, headSize2 * headSize1Value, headSize2 * headSize1Value);
    }

    // see discussion at https://forum.unity.com/threads/manually-updating-a-skinned-mesh.67047/
    void ApplyNotBoneBasedBumps()
    {
        float whip = 8; // size of effect of bump // // HIER EINFACH WERT EINGESETZT
        DisplacedVertices = new Vector3[OriginalVertices.Length]; // first simply take original vertices as baseline
        for (int i = 0; i < OriginalVertices.Length; i++)
        {
            DisplacedVertices[i] = OriginalVertices[i];
        }

        for (int i = 0; i < DisplacedVertices.Length; i++)
        {
            // first Apply Hip Bump
            float distance = Mathf.Abs(DisplacedVertices[i].y - centroid_Hips.y);
            if (distance < 0.5f * Mathf.PI / whip)
            {
                float stretch = hips_shape * Mathf.Sin(distance * whip + 0.5f * Mathf.PI);
                DisplacedVertices[i].x = (OriginalVertices[i].x - centroid_Hips.x) * (stretch + 1) + centroid_Hips.x;
                DisplacedVertices[i].z = (OriginalVertices[i].z - centroid_Hips.z) * (stretch * 0.5f + 1) + centroid_Hips.z; // HIER EINFACH WERT EINGESETZT: Hüften gehen 2* so stark in die Breite wie in die Tiefe
            }

            // then apply waist bump
            float wwaist = 6;
            distance = Mathf.Abs(DisplacedVertices[i].y - centroid_Waist.y);
            if (distance < 0.5f * Mathf.PI / wwaist)
            {
                float stretch = waist_shape * Mathf.Sin(distance * wwaist + 0.5f * Mathf.PI);
                DisplacedVertices[i].x = (DisplacedVertices[i].x - centroid_Waist.x) * (stretch + 1) + centroid_Waist.x;
                DisplacedVertices[i].z = (DisplacedVertices[i].z - centroid_Waist.z) * (stretch + 1) + centroid_Waist.z;
            }

        }

        mesh.vertices = DisplacedVertices;
    }

    // takes a mesh vertices, selects those within a certain hight range, computes average x and z coordinates of that selection. This is then called the centroid at that height. 
    public Vector3 Centroid_Mesh_HeightY(Vector3[] vertices, float height, float heightrange)
    {
        Vector3 result = Vector3.zero;
        List<float> Point_X = new List<float>();
        List<float> Point_Z = new List<float>();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (Mathf.Abs(vertices[i].y - height) < heightrange)
            {
                Point_X.Add(vertices[i].x);
                Point_Z.Add(vertices[i].z);
            }
        }

        for (int i = 0; i < Point_X.Count; i++) { result.x += Point_X[i]; }
        result.x = result.x / Point_X.Count;

        for (int i = 0; i < Point_Z.Count; i++) { result.z += Point_Z[i]; }
        result.z = result.z / Point_Z.Count;

        result.y = height;

        return result;
    }
}
