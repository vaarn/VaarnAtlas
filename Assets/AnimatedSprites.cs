using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprites : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public double waittime;

    private SpriteRenderer spriterenderer;
    private int index;
    private double timer;

    // Start is called before the first frame update
    void Start()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        timer = 0;
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (sprites.Count > 0)
        {
            if (timer <= 0)
            {
                index++;
                if (index >= sprites.Count)
                    index = 0;
                spriterenderer.sprite = sprites[index];
                timer = waittime;
            }
            timer -= Time.deltaTime;
        }
    }
}
