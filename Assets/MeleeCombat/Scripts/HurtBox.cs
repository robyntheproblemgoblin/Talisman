using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]

public class HurtBox
{
    public HurtBox(Shape shape, Transform parent)
    {
        m_shape = shape;
        Assert.IsTrue(parent.GetComponent<CombatItem>() != null, "HurtBox Not Parented to a combat item");
        m_owner = parent.GetComponent<CombatItem>();
        m_hurtBoxParentObject = m_owner.transform;
        m_hurtBoxMaterial = Resources.Load("Materials/HurtBoxMaterial") as Material;
        ConstructHurtbox();
    }
    [SerializeField] public CombatItem m_owner;
    [SerializeField] public Collider m_collider;
    [SerializeField] public Vector3 m_center = Vector3.zero;
    [SerializeField] public HurtBoxObject m_hurtBoxObject;
    [SerializeField] private Transform m_hurtBoxParentObject;
    private Material m_hurtBoxMaterial;
    public enum Shape
    {
        Box,
        Sphere,
        Capsule
    }
    [SerializeField] Shape m_shape;


    //box
    [SerializeField] private float m_width = 1, m_height = 1, m_depth = 1;

    //sphere and capsule
    [SerializeField] private float m_radius = 1;

    HurtBoxObject ConstructHurtbox()
    {
        GameObject newHurtBoxObject = new GameObject();
        newHurtBoxObject.AddComponent<HurtBoxObject>();
        newHurtBoxObject.GetComponent<HurtBoxObject>().m_hurtbox = this;
        newHurtBoxObject.transform.parent = m_owner.transform;
        newHurtBoxObject.transform.localPosition = Vector3.zero;
        Rigidbody newHurtBoxRigidbody = newHurtBoxObject.AddComponent<Rigidbody>();
        newHurtBoxRigidbody.isKinematic = true;
        Assert.IsFalse(LayerMask.NameToLayer("Melee") == -1, "You Need To Create A Layer Named \"Melee\"");
        newHurtBoxObject.layer = LayerMask.NameToLayer("Melee");

        newHurtBoxObject.AddComponent<MeshRenderer>();
        MeshRenderer newHurtBoxMeshRenderer = newHurtBoxObject.GetComponent<MeshRenderer>();
        
        newHurtBoxMeshRenderer.material = m_hurtBoxMaterial;
        newHurtBoxObject.AddComponent<MeshFilter>();
        MeshFilter meshFilter = newHurtBoxObject.GetComponent<MeshFilter>();
        Mesh newMesh = new Mesh();
        m_hurtBoxObject = newHurtBoxObject.GetComponent<HurtBoxObject>();


        switch (m_shape)
        {
            case Shape.Box:
                BoxCollider boxCollider = newHurtBoxObject.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(m_width, m_height, m_depth);
                boxCollider.isTrigger = true;

                GameObject tempBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newMesh.vertices = tempBox.GetComponent<MeshFilter>().sharedMesh.vertices;
                newMesh.triangles = tempBox.GetComponent<MeshFilter>().sharedMesh.triangles;
                GameObject.DestroyImmediate(tempBox);
                newMesh.name = "Box";
                ScaleHurtBoxMesh(newMesh, new Vector3(m_width, m_height, m_depth));
                meshFilter.mesh = newMesh;
                break;

            case Shape.Sphere:
                SphereCollider sphereCollider = newHurtBoxObject.AddComponent<SphereCollider>();
                sphereCollider.radius = m_radius;
                sphereCollider.isTrigger = true;

                GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newMesh.vertices = tempSphere.GetComponent<MeshFilter>().sharedMesh.vertices;
                newMesh.triangles = tempSphere.GetComponent<MeshFilter>().sharedMesh.triangles;
                GameObject.DestroyImmediate(tempSphere);
                newMesh.name = "SphereBox";
                ScaleHurtBoxMesh(newMesh, new Vector3(m_radius * 2, m_radius * 2, m_radius * 2));
                meshFilter.mesh = newMesh;
                break;

            case Shape.Capsule:
                CapsuleCollider capsuleCollider = newHurtBoxObject.AddComponent<CapsuleCollider>();
                capsuleCollider.height = m_height;
                capsuleCollider.radius = m_radius;
                capsuleCollider.center = m_center;
                capsuleCollider.isTrigger = true;

                GameObject tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                newMesh.vertices = tempCylinder.GetComponent<MeshFilter>().sharedMesh.vertices;
                newMesh.triangles = tempCylinder.GetComponent<MeshFilter>().sharedMesh.triangles;
                GameObject.DestroyImmediate(tempCylinder);
                ScaleHurtBoxMesh(newMesh, new Vector3(1, 0.5f, 1));
                ScaleHurtBoxMesh(newMesh, new Vector3(m_radius * 2, m_height - m_radius, m_radius * 2));
                meshFilter.mesh = newMesh;

                Mesh capsuleCapMesh = new Mesh();
                GameObject tempCapsuleCap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GameObject capsuleCapObj = new GameObject();
                capsuleCapObj.transform.parent = m_hurtBoxObject.transform;
                capsuleCapObj.name = "Capsule Cap";
                capsuleCapObj.AddComponent<MeshFilter>();
                capsuleCapObj.AddComponent<MeshRenderer>();
                capsuleCapObj.GetComponent<MeshRenderer>().material = m_hurtBoxMaterial;
                GameObject capsuleCapObj2 = new GameObject();
                capsuleCapObj2.transform.parent = m_hurtBoxObject.transform;
                capsuleCapObj2.name = "Capsule Cap";
                capsuleCapObj2.AddComponent<MeshFilter>();
                capsuleCapObj2.AddComponent<MeshRenderer>();
                capsuleCapObj2.GetComponent<MeshRenderer>().material = m_hurtBoxMaterial;

                capsuleCapMesh.vertices  = tempCapsuleCap.GetComponent<MeshFilter>().sharedMesh.vertices;
                capsuleCapMesh.triangles = tempCapsuleCap.GetComponent<MeshFilter>().sharedMesh.triangles;

                GameObject.DestroyImmediate(tempCapsuleCap);

                capsuleCapObj.GetComponent<MeshFilter>().sharedMesh = capsuleCapObj2.GetComponent<MeshFilter>().sharedMesh = capsuleCapMesh;


                ScaleHurtBoxMesh(capsuleCapMesh, new Vector3(m_radius *  2, m_radius * 2, m_radius * 2));
                m_hurtBoxObject.transform.GetChild(0).transform.localPosition = new Vector3(0, (-m_height / 2.0f) + m_radius, 0);
                m_hurtBoxObject.transform.GetChild(1).transform.localPosition = new Vector3(0, (m_height / 2.0f) - m_radius, 0);
                break;
        }



        if (m_owner.m_debugHurtBoxes == false)
        {
            newHurtBoxMeshRenderer.enabled = false;
            if (m_shape != Shape.Capsule)
            {
                return m_hurtBoxObject;
            }
            foreach (MeshRenderer mR in m_hurtBoxObject.GetComponentsInChildren<MeshRenderer>())
            {
                mR.enabled = false;
            }
        }
        return m_hurtBoxObject;

    }

