using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise{

    public enum NormaliseMode{
        Local,
        Global
    };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormaliseMode normaliseMode){
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octavesOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1f;
        float frequency = 1f;

        for (int i = 0; i < octaves; i++){
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if(scale <=0)
            scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float haldHeight = mapHeight / 2f;

        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){
                amplitude = 1f;
                frequency = 1f;
                float noiseHeight= 0;

                for(int i = 0; i < octaves; i++){
                    float sampleX = (x - halfWidth + octavesOffsets[i].x) / scale * frequency;
                    float sampleY = (y - haldHeight + octavesOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                
                if(noiseHeight > maxNoiseHeight){
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight){
                    minNoiseHeight = noiseHeight;
                }
                

                noiseMap[x, y] = noiseHeight;
            }
        }

        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){
                if(normaliseMode == NormaliseMode.Local){
                    noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
                }
                else if(normaliseMode == NormaliseMode.Global){
                    float normalisedHeight = (noiseMap[x,y] + 1) /(2f * maxPossibleHeight / 1.75f);
                    noiseMap[x,y] = Mathf.Clamp(normalisedHeight,0, int.MaxValue);
                }
                
            }
        }

        return noiseMap;
    }
}
