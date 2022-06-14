using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AreaInfo
{
    public List<Color> colors;
    public Color backgroundcolor;
    public string name;
    [TextArea]
    public string info;
    public string generator_link;
    public Color textcolor;
    public int order;
}

public class GenerateWorld : MonoBehaviour
{
    public CursorScript cursorscript;

    [TextArea]
    public string worldmap;

    [TextArea]
    public string worldareas;

    public List<AreaInfo> areainfo;

    public string cloudchars;

    public double randomanimate_chance;

    public Color ui_back_color;
    public Color ui_front_color;
    public Color generator_color;

    public List<GameObject> enable_objects;
    public List<GameObject> disable_objects;

    private SpriteFromText spritefromtext;
    private GenerateMap generate;

    private int width, height;

    private List<SpriteRenderer> animate;

    private List<Vector2Int> generator_pos;
    private List<string> generator_link;

    private int textwidth, textheight;

    public AudioSource regionsound;

    private bool started = false;
    private int textbox_wider_default;

    public AudioSource rerollsound;

    // Start is called before the first frame update
    void Start()
    {
        if (!started)
        {
            generate = GetComponent<GenerateMap>();
            spritefromtext = generate.spritefromtext;
            animate = new List<SpriteRenderer>();
            generator_link = new List<string>();
            generator_pos = new List<Vector2Int>();
            started = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animate.Count > 0)
        {
            foreach (SpriteRenderer s in animate)
            {
                if (s)
                {
                    if (UnityEngine.Random.Range(0f, 1f) <= randomanimate_chance)
                    {
                        s.sprite = spritefromtext.SpriteFromChar(cloudchars[UnityEngine.Random.Range(0, cloudchars.Length)]);
                    }
                }
            }
        }
    }

    public void Destroy()
    {
        animate = new List<SpriteRenderer>();
        generator_link = new List<string>();
        generator_pos = new List<Vector2Int>();

        //Destroy previous children
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        foreach (GameObject obj in enable_objects)
            obj.SetActive(false);
        foreach (GameObject obj in disable_objects)
            obj.SetActive(true);

        generate.textbox_wider_default = textbox_wider_default;
        GetComponent<UIRegions>().ResetRegions();
    }

