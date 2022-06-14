using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFromText : MonoBehaviour
{
    private bool readable;
    public string text;
    public List<Sprite> sprites;
    public List<Sprite> spritesreadable;

    public void CheckUpdate()
    {
        SwitchReadability(readable);
    }

    public void SwitchReadability(bool readable)
    {
        this.readable = readable;

        foreach (SpriteRenderer sp in GameObject.FindObjectsOfType<SpriteRenderer>())
        {
            if (readable)
            {
                int i = 0;
                foreach (Sprite s in sprites)
                {
                    if (sp.sprite == s)
                        sp.sprite = spritesreadable[i];
                    i++;
                }
            }
            else
            {
                int i = 0;
                foreach (Sprite s in spritesreadable)
                {
                    if (sp.sprite == s)
                        sp.sprite = sprites[i];
                    i++;
                }
            }
        }
    }

    public void CheckReadability(GameObject obj)
    {
        foreach (SpriteRenderer sp in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            if (readable)
            {
                int i = 0;
                foreach (Sprite s in sprites)
                {
                    if (sp.sprite == s)
                        sp.sprite = spritesreadable[i];
                    i++;
                }
            }
            else
            {
                int i = 0;
                foreach (Sprite s in spritesreadable)
                {
                    if (sp.sprite == s)
                        sp.sprite = sprites[i];
                    i++;
                }
            }
        }
    }

    public Sprite SpriteFromChar(char c)
    {
        if (!readable)
        {
            int i = text.IndexOf(c);
            if (i < 0 || i >= sprites.Count)
                return null;
            return sprites[i];
        }
        else
        {
            int i = text.IndexOf(c);
            if (i < 0 || i >= spritesreadable.Count)
                return null;
            return spritesreadable[i];
        }
    }

    public char CharFromSprite(Sprite s)
    {
        if (!readable)
        {
            int index = 0;
            foreach (Sprite s_check in sprites)
            {
                if (s_check == s)
                    return text[index];
                index++;
            }
            return ' ';
        }
        else
        {
            int index = 0;
            foreach (Sprite s_check in spritesreadable)
            {
                if (s_check == s)
                    return text[index];
                index++;
            }
            return ' ';
        }
    }
}
