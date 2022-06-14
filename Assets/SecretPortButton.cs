using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SecretPortButton : MonoBehaviour
{
    public CursorScript cursorscript;
    public AudioSource soundeffect;
    public List<Sprite> secretsprite;
    private Sprite defaultsprite;
    public float chance;
    public float time;
    public SpriteRenderer spriterenderer;

    private float timepassed;


    public TextAsset viruschars;

    public GenerateMap generatemap;
    public SpriteFromText spritefromtext;
    public ColorButton colorbutton;
    public Color backgroundcolor;
    public Color foregroundcolor;
    public float animationspeed;
    public float randomripple;
    public string randomchars;
    public string pupilchars;
    public double replacechance;

    [TextArea]
    public string dialogue1;

    [TextArea]
    public string dialogue2;

    public Vector2Int dialogue_offset;
    public float dialogue_speed;

    private float animatetime;
    private bool isactive;
    private float char_time;
    private List<SpriteRenderer> sprites;
    private List<SpriteRenderer> pupils;
    private List<float> ripple;

    public GameObject button;
    public GameObject button2;

    public List<GameObject> disableobjects;
    public List<GameObject> enableobjects;
    private List<bool> objectsactive;

    private List<char> letters;
    private List<Vector3> letterpositions;
    private List<Color> lettercolors;

    public AudioSource textsound;
    public GameObject laugh;

    public float randompitch;

    public List<PostProcessVolume> postproc;
    public float lens_offset;

    private List<GameObject> allnewobjects;

    public float max_death_time;
    public AudioSource dyingsound;
    public AudioSource faildismisssound;

    public EasterEggButton virusegg;

    private void Start()
    {
        defaultsprite = spriterenderer.sprite;
    }

    private void Update()
    {
        if (timepassed <= 0)
        {
            timepassed = time;
            if (Random.Range(0f, 1f) < chance)
                spriterenderer.sprite = secretsprite[Random.Range(0, secretsprite.Count)];
            else
                spriterenderer.sprite = defaultsprite;
        }
        timepassed -= Time.deltaTime;


        if (isactive)
        {
            int i = 0;
            foreach (SpriteRenderer sprite in sprites)
            {
                float distance = sprite.transform.position.magnitude;
                float sin = Mathf.Sin(distance + animatetime * animationspeed + ripple[i]);

                if (Random.Range(0f, 1f) <= replacechance && sin > 0.5f)
                {
                    char chosenchar = randomchars[Random.Range(0, randomchars.Length)];
                    if (chosenchar == '~')
                        sprite.gameObject.SetActive(false);
                    else
                    {
                        sprite.gameObject.SetActive(true);
                        sprite.sprite = spritefromtext.SpriteFromChar(chosenchar);
                    }
                }

                colorbutton.UpdateColor(sprite, foregroundcolor);
                colorbutton.UpdateColor(sprite.transform.GetChild(0).GetComponent< SpriteRenderer>(), backgroundcolor);

                i++;
            }
            foreach (SpriteRenderer sprite in pupils)
            {
                float distance = sprite.transform.position.magnitude;
                float sin = Mathf.Sin(distance + animatetime * animationspeed);

                if (Random.Range(0f, 1f) <= replacechance && sin > 0.5f)
                {
                    sprite.sprite = spritefromtext.SpriteFromChar(pupilchars[Random.Range(0, pupilchars.Length)]);
                }
            }

            animatetime += Time.deltaTime;

            if (char_time < 0)
            {
                if ((!button.activeInHierarchy && !laugh.activeInHierarchy) || (!button2.activeInHierarchy && laugh.activeInHierarchy))
                {
                    AddChar();
                    char_time = dialogue_speed;
                }
            }
            char_time -= Time.deltaTime;

            foreach (GameObject obj in disableobjects)
            {
                obj.SetActive(false);
            }

            if (laugh.activeInHierarchy)
            {
                enableobjects[0].GetComponent<AudioSource>().pitch = 1f + Random.Range(-randompitch, randompitch);
                float sin = Mathf.Sin(animatetime * animationspeed);
                float sin2 = Mathf.Sin(animatetime * (animationspeed/2));
                foreach (PostProcessVolume post in postproc)
                {
                    LensDistortion lens;
                    if (post.profile.TryGetSettings<LensDistortion>(out lens))
                    {
                        lens.centerX.value = lens_offset * sin;
                        lens.centerY.value = lens_offset * sin2;
                    }
                }
            }
        }
    }

    public void OnMouseDown()
    {
        if (!isactive)
        {
            soundeffect.Play();
            GenerateVirus();
        }
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf Vaarn Atlas\n\nYou can see a hidden panel in the plasteel shell of this mapping tablet. Inside you find some kind of ancient hidden port.\n\n~#F8961D This Hegemony technology may still hide some secrets.");
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }

    private void GenerateVirus()
    {
        objectsactive = new List<bool>();
        foreach (GameObject obj in disableobjects)
        {
            objectsactive.Add(obj.activeInHierarchy);
            obj.SetActive(false);
        }
        foreach (GameObject obj in enableobjects)
        {
            obj.SetActive(true);
        }

        allnewobjects = new List<GameObject>();
        sprites = new List<SpriteRenderer>();
        pupils = new List<SpriteRenderer>();
        ripple = new List<float>();

        enableobjects[0].GetComponent<AudioSource>().pitch = 1;

        string[] virus_lines = viruschars.text.Split('\n');
        int width = virus_lines[0].Length - 1;
        int height = virus_lines.Length;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (virus_lines[y].Length > 0)
                {
                    if (virus_lines[y][x] != ' ')
                    {
                        GameObject top = new GameObject();
                        GameObject bottom = new GameObject();
                        bottom.name = "background";
                        bottom.transform.SetParent(top.transform);
                        bottom.transform.localPosition = Vector3.zero;
                        generatemap.SetSprite(bottom, generatemap.filledsprite, backgroundcolor, 5);

                        top.name = "top";
                        top.transform.SetParent(generatemap.transform);
                        allnewobjects.Add(top);
                        top.transform.localPosition = new Vector3(x, -y, 0);
                        generatemap.SetSprite(top, spritefromtext.SpriteFromChar(virus_lines[y][x]), foregroundcolor, 6);

                        generatemap.AddDice(new Vector2Int(x, y), backgroundcolor, foregroundcolor);

                        if (virus_lines[y][x] == '#')
                        {
                            sprites.Add(top.GetComponent<SpriteRenderer>());
                            ripple.Add(Random.Range(0f, randomripple));
                        }

                        if (virus_lines[y][x] == 'ú')
                        {
                            pupils.Add(top.GetComponent<SpriteRenderer>());
                        }
                    }
                }
            }
        }

        SolveDialogue(dialogue1);

        isactive = true;
    }

    private void SolveDialogue(string text)
    {
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(Vector2.one);
        int width = Mathf.RoundToInt(edgeVector.x) * 2 - 1;

        letters = new List<char>();
        letterpositions = new List<Vector3>();
        lettercolors = new List<Color>();

        int y = 0;
        foreach (string sentence in text.Split('\n'))
        {
            int x = 0;
            if (sentence.Length > 0)
            {
                Color color = foregroundcolor;
                foreach (string word in sentence.Split(' '))
                {
                    if (x + word.Length > width - (dialogue_offset.x * 2))
                    {
                        y++;
                        x = 0;
                    }

                    if (word.Length > 0)
                    {
                        if (word[0] == '~')
                            ColorUtility.TryParseHtmlString(word.Trim('~'), out color);
                        else
                        {
                            foreach (char c in word + " ")
                            {
                                letters.Add(c);
                                letterpositions.Add(new Vector3(x + dialogue_offset.x, -y - dialogue_offset.y, 0));
                                lettercolors.Add(color);
                                x++;
                            }
                        }
                    }
                }
            }
            y++;
        }
    }

    private void AddChar()
    {
        if (letters.Count > 0)
        {
            textsound.Play();
            GameObject top = new GameObject();
            GameObject bottom = new GameObject();
            bottom.name = "background";
            bottom.transform.SetParent(top.transform);
            bottom.transform.localPosition = Vector3.zero;
            generatemap.SetSprite(bottom, generatemap.filledsprite, backgroundcolor, 7);

            top.name = "virus_letter_top";
            top.transform.SetParent(generatemap.transform);
            allnewobjects.Add(top);
            top.transform.localPosition = letterpositions[0];
            generatemap.SetSprite(top, spritefromtext.SpriteFromChar(letters[0]), lettercolors[0], 8);

            generatemap.AddDice(new Vector2Int(Mathf.RoundToInt(letterpositions[0].x), Mathf.RoundToInt(-letterpositions[0].y)), backgroundcolor, lettercolors[0]);

            letters.RemoveAt(0);
            letterpositions.RemoveAt(0);
            lettercolors.RemoveAt(0);
        }
        else
        {
            if (laugh.activeInHierarchy)
                button2.SetActive(true);
            else
                button.SetActive(true);
            generatemap.colorbutton.CheckForButtons();
            generatemap.spritefromtext.CheckReadability(button);
            generatemap.spritefromtext.CheckReadability(button2);
        }
    }

    public void ClickedDismiss()
    {
        foreach(SpriteRenderer sp in GameObject.FindObjectsOfType<SpriteRenderer>())
        {
            if (sp.name.Contains("virus_letter"))
                Destroy(sp.gameObject);
        }

        SolveDialogue(dialogue2);
        button.SetActive(false);
        laugh.SetActive(true);
        faildismisssound.Play();
    }

    public void DestroyVirus()
    {
        int i = 0;
        foreach (GameObject obj in disableobjects)
        {
            obj.SetActive(objectsactive[i]);
            i++;
        }
        foreach (GameObject obj in enableobjects)
        {
            obj.SetActive(false);
        }
        laugh.SetActive(false);

        isactive = false;
        button2.SetActive(false);
        soundeffect.Play();
        dyingsound.Play();

        foreach(GameObject obj in allnewobjects)
        {
            Destroy(obj, Random.Range(0f, max_death_time));
        }

        StartCoroutine(GetEasterEgg());
    }

    public IEnumerator GetEasterEgg()
    {
        yield return new WaitForSeconds(max_death_time);
        virusegg.Found();
    }
}
