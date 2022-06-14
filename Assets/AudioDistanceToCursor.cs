using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDistanceToCursor : MonoBehaviour
{
    private AudioSource audiosource;
    private float maxvolume;

    public GameObject cursor;
    public float min_distance;
    public float max_distance;

    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        maxvolume = audiosource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, cursor.transform.position);
        audiosource.volume = (Mathf.Max(max_distance - Mathf.Max(distance - min_distance, 0), 0) / max_distance) * maxvolume;
    }
}
