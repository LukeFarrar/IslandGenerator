using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture){
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture){
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void DrawObjects(MapData mapData, LifeType[] life, float heightMultiplier, AnimationCurve heightCurve){
        int width = mapData.heightMap.GetLength(0);
        int height = mapData.heightMap.GetLength(1);
        float topLeftX = (width-1)/-2f;
        float topLeftZ = (height-1)/2f;
        
        for(int y =0; y < MapGenerator.mapChunkSize; y++){
            for(int x = 0; x < MapGenerator.mapChunkSize; x++){
                for(int i = 0; i < life.Length; i++){
                    if(mapData.heightMap[x,y] >= life[i].minHeight && mapData.heightMap[x,y] < life[i].maxHeight){
                        GameObject item = Instantiate(life[i].prefab, new Vector3(x, mapData.heightMap[x,y], y), Quaternion.identity);
                        item.transform.position = new Vector3((topLeftX + x) * 10f, heightCurve.Evaluate(mapData.heightMap[x,y]) * heightMultiplier, (topLeftZ - y)* 10f);
                    }
                }
            }
        }
    }
}
