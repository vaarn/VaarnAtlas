using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    public Vector2Int cursorpos;
    public GenerateMap genmap;
    private GenerateWorld worldmap;
    public SpriteRenderer uiselect;

    public AudioSource selectsound;

    public GameObject dot;

    public double holdtime;
    public double firstholdtime;
    private double timeheld;

    private Vector2Int prevpos;
    private Vector2Int prevmouse;
    private bool placed;
    private string buttoninfotext;

    private bool mousecontrol;

    private string url;

    public double doubleclicktime;
    private double timesinceclick;

    public CursorTriggerControl triggercontrol;

    public Sprite linksprite;
    private Sprite originaldotsprite;
    public Sprite linksprite_back;
    public SpriteRenderer dotbackground;
    private Sprite originaldotbackgroundsprite;

    [DllImport("__Internal")]
    private static extern void openWindow(string url);

    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor)
            Cursor.visible = false;
        prevpos = Vector2Int.zero;
        placed = false;
        //dot.SetActive(false);
        buttoninfotext = "";
        mousecontrol = true;
        timeheld = 0;
        triggercontrol.gameObject.SetActive(false);
        worldmap = genmap.GetComponent<GenerateWorld>();
        originaldotsprite = dot.GetComponent<SpriteRenderer>().sprite;
        originaldotbackgroundsprite = dotbackground.GetComponent<SpriteRenderer>().sprite;
    }

    public void SetButtonText(string text)
    {
        buttoninfotext = text;
        if (!placed)
        {
            if (text != "")
                genmap.AddTextBox(text);
        }
    }

    public void UnPlace()
    {
        placed = false;
        //dot.SetActive(false);
        url = "";
        buttoninfotext = "";
    }

    private void Update()
    {
        Vector3 original_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        original_pos.z = 0;
        dot.transform.position = original_pos;
    }

    void LateUpdate()
    {
        triggercontrol.UpdateCursorTrigger();

        bool click = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);

        Vector3 original_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        original_pos.z = 0;
        Vector3 newpos = new Vector3(Mathf.Round(((original_pos.x + 0.5f) * 8) / 8) - 0.5f, Mathf.Round(((original_pos.y + 0.5f) * 8) / 8) - 0.5f, 0);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(Vector2.one);
        Vector2Int temppos = new Vector2Int(Mathf.FloorToInt(newpos.x + edgeVector.x), Mathf.FloorToInt(edgeVector.y - newpos.y));
        if (temppos.x != prevmouse.x || temppos.y != prevmouse.y)
        {
            mousecontrol = true;
            triggercontrol.gameObject.SetActive(false);
        }

        if (mousecontrol)
        {
            cursorpos = temppos;
            dot.transform.position = original_pos;
        }
        else
        {
            newpos = new Vector3(cursorpos.x - edgeVector.x + 0.5f, edgeVector.y - cursorpos.y - 0.5f, 0);
            dot.transform.position = newpos;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow))
        {
            prevmouse = temppos;
            mousecontrol = false;
            if (timeheld <= 0)
            {
                if (Input.GetKey(KeyCode.RightArrow) && cursorpos.x < (Mathf.RoundToInt(edgeVector.x) * 2) - 1)
                    cursorpos.x++;
                if (Input.GetKey(KeyCode.LeftArrow) && cursorpos.x > 0)
                    cursorpos.x--;
                if (Input.GetKey(KeyCode.UpArrow) && cursorpos.y > 0)
                    cursorpos.y--;
                if (Input.GetKey(KeyCode.DownArrow) && cursorpos.y < (Mathf.RoundToInt(edgeVector.y) * 2) - 1)
                    cursorpos.y++;
                timeheld = holdtime;
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                    timeheld = firstholdtime;
            }
            triggercontrol.gameObject.SetActive(true);
        }
        timeheld -= Time.deltaTime;

        url = genmap.ClickableLink(cursorpos);
        timesinceclick -= Time.deltaTime;
        if (click)
        {
            if (timesinceclick >= 0)
            {
                url = genmap.FirstURL();
                string generator = worldmap.GetGeneratorLinkAt(cursorpos);
                if (generator != "")
                {
                    GameObject.FindObjectOfType<UrthMap>().Flip();
                    placed = false;
                    //dot.SetActive(false);
                    worldmap.Destroy();
                    genmap.Generate(generator);
                    click = false;
                }
            }    
            timesinceclick = doubleclicktime;
        }

        if (url != "")
        {
            dot.GetComponent<SpriteRenderer>().sprite = linksprite;
            dotbackground.GetComponent<SpriteRenderer>().sprite = linksprite_back;
            if (click)
            {
                if (Application.isEditor)
                    Application.OpenURL(url);
                else
                    openWindow(url);
            }
        }
        else
        {
            dot.GetComponent<SpriteRenderer>().sprite = originaldotsprite;
            dotbackground.GetComponent<SpriteRenderer>().sprite = originaldotbackgroundsprite;
        }

        if (!placed)
        {
            transform.position = newpos;
            /*if (mousecontrol)
                transform.position = original_pos;
            else
                transform.position = newpos;*/
            triggercontrol.transform.position = newpos;
            //int MajorLocation = genmap.MajorLocation(cursorpos);
            //if (MajorLocation != -1)
            if (genmap.GetComponent<UIRegions>().CheckPosition(cursorpos))
            {
                //uiselect.gameObject.SetActive(true);
                GetComponent<Animator>().SetBool("Blinking", true);
                //uiselect.GetComponent<Animator>().SetBool("Blinking", true);
                int width = genmap.GetNameAt(cursorpos).Length + 5;
                //uiselect.size = new Vector2(width, 1);
                //uiselect.transform.localPosition = new Vector3(((float)width) / 2f + 0.5f, -(genmap.AvailableHeight() + MajorLocation + 1), 0);
                if (cursorpos.x != prevpos.x || cursorpos.y != prevpos.y)
                {
                    //selectsound.pitch = Random.Range(0.9f, 1.1f);
                    //selectsound.Play();
                    if (genmap.GetDetailAt(cursorpos) != "")
                        genmap.AddTextBox(genmap.GetDetailAt(cursorpos));
                }

                if (click)
                {
                    selectsound.pitch = Random.Range(0.9f, 1.1f);
                    selectsound.Play();
                    placed = true;
                    //dot.SetActive(true);
                }
            }
            else
            {
                //uiselect.gameObject.SetActive(false);

                if (genmap.GetNameAt(cursorpos) != "")
                {
                    if (cursorpos.x != prevpos.x || cursorpos.y != prevpos.y)
                    {
                        genmap.AddTextBox(genmap.GetDetailAt(cursorpos));
                    }
                    if (genmap.GetNameAt(cursorpos) != "Featureless Sands")
                    {
                        GetComponent<Animator>().SetBool("Blinking", true);
                        if (click)
                        {
                            selectsound.pitch = Random.Range(0.9f, 1.1f);
                            selectsound.Play();
                            placed = true;
                            //dot.SetActive(true);
                        }
                    }
                    else
                        GetComponent<Animator>().SetBool("Blinking", false);
                }
                else
                {
                    if (cursorpos.x != prevpos.x || cursorpos.y != prevpos.y)
                    {
                        if (buttoninfotext == "")
                            genmap.AddTextBox("~#f5bccf Vaarn Atlas\n\nThis Plasteel Mapping Tablet Belonged To ~#E35F8B Idris Ida-Null, ~#f5bccf Hegemony Cartographer");
                        else
                            genmap.AddTextBox(buttoninfotext);
                    }
                    GetComponent<Animator>().SetBool("Blinking", false);

                    if (click && genmap.FirstURL() != "")
                    {
                        selectsound.pitch = Random.Range(0.9f, 1.1f);
                        selectsound.Play();
                        placed = true;
                        //dot.SetActive(true);
                    }
                }
            }

            if (cursorpos.x != prevpos.x || cursorpos.y != prevpos.y)
            {
                prevpos.x = cursorpos.x;
                prevpos.y = cursorpos.y;
            }
        }
        else
        {
            //dot.transform.position = newpos;
            triggercontrol.transform.position = newpos;
            if (click && genmap.GetNameAt(cursorpos) != "")
            {
                selectsound.pitch = Random.Range(0.9f, 1.1f);
                selectsound.Play();
                //dot.SetActive(false);
                placed = false;
            }
        }
    }
}
