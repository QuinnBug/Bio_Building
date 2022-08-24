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
    private float currentWaterPercent;

    [Space]
    [Range(0,100)]
    public float[] impactLevels = new float[5];
    private float[] prevImpactLevels = new float[5];
    [Space]
    public float animDuration;
    public float postAnimDuration;
    [Space]
    [SerializeField] private float[] targetValues;
    [SerializeField] internal bool animating;
    [SerializeField] private bool postAnimStarted;
    [SerializeField] private float animTimer;

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

        ResetLevels();
    }

    internal void StartAnimation(float[] newValues)
    {
        animTimer = 0;
        postAnimStarted = false;
        targetValues = newValues;
        animating = true;
    }

    internal void EndAnimation()
    {
        animating = false;
        Debug.Log("End Animation");
    }

    private void Update()
    {
        if (animating)
        {
            Debug.Log("animating");
            animTimer += Time.deltaTime;

            if (!postAnimStarted)
            {
                for (int i = 0; i < impactLevels.Length; i++)
                {
                    impactLevels[i] = Mathf.Lerp(50, targetValues[i], Mathf.Clamp01(animTimer / animDuration));
                }
                UpdateClimateLevel();

                postAnimStarted = animTimer >= animDuration;
            }
            else if(animTimer > animDuration + postAnimDuration)
            {
                EndAnimation();
            }
        }
    }

    private void UpdateClimateLevel() 
    {
        //bool update = false;
        //for (int i = 0; i < 5; i++)
        //{
        //    if (impactLevels[i] == prevImpactLevels[i]) continue;

        //    update = true;

        //    prevImpactLevels[i] = impactLevels[i];
        //}

        //if (!update) return;

        UpdatePercents();

        #region Trees
        float inactiveTreeCount = trees.Length - (trees.Length * currentTreePercent);
        inactiveTreeCount = Mathf.Clamp(inactiveTreeCount, 0, trees.Length);

        List<int> inactiveIndices = new List<int>();
        List<int> activeIndices = new List<int>();
        List<int> indices = new List<int>();

        for (int i = 0; i < trees.Length; i++)
        {
            if (trees[i].enabled) activeIndices.Add(i);
            else inactiveIndices.Add(i);
        }

        //how many trees we need to change to get to the correct value (we abs the value before looping for negitives)
        float inactiveDiff = inactiveTreeCount - inactiveIndices.Count;

        if(Mathf.Abs(inactiveDiff) < 1) 
        {
            //targetState = the state that the selected trees will be set to
            bool targetState = inactiveDiff < 0;
            indices = targetState ? inactiveIndices : activeIndices;

            inactiveDiff = Mathf.Abs(inactiveDiff);
            for (int i = 0; i < inactiveDiff; i++)
            {
                int j = Random.Range(0, indices.Count);
                trees[indices[j]].enabled = targetState;
                indices.RemoveAt(j);
            }
        }

        #endregion

        #region Water
        Vector3 targetPosition = water.position;
        targetPosition.y = Mathf.Lerp(waterLevels.min, waterLevels.max, currentWaterPercent);
        water.position = targetPosition;

        #endregion

        #region Flames
        inactiveIndices = new List<int>();
        activeIndices = new List<int>();
        indices = new List<int>();

        float amountOfFlames = currentFlamePercent * flameEmitters.Length;

        for (int i = 0; i < flameEmitters.Length; i++)
        {
            if(flameEmitters[i].isPlaying)activeIndices.Add(i);
            else inactiveIndices.Add(i);
        }

        //how many flames we need to change to get to the correct value (we abs the value before looping for negitives)
        float activeDiff = amountOfFlames - activeIndices.Count;

        if (Mathf.Abs(activeDiff) > 1)
        {
            //targetState = the state that the selected trees will be set to
            bool targetState = activeDiff > 0;
            indices = targetState ? inactiveIndices : activeIndices;

            activeDiff = Mathf.Abs(activeDiff);
            for (int i = 0; i < activeDiff; i++)
            {
                int j = Random.Range(0, indices.Count);
                if (targetState) flameEmitters[indices[j]].Play();
                else flameEmitters[indices[j]].Stop();
                indices.RemoveAt(j);
            }
        }

        #endregion
    }

    public void UpdatePercents() 
    {
        currentTreePercent =  Mathf.Clamp((treeChangeRange.NormaliseToRange(impactLevels[0])), 0, 1);
        currentWaterPercent = 1 - Mathf.Clamp((waterChangeRange.NormaliseToRange(impactLevels[1])), 0, 1);
        currentFlamePercent = 1 - Mathf.Clamp((flameChangeRange.NormaliseToRange(impactLevels[2])), 0, 1);
    }

    public void ResetLevels() 
    {
        impactLevels = new float[] {50, 50, 50, 50, 50};
        UpdateClimateLevel();
    }
}
