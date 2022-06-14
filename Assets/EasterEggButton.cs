using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEggButton : MonoBehaviour
{
    public CursorScript cursorscript;
    public string eastertext;
    public AudioSource soundeffect;
    public SpriteRenderer spriterenderer;
    public SpriteFromText spritefromtext;

    private bool found;

    private void Start()
    {
        found = false;
    }

    public void Found()
    {
        if (!found)
        {
            soundeffect.Play();
            spriterenderer.sprite = spritefromtext.SpriteFromChar('♂');
            found = true;
        }
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf " + eastertext);
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
