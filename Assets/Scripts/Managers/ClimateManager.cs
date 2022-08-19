using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Range 
{
    public float min;
    public float max;
    public Range(float _min, float _max) 
    {
        min = _min;
        max = _max;
    }
}

public class ClimateManager : Singleton<ClimateManager>
{
    private MeshRenderer[] trees;
    private ParticleSystem[] flameEmitters;
    public int flameStartBoundary; 
    public int treeEndBoundary; 
    [Space]
    public Transform water;
    public Range waterLevels;
    [Space]
    [Range(0,100)]
    public int environmentalLevel;
    public int previousEnvironmentalLevel;
    public float treePercent;

    private void Start()
    {
        GameObject[] tempArray = GameObject.FindGameObjectsWithTag("Trees");
        List<MeshRenderer> renderers = new List<MeshRenderer>();

        foreach (GameObject item in tempArray)
        {
            MeshRenderer mr = item.GetComponentInChildren<MeshRenderer>();
            renderers.Add(mr);
        }

        trees = renderers.ToArray();

        treePercent = trees.Length / 100.0f;

        tempArray = GameObject.FindGameObjectsWithTag("Flames");
        List<ParticleSystem> emitters = new List<ParticleSystem>();

        foreach (GameObject item in tempArray)
        {
            ParticleSystem ps = item.GetComponentInChildren<ParticleSystem>();
            emitters.Add(ps);
        }

        flameEmitters = emitters.ToArray();
    }

    private void Update()
    {
        UpdateClimateLevel();
    }

    private void UpdateClimateLevel() 
    {
        if (environmentalLevel == previousEnvironmentalLevel) return;

        //Trees

        //int treeCount = (int)(treePercent * (100 - environmentalLevel));
        float treeCount = treePercent * (
            (environmentalLevel - treeEndBoundary) *
            (100 / (100 - treeEndBoundary))
            );
        treeCount = trees.Length - treeCount;
        treeCount = Mathf.Clamp(treeCount, 0, trees.Length);
        //Debug.Log("Tree Count = " + treeCount);

        List<int> availableIndices = new List<int>();

        for (int i = 0; i < trees.Length; i++)
        {
            availableIndices.Add(i);
            trees[i].enabled = true;
        }

        for (int i = 0; i < treeCount; i++)
        {
            int j = Random.Range(0, availableIndices.Count);
            trees[availableIndices[j]].enabled = false;
            availableIndices.RemoveAt(j);
        }

        //Water
        Vector3 targetPosition = water.position;
        targetPosition.y = Mathf.Lerp(waterLevels.min, waterLevels.max, (100-environmentalLevel)/100.0f);
        water.position = targetPosition;

        //Flames
        List<int> emittersToBeActive = new List<int>();
        float amountOfFlames = flameStartBoundary - environmentalLevel;
        if(amountOfFlames > 0) 
        {
            float x = (100.0f / flameStartBoundary);
            float y = amountOfFlames * x;
            float z = (flameEmitters.Length / 100.0f) * y;

            amountOfFlames = Mathf.CeilToInt(z);

            Debug.Log("AOF = " + amountOfFlames + " -> total:" + flameEmitters.Length);
            for (int i = 0; i < flameEmitters.Length; i++)
            {
                availableIndices.Add(i);
            }

            for (int i = 0; i < amountOfFlames; i++)
            {
                int rnd = Random.Range(0, availableIndices.Count);
                emittersToBeActive.Add(availableIndices[rnd]);
                availableIndices.RemoveAt(rnd);
            }
        }

        int idx = 0;
        foreach (ParticleSystem emitter in flameEmitters)
        {
            if (emitter.isStopped && emittersToBeActive.Contains(idx))
            {
                emitter.Play();
            }
            else if (emitter.isPlaying && !emittersToBeActive.Contains(idx))
            {
                emitter.Stop();
            }

            idx++;
        }

        previousEnvironmentalLevel = environmentalLevel;
    }
}
