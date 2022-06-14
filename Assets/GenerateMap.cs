using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    public SpriteFromText spritefromtext;
    public MarkovChain markov;
    public MarkovChain markov_npc;
    public TextAsset data;
    public Sprite filledsprite;
    public Color sand_background_color;
    public Color landmark_background_color;

    public int location_min, location_max;
    public int cellsize;
    public int cellborder;

    public int landscape_distance;
    public int landscape_count_offset;

    public string random_details;
    public int random_detail_count;
    public int random_detail_group_min;
    public int random_detail_group_max;

    public Sprite edgesprite;

    public string dicechars;
    public float dice_min_time;
    public float dice_max_time;

    public AudioSource rerollsound;

    public Color default_text_color;
    public Color secondary_text_color;

    public Color corpsecolor;

    public Color hazardcolor;
    public Color caravancolor;
    public Color npccolor;

    public char WE;
    public char NS;
    public char WN;
    public char WS;
    public char EN;
    public char ES;
    public char WNS;
    public char WES;
    public char WNE;
    public char ENS;
    public char WENS;
    public double skip1_chance;
    public double skip2_chance;

    public double dice_distance_1;
    public double dice_distance_2;

    public ColorButton colorbutton;

    public double rapidclick;
    private double timesinceclick;
    public int rapidclick_count;
    private int howmanyrapidclicks;
    private int timessummonedkronos;

    private XmlDocument xmlDoc;
    private int width, height;
    private int available_height;

    private List<string> location_names;
    private List<string> location_details;
    private List<Vector2Int> location_positions;

    private List<Vector2Int> major_location_positions;
    private List<int> major_location_index;

    private List<string> routedetails;

    private int location_text_width;
    private GameObject textholder;

    private string seed_interior;
    private string shareurl;

    private bool active;

    public EasterEggButton waldobutton;
    public EasterEggButton kronosbutton;
    public EasterEggButton venkarthimbutton;

    public Color link_background_color;

    private List<Vector2Int> link_positions;
    private List<string> link_url;

    public GameObject tutorial;
    public Sprite underline;

    public int textbox_length_limit;
    public int textbox_wider_default;
    private int textbox_wider_default_backup; //Because my code is bad :(

    private string previous_region;

    private string last_textboxtext;

    private char[,] map_chars;

    private bool drawtextbackground = true;

    private GenerateKronos kronosgen;
    private GenerateGnomon gnomongen;
    private GenerateWorld worldgen;
    private CursorScript cursorscript;
    private UrthMap urthmapbutton;

    [DllImport("__Internal")]
    private static extern string GetURLFromPage();

    // Start is called before the first frame update
    void Start()
    {
        kronosgen = GetComponent<GenerateKronos>();
        gnomongen = GetComponent<GenerateGnomon>();
        worldgen = GetComponent<GenerateWorld>();
        cursorscript = GameObject.FindObjectOfType<CursorScript>();
        urthmapbutton = GameObject.FindObjectOfType<UrthMap>();

        textbox_wider_default_backup = textbox_wider_default;
        previous_region = "";
        seed_interior = "";
        if (PlayerPrefs.HasKey("int_seed"))
        {
            seed_interior = PlayerPrefs.GetString("int_seed");
        }
        gnomongen.Start();

        xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(data.text));

        timessummonedkronos = 0;
        howmanyrapidclicks = 0;
        timesinceclick = 0;
        active = true;

        GetComponent<UIRegions>().ResetRegions();

        location_names = new List<string>();
        location_details = new List<string>();
        location_positions = new List<Vector2Int>();
        major_location_positions = new List<Vector2Int>();
        major_location_index = new List<int>();
        link_positions = new List<Vector2Int>();
        link_url = new List<string>();
        routedetails = new List<string>();
        map_chars = null;

        if (!Application.isEditor && GetURLFromPage().Contains("v="))
        {
            string parameters = GetURLFromPage().Substring(GetURLFromPage().IndexOf("v=") + 2);
            previous_region = parameters.Substring(0, 3);
            int integerseed;
            if (int.TryParse(parameters.Substring(3), out integerseed))
            {
                shareurl = GetURLFromPage();
                Generate(previous_region, integerseed.ToString());
            }
            else
            {
                if (parameters == "kronos")
                {
                    timesinceclick = 1;
                    howmanyrapidclicks = rapidclick_count;
                    tutorial.SetActive(false);
                    Generate("int");
                }
                else if (parameters == "urt")
                {
                    Generate("urt");
                }
                else if (parameters == "gno")
                {
                    Generate("gno");
                }
                else if (parameters == "int")
                {
                    Generate("int");
                }
                else
                    CheckForSaveData();
            }
        }
        else
            CheckForSaveData();
    }

    private void CheckForSaveData()
    {
        if (PlayerPrefs.HasKey("seed"))
        {
            previous_region = PlayerPrefs.GetString("seed").Substring(0, 3);
            int integerseed;
            if (int.TryParse(PlayerPrefs.GetString("seed").Substring(3), out integerseed))
            {
                Generate(previous_region, integerseed.ToString());
            }
            else
            {
                if (PlayerPrefs.GetString("seed") == "kronos")
                {
                    timesinceclick = 1;
                    howmanyrapidclicks = rapidclick_count;
                    tutorial.SetActive(false);
                    Generate("int");
                }
                else if (PlayerPrefs.GetString("seed") == "urt")
                {
                    Generate("urt");
                }
                else if (PlayerPrefs.GetString("seed") == "gno")
                {
                    Generate("gno");
                }
                else
                    Generate("int");
            }
        }
        else
            Generate("int");
    }

    private void Update()
    {
        if (timesinceclick > 0)
        {
            timesinceclick -= Time.deltaTime;
        }
    }

    public void GenerateRandomize()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        string seed = Random.Range(111111, 999999).ToString();
        //string seed = ((int)System.DateTime.Now.Ticks).ToString();

        bool summonedkronos = false;
        if (timessummonedkronos < 2)
        {
            if (timesinceclick > 0)
            {
                howmanyrapidclicks++;
                if (howmanyrapidclicks > rapidclick_count)
                {
                    if (howmanyrapidclicks > rapidclick_count + 1)
                        kronosgen.DestroyKronos();
                    else
                    {
                        active = false;
                        seed = "kronos";
                        kronosbutton.Found();
                        cursorscript.SetButtonText("");
                        howmanyrapidclicks = 0;
                        ResetMap();
                        gnomongen.Destroy();
                        kronosgen.Generate();
                        summonedkronos = true;
                        timessummonedkronos++;
                    }
                }
            }
        }

        if (!summonedkronos)
        {
            SetURL(previous_region, seed);

            active = true;
            int integerseed;
            if (int.TryParse(seed, out integerseed))
                Generate(previous_region, seed);
        }

        timesinceclick = rapidclick;

        colorbutton.CheckForButtons();
    }

    public void Generate()
    {
        Generate(previous_region);
    }

    public void Generate(string generator)
    {
        Generate(generator, "~");
    }

    public void Generate(string generator, string seed)
    {
        if (generator == "urt")
        {
            urthmapbutton.Flip();
            ResetMap();
            gnomongen.Destroy();
            worldgen.Generate();
        }
        if (generator == "int")
        {
            //gnomongen().Destroy();
            previous_region = "int";
            int result;
            string seed2 = seed;
            if (seed == "~")
                seed2 = seed_interior;
            if (int.TryParse(seed2, out result))
                GenerateInterior(result);
            else
                GenerateRandomize();
            rerollsound.pitch = Random.Range(0.9f, 1.1f);
            rerollsound.Play();
        }
        if (generator == "gno")
        {
            previous_region = "gno";
            ResetMap();
            int result;
            string seed2 = seed;
            if (seed == "~")
                seed2 = gnomongen.PreviousSeed();
            if (int.TryParse(seed2, out result))
                gnomongen.Generate(result);
            else
                gnomongen.Generate();
            rerollsound.pitch = Random.Range(0.9f, 1.1f);
            rerollsound.Play();
        }

        cursorscript.UnPlace();
    }

    public string PreviousSeed()
    {
        return seed_interior;
    }

    public string PreviousRegion()
    {
        return previous_region;
    }

    public void SetURL(string region, string seed)
    {
        string in_url = "";
        if (Application.isEditor)
            in_url = "https://siofragames.itch.io/vaarn-atlas?secret=k1H1L9tpijAHhcBJQV58iawMUY";
        else
            in_url = GetURLFromPage();

        if (in_url.Contains("v="))
        {
            shareurl = in_url.Substring(0, in_url.IndexOf("v=") + 2) + region + seed.ToString();
        }
        else
        {
            if (in_url.Contains("?"))
                shareurl = in_url + "&v=" + region + seed.ToString();
            else
                shareurl = in_url + "?v=" + region + seed.ToString();
        }

        PlayerPrefs.SetString("seed", region + seed);
        PlayerPrefs.Save();
    }

    public void ResetMap()
    {
        //Destroy previous children
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        location_names = new List<string>();
        location_details = new List<string>();
        location_positions = new List<Vector2Int>();
        major_location_positions = new List<Vector2Int>();
        major_location_index = new List<int>();
        map_chars = null;
        routedetails = new List<string>();
        link_url = new List<string>();
        link_positions = new List<Vector2Int>();
    }

    public void GenerateInterior(int seed)
    {
        textbox_wider_default = textbox_wider_default_backup;

        Random.InitState(seed);
        int wastenumber = Random.Range(0, 30); //Turns out I had a random pitch adjustment here in an old version.
        seed_interior = seed.ToString();
        previous_region = "int";
        SetURL(previous_region, seed_interior);
        PlayerPrefs.SetString("int_seed", seed_interior);
        PlayerPrefs.Save();

        location_text_width = 0;
        drawtextbackground = true;
        GetComponent<UIRegions>().ResetRegions();

        //Destroy previous children
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag != "Don'tDestroy")
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        //Begin parsing xml file with data
        XmlNodeList location_nodes = NodesFromSearch("/vaarn/interior/locations/location");
        XmlNodeList landscape_nodes = NodesFromSearch("/vaarn/interior/landscapetypes/landscapetype");

        //Create UI holder
        GameObject LocationUI = new GameObject();
        LocationUI.name = "location_ui";
        LocationUI.transform.SetParent(transform);
        LocationUI.transform.localPosition = Vector3.zero;

        //Get the width and height of the screen in character counts
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(Vector2.one);
        width = Mathf.RoundToInt(edgeVector.x) * 2 - 1;
        height = Mathf.RoundToInt(edgeVector.y) * 2 - 1;

        //Choose how many locations to generate and allow enough height for descriptions
        int location_count = Random.Range(location_min, location_max);
        available_height = height - location_count - 1;
        map_chars = new char[width, available_height+1];
        routedetails = new List<string>();

        //Set location positions within cells
        location_positions = new List<Vector2Int>();
        bool[,] chosencells = new bool[width / cellsize, available_height / cellsize];
        bool[,] takenchars = new bool[width, available_height+1];
        for (int i = 0; i < width / cellsize; i++)
        {
            for (int j = 0; j < available_height / cellsize; j++)
                chosencells[i, j] = false;
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j <= available_height; j++)
            {
                takenchars[i, j] = false;
                map_chars[i, j] = ' ';
            }
        }
        float cell_x = 0;
        float cell_step = (width / cellsize) / (float)location_count;
        major_location_positions = new List<Vector2Int>();
        location_names = new List<string>();
        location_details = new List<string>();
        major_location_index = new List<int>();

        for (int i = 0; i < location_count; i++)
        {
            Vector2Int cell;
            int safety = 1000;
            do
            {
                //cell = new Vector2Int(Random.Range(0, width / cellsize), Random.Range(0, available_height / cellsize));
                cell = new Vector2Int(Mathf.RoundToInt(cell_x), Random.Range(0, available_height / cellsize));
                safety--;
            } while (chosencells[cell.x, cell.y] && safety > 0);
            chosencells[cell.x, cell.y] = true;
            cell_x += cell_step;

            Vector2Int position = new Vector2Int(Random.Range((cell.x * cellsize) + cellborder + 1, (cell.x * cellsize) + cellsize - cellborder),
                                                 Random.Range((cell.y * cellsize) + cellborder + 1, (cell.y * cellsize) + cellsize - cellborder));

            major_location_positions.Add(position);
            takenchars[position.x, position.y] = true;
            takenchars[position.x, position.y + 1] = true;
            if (i >= 9)
                takenchars[position.x + 1, position.y + 1] = true;
        }

        //Set up routes
        int[,] routes = new int[width, available_height + 1];
        for (int i = 0; i < location_count - 1; i++)
        {
            routes = new int[width, available_height + 1];
            bool route1 = RouteTo(i, i+1, takenchars, routes);
            bool route2 = true;
            InstantiateRoutes(routes);
            if ((!route1 || Random.Range(0f, 1f) <= skip1_chance) && i + 2 < location_count - 1)
            {
                routes = new int[width, available_height + 1];
                route2 = RouteTo(i, i + 2, takenchars, routes);
                InstantiateRoutes(routes);
            }
            if ((!route2 || Random.Range(0f, 1f) <= skip2_chance) && i + 3 < location_count - 1)
            {
                routes = new int[width, available_height + 1];
                RouteTo(i, i + 3, takenchars, routes);
                InstantiateRoutes(routes);
            }
        }

        //Create locations
        for (int i = 0; i < location_count; i++)
        {
            major_location_index.Add(location_names.Count);

            //Set up the location name, sprite, and color
            XmlNode location_node = location_nodes[Random.Range(0, location_nodes.Count)];
            GameObject location_object = new GameObject();
            GameObject background_object = new GameObject();
            background_object.name = "location_background";
            background_object.transform.SetParent(location_object.transform);
            background_object.transform.localPosition = Vector3.zero;
            SetSprite(background_object, filledsprite, landmark_background_color, 0);
            GameObject number_object = new GameObject();
            GameObject number_object2 = null;
            number_object.name = (i + 1).ToString()[0].ToString() + "_light";
            number_object.transform.SetParent(location_object.transform);
            GameObject background_number_object = new GameObject();
            background_number_object.name = "location_bg";
            background_number_object.transform.SetParent(number_object.transform);
            background_number_object.transform.localPosition = Vector3.zero;
            SetSprite(background_number_object, filledsprite, landmark_background_color, 0);
            if (i >= 9)
            {
                number_object2 = new GameObject();
                number_object2.name = (i + 1).ToString()[1].ToString() + "_light";
                number_object2.transform.SetParent(location_object.transform);
                GameObject background_number_object2 = new GameObject();
                background_number_object2.name = "location_bg";
                background_number_object2.transform.SetParent(number_object2.transform);
                background_number_object2.transform.localPosition = Vector3.zero;
                SetSprite(background_number_object2, filledsprite, landmark_background_color, 0);
            }
            string name = " ";
            char symbol = '?';
            string details = "";
            string colortext = "";
            int previous_table_selection = -1;
            Color col = new Color(default_text_color.r, default_text_color.g, default_text_color.b);
            foreach (XmlNode child in location_node.ChildNodes)
            {
                if (child.Name == "name")
                {
                    name = markov.NewWord(8) + " " + child.InnerText;
                    location_positions.Add(major_location_positions[i]);
                    location_names.Add(name);
                    location_object.name = name;
                    location_object.transform.SetParent(transform);
                    location_object.transform.localPosition = new Vector3(major_location_positions[i].x, -major_location_positions[i].y, 0);
                    number_object.transform.localPosition = new Vector3(0, -1, 0);
                    location_names.Add(name);
                    location_positions.Add(new Vector2Int(major_location_positions[i].x, major_location_positions[i].y+1));
                    map_chars[major_location_positions[i].x, major_location_positions[i].y] = child.InnerText.ToUpper()[0];
                    map_chars[major_location_positions[i].x, major_location_positions[i].y+1] = (i+1).ToString()[0];
                    map_chars[major_location_positions[i].x + 0, major_location_positions[i].y + 2] = WE;
                    map_chars[major_location_positions[i].x - 1, major_location_positions[i].y + 2] = EN;
                    map_chars[major_location_positions[i].x - 1, major_location_positions[i].y + 1] = NS;
                    map_chars[major_location_positions[i].x - 1, major_location_positions[i].y + 0] = NS;
                    map_chars[major_location_positions[i].x - 1, major_location_positions[i].y - 1] = ES;
                    map_chars[major_location_positions[i].x + 0, major_location_positions[i].y - 1] = WE;
                    if (i >= 9)
                    {
                        number_object2.transform.localPosition = new Vector3(1, -1, 0);
                        location_names.Add(name);
                        location_positions.Add(new Vector2Int(major_location_positions[i].x+1, major_location_positions[i].y + 1));
                        map_chars[major_location_positions[i].x+1, major_location_positions[i].y+1] = (i + 1).ToString()[1];
                        map_chars[major_location_positions[i].x + 1, major_location_positions[i].y - 1] = WE;
                        map_chars[major_location_positions[i].x + 2, major_location_positions[i].y - 1] = WS;
                        map_chars[major_location_positions[i].x + 2, major_location_positions[i].y + 0] = NS;
                        map_chars[major_location_positions[i].x + 2, major_location_positions[i].y + 1] = NS;
                        map_chars[major_location_positions[i].x + 2, major_location_positions[i].y + 2] = WN;
                        map_chars[major_location_positions[i].x + 1, major_location_positions[i].y + 2] = WE;
                    }
                    else
                    {
                        map_chars[major_location_positions[i].x + 1, major_location_positions[i].y - 1] = WS;
                        map_chars[major_location_positions[i].x + 1, major_location_positions[i].y + 0] = NS;
                        map_chars[major_location_positions[i].x + 1, major_location_positions[i].y + 1] = NS;
                        map_chars[major_location_positions[i].x + 1, major_location_positions[i].y + 2] = WN;
                    }
                }
                if (child.Name == "symbol")
                {
                    symbol = child.InnerText[0];
                }
                if (child.Name == "color")
                {
                    colortext = child.InnerText;
                    if (ColorUtility.TryParseHtmlString(child.InnerText, out col))
                    {
                        SetSprite(location_object, spritefromtext.SpriteFromChar(symbol), col, 1);
                        //location_object.GetComponent<SpriteRenderer>().color = col;
                        AddDice(major_location_positions[i], landmark_background_color, col);

                        details += "~" + child.InnerText + " " + name + "\n";
                        XmlNodeList region_name_nodes = NodesFromSearch("/vaarn/interior/regionnames/regionname");
                        if (col.Equals(default_text_color))
                            details += "~#A9CBDB The Region Is Named For: ~#" + ColorUtility.ToHtmlStringRGB(secondary_text_color) + " " + region_name_nodes[Random.Range(0, region_name_nodes.Count)].InnerText + "\n";
                        else
                            details += "~" + colortext + " The Region Is Named For: ~#A9CBDB " + region_name_nodes[Random.Range(0, region_name_nodes.Count)].InnerText + "\n";

                        SetSprite(number_object, spritefromtext.SpriteFromChar((i + 1).ToString()[0]), col, 1);
                        if (i >= 9)
                            SetSprite(number_object2, spritefromtext.SpriteFromChar((i + 1).ToString()[1]), col, 1);

                        //Add UI label
                        GameObject uilabel = new GameObject();
                        uilabel.name = (i + 1).ToString() + " " + name;
                        uilabel.transform.SetParent(LocationUI.transform);
                        uilabel.transform.localPosition = Vector3.zero;
                        int ui_x = 1;
                        foreach (char c in (i >= 9 ? "" : " ") + (i + 1).ToString() + " " + symbol + " " + name)
                        {
                            GameObject letter = new GameObject();
                            letter.name = c.ToString();
                            letter.transform.SetParent(uilabel.transform);
                            letter.transform.localPosition = new Vector3(ui_x, -(available_height + i + 1), 0);
                            SetSprite(letter, spritefromtext.SpriteFromChar(c), col, 1);

                            AddDice(new Vector2Int(ui_x, available_height + i + 1), landmark_background_color, col);

                            ui_x++;
                        }
                        if (((i >= 9 ? "" : " ") + (i + 1).ToString() + " " + symbol + " " + name).Length + 1 >= location_text_width)
                            location_text_width = ((i >= 9 ? "" : " ") + (i + 1).ToString() + " " + symbol + " " + name).Length + 1;

                        //Replace route colors
                        for (int k = 0; k < location_details.Count; k++)
                        {
                            if (location_details[k].Substring(0, 14) == "~#A9CBDB Route")
                            {
                                location_details[k] = location_details[k].Replace(" " + (i+1).ToString()+"toreplace", " ~" + colortext + " " + (i + 1).ToString());
                            }
                        }
                    }
                }
                if (child.Name == "table")
                {
                    bool same = false;
                    foreach (XmlAttribute attribute in child.Attributes)
                    {
                        if (attribute.Name == "same" && attribute.Value == "True")
                            same = true;
                    }
                    if (!same)
                        previous_table_selection = Random.Range(0, child.ChildNodes.Count);

                    if (child.Attributes[0].Value.Contains("NPC A"))
                        details += "\n\n";
                    if (col.Equals(default_text_color))
                    {
                        details += "~#A9CBDB " + child.Attributes[0].Value + ": ~#" + ColorUtility.ToHtmlStringRGB(secondary_text_color) + " ";
                        if (child.ChildNodes.Count > 0)
                            details += child.ChildNodes[previous_table_selection].InnerText + " ";
                    }
                    else
                    {
                        details += "~" + colortext + " " + child.Attributes[0].Value + ": ~#A9CBDB ";
                        if (child.ChildNodes.Count > 0)
                            details += child.ChildNodes[previous_table_selection].InnerText + " ";
                    }
                }
            }
            location_details.Add(details);
            location_details.Add(details);
            if (i >= 9)
                location_details.Add(details);

            //Add selector region
            Vector2Int topleft = new Vector2Int(1, -(available_height + i + 1));
            Vector2Int bottomright = new Vector2Int(name.Length + 5, -(available_height + i + 1));
            GetComponent<UIRegions>().AddArea(new Vector2Int(major_location_positions[i].x, major_location_positions[i].y), topleft, bottomright);
            GetComponent<UIRegions>().AddArea(new Vector2Int(major_location_positions[i].x, major_location_positions[i].y + 1), topleft, bottomright);
            if (i >= 9)
                GetComponent<UIRegions>().AddArea(new Vector2Int(major_location_positions[i].x + 1, major_location_positions[i].y + 1), topleft, bottomright);
            Vector2Int topleft2 = new Vector2Int(major_location_positions[i].x, -major_location_positions[i].y);
            Vector2Int bottomright2 = new Vector2Int(major_location_positions[i].x + (i >= 9 ? 1 : 0), -(major_location_positions[i].y - 1));
            for (int j = 1; j <= name.Length + 5; j++)
            {
                GetComponent<UIRegions>().AddArea(new Vector2Int(j, available_height + i + 1), topleft2, bottomright2);
            }

            //Add landscapedetails
            XmlNode landscape_node = landscape_nodes[Random.Range(0, landscape_nodes.Count)];
            int original_quantity = System.Convert.ToInt32(landscape_node.SelectNodes("./quantity")[0].InnerText);
            int quantity = Mathf.Clamp(Random.Range(original_quantity - landscape_count_offset, original_quantity + landscape_count_offset), 1, 15);
            for (int j = 0; j < quantity; j++)
            {
                Vector2Int position;
                int safety = 1000;
                do
                {
                    position = new Vector2Int(Random.Range(major_location_positions[i].x - landscape_distance, major_location_positions[i].x + landscape_distance),
                                              Random.Range(major_location_positions[i].y - landscape_distance, major_location_positions[i].y + landscape_distance));
                    if (position.x < 1)
                        position = new Vector2Int(1, position.y);
                    if (position.x >= width - 1)
                        position = new Vector2Int(width - 2, position.y);
                    if (position.y < 1)
                        position = new Vector2Int(position.x, 1);
                    if (position.y > available_height)
                        position = new Vector2Int(position.x, available_height);
                    safety--;
                } while (takenchars[position.x, position.y] && safety > 0);
                takenchars[position.x, position.y] = true;

                if (safety > 0)
                {
                    GameObject landscape_obj = new GameObject();
                    landscape_obj.transform.SetParent(transform);
                    landscape_obj.transform.localPosition = new Vector3(position.x, -position.y, 0);

                    string name_detail = "";
                    char symbol_chosen = '?';
                    foreach (XmlNode child in landscape_node.ChildNodes)
                    {
                        if (child.Name == "name")
                        {
                            name_detail = child.InnerText;
                            location_positions.Add(position);
                            location_names.Add(name_detail);
                            landscape_obj.name = name_detail;
                        }
                        if (child.Name == "symbol")
                        {
                            symbol_chosen = child.InnerText[Random.Range(0, child.InnerText.Length)];
                        }
                        if (child.Name == "color")
                        {
                            Color col_detail;
                            if (ColorUtility.TryParseHtmlString(child.InnerText, out col_detail))
                                SetSprite(landscape_obj, spritefromtext.SpriteFromChar(symbol_chosen), col_detail, 1);
                            AddDice(position, sand_background_color, col_detail);

                            location_details.Add("~" + child.InnerText + " " + name_detail);
                        }
                    }
                }
            }
        }

        //Add random details
        for (int i = 0; i < random_detail_count; i++)
        {
            Vector2Int position;
            int safety = 1000;
            do
            {
                position = new Vector2Int(Random.Range(1, width), Random.Range(1, available_height+1));
                safety--;
            } while (takenchars[position.x, position.y] && safety > 0);
            if (safety > 0)
            {
                for (int j = 0; j < Random.Range(random_detail_group_min, random_detail_group_max); j++)
                {
                    Vector2Int newpos;
                    safety = 1000;
                    do
                    {
                        newpos = new Vector2Int(position.x + Random.Range(position.x > 1 ? -1 : 0, position.x < width ? 1 : 0),
                                              position.y + Random.Range(position.y > 1 ? -1 : 0, position.y < available_height+1 ? 1 : 0));
                        safety--;
                    } while (takenchars[newpos.x, newpos.y] && safety > 0);
                    if (safety > 0)
                    {
                        position = newpos;
                        takenchars[position.x, position.y] = true;
                        GameObject detail_object = new GameObject();
                        detail_object.name = "detail object " + (i + 1).ToString() + "_" + (j + 1).ToString();
                        AddSandDetail(position);
                        detail_object.transform.SetParent(transform);
                        detail_object.transform.localPosition = new Vector3(position.x, -position.y, 0);
                        SetSprite(detail_object, spritefromtext.SpriteFromChar(random_details[Random.Range(0, random_details.Length)]), landmark_background_color, 0);
                        AddDice(position);
                    }
                }
            }
        }

        //Add corpses
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            Vector2Int position;
            int safety = 1000;
            do
            {
                position = new Vector2Int(Random.Range(1, width), Random.Range(1, available_height + 1));
                safety--;
            } while (takenchars[position.x, position.y] && safety > 0);
            if (safety > 0)
            {
                takenchars[position.x, position.y] = true;
                location_positions.Add(position);
                location_names.Add("corpse");
                string details = "~#e35f8b You Found A Corpse\n\nCondition: ~#ffedf3 ";
                XmlNodeList conditions = NodesFromSearch("/vaarn/interior/corpse/conditions/condition");
                XmlNodeList corpseitems = NodesFromSearch("/vaarn/interior/corpse/items/item");
                details += conditions[Random.Range(0, conditions.Count)].InnerText;
                details += "\n~#e35f8b They Were Carrying: ~#ffedf3 ";
                details += corpseitems[Random.Range(0, corpseitems.Count)].InnerText;
                location_details.Add(details);

                GameObject corpse = new GameObject();
                corpse.name = "corpse";
                corpse.transform.SetParent(transform);
                corpse.transform.localPosition = new Vector3(position.x, -position.y, 0);
                SetSprite(corpse, spritefromtext.SpriteFromChar('◘'), corpsecolor, 1);
            }
        }

        //Add UI Background
        for (int x = 1; x <= width-1; x++)
        {
            for (int y = 1; y < height - available_height; y++)
            {
                GameObject background = new GameObject();
                background.name = "ui_background";
                background.transform.SetParent(transform);
                background.transform.localPosition = new Vector3(x, -(height - y), 0);
                SetSprite(background, filledsprite, landmark_background_color, 0);
            }
        }
        for (int y = 0; y <= height; y++)
        {
            GameObject background = new GameObject();
            GameObject background_top = new GameObject();
            background_top.name = "edge_top";
            background_top.transform.SetParent(background.transform);
            SetSprite(background_top, edgesprite, sand_background_color, 1);

            background.name = "ui_background";
            background.transform.SetParent(transform);
            background.transform.localPosition = new Vector3(0, -y, 0);
            SetSprite(background, filledsprite, landmark_background_color, 0);
        }
        for (int y = 0; y <= height; y++)
        {
            GameObject background = new GameObject();
            GameObject background_top = new GameObject();
            background_top.name = "edge_top";
            background_top.transform.SetParent(background.transform);
            SetSprite(background_top, edgesprite, sand_background_color, 1);

            background.name = "ui_background";
            background.transform.SetParent(transform);
            background.transform.localPosition = new Vector3(width, -y, 0);
            SetSprite(background, filledsprite, landmark_background_color, 0);
        }
        for (int x = 1; x <= width - 1; x++)
        {
            GameObject background = new GameObject();
            GameObject background_top = new GameObject();
            background_top.name = "edge_top";
            background_top.transform.SetParent(background.transform);
            SetSprite(background_top, edgesprite, sand_background_color, 1);

            background.name = "ui_background";
            background.transform.SetParent(transform);
            background.transform.localPosition = new Vector3(x, 0, 0);
            SetSprite(background, filledsprite, landmark_background_color, 0);
        }
        for (int x = 1; x <= width - 1; x++)
        {
            GameObject background = new GameObject();
            GameObject background_top = new GameObject();
            background_top.name = "edge_top";
            background_top.transform.SetParent(background.transform);
            SetSprite(background_top, edgesprite, sand_background_color, 1);

            background.name = "ui_background";
            background.transform.SetParent(transform);
            background.transform.localPosition = new Vector3(x, -height, 0);
            SetSprite(background, filledsprite, landmark_background_color, 0);
        }

        //Add random sand details
        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y <= available_height; y++)
            {
                if (!takenchars[x, y])
                {
                    location_positions.Add(new Vector2Int(x, y));
                    location_names.Add("Featureless Sands");
                    location_details.Add("~#" + ColorUtility.ToHtmlStringRGB(sand_background_color) + " Featureless Sands");
                }
            }
        }

        colorbutton.CheckForButtons();
        //spritefromtext.CheckUpdate();
    }

    private bool RouteTo(int index1, int index2, bool[,] takenchars, int[,] routes)
    {
        int x = major_location_positions[index1].x;
        int y = major_location_positions[index1].y;
        int distance = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(major_location_positions[index2].x - x, 2) + Mathf.Pow(major_location_positions[index2].y - y, 2)));
        routes[x, y] = 2;

        string routename = "Route " + (index1 + 1).ToString() + " to " + (index2 + 1).ToString();
        string details = "~#A9CBDB Route " + (index1 + 1).ToString() + "toreplace ~#A9CBDB to " + (index2 + 1).ToString() + "toreplace";
        int distance_roll = 0;
        if (distance < dice_distance_1)
            distance_roll = Random.Range(1, 7);
        else if (distance < dice_distance_2)
            distance_roll = Random.Range(1, 7) + Random.Range(1, 7);
        else
            distance_roll = Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7);
        details += "\n\n~#A9CBDB Takes ~#F8961D " + distance_roll.ToString() + " days ~#A9CBDB to travel";
        bool hazard = Random.Range(1, 4) == 1;
        bool caravan = Random.Range(1, 10) == 1;
        bool npc = Random.Range(1, 10) == 1;

        if (caravan && npc)
        {
            if (Random.Range(0f, 1f) < 0.5f)
                caravan = false;
            else
                npc = false;
        }

        XmlNodeList hazards = NodesFromSearch("/vaarn/interior/travelhazards/travelhazard");
        XmlNode chosen_hazard = hazards[Random.Range(0, hazards.Count)];
        if (hazard)
        {
            details += "\n\n~#" + ColorUtility.ToHtmlStringRGB(hazardcolor) + " Travel Hazard: ~#A9CBDB " + chosen_hazard.InnerText;
            hazard = true;
        }
        XmlNodeList caravan_tables = NodesFromSearch("/vaarn/interior/tradecaravan/table");
        if (caravan)
        {
            details += "\n\n~#" + ColorUtility.ToHtmlStringRGB(caravancolor) + " A Trade Caravan Travels The Road ";
            foreach (XmlNode table in caravan_tables)
            {
                details += "~#" + ColorUtility.ToHtmlStringRGB(caravancolor) + " " + table.Attributes[0].Value + ": ~#A9CBDB ";
                if (table.ChildNodes.Count > 0)
                    details += table.ChildNodes[Random.Range(0, table.ChildNodes.Count)].InnerText + " ";
            }
            caravan = true;
        }
        char npcsymbol = ' ';
        XmlNodeList npc_tables = NodesFromSearch("/vaarn/interior/npc/table");
        if (npc)
        {
            details += "\n\n~#" + ColorUtility.ToHtmlStringRGB(npccolor) + " You Meet Someone Along The Road ";
            details += "~#" + ColorUtility.ToHtmlStringRGB(npccolor) + " Name: ~#A9CBDB " + markov_npc.NewWord(8) + " ";
            foreach (XmlNode table in npc_tables)
            {
                XmlNode entry = table.ChildNodes[Random.Range(0, table.ChildNodes.Count)];
                if (entry.Attributes.Count > 0)
                {
                    if (entry.Attributes[0].Name == "symbol")
                        npcsymbol = entry.Attributes[0].Value[0];
                }
                details += "~#" + ColorUtility.ToHtmlStringRGB(npccolor) + " " + table.Attributes[0].Value + ": ~#A9CBDB ";
                details += entry.InnerText + " ";
            }
            npc = true;
        }

        int safety = 1000;
        int distancetraveled = 0;

        //Test route first
        int takencharslimit = 3;
        while (!(x == major_location_positions[index2].x && y == major_location_positions[index2].y) && safety > 0)
        {
            int x_dist = major_location_positions[index2].x - x;
            int y_dist = major_location_positions[index2].y - y;

            if (Mathf.Abs(x_dist) >= Mathf.Abs(y_dist))
                x += x_dist > 0 ? 1 : -1;
            else
                y += y_dist > 0 ? 1 : -1;

            if (takenchars[x, y] && x < major_location_positions[index2].x)
            {
                foreach (Vector2Int loc in major_location_positions)
                {
                    if (loc.x == x && loc.y == y)
                    {
                        if (!(x == major_location_positions[index2].x && y == major_location_positions[index2].y))
                            return false;
                    }
                }

                takencharslimit--;
                if (takencharslimit < 0)
                    return false;
                //x++;
            }
        }

        x = major_location_positions[index1].x;
        y = major_location_positions[index1].y;

        while (!(x == major_location_positions[index2].x && y == major_location_positions[index2].y) && safety > 0)
        {
            int x_dist = major_location_positions[index2].x - x;
            int y_dist = major_location_positions[index2].y - y;

            if (Mathf.Abs(x_dist) >= Mathf.Abs(y_dist))
                x += x_dist > 0 ? 1 : -1;
            else
                y += y_dist > 0 ? 1 : -1;

            if (takenchars[x, y] && x < major_location_positions[index2].x)
            {
                if (routes[x, y] == 0)
                    routes[x, y] = 2;
                //x++;
            }

            if (!takenchars[x, y])
            {
                routes[x, y] = 1;
                takenchars[x, y] = true;
                location_names.Add(routename);
                location_positions.Add(new Vector2Int(x, y));
                location_details.Add(details);
            }
            else
            {
                if (routes[x, y] == 0)
                    routes[x, y] = 2;
            }

            safety--;
            distancetraveled++;
            if ((hazard || caravan || npc) && distancetraveled > distance / 2)
            {
                distancetraveled = -1000;
                if (hazard)
                {
                    //Add character
                    if (chosen_hazard.Attributes.Count > 0)
                    {
                        int char_x = x;
                        int char_y = y;
                        int safety2 = 100;
                        float range = 0;
                        while (takenchars[char_x, char_y] && safety2 > 0)
                        {
                            char_x = Random.Range(-Mathf.RoundToInt(range), Mathf.RoundToInt(range)) + x;
                            char_y = Random.Range(-Mathf.RoundToInt(range), Mathf.RoundToInt(range)) + y;
                            if (char_x < 0)
                                char_x = 0;
                            if (char_x >= width)
                                char_x = width - 1;
                            if (char_y < 0)
                                char_y = 0;
                            if (char_y > available_height)
                                char_y = available_height;
                            safety2--;
                            range += 0.25f;
                        }
                        if (safety2 > 0)
                        {
                            //Add route character
                            takenchars[char_x, char_y] = true;
                            location_positions.Add(new Vector2Int(char_x, char_y));
                            location_names.Add("route character");
                            location_details.Add(details);
                            //location_details.Add("~#F8961D Travel Hazard: ~#A9CBDB " + chosen_hazard.InnerText);

                            GameObject route_character = new GameObject();
                            route_character.name = "route chracter";
                            route_character.transform.SetParent(transform);
                            route_character.transform.localPosition = new Vector3(char_x, -char_y, 0);
                            SetSprite(route_character, spritefromtext.SpriteFromChar(chosen_hazard.Attributes[0].Value[0]), hazardcolor, 1);
                            if (chosen_hazard.Attributes[0].Value.Length > 1)
                            {
                                AnimatedSprites anm = route_character.AddComponent<AnimatedSprites>();
                                foreach (char c in chosen_hazard.Attributes[0].Value)
                                    anm.sprites.Add(spritefromtext.SpriteFromChar(c));
                                if (chosen_hazard.Attributes.Count > 2)
                                    anm.waittime = Random.Range(float.Parse(chosen_hazard.Attributes[1].Value), float.Parse(chosen_hazard.Attributes[2].Value));
                                else
                                    anm.waittime = Random.Range(1f, 3f);
                            }
                        }
                    }
                }
                if (caravan)
                {
                    int char_x = x;
                    int char_y = y;
                    int safety2 = 100;
                    float range = 0;
                    while (takenchars[char_x, char_y] && safety2 > 0)
                    {
                        char_x = Random.Range(-Mathf.RoundToInt(range), Mathf.RoundToInt(range)) + x;
                        char_y = Random.Range(-Mathf.RoundToInt(range), Mathf.RoundToInt(range)) + y;
                        if (char_x < 0)
                            char_x = 0;
                        if (char_x >= width)
                            char_x = width - 1;
                        if (char_y < 0)
                            char_y = 0;
                        if (char_y > available_height)
                            char_y = available_height;
                        safety2--;
                        range += 0.25f;
                    }
                    if (safety2 > 0)
                    {
                        //Add route character
                        takenchars[char_x, char_y] = true;
                        location_positions.Add(new Vector2Int(char_x, char_y));
                        location_names.Add("trade caravan");
                        location_details.Add(details);

                        GameObject route_character = new GameObject();
                        route_character.name = "trade caravan";
                        route_character.transform.SetParent(transform);
                        route_character.transform.localPosition = new Vector3(char_x, -char_y, 0);
                        SetSprite(route_character, spritefromtext.SpriteFromChar('☺'), caravancolor, 1);
                    }
                }
                if (npc)
                {
                    int char_x = x;
                    int char_y = y;
                    int safety2 = 100;
                    float range = 0;
                    while (takenchars[char_x, char_y] && safety2 > 0)
                    {
                        char_x = Random.Range(-Mathf.RoundToInt(range), Mathf.RoundToInt(range)) + x;
                        char_y = Random.Range(-Mathf.RoundToInt(range), Mathf.RoundToInt(range)) + y;
                        if (char_x < 0)
                            char_x = 0;
                        if (char_x >= width)
                            char_x = width - 1;
                        if (char_y < 0)
                            char_y = 0;
                        if (char_y > available_height)
                            char_y = available_height;
                        safety2--;
                        range += 0.25f;
                    }
                    if (safety2 > 0)
                    {
                        //Add route character
                        takenchars[char_x, char_y] = true;
                        location_positions.Add(new Vector2Int(char_x, char_y));
                        location_names.Add("npc");
                        location_details.Add(details);

                        GameObject route_character = new GameObject();
                        route_character.name = "npc";
                        route_character.transform.SetParent(transform);
                        route_character.transform.localPosition = new Vector3(char_x, -char_y, 0);
                        SetSprite(route_character, spritefromtext.SpriteFromChar(npcsymbol), npccolor, 1);
                    }
                }
            }
        }
        routedetails.Add(details);
        return true;
    }

    private void InstantiateRoutes(int[,] routes)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < available_height; y++)
            {
                bool west = x - 1 > 0 ? routes[x - 1, y] > 0 : false;
                bool north = y - 1 > 0 ? routes[x, y - 1] > 0 : false;
                bool east = x + 1 < width ? routes[x + 1, y] > 0 : false;
                bool south = y + 1 < available_height + 1 ? routes[x, y + 1] > 0 : false;
                char chosenchar = WE;
                if (west && east && north && south)
                    chosenchar = WENS;
                else if (west && north && south)
                    chosenchar = WNS;
                else if (west && east && south)
                    chosenchar = WES;
                else if (west && north && east)
                    chosenchar = WNE;
                else if (east && north && south)
                    chosenchar = ENS;
                else if (west && east)
                    chosenchar = WE;
                else if (north && south)
                    chosenchar = NS;
                else if (west && north)
                    chosenchar = WN;
                else if (west && south)
                    chosenchar = WS;
                else if (east && north)
                    chosenchar = EN;
                else if (east && south)
                    chosenchar = ES;
                if (routes[x, y] == 1)
                {
                    GameObject route = new GameObject();
                    route.name = "route";
                    route.transform.SetParent(transform);
                    route.transform.localPosition = new Vector3(x, -y, 0);
                    SetSprite(route, spritefromtext.SpriteFromChar(chosenchar), default_text_color, 1);

                    AddDice(new Vector2Int(x, y), sand_background_color, default_text_color);
                }
                if (routes[x, y] > 0)
                {
                    if (char.IsWhiteSpace(map_chars[x, y]))
                        map_chars[x, y] = chosenchar;
                }
            }
        }
    }

    private void AddSandDetail(Vector2Int pos)
    {
        location_positions.Add(pos);
        location_names.Add("The Never Ceasing Azure Sands of Vaarn");
        string detail = "~#" + ColorUtility.ToHtmlStringRGB(sand_background_color) + " The Never Ceasing Azure Sands of Vaarn\n\n";
        detail +="What You Find In The Sand: ~#" + ColorUtility.ToHtmlStringRGB(secondary_text_color) + " ";
        XmlNodeList item_nodes = NodesFromSearch("/vaarn/interior/iteminsand/item");
        detail += item_nodes[Random.Range(0, item_nodes.Count)].InnerText;
        location_details.Add(detail);
    }

    public void AddDice(Vector2Int pos)
    {
        AddDice(pos, sand_background_color, landmark_background_color);
    }

    public void AddDice(Vector2Int pos, Color bg, Color fg)
    {
        GameObject dice = new GameObject();
        dice.name = "dice";
        dice.transform.SetParent(this.transform);
        dice.transform.localPosition = new Vector3(pos.x, -pos.y, 0);
        SetSprite(dice, spritefromtext.SpriteFromChar(dicechars[Random.Range(0, dicechars.Length)]), fg, 5);
        GameObject dice_background = new GameObject();
        dice_background.name = "dice_background";
        dice_background.transform.SetParent(dice.transform);
        dice_background.transform.localPosition = Vector3.zero;
        SetSprite(dice_background, filledsprite, bg, 4);

        Destroy(dice, Random.Range(dice_min_time, dice_max_time));
    }

    public void SetSprite(GameObject obj, Sprite sprite, Color color, int sortingorder)
    {
        if (!obj.GetComponent<SpriteRenderer>())
            obj.AddComponent<SpriteRenderer>();
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        obj.GetComponent<SpriteRenderer>().color = color;
        obj.GetComponent<SpriteRenderer>().sortingOrder = sortingorder;

        //Tell color button we added an object in case it needs to color it.
        colorbutton.AddedObject(obj.GetComponent<SpriteRenderer>());
    }

    public void AddLocation(string name, Vector2Int pos, string details)
    {
        location_names.Add(name);
        location_positions.Add(pos);
        location_details.Add(details);
    }

    public void AppendToLocation(Vector2Int pos, string details)
    {
        int index = 0;
        foreach(Vector2Int p in location_positions)
        {
            if (p.x == pos.x && p.y == pos.y)
            {
                location_details[index] += details;
            }
            index++;
        }
    }
    
    public XmlNodeList NodesFromSearch(string search)
    {
        return xmlDoc.SelectNodes(search);
    }

    public string GetNameAt(Vector2Int pos)
    {
        int index = 0;
        foreach (Vector2Int p in location_positions)
        {
            if (p.x == pos.x && p.y == pos.y)
                return location_names[index];
            index++;
        }
        return "";
    }

    public string GetDetailAt(Vector2Int pos)
    {
        int index = 0;
        foreach (Vector2Int p in location_positions)
        {
            if (p.x == pos.x && p.y == pos.y)
            {
                if (location_details[index].Contains("Photo of a True-Kin, Wearing Spectacles and Dressed in Red and White Striped Clothing"))
                    waldobutton.Found();
                if (location_details[index].Contains("whose divine tongue is two-score cubits in length!"))
                    venkarthimbutton.Found();
                return location_details[index];
            }
            index++;
        }
        return "";
    }

    public int MajorLocation(Vector2Int pos)
    {
        int index = 0;
        foreach (Vector2Int p in major_location_positions)
        {
            if (p.x == pos.x && p.y == pos.y)
                return index;
            index++;
        }
        return -1;
    }

    public string LocationName(int index)
    {
        return location_names[index];
    }

    public string ClickableLink(Vector2Int pos)
    {
        int index = 0;
        foreach (Vector2Int p in link_positions)
        {
            if (pos.x == p.x && pos.y == p.y)
                return link_url[index];
            index++;
        }
        return "";
    }

    public string FirstURL()
    {
        if (link_url.Count > 0)
            return link_url[0];
        return "";
    }

    public int Width()
    {
        return width;
    }

    public int AvailableHeight()
    {
        return available_height;
    }

    public void SetUpDetails(int available_height, int width, int height, int location_text_width)
    {
        this.available_height = available_height;
        this.width = width;
        this.height = height;
        this.location_text_width = location_text_width;
        drawtextbackground = true;
    }

    public void SetUpDetails(int available_height, int width, int height, int location_text_width, bool drawtextbackground)
    {
        this.available_height = available_height;
        this.width = width;
        this.height = height;
        this.location_text_width = location_text_width;
        this.drawtextbackground = drawtextbackground;
    }

    public void AddTextBox(string text)
    {
        if (text == last_textboxtext)
            return;
        last_textboxtext = text;
        if (!active)
            return;
        if (width == 0 || height == 0)
            return;
        if (textholder)
            Destroy(textholder);
        textholder = new GameObject();
        textholder.name = "textholder";
        textholder.transform.SetParent(transform);
        textholder.transform.localPosition = Vector3.zero;

        //Check for hyperlinks
        string acceptablesymbols = " .,'-:;()?!sy\"\t\0\n";
        if (text.Length > 0)
        {
            link_positions = new List<Vector2Int>();
            link_url = new List<string>();
            XmlNodeList srdlinks = NodesFromSearch("/vaarn/srdlinks/srdlink");
            foreach (XmlNode node in srdlinks)
            {
                if (text.ToLower().Contains(node.Attributes[0].Value.ToLower()))
                {
                    bool biggerfish = false;
                    //Check if there are any nodes bigger that are also found
                    foreach (XmlNode node2 in srdlinks)
                    {
                        if (node != node2)
                        {
                            if (text.ToLower().Contains(node2.Attributes[0].Value.ToLower()) && node2.Attributes[0].Value.ToLower().Contains(node.Attributes[0].Value.ToLower()))
                                biggerfish = true;
                        }
                    }

                    if (!biggerfish)
                    {
                        int index = 0;
                        int safety = 100;
                        while (index != -1 && safety > 0)
                        {
                            index = text.ToLower().IndexOf(node.Attributes[0].Value.ToLower(), index);
                            if (index != -1)
                            {
                                if (index == 0 || acceptablesymbols.Contains(text[index - 1].ToString()))
                                {
                                    int longer = 0;
                                    bool isacceptable = false;
                                    if (index + node.Attributes[0].Value.Length >= text.Length)
                                        isacceptable = true;
                                    else
                                    {
                                        if (acceptablesymbols.Contains(text[index + node.Attributes[0].Value.Length].ToString()))
                                        {
                                            isacceptable = true;
                                            longer++;
                                        }
                                    }
                                    if (isacceptable)
                                    {
                                        text = text.Insert(index, " {" + node.InnerText + " ");
                                        if (index + node.InnerText.Length + 3 + node.Attributes[0].Value.Length + longer < text.Length)
                                            text = text.Insert(index + node.InnerText.Length + 3 + node.Attributes[0].Value.Length + longer, " } ");
                                        else
                                            text = text + " }";
                                        index += node.InnerText.Length + 3 + node.Attributes[0].Value.Length + longer;
                                    }
                                    else
                                        index++;
                                }
                            }
                            safety--;
                        }
                    }
                }
            }
        }

        int x = 0;
        int available_width = width - location_text_width;
        int extrawidth = GetTextBoxExtraWidth(text);
        int y = 0;
        Color color = default_text_color;

        //Draw edge
        for (int i = available_height + 1; i <= height-1; i++)
        {
            GameObject background = new GameObject();
            GameObject background_top = new GameObject();
            background_top.name = "edge_top";
            background_top.transform.SetParent(background.transform);
            SetSprite(background_top, edgesprite, sand_background_color, 4);

            background.name = "ui_background";
            background.transform.SetParent(textholder.transform);
            background.transform.localPosition = new Vector3(x + location_text_width - extrawidth, -i, 0);
            SetSprite(background, filledsprite, landmark_background_color, 3);
        }

        string url = "";
        foreach (string sentence in text.Split('\n'))
        {
            x = 0;
            if (sentence.Length > 0)
            {
                foreach (string word in sentence.Split(' '))
                {
                    if (word.Length > 0)
                    {
                        if (word[0] == '~')
                        {
                            ColorUtility.TryParseHtmlString(word.Trim('~'), out color);
                            if (color.Equals(landmark_background_color))
                                color = default_text_color;
                        }
                        else if (word[0] == '{')
                        {
                            url = word.Remove(0, 1);
                        }
                        else if (word[0] == '}')
                        {
                            url = "";
                        }
                        else
                        {
                            if (x + word.Length + 1 >= available_width + extrawidth)
                            {
                                while (x < available_width + extrawidth - 1 && drawtextbackground)
                                {
                                    GameObject background = new GameObject();
                                    background.name = "background";
                                    background.transform.SetParent(textholder.transform);
                                    background.transform.localPosition = new Vector3(x + location_text_width + 1 - extrawidth, -(available_height + y + 1), 0);
                                    SetSprite(background, filledsprite, landmark_background_color, 3);
                                    x++;
                                }
                                y++;
                                x = 0;
                            }

                            foreach (char c in word + ' ')
                            {
                                GameObject letter = new GameObject();
                                if (drawtextbackground || url != "")
                                {
                                    GameObject background = new GameObject();
                                    background.name = "background";
                                    background.transform.SetParent(letter.transform);
                                    if (url == "")
                                        SetSprite(background, filledsprite, landmark_background_color, 3);
                                    else
                                    {
                                        link_positions.Add(new Vector2Int(x + location_text_width + 1 - extrawidth, available_height + y + 1));
                                        link_url.Add(url);
                                        SetSprite(background, filledsprite, link_background_color, 3);
                                        GameObject underline_obj = new GameObject();
                                        underline_obj.name = "underline";
                                        underline_obj.transform.SetParent(letter.transform);
                                        SetSprite(underline_obj, underline, color, 4);
                                    }
                                }

                                letter.name = c.ToString();
                                letter.transform.SetParent(textholder.transform);
                                letter.transform.localPosition = new Vector3(x + location_text_width + 1 - extrawidth, -(available_height + y + 1), 0);
                                SetSprite(letter, spritefromtext.SpriteFromChar(c), color, 4);
                                x++;
                            }
                        }
                    }
                }
            }
            while (x < available_width + extrawidth - 1 && drawtextbackground)
            {
                GameObject background = new GameObject();
                background.name = "background";
                background.transform.SetParent(textholder.transform);
                background.transform.localPosition = new Vector3(x + location_text_width + 1 - extrawidth, -(available_height + y + 1), 0);
                SetSprite(background, filledsprite, landmark_background_color, 3);
                x++;
            }
            y++;
        }
        while (y < (height - available_height) - 1 && drawtextbackground)
        {
            x = 0;
            while (x < available_width + extrawidth - 1)
            {
                GameObject background = new GameObject();
                background.name = "background";
                background.transform.SetParent(textholder.transform);
                background.transform.localPosition = new Vector3(x + location_text_width + 1 - extrawidth, -(available_height + y + 1), 0);
                SetSprite(background, filledsprite, landmark_background_color, 3);
                x++;
            }
            y++;
        }
    }

    private int GetTextBoxExtraWidth(string text)
    {
        int extrawidth = 0;
        int available_width = width - location_text_width;
        if (text.Split(' ').Length > textbox_length_limit)
            extrawidth = textbox_wider_default;
        bool fit = false;

        while (!fit)
        {
            int x = 0;
            int y = 0;
            int longestword = 0;

            foreach (string sentence in text.Split('\n'))
            {
                x = 0;
                if (sentence.Length > 0)
                {
                    foreach (string word in sentence.Split(' '))
                    {
                        if (word.Length > 0)
                        {
                            if (word[0] != '~' && word[0] != '{' && word[0] != '}')
                            {
                                if (word.Length > longestword)
                                    longestword = word.Length;
                                if (x + word.Length + 1 >= available_width + extrawidth)
                                {
                                    y++;
                                    x = 0;
                                }

                                foreach (char c in word + ' ')
                                {
                                    x++;
                                }
                            }
                        }
                    }
                }
                y++;
            }

            if (y >= height - available_height || longestword >= available_width + extrawidth)
                extrawidth++;
            else
                fit = true;
        }

        return extrawidth;
    }

    public string ShareURL()
    {
        return shareurl;
    }

    public string ShareSeed()
    {
        return shareurl.Substring(shareurl.IndexOf("v=")+2);
    }

    public char[,] MapChars()
    {
        return map_chars;
    }

    public List<string> AllMajorDetails()
    {
        List<string> majordetails = new List<string>();
        foreach (int i in major_location_index)
        {
            majordetails.Add(location_details[i]);
        }
        return majordetails;
    }

    public List<string> RouteDetails()
    {
        return routedetails;
    }
}