    public void Generate()
    {
        Start();

        rerollsound.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        rerollsound.Play();

        generate = GetComponent<GenerateMap>();
        generate.SetURL("urt", "");
        spritefromtext = generate.spritefromtext;
        GetComponent<UIRegions>().ResetRegions();

        textbox_wider_default = generate.textbox_wider_default;
        generate.textbox_wider_default = 0;

        //Destroy previous children
        for (int i = 0; i < transform.childCount; i++)
        {
                Destroy(transform.GetChild(i).gameObject);
        }

        foreach (GameObject obj in enable_objects)
            obj.SetActive(true);
        foreach (GameObject obj in disable_objects)
            obj.SetActive(false);

        //Get the width and height of the screen in character counts
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(Vector2.one);
        width = Mathf.RoundToInt(edgeVector.x) * 2 - 1;
        height = Mathf.RoundToInt(edgeVector.y) * 2 - 1;
        generator_link = new List<string>();
        generator_pos = new List<Vector2Int>();

        string[] map_lines = worldmap.Split('\n');
        string[] area_lines = worldareas.Split('\n');
        textwidth = map_lines[0].Length;
        textheight = map_lines.Length;

        generate.SetUpDetails(0, width, height-(areainfo.Count-1), textwidth);

        animate = new List<SpriteRenderer>();
        int[] tops = new int[12];
        int[] lefts = new int[12];
        int[] rights = new int[12];
        int[] bottoms = new int[12];
        for (int j = 0; j < 12; j++)
        {
            tops[j] = 1000;
            lefts[j] = 1000;
            rights[j] = -1000;
            bottoms[j] = -1000;
        }
        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                if (x == 0 || x == width || y == 0 || y == height)
                {
                    GameObject background = new GameObject();
                    GameObject background_top = new GameObject();
                    background_top.name = "edge_top";
                    background_top.transform.SetParent(background.transform);
                    generate.SetSprite(background_top, generate.edgesprite, ui_front_color, 1);

                    background.name = "ui_background";
                    background.transform.SetParent(transform);
                    background.transform.localPosition = new Vector3(x, -y, 0);
                    generate.SetSprite(background, generate.filledsprite, ui_back_color, 0);
                }
                else if (x-1 < textwidth-1 && y < textheight)
                {
                    int index = "0123456789A ".IndexOf(area_lines[y][x - 1]);
                    if (index != -1)
                    {
                        if (y < tops[index])
                            tops[index] = y;
                        if (y > bottoms[index])
                            bottoms[index] = y;
                        if (x > rights[index])
                            rights[index] = x;
                        if (x < lefts[index])
                            lefts[index] = x;

                        AreaInfo area = areainfo[index];
                        GameObject top = new GameObject();
                        GameObject bottom = new GameObject();
                        bottom.name = "background";
                        bottom.transform.SetParent(top.transform);
                        bottom.transform.localPosition = Vector3.zero;
                        generate.SetSprite(bottom, generate.filledsprite, area.backgroundcolor, 0);

                        top.name = "top";
                        top.transform.SetParent(transform);
                        top.transform.localPosition = new Vector3(x, -y, 0);
                        int chosencol = UnityEngine.Random.Range(0, area.colors.Count);
                        generate.SetSprite(top, spritefromtext.SpriteFromChar(map_lines[y][x-1]), area.colors[chosencol], 1);

                        string detail = "~#" + ColorUtility.ToHtmlStringRGB(area.textcolor) + " " + area.info;
                        if (area.generator_link != "")
                        {
                            generator_pos.Add(new Vector2Int(x, y));
                            generator_link.Add(area.generator_link);
                            detail += "\n\n~#" + ColorUtility.ToHtmlStringRGB(generator_color) + " >Double Click To Go To This Generator<";
                        }
                        generate.AddLocation(area.name, new Vector2Int(x, y), detail);

                        if ("0123456789A ".IndexOf(area_lines[y][x-1]) == 11)
                            animate.Add(top.GetComponent<SpriteRenderer>());

                        generate.AddDice(new Vector2Int(x, y), area.backgroundcolor, area.colors[chosencol]);

                        GetComponent<UIRegions>().AddArea(new Vector2Int(x, y),
                            new Vector2Int(textwidth + 1, -((height - areainfo.Count + 1) + area.order)),
                            new Vector2Int(textwidth + area.name.Length + 1, -((height - areainfo.Count + 1) + area.order)));
                    }
                }
                else
                {
                    GameObject bottom = new GameObject();
                    bottom.name = "background";
                    bottom.transform.SetParent(transform);
                    bottom.transform.localPosition = new Vector3(x, -y, 0);
                    generate.SetSprite(bottom, generate.filledsprite, ui_back_color, 0);
                }
            }
        }

        for (int y = height - areainfo.Count + 1; y < height; y++)
        {
            GameObject background = new GameObject();
            GameObject background_top = new GameObject();
            background_top.name = "edge_top";
            background_top.transform.SetParent(background.transform);
            generate.SetSprite(background_top, generate.edgesprite, ui_front_color, 1);

            background.name = "ui_background";
            background.transform.SetParent(transform);
            background.transform.localPosition = new Vector3(textwidth, -y, 0);
            generate.SetSprite(background, generate.filledsprite, ui_back_color, 0);
        }

        /*for (int x = textwidth; x < width; x++)
        {
            GameObject background = new GameObject();
            GameObject background_top = new GameObject();
            background_top.name = "edge_top";
            background_top.transform.SetParent(background.transform);
            generate.SetSprite(background_top, generate.edgesprite, ui_front_color, 1);

            background.name = "ui_background";
            background.transform.SetParent(transform);
            background.transform.localPosition = new Vector3(x, -(height - areainfo.Count + 1), 0);
            generate.SetSprite(background, generate.filledsprite, ui_back_color, 0);
        }*/

        int k = 0;
        foreach (AreaInfo info in areainfo)
        {
            if (info.order != -1)
            {
                AddRegionButton(info.order, info, regionsound);
                for (int x = textwidth + 1; x < textwidth + info.name.Length + 2; x++)
                {
                    GetComponent<UIRegions>().AddArea(new Vector2Int(x, (height - areainfo.Count + 1) + info.order),
                        new Vector2Int(lefts[k], -tops[k]),
                        new Vector2Int(rights[k], -bottoms[k]));
                }
            }
            k++;
        }

        generate.colorbutton.CheckForButtons();
        //generate.spritefromtext.CheckUpdate();
    }

    public string GetGeneratorLinkAt(Vector2Int pos)
    {
        int index = 0;
        foreach (Vector2Int p in generator_pos)
        {
            if (p.x == pos.x && p.y == pos.y)
            {
                return generator_link[index];
            }
            index++;
        }
        return "";
    }

    public void AddRegionButton(int y, AreaInfo info, AudioSource soundeffect)
    {
        string button_text = (info.generator_link == "" ? " " : "→") + info.name;
        while (button_text.Length < (width - (textwidth + 1)))
            button_text += " ";
        GameObject button = AddButton(button_text, new Vector2Int(textwidth + 1, (height - areainfo.Count + 1) + y), info.backgroundcolor, info.colors[0], generator_color);
        RegionLinkButton region = button.AddComponent<RegionLinkButton>();
        region.generate = generate;
        region.cursorscript = cursorscript;
        region.soundeffect = soundeffect;
        if (info.generator_link != "")
            region.hovertext = "Click To Open The Region Generator For: " + info.name;
        else
            region.hovertext = "Region: " + info.name + " Does Not Have A Generator Yet";
        region.regionlink = info.generator_link;
    }

    public GameObject AddButton(string text, Vector2Int pos, Color bg, Color fg, Color click)
    {
        GameObject buttonholder = new GameObject();
        buttonholder.name = "button_holder";
        buttonholder.transform.SetParent(transform);
        buttonholder.transform.localPosition = new Vector3(pos.x, -pos.y, 0);
        BoxCollider2D box = buttonholder.AddComponent<BoxCollider2D>();
        box.offset = new Vector2((text.Length / 2f) - 0.5f, 0);
        box.size = new Vector2(text.Length, 1);
        ButtonAnimation btn = buttonholder.AddComponent<ButtonAnimation>();
        btn.bg_col = bg;
        btn.fg_col = fg;
        btn.bg_col_highlight = fg;
        btn.fg_col_highlight = bg;
        btn.bg_col_click = bg;
        btn.fg_col_click = click;
        btn.color_speed = 0.5;
        btn.backgrounds = new List<SpriteRenderer>();
        btn.foregrounds = new List<SpriteRenderer>();

        int x = 0;
        foreach (char c in text)
        {
            GameObject background = new GameObject();
            GameObject top = new GameObject();
            background.name = "background";
            background.transform.SetParent(top.transform);
            background.transform.localPosition = Vector3.zero;
            generate.SetSprite(background, generate.filledsprite, bg, 1);
            btn.backgrounds.Add(background.GetComponent<SpriteRenderer>());

            top.name = "top";
            top.transform.SetParent(buttonholder.transform);
            top.transform.localPosition = new Vector3(x, 0, 0);
            generate.SetSprite(top, spritefromtext.SpriteFromChar(c), fg, 2);
            btn.foregrounds.Add(top.GetComponent<SpriteRenderer>());

            x++;
        }

        return buttonholder;
    }
}
