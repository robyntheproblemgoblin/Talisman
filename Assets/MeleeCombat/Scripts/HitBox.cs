using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class HitBox
{

    // when the move animation is changed,
    // change slider values for start and end frames 

    public enum Shape
    {
        Box,
        Sphere,
        Capsule
    }

    [HideInInspector] public CombatItem m_owner;
    [SerializeField] Transform m_parentTransform;
    [SerializeField] Vector3 m_offset;
    [SerializeField] Vector3 m_rotationOffset;
    [SerializeField] public Shape m_shape;
    [SerializeField] public int m_startFrame = 0;
    [SerializeField] public int m_endFrame = 1;
    [SerializeField] [Tooltip("Calculate Damage Automatically based on animation length and hit box alive time, NOT RECOMMENDED BUT A FEATURE")] public bool m_automaticDamageCalculation;
    [SerializeField] public float m_damage;
    [SerializeField] public float m_knockbackDistance;
    [SerializeField] [Tooltip("Calculate Knockback Automatically based on angle that hitbox hits a hurtbox")] public bool m_automaticKnockbackAngle;
    [SerializeField] public Vector3 m_knockbackAngle;
    [SerializeField] [Tooltip("Time in seconds for knockbackto start")] public float m_knockbackTime;
    [SerializeField] [Tooltip("Calculate Hit Stop scaling with damage dealt, more damage more hit stop")] public bool m_automaticHitStop;
    [SerializeField] public float m_hitStopLength;
    [SerializeField] public float m_hitStopMultiplier;
    [HideInInspector] public bool m_isColliding;
    [HideInInspector] public HurtBox m_collidingHurtBox;
    [HideInInspector] public HitBoxObject m_hitBoxObject;
    private Material m_hitBoxMaterial;


    //box
    [SerializeField] private float m_width, m_height, m_depth;

    //sphere and capsule
    [SerializeField] private float m_radius;



    public HitBoxObject ConstructHitbox()
    {
        GameObject newHitBox = new GameObject();
        newHitBox.AddComponent<HitBoxObject>();
        newHitBox.GetComponent<HitBoxObject>().m_hitbox = this;
        Assert.IsFalse(LayerMask.NameToLayer("Melee") == -1, "You Need To Create A Layer Named \"Melee\"");
        newHitBox.layer = LayerMask.NameToLayer("Melee");
        
        m_owner = m_parentTransform.GetComponent<CombatItem>();
        if (m_owner == null)
        {
            m_owner = m_parentTransform.GetComponentInParent<CombatItem>();
        }
        newHitBox.transform.parent = m_parentTransform;
        newHitBox.transform.localPosition = m_offset;
        newHitBox.transform.localRotation = Quaternion.Euler(m_rotationOffset);
        newHitBox.name = $"Hit Box: {m_shape}";
        Rigidbody newHitBoxRigidbody = newHitBox.AddComponent<Rigidbody>();
        
        newHitBoxRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        newHitBoxRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        newHitBoxRigidbody.useGravity = false;

        // debug mesh renderer stuff

        newHitBox.AddComponent<MeshRenderer>();
        MeshRenderer newHitBoxMeshRenderer = newHitBox.GetComponent<MeshRenderer>();
        m_hitBoxMaterial = Resources.Load("Materials/HitBoxMaterial") as Material;
        newHitBoxMeshRenderer.material = m_hitBoxMaterial;
        newHitBox.AddComponent<MeshFilter>();
        MeshFilter meshFilter = newHitBox.GetComponent<MeshFilter>();
        Mesh newMesh = new Mesh();
        m_hitBoxObject = newHitBox.GetComponent<HitBoxObject>();

        switch (m_shape)
        {
            case Shape.Box:
                //collider
                BoxCollider boxCollider = newHitBox.AddComponent<BoxCollider>();
                //set sizes of stuff
                boxCollider.size = new Vector3(m_width, m_height, m_depth);

                //mesh
                GameObject tempBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newMesh.vertices = tempBox.GetComponent<MeshFilter>().sharedMesh.vertices;
                newMesh.triangles = tempBox.GetComponent<MeshFilter>().sharedMesh.triangles;
                GameObject.DestroyImmediate(tempBox);
                newMesh.name = "Box";
                ScaleHitBoxMesh(newMesh, new Vector3(m_width, m_height, m_depth));
                meshFilter.mesh = newMesh;
                break;

            case Shape.Sphere:
                //collider
                SphereCollider sphereCollider = newHitBox.AddComponent<SphereCollider>();
                //set sizes of stuff
                sphereCollider.radius = m_radius;

                //mesh
                GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newMesh.vertices = tempSphere.GetComponent<MeshFilter>().sharedMesh.vertices;
                newMesh.triangles = tempSphere.GetComponent<MeshFilter>().sharedMesh.triangles;
                newMesh.normals = tempSphere.GetComponent<MeshFilter>().sharedMesh.normals;
                GameObject.DestroyImmediate(tempSphere);
                newMesh.name = "SphereBox";
                ScaleHitBoxMesh(newMesh, new Vector3(m_radius * 2, m_radius * 2, m_radius * 2));
                meshFilter.mesh = newMesh;
                break;

            case Shape.Capsule:
                //collider
                CapsuleCollider capsuleCollider = newHitBox.AddComponent<CapsuleCollider>();
                //set sizes of stuff
                capsuleCollider.height = m_height;
                capsuleCollider.radius = m_radius;

                //mesh
                GameObject tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                newMesh.vertices = tempCylinder.GetComponent<MeshFilter>().sharedMesh.vertices;
                newMesh.triangles = tempCylinder.GetComponent<MeshFilter>().sharedMesh.triangles;
                newMesh.normals = tempCylinder.GetComponent<MeshFilter>().sharedMesh.normals;
                GameObject.DestroyImmediate(tempCylinder);
                ScaleHitBoxMesh(newMesh, new Vector3(1, 0.5f, 1));
                ScaleHitBoxMesh(newMesh, new Vector3(m_radius * 2, m_height - m_radius, m_radius * 2));
                meshFilter.mesh = newMesh;

                Mesh capsuleCapMesh = new Mesh();
                GameObject tempCapsuleCap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GameObject capsuleCapObj = new GameObject();
                capsuleCapObj.transform.parent = m_hitBoxObject.transform;
                capsuleCapObj.name = "Capsule Cap";
                capsuleCapObj.AddComponent<MeshFilter>();
                capsuleCapObj.AddComponent<MeshRenderer>();
                capsuleCapObj.GetComponent<MeshRenderer>().material = m_hitBoxMaterial;
                GameObject capsuleCapObj2 = new GameObject();
                capsuleCapObj2.transform.parent = m_hitBoxObject.transform;
                capsuleCapObj2.name = "Capsule Cap";
                capsuleCapObj2.AddComponent<MeshFilter>();
                capsuleCapObj2.AddComponent<MeshRenderer>();
                capsuleCapObj2.GetComponent<MeshRenderer>().material = m_hitBoxMaterial;

                capsuleCapMesh.vertices = tempCapsuleCap.GetComponent<MeshFilter>().mesh.vertices;
                capsuleCapMesh.triangles = tempCapsuleCap.GetComponent<MeshFilter>().mesh.triangles;
                capsuleCapMesh.normals = tempCapsuleCap.GetComponent<MeshFilter>().mesh.normals;

                GameObject.DestroyImmediate(tempCapsuleCap);

                capsuleCapObj.GetComponent<MeshFilter>().mesh = capsuleCapObj2.GetComponent<MeshFilter>().mesh = capsuleCapMesh;


                ScaleHitBoxMesh(capsuleCapMesh, new Vector3(m_radius * 2, m_radius * 2, m_radius * 2));
                m_hitBoxObject.transform.GetChild(0).transform.localPosition = new Vector3(0, (-m_height / 2.0f) + m_radius, 0);
                m_hitBoxObject.transform.GetChild(1).transform.localPosition = new Vector3(0, (m_height / 2.0f) - m_radius, 0);
                break;
        }


        if (m_owner.m_debugHitBoxes == false)
        {
            newHitBoxMeshRenderer.enabled = false;
            if (m_shape != Shape.Capsule)
            {
                return m_hitBoxObject;
            }
            foreach (MeshRenderer mR in m_hitBoxObject.GetComponentsInChildren<MeshRenderer>())
            {
                mR.enabled = false;
            }
        }
        return m_hitBoxObject;
    }

    public void UpdateDebugMeshSize()
    {
        Mesh hitBoxMesh = m_hitBoxObject.GetComponent<MeshFilter>().mesh;

        switch (m_shape)
        {
            case Shape.Box:

                GameObject tempBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                hitBoxMesh.vertices = tempBox.GetComponent<MeshFilter>().mesh.vertices;
                hitBoxMesh.triangles = tempBox.GetComponent<MeshFilter>().mesh.triangles;
                GameObject.DestroyImmediate(tempBox);
                ScaleHitBoxMesh(hitBoxMesh, new Vector3(m_width, m_height, m_depth));
                m_hitBoxObject.GetComponent<MeshFilter>().mesh = hitBoxMesh;
                break;

            case Shape.Sphere:

                GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hitBoxMesh.vertices = tempSphere.GetComponent<MeshFilter>().mesh.vertices;
                hitBoxMesh.triangles = tempSphere.GetComponent<MeshFilter>().mesh.triangles;
                GameObject.DestroyImmediate(tempSphere);
                ScaleHitBoxMesh(hitBoxMesh, new Vector3(m_radius * 2, m_radius * 2, m_radius * 2));
                m_hitBoxObject.GetComponent<MeshFilter>().mesh = hitBoxMesh;
                break;

            case Shape.Capsule:
                GameObject tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                hitBoxMesh.vertices = tempCylinder.GetComponent<MeshFilter>().mesh.vertices;
                hitBoxMesh.triangles = tempCylinder.GetComponent<MeshFilter>().mesh.triangles;
                GameObject.DestroyImmediate(tempCylinder);
                ScaleHitBoxMesh(hitBoxMesh, new Vector3(1, 0.5f, 1));
                ScaleHitBoxMesh(hitBoxMesh, new Vector3(m_radius * 2, m_height - m_radius * 2, m_radius * 2));
                m_hitBoxObject.GetComponent<MeshFilter>().mesh = hitBoxMesh;

                Mesh capsuleCapMesh = new Mesh();
                GameObject tempCapsuleCap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GameObject capsuleCapObj = m_hitBoxObject.transform.GetChild(0).gameObject;
                GameObject capsuleCapObj2 = m_hitBoxObject.transform.GetChild(1).gameObject;

                capsuleCapMesh.vertices = tempCapsuleCap.GetComponent<MeshFilter>().mesh.vertices;
                capsuleCapMesh.triangles = tempCapsuleCap.GetComponent<MeshFilter>().mesh.triangles;

                GameObject.DestroyImmediate(tempCapsuleCap);
                ScaleHitBoxMesh(capsuleCapMesh, new Vector3(m_radius * 2, m_radius * 2, m_radius * 2));

                capsuleCapObj.GetComponent<MeshFilter>().mesh = capsuleCapObj2.GetComponent<MeshFilter>().mesh = capsuleCapMesh;


                m_hitBoxObject.transform.GetChild(0).transform.localPosition = new Vector3(0, (-m_height / 2.0f) + m_radius, 0);
                m_hitBoxObject.transform.GetChild(1).transform.localPosition = new Vector3(0, (m_height / 2.0f) - m_radius, 0);
                break;
        }
    }

    //Turn Hit Box debug on or off true = on false = off
    public void DebugHitBoxesSwitch(bool onOff)
    {
        if (m_hitBoxObject == null)
        {
            return;
        }
        m_hitBoxObject.GetComponent<MeshRenderer>().enabled = onOff;
        if (m_shape != Shape.Capsule)
        {
            return;
        }
        foreach (MeshRenderer mR in m_hitBoxObject.GetComponentsInChildren<MeshRenderer>())
        {
            mR.enabled = onOff;
        }
    }


    public void DestroyHitBoxObject()
    {
        GameObject.Destroy(m_hitBoxObject.gameObject);
    }
    private void ScaleHitBoxMesh(Mesh mesh, Vector3 scale)
    {
        Vector3[] baseVertices = mesh.vertices;
        Vector3[] vertices = new Vector3[mesh.vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseVertices[i];
            vertex.x *= scale.x;
            vertex.y *= scale.y;
            vertex.z *= scale.z;

            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
    }

}
