using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSummoning : MonoBehaviour
{
    public Transform environment;
    public ParticleSystem zoomEffect;

    public bool active = false;
    private float speed = 1000;

    public Vector3 startPos;
    public Vector3 endPos;

    private void Start()
    {
        environment.position = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(environment.position, active ? endPos : startPos) > 0.01f)
        {
            environment.position = Vector3.MoveTowards(environment.position, active ? endPos : startPos, speed * Time.deltaTime);
            if (zoomEffect.isStopped & active) zoomEffect.Play();
        }
        else if (zoomEffect.isPlaying) zoomEffect.Stop();

        environment.gameObject.SetActive(Vector3.Distance(environment.position, startPos) > 0.01f && active);
    }

    public void UpdateSpeed(float duration) 
    {
        speed = Vector3.Distance(endPos, startPos) / duration;
    }
}
