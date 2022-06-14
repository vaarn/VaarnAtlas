using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateKronos : MonoBehaviour
{
    public SpriteFromText spritefromtext;
    public ColorButton colorbutton;
    public Color backgroundcolor;
    public List<Color> colorwave;
    public float animationspeed;
    public float randomripple;
    public string randomchars;
    public string replacewith;
    public double replacechance;

    public AudioSource othermusic;
    private float othervolume;
    public AudioSource kronosmusic;
    private float kronosvolume;
    public AudioSource textsound;
    public float musicswitchspeed;
    private float musicswitchtime;

    public List<GameObject> disableobjects;
    public List<GameObject> enableobjects;

    private bool isactive;

    [TextArea]
    public string KRONOS;

    [TextArea]
    public string dialogue1;

    [TextArea]
    public string dialogue2;

    public Vector2Int dialogue_offset;
    public float dialogue_speed;

    public GameObject button;

    private float char_time;

    private GenerateMap generatemap;
    private int width, height;
    private SpriteRenderer[,] sprites;
    private char[,] chars;
    private int[,] ints;
    private float[,] ripple;
    private float time;

    private List<char> letters;
    private List<Vector3> letterpositions;
    private List<Color> lettercolors;

    private bool hasstarted = false;

    private int timessummoned;

    // Start is called before the first frame update
    void Start()
    {
        if (!hasstarted)
        {
            timessummoned = 0;
            othervolume = othermusic.volume;
            kronosvolume = kronosmusic.volume;
            kronosmusic.volume = 0;
            generatemap = GetComponent<GenerateMap>();
            hasstarted = true;
            musicswitchtime = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isactive)
        {
            if (sprites.Length > 0)
            {
                for (int x = 0; x <= width; x++)
                {
                    for (int y = 0; y <= height; y++)
                    {
                        float distance = Mathf.Sqrt((((width / 2) - x + 1) * ((width / 2) - x + 1)) + (((height / 2) - y - 3) * ((height / 2) - y - 3)));
                        float sin = Mathf.Sin(distance + time * animationspeed + ripple[x, y]);
                        int index = Mathf.RoundToInt(((sin + 1f) / 2f) * (colorwave.Count - 1));
                        sprites[x, y].color = colorwave[index];
                        colorbutton.UpdateColor(sprites[x, y]);

                        if (ints[x, y] != index)
                        {
                            ints[x, y] = index;
                            if (randomchars.Contains(chars[x, y].ToString()))
                            {
                                if (Random.Range(0f, 1f) <= replacechance && index != colorwave.Count - 1)
                                {
                                    sprites[x, y].sprite = spritefromtext.SpriteFromChar(replacewith[Random.Range(0, replacewith.Length)]);
                                }
                                else
                                    sprites[x, y].sprite = spritefromtext.SpriteFromChar(chars[x, y]);
                            }
                        }
                    }
                }

                time += Time.deltaTime;
            }

            if (musicswitchtime >= 0)
            {
                othermusic.volume = Mathf.Lerp(0, othervolume, musicswitchtime / musicswitchspeed);
                musicswitchtime -= Time.deltaTime;
            }

            if (char_time < 0)
            {
                if (!button.activeInHierarchy)
                {
                    AddChar();
                    char_time = dialogue_speed;
                }
            }
            char_time -= Time.deltaTime;
        }
        else
        {
            if (musicswitchtime >= 0)
            {
                othermusic.volume = Mathf.Lerp(othervolume, 0, musicswitchtime / musicswitchspeed);
                kronosmusic.volume = Mathf.Lerp(0, kronosvolume, musicswitchtime / musicswitchspeed);
                musicswitchtime -= Time.deltaTime;
            }
        }
    }

    public void DestroyKronos()
    {
        isactive = false;
        musicswitchtime = musicswitchspeed;
        button.SetActive(false);

        //Destroy previous children
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        foreach (GameObject obj in disableobjects)
            obj.SetActive(true);

        foreach (GameObject obj in enableobjects)
            obj.SetActive(false);
    }

    public void Generate()
    {
        Start();
        generatemap.SetURL("", "kronos");

        timessummoned++;

        foreach (GameObject obj in disableobjects)
            obj.SetActive(false);

        foreach (GameObject obj in enableobjects)
            obj.SetActive(true);

        isactive = true;
        kronosmusic.volume = kronosvolume;
        kronosmusic.Stop();
        kronosmusic.Play();
        musicswitchtime = musicswitchspeed;

        //Destroy previous children
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        //Get the width and height of the screen in character counts
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(Vector2.one);
        width = Mathf.RoundToInt(edgeVector.x) * 2 - 1;
        height = Mathf.RoundToInt(edgeVector.y) * 2 - 1;
        sprites = new SpriteRenderer[width + 1, height + 1];
        chars = new char[width + 1, height + 1];
        ints = new int[width + 1, height + 1];
        ripple = new float[width + 1, height + 1];

        string[] KRONOS_lines = KRONOS.Split('\n');
        int textwidth = KRONOS_lines[0].Length;
        int textheight = KRONOS_lines.Length;

        Vector2Int offset = new Vector2Int((textwidth - width) / 2, (textheight - height) / 2);

        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                GameObject top = new GameObject();
                GameObject bottom = new GameObject();
                bottom.name = "background";
                bottom.transform.SetParent(top.transform);
                bottom.transform.localPosition = Vector3.zero;
                generatemap.SetSprite(bottom, generatemap.filledsprite, backgroundcolor, 0);

                top.name = "top";
                top.transform.SetParent(transform);
                top.transform.localPosition = new Vector3(x, -y, 0);
                chars[x, y] = KRONOS_lines[y + offset.y + 3][x];
                float distance = Mathf.Sqrt((((width / 2) - x + 1) * ((width / 2) - x + 1)) + (((height / 2) - y - 3) * ((height / 2) - y - 3)));
                float sin = Mathf.Sin(distance + time * animationspeed + ripple[x, y]);
                int index = Mathf.RoundToInt(((sin + 1f) / 2f) * (colorwave.Count - 1));
                generatemap.SetSprite(top, spritefromtext.SpriteFromChar(chars[x, y]), colorwave[index], 1);
                sprites[x, y] = top.GetComponent<SpriteRenderer>();

                generatemap.AddDice(new Vector2Int(x, y), backgroundcolor, colorwave[index]);

                ripple[x, y] = Random.Range(0f, randomripple);
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("textholder"))
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        if (timessummoned == 1)
            SolveDialogue(dialogue1);
        else
            SolveDialogue(dialogue2);

        generatemap.colorbutton.CheckForButtons();
    }

    private void SolveDialogue(string text)
    {
        letters = new List<char>();
        letterpositions = new List<Vector3>();
        lettercolors = new List<Color>();

        int y = 0;
        Color color = colorwave[0];
        foreach (string sentence in text.Split('\n'))
        {
            int x = 0;
            if (sentence.Length > 0)
            {
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
            generatemap.SetSprite(bottom, generatemap.filledsprite, backgroundcolor, 2);

            top.name = "top";
            top.transform.SetParent(transform);
            top.transform.localPosition = letterpositions[0];
            generatemap.SetSprite(top, spritefromtext.SpriteFromChar(letters[0]), lettercolors[0], 3);

            generatemap.AddDice(new Vector2Int(Mathf.RoundToInt(letterpositions[0].x), Mathf.RoundToInt(-letterpositions[0].y)), backgroundcolor, lettercolors[0]);

            letters.RemoveAt(0);
            letterpositions.RemoveAt(0);
            lettercolors.RemoveAt(0);
        }
        else
        {
            button.SetActive(true);
            generatemap.colorbutton.CheckForButtons();
            generatemap.spritefromtext.CheckReadability(button);
        }
    }
}
