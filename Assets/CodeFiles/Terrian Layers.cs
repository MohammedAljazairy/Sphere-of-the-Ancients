using System.Xml.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TerrianLayers : MonoBehaviour
{
    private Terrain terrain;
    public float[] thresholds;
    void Start()
    {
        //terrain=gameObject.GetComponent<Terrain>();
        //ApplyTerrianBasedonElevation();
    }
    void ApplyTerrianBasedonElevation()
    {
        TerrainData terrainData=terrain.terrainData;
        int width=terrainData.alphamapWidth;
        int height=terrainData.alphamapHeight;
        int layers=terrainData.alphamapLayers;

        float maxheight = GetMaxheight();
        float[,,] splatMap=new float[height,width,layers];
        for (int x=0;x<width;x++)
        {
            for(int y=0;y<height;y++)
            {
                float h=terrainData.GetHeight(y,x);
                float normalized=h/maxheight;
                int layerIndex=GetLayerbasedonElevation(normalized);
                for (int l=0;l<layers;l++)
                {
                    if(l==layerIndex)
                        splatMap[x,y,l]=1f;
                    else
                        splatMap[x,y,l]=0f;    
                }
            }
        }
        terrainData.SetAlphamaps(0,0,splatMap);
    }
    int GetLayerbasedonElevation(float height)
    {
        TerrainData terrainData=terrain.terrainData;
        if(terrainData.alphamapLayers==1)
            return 0;
        int l;
        for(l=0;l<thresholds.Length;l++)
        {
            if(height<=thresholds[l])
                return l;
        }

        return l;
    }
    float GetMaxheight()
    {
        TerrainData data=terrain.terrainData;
        int width=data.heightmapResolution;
        int height=data.heightmapResolution;
        float maxheight=float.MinValue;
        for(int y=0;y<height;y++)
        {
            for(int x=0;x<width;x++)
            {
                float heightvalue=data.GetHeight(y,x);
                if(heightvalue>maxheight)
                    maxheight=heightvalue;

            }
            
        }
        return maxheight;
    }
}
