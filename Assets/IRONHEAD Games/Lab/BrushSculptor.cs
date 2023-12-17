using UnityEngine;

public enum BrushType { Raise, Lower, Smooth }

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class BrushSculptor : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    Vector3[] originalVertices;
    MeshCollider meshCollider;

    public float brushRadius = 3f;
    public float brushStrength = 0.1f; // Adjusted for direct manipulation without Time.deltaTime
    public BrushType brushType = BrushType.Raise;

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        InitializeMesh();
    }

    void InitializeMesh()
    {
        var meshFilter = GetComponent<MeshFilter>();
        mesh = Instantiate(meshFilter.sharedMesh);
        vertices = mesh.vertices;
        originalVertices = (Vector3[])vertices.Clone();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
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
                    ApplyBrush(hitPoint);
                }
            }
        }
    }

    void ApplyBrush(Vector3 hitPoint)
    {
        hitPoint = transform.InverseTransformPoint(hitPoint);

        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = (hitPoint - vertices[i]).magnitude;
            if (distance < brushRadius)
            {
                float strength = (brushRadius - distance) / brushRadius * brushStrength;
                Vector3 direction = (brushType == BrushType.Lower) ? Vector3.down : Vector3.up;
                
                if (brushType != BrushType.Smooth)
                {
                    vertices[i] += direction * strength;
                }
                else
                {
                    vertices[i] = Vector3.Lerp(vertices[i], originalVertices[i], strength);
                }
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds(); // Added for recalculating the mesh bounds
        meshCollider.sharedMesh = mesh;
    }
}
