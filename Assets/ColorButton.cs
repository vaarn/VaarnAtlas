using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorButton : MonoBehaviour
{
    public GameObject generate;
    public AudioSource soundeffect;
    public EffectsButton effects;
    public CursorScript cursorscript;

    public List<SpriteRenderer> othersprites;
    public List<SpriteRenderer> whitesprites;

    public List<SpriteRenderer> spriteswitch;
    private List<Sprite> originalsprites;
    public List<Sprite> bwsprites;

    private List<SpriteRenderer> toreset;
    private List<Color> oldcolor;
    private Color cameracol;
    //private bool incolor;
    private int incolor; //0 = color, 1 = b&w, 2 = grayscale

    public SpriteRenderer checkbox;
    public Sprite checkon;
    public Sprite checkoff;

    // Start is called before the first frame update
    void Start()
    {
        //incolor = true;
        incolor = 0;
        toreset = new List<SpriteRenderer>();
        oldcolor = new List<Color>();

        originalsprites = new List<Sprite>();
        foreach (SpriteRenderer spriterenderer in spriteswitch)
            originalsprites.Add(spriterenderer.sprite);

        if (PlayerPrefs.HasKey("color"))
        {
            if (PlayerPrefs.GetInt("color") == 1)
            {
                incolor = 1;
                UpdateStuff();
            }
            if (PlayerPrefs.GetInt("color") == 2)
            {
                incolor = 1;
                UpdateStuff();
                incolor = 2;
                UpdateStuff();
            }
        }
    }

    public void CheckForButtons()
    {
        GetComponent<ButtonAnimation>().BlackAndWhite(incolor != 0);
        foreach (ButtonAnimation btn in GameObject.FindObjectsOfType<ButtonAnimation>())
        {
            btn.BlackAndWhite(incolor != 0);
        }
    }

    public void AddedObject(SpriteRenderer spriterenderer)
    {
        if (incolor == 1)
        {
            toreset.Add(spriterenderer);
            oldcolor.Add(spriterenderer.color);

            if (spriterenderer.name.Contains("background") || spriterenderer.name.Contains("light"))
                spriterenderer.color = Color.white;
            else
                spriterenderer.color = Color.black;
        }
        else if (incolor == 2)
        {
            toreset.Add(spriterenderer);
            oldcolor.Add(spriterenderer.color);

            if (spriterenderer.name.Contains("background") || spriterenderer.name.Contains("light"))
                spriterenderer.color = Color.white;
            else
                spriterenderer.color = ToGray(spriterenderer.color);
        }
    }

    public void UpdateColor(SpriteRenderer spriterenderer, Color originalcol)
    {
        if (incolor == 0)
            spriterenderer.color = originalcol;
        else
            UpdateColor(spriterenderer);
    }

    public void UpdateColor(SpriteRenderer spriterenderer)
    {
        if (incolor == 1)
        {
            if (spriterenderer.name.Contains("background") || spriterenderer.name.Contains("light"))
                spriterenderer.color = Color.white;
            else
                spriterenderer.color = Color.black;
        }
        else if (incolor == 2)
        {
            if (spriterenderer.name.Contains("background") || spriterenderer.name.Contains("light"))
                spriterenderer.color = Color.white;
            else
                spriterenderer.color = ToGray(spriterenderer.color);
        }
    }

    public void OnMouseDown()
    {
        soundeffect.Play();
        //incolor = !incolor;
        incolor++;
        if (incolor > 2)
            incolor = 0;
        PlayerPrefs.SetInt("color", incolor);
        PlayerPrefs.Save();
        UpdateStuff();
    }

    private void UpdateStuff()
    {
        checkbox.sprite = incolor == 0 ? checkon : checkoff;
        effects.SwitchBW(incolor != 0);
        GetComponent<ButtonAnimation>().BlackAndWhite(incolor != 0);
        foreach (ButtonAnimation btn in GameObject.FindObjectsOfType<ButtonAnimation>())
        {
            btn.BlackAndWhite(incolor != 0);
        }

        if (incolor == 1)
        {
            toreset = new List<SpriteRenderer>();
            oldcolor = new List<Color>();
            cameracol = Camera.main.backgroundColor;
            Camera.main.backgroundColor = Color.white;
            foreach (SpriteRenderer spriterenderer in generate.GetComponentsInChildren<SpriteRenderer>())
            {
                toreset.Add(spriterenderer);
                oldcolor.Add(spriterenderer.color);

                if (spriterenderer.name.Contains("background") || spriterenderer.name.Contains("light"))
                    spriterenderer.color = Color.white;
                else
                    spriterenderer.color = Color.black;
            }

            foreach (SpriteRenderer spriterenderer in othersprites)
            {
                toreset.Add(spriterenderer);
                oldcolor.Add(spriterenderer.color);
                spriterenderer.color = Color.black;
            }

            foreach (SpriteRenderer spriterenderer in whitesprites)
            {
                toreset.Add(spriterenderer);
                oldcolor.Add(spriterenderer.color);
                spriterenderer.color = new Color(1, 1, 1, spriterenderer.color.a);
            }

            int index = 0;
            foreach (SpriteRenderer spriterenderer in spriteswitch)
            {
                spriterenderer.sprite = bwsprites[index];
                index++;
            }
        }
        else if (incolor == 2)
        {
            for (int i = 0; i < toreset.Count; i++)
            {
                if (toreset[i])
                {
                    if (toreset[i].name.Contains("background") || toreset[i].name.Contains("light"))
                        toreset[i].color = Color.white;
                    else
                    {
                        toreset[i].color = ToGray(oldcolor[i]);
                    }
                }
            }

            int index = 0;
            foreach (SpriteRenderer spriterenderer in spriteswitch)
            {
                spriterenderer.sprite = bwsprites[index];
                index++;
            }
        }
        else
        {
            Camera.main.backgroundColor = cameracol;
            for (int i = 0; i < toreset.Count; i++)
            {
                if (toreset[i])
                    toreset[i].color = oldcolor[i];
            }

            int index = 0;
            foreach (SpriteRenderer spriterenderer in spriteswitch)
            {
                spriterenderer.sprite = originalsprites[index];
                index++;
            }
        }
    }

    private Color ToGray(Color col)
    {
        float h, s, v;
        Color.RGBToHSV(col, out h, out s, out v);
        v -= 0.3f;
        if (v < 0)
            v = 0;
        return new Color(v, v, v, col.a);
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf Click To Switch Between Full Color, Black & White, and Grayscale.");
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
