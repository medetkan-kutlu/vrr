using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.UI;
using System;
using TMPro;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshDeformer : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider meshCollider;

    private NativeArray<Vector3> vertices;
    private NativeArray<Vector3> normals;
    private bool scheduled = false;
    private MeshDeformerJob job;
    private JobHandle handle;

    private float radiusInit = 0.002f;
    private float forceInit = 0.0003f;
    public float radius = 0.002f;
    public float force = 0.0003f;
    public Button directionButton;
    public Slider radiusSlider;
    public Slider forceSlider;
    float multiplier = 2f;
    bool isRaise = true;
    RaycastHit hit;
    Vector3 hitPoint;

    private void Start()
    {
        
        forceInit = force;
        radiusInit = radius;

        directionButton = GameObject.FindWithTag("DirectionButton").GetComponent<Button>();
        radiusSlider = GameObject.FindWithTag("RadiusSlider").GetComponent<Slider>();
        forceSlider = GameObject.FindWithTag("StrengthSlider").GetComponent<Slider>();

        forceSlider.minValue = forceInit / multiplier;
        forceSlider.maxValue = forceInit * multiplier;

        radiusSlider.minValue = radiusInit / multiplier;
        radiusSlider.maxValue = radiusInit * multiplier;

        forceSlider.value = forceInit;
        radiusSlider.value = radiusInit;

        forceSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        radiusSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        directionButton.onClick.AddListener(delegate { ChangeDirection(); });

        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.MarkDynamic();

        meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        // This memory setup assumes the vertex count will not change.
        vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.Persistent);
        normals = new NativeArray<Vector3>(mesh.normals, Allocator.Persistent);
    }

    private void ChangeDirection()
    {
        if(isRaise)
        {
            isRaise = false;
            force = -force;
            directionButton.GetComponentInChildren<TMP_Text>().text = "Lower";
        }
        else
        {
            isRaise = true;
            force = -force;
            directionButton.GetComponentInChildren<TMP_Text>().text = "Raise";
        }
    }

    private void ValueChangeCheck()
    {
        print("Radius: " + radiusSlider.value + " Force: " + forceSlider.value);
        radius = radiusSlider.value;
        if(isRaise){
            force = forceSlider.value;
        }
        else{
            force = -forceSlider.value;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // Assuming a layer named "Sculptable"
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Vector3 hitPoint = hit.point;
                    Deform(hitPoint, radius, force);
                }
            }
        }
    }
    private void LateUpdate()
    {
        if( !scheduled )
        {
            return;
        }

        handle.Complete();
        scheduled = false;

        // Copy the results to the managed array.
        job.vertices.CopyTo(vertices);

        // Assign the modified vertices to the mesh.
        mesh.vertices = vertices.ToArray();

        // Normals and tangents have not changed but the mesh has new bounds.
        //mesh.RecalculateNormals();
        //mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        // There is an odd behaviour with the mesh collider, the mesh is updated as expected but internally it still uses the unmodified mesh.
        meshCollider.enabled = false;
        meshCollider.enabled = true;
    }

    private void OnDestroy()
    {
        vertices.Dispose();
        normals.Dispose();
    }

    public void Deform( Vector3 point, float radius, float force )
    {
        job = new MeshDeformerJob
        {
            deltaTime = Time.deltaTime,
            center = transform.InverseTransformPoint(point), // Transform the point from world space to local space.
            radius = radius,
            force = force,
            vertices = vertices,
            normals = normals
        };

        scheduled = true;
        handle = job.Schedule(vertices.Length, 64);
    }
}