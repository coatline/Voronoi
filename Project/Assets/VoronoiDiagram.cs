using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{
    [SerializeField] List<Vector2Int> centroidPositions;
    [SerializeField] float randPointVariablility;
    [SerializeField] GameObject cubePrefab;
    [SerializeField] Vector2Int imageDim;
    [SerializeField] Biome[] biomes;
    [SerializeField] bool showDist;

    void Start()
    {
        Vector2 noiseOffset = new Vector2(Random.Range(0, 999999f), Random.Range(0, 999999f));
        centroidPositions = new List<Vector2Int>();

        Dictionary<Vector2Int, Biome> centroids = GetCentroids();
        Biome[,] regions = GetRegions(centroids);

        if (cubePrefab)
        {
            for (int x = 1; x < imageDim.x - 1; x++)
            {
                for (int y = 1; y < imageDim.y - 1; y++)
                {
                    float noise1 = Mathf.PerlinNoise((float)(x + noiseOffset.x) * (float)regions[x, y].scale, (float)(y + noiseOffset.y) * (float)regions[x, y].scale);
                    float noise2 = Mathf.PerlinNoise((float)(x + 1 + noiseOffset.x) * (float)regions[x + 1, y].scale, (float)(y + noiseOffset.y) * (float)regions[x + 1, y].scale);
                    float noise3 = Mathf.PerlinNoise((float)(x - 1 + noiseOffset.x) * (float)regions[x - 1, y].scale, (float)(y + noiseOffset.y) * (float)regions[x - 1, y].scale);
                    float noise4 = Mathf.PerlinNoise((float)(x + noiseOffset.x) * (float)regions[x, y + 1].scale, (float)(y + 1 + noiseOffset.y) * (float)regions[x, y + 1].scale);
                    float noise5 = Mathf.PerlinNoise((float)(x + noiseOffset.x) * (float)regions[x, y - 1].scale, (float)(y - 1 + noiseOffset.y) * (float)regions[x, y - 1].scale);
                    var noise = (noise1 + noise2 + noise3 + noise4 + noise5) / 5;
                    var nCube = Instantiate(cubePrefab, new Vector3Int(x, ((int)(noise * regions[x, y].height)), y), Quaternion.identity);
                    nCube.GetComponent<MeshRenderer>().material.color = regions[x, y].color;
                }
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(showDist ? GetDiagramByDistance(centroids, regions) : GetDiagram(regions), new Rect(0, 0, imageDim.x, imageDim.y), Vector2.one * .5f);
        }
    }

    Dictionary<Vector2Int, Biome> GetCentroids()
    {
        Dictionary<Vector2Int, Biome> centroids = new Dictionary<Vector2Int, Biome>();

        for (int i = 0; i < biomes.Length; i++)
        {
            centroids.Add(new Vector2Int(Random.Range(0, imageDim.x), Random.Range(0, imageDim.y)), biomes[i]);
        }

        return centroids;
    }

    Biome[,] GetRegions(Dictionary<Vector2Int, Biome> centroids)
    {
        Biome[,] regions = new Biome[imageDim.x, imageDim.y];

        for (int x = 0; x < imageDim.x; x++)
        {
            for (int y = 0; y < imageDim.y; y++)
            {
                var offset = new Vector2Int((int)(Mathf.PerlinNoise(x, y) * randPointVariablility), (int)(Mathf.PerlinNoise(x, y) * randPointVariablility));
                regions[x, y] = centroids[GetClosestCentroidKey(new Vector2Int(x, y), centroids)];
            }
        }

        return regions;
    }

    Texture2D GetDiagram(Biome[,] regions)
    {
        Color[,] pixelColors = new Color[imageDim.x, imageDim.y];

        for (int x = 0; x < imageDim.x; x++)
        {
            for (int y = 0; y < imageDim.y; y++)
            {
                pixelColors[x, y] = regions[x, y].color;
            }
        }

        //    for (int i = 0; i < centroids.; i++)
        //{
        //    int index = centroids[i].x * imageDim.x + centroids[i].y;
        //    pixelColors[index] = Color.black;
        //}

        return GetImageFromColorArray(pixelColors);
    }

    Texture2D GetDiagramByDistance(Dictionary<Vector2Int, Biome> centroids, Biome[,] regions)
    {
        Color[,] pixelColors = new Color[imageDim.x, imageDim.y];

        for (int x = 0; x < imageDim.x; x++)
        {
            for (int y = 0; y < imageDim.x; y++)
            {
                var dist = Vector2.Distance(GetClosestCentroidKey(new Vector2Int(x, y), centroids), new Vector2(x, y));
                pixelColors[x, y] = new Color(dist / 255, dist / 255, dist / 255, 1f);
            }
        }

        return GetImageFromColorArray(pixelColors);
    }

    Vector2Int GetClosestCentroidKey(Vector2Int pixelPos, Dictionary<Vector2Int, Biome> centroids)
    {
        float smallestDist = float.MaxValue;
        Vector2Int key = Vector2Int.zero;

        foreach (KeyValuePair<Vector2Int, Biome> k in centroids)
        {
            centroidPositions.Add(k.Key);

            var dist = Vector2.Distance(pixelPos, k.Key);

            if (dist < smallestDist)
            {
                smallestDist = dist;
                key = k.Key;
            }
        }

        return key;
    }

    Texture2D GetImageFromColorArray(Color[,] pixelColors)
    {
        Texture2D tex = new Texture2D(imageDim.x, imageDim.y);
        tex.filterMode = FilterMode.Point;
        Color[] pc = new Color[pixelColors.Length];

        var colmns = (pixelColors.Length / imageDim.y);
        var ind = 0;
        for (int x = 0; x < imageDim.x; x++)
        {
            for (int y = 0; y < imageDim.y; y++)
            {
                pc[ind] = pixelColors[x, y];
                print(pc[ind]);
                ind++;
            }
        }

        tex.SetPixels(pc);
        tex.Apply();
        return tex;
    }
}
