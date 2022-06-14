using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrthMap : MonoBehaviour
{
    public GenerateMap generate;
    public CursorScript cursorscript;
    public GameObject urth;
    public GameObject previous;
    private bool urthbutton;

    private void Start()
    {
        urthbutton = true;
    }

    public void Flip()
    {
        if(urthbutton)
        {
            urth.SetActive(false);
            previous.SetActive(true);
            urthbutton = false;
        }
        else
        {
            urth.SetActive(true);
            previous.SetActive(false);
            urthbutton = true;
        }
    }

    public void OnMouseDown()
    {
        if (urthbutton)
        {
            string pretty_region = "";
            if (generate.PreviousRegion() == "int")
                pretty_region = "The Interior";
            if (generate.PreviousRegion() == "gno")
                pretty_region = "Gnomon";
            cursorscript.SetButtonText("~#f5bccf Click To Open The Previous Region:\n\n" + pretty_region);
            generate.GetComponent<GenerateGnomon>().Destroy();
            generate.Generate("urt");
            urth.SetActive(false);
            previous.SetActive(true);
            urthbutton = false;
        }
        else
        {
            cursorscript.SetButtonText("~#f5bccf Click To Open The Map Of Urth.\n\nThis World Map Will Let You Access Other Region Generators.");
            generate.GetComponent<GenerateWorld>().Destroy();
            if (generate.PreviousRegion() == "urt")
                generate.Generate("int");
            else
                generate.Generate(generate.PreviousRegion());
            urth.SetActive(true);
            previous.SetActive(false);
            urthbutton = true;
        }
    }

    public void OnMouseEnter()
    {
        if (urthbutton)
            cursorscript.SetButtonText("~#f5bccf Click To Open The Map Of Urth.\n\nThis World Map Will Let You Access Other Region Generators.");
        else
        {
            string pretty_region = "";
            if (generate.PreviousRegion() == "int")
                pretty_region = "The Interior";
            if (generate.PreviousRegion() == "gno")
                pretty_region = "Gnomon";
            cursorscript.SetButtonText("~#f5bccf Click To Open The Previous Region.\n\n" + pretty_region);
        }
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
