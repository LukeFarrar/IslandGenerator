using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public Material terrainMaterial;
    public float waterLevel = .1f;
    public float shallowLevel = .3f;
    public float sandLevel = .45f;
    public float mountainLevel = 0.95f;
    public float iceLevel = 0.9f;
    public float scale = .1f;
    public int size = 100;

    Cell[,] grid;

    void Start() {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        float[,] falloffMap = new float[size, size];
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }

        grid = new Cell[size, size];
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y];
                if(noiseValue < waterLevel)
                {
                    Cell cell = new Cell(Terrain.Water);
                    grid[x, y] = cell;
                    Debug.Log(noiseValue);
                }
                else if(noiseValue < sandLevel)
                {
                    Cell cell = new Cell(Terrain.Sand);
                    grid[x, y] = cell;  
                }
                else if(noiseValue >= mountainLevel)
                {
                    Cell cell = new Cell(Terrain.Ice);
                    grid[x, y] = cell;                    
                }
                else if(noiseValue >= iceLevel)
                {
                    Cell cell = new Cell(Terrain.Mountain);
                    grid[x, y] = cell;
                }
                else
                {
                    Cell cell = new Cell(Terrain.Land);
                    grid[x, y] = cell;
                }          
            }
        }

        DrawTerrainMesh(grid);
        DrawTexture(grid);
    }

    void DrawTerrainMesh(Cell[,] grid) {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Cell cell = grid[x, y];
                /* if(!cell.isWater) { */
                    Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                    Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                    Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                    Vector3 d = new Vector3(x + .5f, 0, y - .5f);
                    Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
                    Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                    Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                    Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                    Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                    for(int k = 0; k < 6; k++) {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[k]);
                    }
                /* } */
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    void DrawTexture(Cell[,] grid) {
        Texture2D texture = new Texture2D(size, size);
        Color[] colorMap = new Color[size * size];
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                Cell cell = grid[x, y];
                if(cell.terrain == Terrain.Water)
                    colorMap[y * size + x] = Color.blue;
                else if(cell.terrain == Terrain.Land)
                    colorMap[y * size + x] = Color.green;
                else if(cell.terrain == Terrain.Mountain)
                    colorMap[y * size + x] = Color.gray;
                else if(cell.terrain == Terrain.Ice)
                    colorMap[y * size + x] = Color.white;
                else if(cell.terrain == Terrain.Sand)
                    colorMap[y * size + x] = Color.yellow;
                else if(cell.terrain == Terrain.Lava)
                    colorMap[y * size + x] = Color.red;
                else if(cell.terrain == Terrain.Shallow)
                    colorMap[y * size + x] = Color.cyan;
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = terrainMaterial;
        meshRenderer.material.mainTexture = texture;
    }


}