using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralTerrain : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI IScale,IHeight,X,Y;
    [SerializeField] Slider S,H,XX,YY;
    public int width = 256;
    public int depth = 256;
    public float scale = 20f;
    public float height = 20f;

    float randomOffsetX;
    float randomOffsetZ;
    public Vector2 offset = Vector2.zero;
    void Start ()
    {
        randomOffsetX = Random.Range(0f, 100f);
        randomOffsetZ = Random.Range(0f, 100f);
        
    }

    private void Update() {
        GenerateTerrain();

        offset.x = XX.value;
        offset.y = YY.value;

        scale = S.value;
        height = H.value;

        IScale.text = "Scale = " + S.value;
        IHeight.text = "Height = " + H.value;
        X.text = "Offset X = " + XX.value;
        Y.text = "Offset Y = " + YY.value;
    }

    void GenerateTerrain()
    {
        
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(width + 1) * (depth + 1)];
        
       
        float centerX = width / 2f;
        float centerZ = depth / 2f;

        
        

        for (int i = 0, z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                
                float distanceFromCenter = Vector2.Distance(new Vector2(x, z), new Vector2(centerX, centerZ));

                
                float noise1 = Mathf.PerlinNoise((x + randomOffsetX + offset.x) * scale * 0.05f, (z + randomOffsetZ + offset.y) * scale * 0.05f);
                float noise2 = Mathf.PerlinNoise((x + randomOffsetX + offset.x) * scale * 0.1f, (z + randomOffsetZ + offset.y) * scale * 0.1f) * 0.5f;
                float noise3 = Mathf.PerlinNoise((x + randomOffsetX + offset.x) * scale * 0.2f, (z + randomOffsetZ + offset.y) * scale * 0.2f) * 0.25f;

                float combinedNoise = noise1 + noise2 + noise3;

                float falloff = Mathf.Clamp01(.9f * combinedNoise - (distanceFromCenter / ((Mathf.Max(centerX, centerZ)))));

                float y = falloff * combinedNoise * height;

                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        mesh.vertices = vertices;

        int[] triangles = new int[width * depth * 6];
        
        for (int z = 0, vert = 0, tris = 0; z < depth; z++, vert++)
        {
            for (int x = 0; x < width; x++, vert++, tris += 6)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;
            }
        }

        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }


}
