using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionLinkButton : MonoBehaviour
{
    public GenerateMap generate;
    public AudioSource soundeffect;
    public CursorScript cursorscript;
    public string hovertext;
    public string regionlink;

    public void OnMouseDown()
    {
        if (regionlink != "")
        {
            soundeffect.Play();
            generate.ResetMap();
            generate.GetComponent<GenerateWorld>().Destroy();
            generate.GetComponent<UIRegions>().ResetRegions();
            generate.Generate(regionlink);
            GameObject.FindObjectOfType<UrthMap>().Flip();
            cursorscript.UnPlace();
        }
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf " + hovertext);
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
