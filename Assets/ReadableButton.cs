using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadableButton : MonoBehaviour
{
    public CursorScript cursorscript;
    public AudioSource soundeffect;
    public SpriteRenderer checkbox;
    public Sprite checkon;
    public Sprite checkoff;

    private bool readable;

    private void Start()
    {
        if (PlayerPrefs.HasKey("readable"))
        {
            readable = PlayerPrefs.GetInt("readable") == 1;
            checkbox.sprite = readable ? checkon : checkoff;
            GameObject.FindObjectOfType<SpriteFromText>().SwitchReadability(readable);
        }
    }

    public void OnMouseDown()
    {
        soundeffect.Play();
        readable = !readable;
        checkbox.sprite = readable ? checkon : checkoff;
        GameObject.FindObjectOfType<SpriteFromText>().SwitchReadability(readable);

        PlayerPrefs.SetInt("readable", readable ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf Click To Switch Between A Readable Font, And A Pixel Art Font.");
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