    public void UpdateDebugMeshSize()
    {
        //band-aid fix and this sucks
        Mesh hurtBoxMesh = m_hurtBoxObject.GetComponent<MeshFilter>().sharedMesh;
        
        switch (m_shape)
        {
            case Shape.Box:

                GameObject tempBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                hurtBoxMesh.vertices = tempBox.GetComponent<MeshFilter>().sharedMesh.vertices;
                hurtBoxMesh.triangles = tempBox.GetComponent<MeshFilter>().sharedMesh.triangles;
                GameObject.DestroyImmediate(tempBox);
                ScaleHurtBoxMesh(hurtBoxMesh, new Vector3(m_width, m_height, m_depth));
                m_hurtBoxObject.GetComponent<MeshFilter>().mesh = hurtBoxMesh;
                break;

            case Shape.Sphere:

                GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hurtBoxMesh.vertices = tempSphere.GetComponent<MeshFilter>().sharedMesh.vertices;
                hurtBoxMesh.triangles = tempSphere.GetComponent<MeshFilter>().sharedMesh.triangles;
                GameObject.DestroyImmediate(tempSphere);
                ScaleHurtBoxMesh(hurtBoxMesh, new Vector3(m_radius * 2, m_radius * 2, m_radius * 2));
                m_hurtBoxObject.GetComponent<MeshFilter>().mesh = hurtBoxMesh;
                break;

            case Shape.Capsule:
                GameObject tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                hurtBoxMesh.vertices = tempCylinder.GetComponent<MeshFilter>().sharedMesh.vertices;
                hurtBoxMesh.triangles = tempCylinder.GetComponent<MeshFilter>().sharedMesh.triangles;
                GameObject.DestroyImmediate(tempCylinder);
                ScaleHurtBoxMesh(hurtBoxMesh, new Vector3(1, 0.5f, 1));
                ScaleHurtBoxMesh(hurtBoxMesh, new Vector3(m_radius * 2, m_height - m_radius * 2, m_radius * 2));
                m_hurtBoxObject.GetComponent<MeshFilter>().mesh = hurtBoxMesh;

                Mesh capsuleCapMesh = new Mesh();
                GameObject tempCapsuleCap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GameObject capsuleCapObj = m_hurtBoxObject.transform.GetChild(0).gameObject;
                GameObject capsuleCapObj2 = m_hurtBoxObject.transform.GetChild(1).gameObject;

                capsuleCapMesh.vertices = tempCapsuleCap.GetComponent<MeshFilter>().sharedMesh.vertices;
                capsuleCapMesh.triangles = tempCapsuleCap.GetComponent<MeshFilter>().sharedMesh.triangles;

                GameObject.DestroyImmediate(tempCapsuleCap);
                ScaleHurtBoxMesh(capsuleCapMesh, new Vector3(m_radius * 2, m_radius * 2, m_radius * 2));

                capsuleCapObj.GetComponent<MeshFilter>().sharedMesh = capsuleCapObj2.GetComponent<MeshFilter>().sharedMesh = capsuleCapMesh;


                m_hurtBoxObject.transform.GetChild(0).transform.localPosition = new Vector3(0, (-m_height / 2.0f) + m_radius, 0);
                m_hurtBoxObject.transform.GetChild(1).transform.localPosition = new Vector3(0, (m_height / 2.0f) - m_radius, 0);
                break;
        }
    }

