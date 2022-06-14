using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsButton : MonoBehaviour
{
    public GameObject effects;
    public GameObject effects_bw;
    public AudioSource soundeffect;
    public CursorScript cursorscript;
    private bool effects_on;

    public SpriteRenderer checkbox;
    public Sprite checkon;
    public Sprite checkoff;

    // Start is called before the first frame update
    void Start()
    {
        effects_on = true;

        if (PlayerPrefs.HasKey("effects"))
        {
            effects_on = PlayerPrefs.GetInt("effects") == 1;
            checkbox.sprite = effects_on ? checkon : checkoff;
            if (GetComponent<ButtonAnimation>().IsBlackAndWhite())
            {
                effects.SetActive(false);
                effects_bw.SetActive(effects_on);
            }
            else
            {
                effects.SetActive(effects_on);
                effects_bw.SetActive(false);
            }
        }
    }

    public void SwitchBW(bool bw)
    {
        if (bw)
        {
            effects.SetActive(false);
            effects_bw.SetActive(effects_on);
        }
        else
        {
            effects.SetActive(effects_on);
            effects_bw.SetActive(false);
        }
    }

    public void OnMouseDown()
    {
        effects_on = !effects_on;
        checkbox.sprite = effects_on ? checkon : checkoff;
        if (GetComponent<ButtonAnimation>().IsBlackAndWhite())
        {
            effects.SetActive(false);
            effects_bw.SetActive(effects_on);
        }
        else
        {
            effects.SetActive(effects_on);
            effects_bw.SetActive(false);
        }
        soundeffect.Play();

        PlayerPrefs.SetInt("effects", effects_on ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf Click To Turn Post-Processing On And Off.");
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
