using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.UI;

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
    public float radius = 1.0f;
    public float force = 1.0f;
    RaycastHit hit;
    Vector3 hitPoint;

    private void Start()
    {
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.MarkDynamic();

        meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        // This memory setup assumes the vertex count will not change.
        vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.Persistent);
        normals = new NativeArray<Vector3>(mesh.normals, Allocator.Persistent);
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