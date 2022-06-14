using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundTime : MonoBehaviour
{
    public float min_wait;
    public float max_wait;
    public float pitch_offset;

    private float chosen_wait;
    private AudioSource audiosource;

    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        chosen_wait = Random.Range(min_wait, max_wait);
    }

    // Update is called once per frame
    void Update()
    {
        if (chosen_wait <= 0)
        {
            audiosource.pitch = 1 + Random.Range(-pitch_offset, pitch_offset);
            audiosource.Play();

            chosen_wait = Random.Range(min_wait, max_wait);
        }

        chosen_wait -= Time.deltaTime;
    }
}
