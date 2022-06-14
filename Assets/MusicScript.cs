using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public List<AudioClip> music;

    private int lastsong;
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        Restart();
    }

    public void Restart()
    {
        lastsong = -1;
    }

    public void PlaySong(int song)
    {
        source.clip = music[song];
        source.Play();
        lastsong = song;
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying)
        {
            if (music.Count > 0)
            {
                int chosen = lastsong;
                while (chosen == lastsong)
                    chosen = Random.Range(0, music.Count);
                PlaySong(chosen);
            }
        }
    }
}