    public void DestroyHurtBox()
    {
        if(m_hurtBoxObject != null)
        {
            m_hurtBoxObject.StupidDestroyHurtBox();
        }
    }
    private void ScaleHurtBoxMesh(Mesh mesh, Vector3 scale)
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

    public void UpdateHurtBoxObject()
    {
        m_hurtBoxObject.transform.parent = m_hurtBoxParentObject;
        switch (m_shape)
        {
            case Shape.Box:
                BoxCollider boxCollider = m_hurtBoxObject.GetComponent<BoxCollider>();
                boxCollider.size = new Vector3(m_width, m_height, m_depth);
                m_hurtBoxObject.transform.localPosition = m_center;
                break;
            case Shape.Sphere:
                SphereCollider sphereCollider = m_hurtBoxObject.GetComponent<SphereCollider>();
                sphereCollider.radius = m_radius;
                m_hurtBoxObject.transform.localPosition = m_center;
                break;
            case Shape.Capsule:
                CapsuleCollider capsuleCollider = m_hurtBoxObject.GetComponent<CapsuleCollider>();
                capsuleCollider.radius = m_radius;
                capsuleCollider.height = m_height;
                m_hurtBoxObject.transform.localPosition = m_center;
                break;
        }
    }

    //Turn Hit Box debug on or off true = on false = off
    public void DebugHurtBoxesSwitch(bool onOff)
    {
        if (m_hurtBoxObject == null)
        {
            return;
        }
        m_hurtBoxObject.GetComponent<MeshRenderer>().enabled = onOff;
        if (m_shape != Shape.Capsule)
        {
            return;
        }
        foreach (MeshRenderer mR in m_hurtBoxObject.GetComponentsInChildren<MeshRenderer>())
        {
            mR.enabled = onOff;
        }
    }

    public void SetBoxHurtBoxObject(Vector3 extents, Vector3 center)
    {
        extents *= 2.0f;
        m_hurtBoxObject.GetComponent<BoxCollider>().size = extents;
        m_width = extents.x; m_height = extents.y; m_depth = extents.z;
        m_hurtBoxObject.GetComponent<BoxCollider>().center = m_center = center;
    }
    public void SetSphereHurtBoxObject(float radius, Vector3 center)
    {
        m_hurtBoxObject.GetComponent<SphereCollider>().radius = m_radius = radius;
        m_hurtBoxObject.GetComponent<SphereCollider>().center = m_center = center;
    }
    public void SetCapsuleHurtBoxObject(float radius, float height, Vector3 center)
    {
        m_hurtBoxObject.GetComponent<CapsuleCollider>().radius = m_radius = radius;
        m_hurtBoxObject.GetComponent<CapsuleCollider>().height = m_height = height;
        m_hurtBoxObject.GetComponent<CapsuleCollider>().center = m_center = center;
    }
    public void NameHurtBoxObject(string name)
    {
        m_hurtBoxObject.gameObject.name = name;
    }
}
