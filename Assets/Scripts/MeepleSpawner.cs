using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeepleSpawner : MonoBehaviour
{
    public int meepleCount = 50;
    public GameObject[] meeplePrefabs;
    public Transform holder;
    public CinemachineSmoothPath[] paths;
    public Range speeds;

    Meeple[] meeplePool = new Meeple[100];
    float meeplesActive = 50;

    // Start is called before the first frame update
    void Start()
    {
        meeplePool = new Meeple[meepleCount];
        for (int i = 0; i < meepleCount; i++)
        {
            meeplePool[i] = Instantiate(meeplePrefabs[Random.Range(0, meeplePrefabs.Length)], holder).GetComponent<Meeple>();
            meeplePool[i].pathIndex = i < meeplesActive ? Random.Range(0, paths.Length) : -1;
            meeplePool[i].pos = Random.Range(0.0f, 0.8f);
            meeplePool[i].speed = speeds.RandomVal();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //meeple count can't be changed at run time
        meepleCount = meeplePool.Length;

        meeplesActive = ClimateManager.Instance.currentMeeplePercent * meepleCount;

        bool active = false;
        for (int i = 0; i < meepleCount; i++)
        {
            active = i < meeplesActive;

            meeplePool[i].gameObject.SetActive(active);
            if (!active) { meeplePool[i].pathIndex = -1; continue; }

            //if at the end of the path (or deactivated), teleport to the start of a new random path
            if (meeplePool[i].pathIndex == -1 || meeplePool[i].pos >= 0.99f)
            {
                meeplePool[i].pos = 0;
                meeplePool[i].pathIndex = Random.Range(0, paths.Length);
                meeplePool[i].speed = speeds.RandomVal();
                //Debug.Log("Reached End " + paths[meeplePool[i].pathIndex].PathLength + " " + meeplePool[i].pos);
            }

            meeplePool[i].pos += meeplePool[i].speed * Time.deltaTime;
            meeplePool[i].transform.position = paths[meeplePool[i].pathIndex].EvaluatePositionAtUnit(meeplePool[i].pos, CinemachinePathBase.PositionUnits.Normalized);
        }
    }
}
