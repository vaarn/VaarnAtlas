using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ShareURLButton : MonoBehaviour
{
    public GenerateMap generate;
    public SpriteFromText spritefromtext;

    public AudioSource copysound;

    public CursorScript cursorscript;

    private string url;
    private string seed;

    private BoxCollider2D boxcollider;
    private ButtonAnimation btnanimation;

    [DllImport("__Internal")]
    private static extern void CopyToClipboard(string text);

    // Start is called before the first frame update
    void Start()
    {
        boxcollider = GetComponent<BoxCollider2D>();
        btnanimation = GetComponent<ButtonAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (generate.ShareURL() != url)
        {
            UpdatedURL();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            OnMouseDown();
            btnanimation.OnMouseDown();
        }
    }

    private void UpdatedURL()
    {
        UpdatedURL("→Copy Link To Clipboard:");
    }

    private void UpdatedURL(string buttontext)
    {
        url = generate.ShareURL();
        seed = buttontext + generate.ShareSeed().ToString();
        if (url.Length > 0)
        {
            //Destroy previous children
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);

            btnanimation.backgrounds = new List<SpriteRenderer>();
            btnanimation.foregrounds = new List<SpriteRenderer>();

            int x = 0;
            for (int i = seed.Length - 1; i >= 0; i--)
            {
                GameObject fg = new GameObject();
                GameObject bg = new GameObject();
                bg.name = "background";
                bg.transform.SetParent(fg.transform);
                generate.SetSprite(bg, generate.filledsprite, btnanimation.bg_col, 3);
                btnanimation.backgrounds.Add(bg.GetComponent<SpriteRenderer>());

                fg.name = seed[i].ToString();
                fg.transform.SetParent(transform);
                fg.transform.localPosition = new Vector3(x, 0, 0);
                generate.SetSprite(fg, spritefromtext.SpriteFromChar(seed[i]), btnanimation.fg_col, 4);
                btnanimation.foregrounds.Add(fg.GetComponent<SpriteRenderer>());

                x--;
            }

            boxcollider.size = new Vector2(seed.Length, 1);
            boxcollider.offset = new Vector2(-((float)seed.Length/2f) + 0.5f, 0);
        }
    }

    public void OnMouseDown()
    {
#if UNITY_WEBGL && UNITY_EDITOR == false
        CopyToClipboard(url);
#else
        GUIUtility.systemCopyBuffer = url;
#endif
        UpdatedURL("→Link Copied!:");
        copysound.pitch = Random.Range(0.9f, 1.1f);
        copysound.Play();
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf Click To Copy A Link To This Map To Your Clipboard. Share The Link To Share This Map.");
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
