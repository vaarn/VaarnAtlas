using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
    public AudioSource soundeffect;
    public CursorScript cursorscript;
    public GameObject tutorial;

    public void OnMouseDown()
    {
        soundeffect.Play();
        tutorial.SetActive(!tutorial.activeInHierarchy);
        if (tutorial.activeInHierarchy)
        {
            tutorial.GetComponent<TutorialScript>().Activate();
            tutorial.GetComponent<TutorialScript>().enabled = true;
        }
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf Click To Open The Help Overlay.\n\n ~#A9CBDB Vaults of Vaarn ~#3E8AB0 is created by ~#A9CBDB Leo Hunt. VoV ~#3E8AB0 can be bought here: {https://graculusdroog.itch.io/vaults-of-vaarn https://graculusdroog.itch.io/vaults-of-vaarn } .\n ~#A9CBDB Vaults of Vaarn ~#3E8AB0 is a Creative Commons Attribution 4.0 Licensed product. The material published here is presented under a Creative Commons Attribution - Share Alike 4.0 license. The font used is ~#A9CBDB FROGBLOCK ~#3E8AB0 by ~#A9CBDB Polyducks ~#3E8AB0 . Get FROGBLOCK here: {https://polyducks.itch.io/frogblock https://polyducks.itch.io/frogblock } .");
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
