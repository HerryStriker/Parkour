using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DynamicPlane : MonoBehaviour
{
    public int width = 10;  // Number of vertices in width
    public int height = 10; // Number of vertices in height
    [SerializeField] float heightScale = 1;
    public float cellSize = 1f; // Size of each grid cell

    [SerializeField] Mesh2D mesh2D;

    void OnValidate()
    {
        GeneratePlane();
    }

    void GeneratePlane()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int verticesCount = (width + 1) * (height + 1);
        Vector3[] vertices = new Vector3[verticesCount];
        Vector2[] uv = new Vector2[verticesCount];
        int[] triangles = new int[width * height * 6];

        // Generate vertices & UVs
        for (int i = 0, z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                int index = z * (width + 1) + x;
                Debug.Log(index);
                float noise_height = mesh2D.heightMap[index] * heightScale;

                vertices[i] = new Vector3(x * cellSize,noise_height , z * cellSize);
                uv[i] = new Vector2((float)x / width, (float)z / height);
            }
        }

        // Generate triangles
        int triIndex = 0;
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int start = z * (width + 1) + x;

                // Triangle 1
                triangles[triIndex++] = start;
                triangles[triIndex++] = start + width + 1;
                triangles[triIndex++] = start + 1;

                // Triangle 2
                triangles[triIndex++] = start + 1;
                triangles[triIndex++] = start + width + 1;
                triangles[triIndex++] = start + width + 2;
            }
        }

        // Assign to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals(); // Compute normal for lighting
    }
}
