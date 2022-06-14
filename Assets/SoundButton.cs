using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    public CursorScript cursorscript;
    public GameObject sound;

    public SpriteRenderer checkbox;
    public Sprite checkon;
    public Sprite checkoff;

    private void Start()
    {
        if (PlayerPrefs.HasKey("sound"))
        {
            checkbox.sprite = PlayerPrefs.GetInt("sound") == 1 ? checkon : checkoff;
            sound.SetActive(PlayerPrefs.GetInt("sound") == 1);
        }
    }

    public void OnMouseDown()
    {
        checkbox.sprite = sound.activeInHierarchy ? checkoff : checkon;
        sound.SetActive(!sound.activeInHierarchy);

        PlayerPrefs.SetInt("sound", sound.activeInHierarchy ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf Click To Turn Sound Off And On.");
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
