using Unity.VisualScripting;
using UnityEngine;

public class Mesh2D : MonoBehaviour

{
    [SerializeField] int xFactor = 1;
    [SerializeField] int yFactor = 1;

    [SerializeField] float xCoord;
    [SerializeField] float yCoord;
    [SerializeField] float heightScale ;

    [SerializeField] float scale = 1;

    [field: SerializeField] public float[] heightMap {get; private set;}

    [SerializeField] Texture2D texture;
    [SerializeField] Color[] noiseColor;
    [SerializeField] Renderer rend;

    [SerializeField] int dimentions = 2;
    int[,] dim;

    int length => dimentions * dimentions;

    void OnValidate()
    {
        rend = GetComponent<Renderer>();

        texture = new Texture2D(xFactor,yFactor);
        noiseColor = new Color[texture.width * texture.height];
        heightMap = new float[texture.width * texture.height];
        rend.material.mainTexture = texture;

        dim = new int[texture.width, texture.height];

        UpdateNoise();
    }

    void UpdateNoise()
    {
        for (float x = 0; x < texture.width; x++)
        {
            for (float y = 0; y < texture.height; y++)
            {
                float coordX = xCoord + x / texture.width * scale;
                float coordY =  yCoord + y / texture.height * scale; 

                float noise = Mathf.PerlinNoise(coordX,coordY);
                noiseColor[(int)y * texture.width + (int)x] = new Color(noise, noise, noise);
                heightMap[(int)y * texture.width + (int)x] = noise;

                //Debug.DrawRay(GetPosition(x, y, noise, heightScale), Vector3.up * heightScale, Color.red);

            }
        }

        texture.SetPixels(noiseColor);
        texture.Apply();
    }

    void Update()
    {
                

    }

    Vector3 GetPosition(int x, int z)
    {
        return new Vector3(x, 0, z);
    }
    Vector3 GetPosition(float x, float z, float h, float scale)
    {
        return new Vector3(x, h * scale, z);
    }


    void OnDrawGizmos()
    {
        // for (int x = 0; x < dim.GetLength(0); x++)
        // {
        //     for (int z = 0; z < dim.GetLength(1); z++)
        //     {
        //         Gizmos.color = Color.red;
        //         Gizmos.DrawSphere(GetPosition(x,z), .1f);
                

        //     }
        // }
    }
}
