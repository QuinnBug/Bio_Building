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

    public float NormaliseToRange(float value)
    {
        return (value - this.min) / (this.max - this.min);
    }
}

public class ClimateManager : Singleton<ClimateManager>
{
    private MeshRenderer[] trees;
    public Range treeChangeRange;
    [SerializeField] private float currentTreePercent;

    [Space]
    private ParticleSystem[] flameEmitters;
    public Range flameChangeRange;
    [SerializeField] private float currentFlamePercent;

    [Space]
    public Transform water;
    public Range waterLevels;
    public Range waterChangeRange;
    [SerializeField] private float currentWaterPercent;

    [Space]
    [Range(0,100)]
    public int environmentalLevel;
    private int previousEnvironmentalLevel;

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
        //singleTreePercent = trees.Length / 100.0f;

        tempArray = GameObject.FindGameObjectsWithTag("Flames");
        List<ParticleSystem> emitters = new List<ParticleSystem>();

        foreach (GameObject item in tempArray)
        {
            ParticleSystem ps = item.GetComponentInChildren<ParticleSystem>();
            emitters.Add(ps);
        }

        flameEmitters = emitters.ToArray();
        //singleFlamePercent = flameEmitters.Length / 100.0f;
    }

    private void Update()
    {
        UpdateClimateLevel();
    }

    private void UpdateClimateLevel() 
    {
        if (environmentalLevel == previousEnvironmentalLevel) return;

        UpdatePercents();

        #region Trees
        float treeCount = trees.Length - (trees.Length * currentTreePercent);
        treeCount = Mathf.Clamp(treeCount, 0, trees.Length);

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

        #endregion

        #region Water
        Vector3 targetPosition = water.position;
        targetPosition.y = Mathf.Lerp(waterLevels.min, waterLevels.max, currentWaterPercent);
        water.position = targetPosition;

        #endregion

        #region Flames
        List<int> emittersToBeActive = new List<int>();
        float amountOfFlames = currentFlamePercent * flameEmitters.Length;
        if(amountOfFlames > 0) 
        {
            Debug.Log("AOF = " + amountOfFlames + " -> total:" + flameEmitters.Length + " -- " + currentFlamePercent);
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

        #endregion

        previousEnvironmentalLevel = environmentalLevel;
    }

    public void UpdatePercents() 
    {
        currentTreePercent =  Mathf.Clamp((treeChangeRange.NormaliseToRange(environmentalLevel)), 0, 1);
        currentWaterPercent = 1 - Mathf.Clamp((waterChangeRange.NormaliseToRange(environmentalLevel)), 0, 1);
        currentFlamePercent = 1 - Mathf.Clamp((flameChangeRange.NormaliseToRange(environmentalLevel)), 0, 1);
    }

    
}
