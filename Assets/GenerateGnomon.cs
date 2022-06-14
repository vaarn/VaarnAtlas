using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct Generator
{
    public string name;
    public char symbol;
    public int weight;
    public string xml;
    public string link;
    public int table_count;
    public Color unique_color;
}

[System.Serializable]
public struct Area
{
    public char c;
    public Color color;
    public Color backgroundcolor;
    public Color text_table_color;
    public Color text_entry_color;
    public string name;
    [TextArea]
    public string info;
    public List<Generator> generators;
}

[System.Serializable]
public struct OtherGenerator
{
    public string word;
    public Generator generator;
}

public class GenerateGnomon : MonoBehaviour
{
    public TextAsset gnomon_symbols;
    public TextAsset gnomon_areas;

    private GenerateMap generate;
    private SpriteFromText spritefromtext;

    private int seed;

    public List<GameObject> enable_objects;
    public List<GameObject> disable_objects;

    private int width, height;
    private int textbox_wider_default;

    public Color ui_back_color;
    public Color ui_front_color;

    public string randombuildings;

    public List<Area> areas;

    public List<OtherGenerator> othergenerators;

    private string previous_seed;

    // Start is called before the first frame update
    public void Start()
    {
        if (PlayerPrefs.HasKey("gno_seed"))
        {
            previous_seed = PlayerPrefs.GetString("gno_seed");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Destroy()
    {
        generate = GetComponent<GenerateMap>();

        //Destroy previous children
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        foreach (GameObject obj in enable_objects)
            obj.SetActive(false);
        foreach (GameObject obj in disable_objects)
            obj.SetActive(true);

        if (textbox_wider_default != 0)
            generate.textbox_wider_default = textbox_wider_default;

        GetComponent<UIRegions>().ResetRegions();
    }

    //Generate a random Gnomon
    public void Generate()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        seed = Random.Range(111111, 999999);
        //seed = (int)System.DateTime.Now.Ticks;
        previous_seed = seed.ToString();
        Generate(seed);
    }

    //Generate a specific seed
    public void Generate(int seed)
    {
        this.seed = seed;
        previous_seed = seed.ToString();
        Random.InitState(this.seed);
        PlayerPrefs.SetString("gno_seed", previous_seed);
        PlayerPrefs.Save();

        generate = GetComponent<GenerateMap>();
        generate.SetURL("gno", seed.ToString());
        spritefromtext = generate.spritefromtext;

        textbox_wider_default = generate.textbox_wider_default;
        generate.textbox_wider_default = 0;

        GetComponent<UIRegions>().ResetRegions();

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

        string[] map_lines = gnomon_symbols.text.Split('\n');
        string[] area_lines = gnomon_areas.text.Split('\n');

        int text_width = map_lines[0].Length;
        int text_height = map_lines.Length;

        int[] tops_left = new int[areas.Count];
        int[] lefts_left = new int[areas.Count];
        int[] rights_left = new int[areas.Count];
        int[] bottoms_left = new int[areas.Count];
        int[] tops_right = new int[areas.Count];
        int[] lefts_right = new int[areas.Count];
        int[] rights_right = new int[areas.Count];
        int[] bottoms_right = new int[areas.Count];
        for (int j = 0; j < areas.Count; j++)
        {
            tops_left[j] = 1000;
            lefts_left[j] = 1000;
            rights_left[j] = -1000;
            bottoms_left[j] = -1000;

            tops_right[j] = 1000;
            lefts_right[j] = 1000;
            rights_right[j] = -1000;
            bottoms_right[j] = -1000;
        }

        generate.SetUpDetails(text_height, width, height, 0, false);

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
                else if (x - 1 < text_width && y <= text_height)
                {
                    Color bg = ui_front_color;
                    Color fg = ui_back_color;

                    char c = map_lines[y - 1][x - 1];
                    string animated = "";
                    bool foundarea = false;
                    int index = 0;
                    foreach (Area a in areas)
                    {
                        if (area_lines[y - 1][x - 1] == a.c)
                        {
                            foundarea = true;
                            bg = a.backgroundcolor;
                            fg = a.color;

                            generate.AddLocation(a.name, new Vector2Int(x, y), "~#" + ColorUtility.ToHtmlStringRGB(a.text_table_color) + " " + a.name + " ");
                            if (a.generators.Count > 0 && x > 26)
                            {
                                List<Generator> weighted_generators = new List<Generator>();
                                foreach (Generator gen in a.generators)
                                {
                                    for (int i = 0; i < gen.weight; i++)
                                        weighted_generators.Add(gen);
                                }
                                Generator chosen_generator = weighted_generators[Random.Range(0, weighted_generators.Count)];
                                if ((chosen_generator.link != "" || chosen_generator.xml != "") && ("1234GS").Contains(a.c.ToString()))
                                {
                                    c = chosen_generator.symbol;
                                }
                                else if (chosen_generator.link == "" && chosen_generator.xml == "")
                                {
                                    c = randombuildings[Random.Range(0, randombuildings.Length)];
                                    generate.AppendToLocation(new Vector2Int(x, y), "\n~#" + ColorUtility.ToHtmlStringRGB(a.text_entry_color) + " " + a.info);
                                }
                                if (chosen_generator.unique_color.r != 0 || chosen_generator.unique_color.g != 0 || chosen_generator.unique_color.b != 0)
                                {
                                    fg = chosen_generator.unique_color;
                                }
                                animated = AddData(new Vector2Int(x, y), chosen_generator, a.text_table_color, a.text_entry_color);
                            }
                            else
                            {
                                generate.AppendToLocation(new Vector2Int(x, y), "\n~#" + ColorUtility.ToHtmlStringRGB(a.text_entry_color) + " " + a.info);
                            }

                            if (x <= 26)
                            {
                                if (y < tops_left[index])
                                    tops_left[index] = y;
                                if (y > bottoms_left[index])
                                    bottoms_left[index] = y;
                                if (x > rights_left[index])
                                    rights_left[index] = x;
                                if (x < lefts_left[index])
                                    lefts_left[index] = x;
                            }
                            else
                            {
                                if (y < tops_right[index])
                                    tops_right[index] = y;
                                if (y > bottoms_right[index])
                                    bottoms_right[index] = y;
                                if (x > rights_right[index])
                                    rights_right[index] = x;
                                if (x < lefts_right[index])
                                    lefts_right[index] = x;
                            }
                        }
                        index++;
                    }
                    
                    if (!foundarea && x != 27)
                        generate.AddLocation("Featureless Sands", new Vector2Int(x, y), "~#" + ColorUtility.ToHtmlStringRGB(generate.sand_background_color) + " Featureless Sands Of The Southern Badlands");

                    if (c != ' ')
                    {
                        GameObject top = new GameObject();
                        if (x == 27)
                        {
                            GameObject bottom = new GameObject();
                            bottom.name = "background";
                            bottom.transform.SetParent(top.transform);
                            bottom.transform.localPosition = Vector3.zero;
                            generate.SetSprite(bottom, generate.filledsprite, fg, 0);
                        }

                        top.name = "top";
                        top.transform.SetParent(transform);
                        top.transform.localPosition = new Vector3(x, -y, 0);
                        generate.SetSprite(top, spritefromtext.SpriteFromChar(c), x == 27 ? bg : fg, 1);
                        if (animated != "")
                        {
                            AnimatedSprites anm = top.AddComponent<AnimatedSprites>();
                            foreach (char a in animated)
                                anm.sprites.Add(spritefromtext.SpriteFromChar(a));
                            anm.waittime = Random.Range(0.5f, 1f);
                        }

                        generate.AddDice(new Vector2Int(x, y), bg, fg);
                    }
                }
                else
                {
                    GameObject bottom = new GameObject();
                    bottom.name = "background";
                    bottom.transform.SetParent(transform);
                    bottom.transform.localPosition = new Vector3(x, -y, 1);
                    generate.SetSprite(bottom, generate.filledsprite, ui_back_color, 3);
                }
            }
        }

        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                if (x - 1 < text_width && y <= text_height)
                {
                    int index = 0;
                    foreach (Area a in areas)
                    {
                        if (area_lines[y - 1][x - 1] == a.c)
                        {
                            if (tops_left[index] != 1000 &&
                                lefts_left[index] != 1000 &&
                                rights_left[index] != -1000 &&
                                bottoms_left[index] != -1000 &&
                                tops_right[index] != 1000 &&
                                lefts_right[index] != 1000 &&
                                rights_right[index] != -1000 &&
                                bottoms_right[index] != -1000)
                            {
                                if (x <= 26)
                                {
                                    GetComponent<UIRegions>().AddArea(new Vector2Int(x, y),
                                        new Vector2Int(lefts_right[index], -tops_right[index]),
                                        new Vector2Int(rights_right[index], -bottoms_right[index]));
                                }
                                else
                                {
                                    GetComponent<UIRegions>().AddArea(new Vector2Int(x, y),
                                        new Vector2Int(lefts_left[index], -tops_left[index]),
                                        new Vector2Int(rights_left[index], -bottoms_left[index]));
                                }
                            }
                        }
                        index++;
                    }
                }
            }
        }

        generate.colorbutton.CheckForButtons();
    }

    private string AddData(Vector2Int pos, Generator generator, Color tablecol, Color textcol)
    {
        if (generator.xml != "")
            return AddDataFromXML(pos, generator.name, generator.xml, tablecol, textcol);
        if (generator.link != "")
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < generator.table_count; i++)
                indices.Add(Random.Range(0, 20));
            AddDataFromURI(pos, generator.name, generator.link, tablecol, textcol, indices);
        }
        return "";
    }

    private string AddDataFromXML(Vector2Int pos, string generator_name, string xml, Color tablecol, Color textcol)
    {
        string details = ": ~#" + ColorUtility.ToHtmlStringRGB(tablecol) + " " + generator_name + "\n";
        XmlNodeList nodes = generate.NodesFromSearch(xml);
        string c = "";
        string checkgen = "";
        if (nodes.Count > 0)
        {
            foreach (XmlNode node in nodes[0].ChildNodes)
            {
                if (node.Attributes.Count > 0 && node.ChildNodes.Count > 0)
                {
                    string name = node.Attributes[0].Value;
                    XmlNode childnode = node.ChildNodes[Random.Range(0, node.ChildNodes.Count)];
                    if (childnode.Attributes.Count > 0)
                    {
                        c = childnode.Attributes[0].Value;
                    }
                    string val = childnode.InnerText;
                    details += "~#" + ColorUtility.ToHtmlStringRGB(tablecol) + " " + name + ": ~#" + ColorUtility.ToHtmlStringRGB(textcol) + " " + val + " ";

                    if (name == "Now")
                        checkgen = val;
                }
            }
            generate.AppendToLocation(pos, details);
        }
        if (checkgen != "")
        {
            foreach (OtherGenerator gen in othergenerators)
            {
                if (gen.word == checkgen)
                {
                    generate.AppendToLocation(pos, "\n\n");
                    AddData(pos, gen.generator, tablecol, textcol);
                }
            }
        }
        return c;
    }

    private void AddDataFromURI(Vector2Int pos, string generator_name, string uri, Color tablecol, Color textcol, List<int> indices)
    {
        uri += "/";
        foreach (int i in indices)
        {
            uri += ("0123456789ABCDEFGHIJK----------------------------")[i];
        }
        StartCoroutine(GetRequest(uri, seed, callback =>
        {
            if (callback != "")
            {
                string details = ": ~#" + ColorUtility.ToHtmlStringRGB(tablecol) + " " + generator_name + "\n";
                while (true)
                {
                    int rollName = callback.IndexOf("\"rollName\":\"");
                    if (rollName == -1)
                        break;
                    callback = callback.Substring(rollName + 12);
                    string name = "";
                    foreach (char c in callback)
                    {
                        if (c == '\"')
                            break;
                        name += c;
                    }

                    int value = callback.IndexOf("\"value\":\"");
                    if (value == -1)
                        break;
                    callback = callback.Substring(value + 9);
                    string val = "";
                    foreach (char c in callback)
                    {
                        if (c == '\"')
                            break;
                        val += c;
                    }

                    details += "~#" + ColorUtility.ToHtmlStringRGB(tablecol) + " " + name + ": ~#" + ColorUtility.ToHtmlStringRGB(textcol) + " " + val + " ";
                }
                details = details.Replace('’', '\'');
                details = details.Replace('ï', 'i');
                generate.AppendToLocation(pos, details);
            }
        }));
    }

    IEnumerator GetRequest(string uri, int seed, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    //Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    callback("");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    //Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    callback("");
                    break;
                case UnityWebRequest.Result.Success:
                    if (seed == this.seed)
                        callback(webRequest.downloadHandler.text);
                    else
                        callback("");
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public string PreviousSeed()
    {
        return previous_seed;
    }
}
