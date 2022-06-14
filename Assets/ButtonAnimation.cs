using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    public Color bg_col;
    public Color fg_col;
    public Color bg_col_highlight;
    public Color fg_col_highlight;
    public Color bg_col_click;
    public Color fg_col_click;
    public double color_speed;

    private double switchtime;
    private Color bg_target;
    private Color fg_target;

    public List<SpriteRenderer> backgrounds;
    public List<SpriteRenderer> foregrounds;

    private BoxCollider2D boxcollider;

    private bool blackandwhite;

    // Start is called before the first frame update
    void Start()
    {
        bg_target = bg_col;
        fg_target = fg_col;
        if (blackandwhite)
        {
            fg_target = Color.black;
            bg_target = Color.white;
        }
        switchtime = color_speed;

        if (backgrounds.Count == 0)
            backgrounds = new List<SpriteRenderer>();
        if (foregrounds.Count == 0)
            foregrounds = new List<SpriteRenderer>();

        boxcollider = GetComponent<BoxCollider2D>();
    }

    public void BlackAndWhite(bool isit)
    {
        switchtime = color_speed;
        blackandwhite = isit;
        if (blackandwhite)
        {
            fg_target = Color.black;
            bg_target = Color.white;
        }
        else
        {
            bg_target = bg_col;
            fg_target = fg_col;
        }
    }

    public bool IsBlackAndWhite()
    {
        return blackandwhite;
    }

    // Update is called once per frame
    void Update()
    {
        if (switchtime > 0)
        {
            switchtime -= Time.deltaTime;
            if (switchtime < 0)
                switchtime = 0;

            foreach (SpriteRenderer bg in backgrounds)
            {
                bg.color = Color.Lerp(bg_target, bg.color, (float)switchtime / (float)color_speed);
            }

            foreach (SpriteRenderer fg in foregrounds)
            {
                fg.color = Color.Lerp(fg_target, fg.color, (float)switchtime / (float)color_speed);
            }
        }
        else
        {
            if (fg_target == fg_col_click)
            {
                switchtime = color_speed;
                bg_target = bg_col;
                fg_target = fg_col;
            }
            if (blackandwhite && fg_target == Color.gray)
            {
                switchtime = color_speed;
                fg_target = Color.black;
                bg_target = Color.white;
            }
        }
    }

    public void OnMouseDown()
    {
        switchtime = color_speed;
        bg_target = bg_col_click;
        fg_target = fg_col_click;

        if (blackandwhite)
        {
            fg_target = Color.gray;
            bg_target = Color.black;
        }
    }

    public void OnMouseEnter()
    {
        switchtime = color_speed;
        bg_target = bg_col_highlight;
        fg_target = fg_col_highlight;

        if (blackandwhite)
        {
            fg_target = Color.white;
            bg_target = Color.black;
        }
    }

    public void OnMouseExit()
    {
        switchtime = color_speed;
        bg_target = bg_col;
        fg_target = fg_col;

        if (blackandwhite)
        {
            fg_target = Color.black;
            bg_target = Color.white;
        }
    }
}
