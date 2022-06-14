using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class SaveTextButton : MonoBehaviour
{
    public GenerateMap generate;
    public AudioSource soundeffect;
    public SpriteFromText spritefromtext;
    public CursorScript cursorscript;

    [DllImport("__Internal")]
    public static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    public void OnMouseDown()
    {
        soundeffect.Play();
        int width = generate.Width();
        int height = generate.AvailableHeight();

        string text = "Vaarn Atlas\nBy Jacob Marks\nAll Vaults Of Vaarn Content By Leo Hunt\nVaults of Vaarn is a Creative Commons Attribution 4.0 Licensed product\nMap url = " + generate.ShareURL() + "\n";

        Vector2Int current_pos = Vector2Int.zero;
        for (int y = 0; y < height; y++)
        {
            current_pos.y = y;
            string line = "";
            for (int x = 0; x < width; x++)
            {
                current_pos.x = x;
                bool foundchar = false;
                if (!char.IsWhiteSpace(generate.MapChars()[x, y]))
                {
                    foundchar = true;
                    line += generate.MapChars()[x, y];
                }
                if (!foundchar)
                    line += " ";
            }
            text += line + "\n";
        }

        text += "Locations:\n\n";

        int index = 1;
        foreach (string detail in generate.AllMajorDetails())
        {
            string[] split = detail.Split('~');
            string newdetail = "";
            int i = 0;
            foreach (string s in split)
            {
                if (s.Length > 0)
                {
                    string cutoff = s.Substring(8);
                    newdetail += cutoff.Replace("\n", "");
                    if (i % 2 == 1)
                        newdetail += "\n";
                }
                i++;
            }
            if (newdetail[newdetail.Length - 1] == '\n')
                newdetail = newdetail.Substring(0, newdetail.Length - 1);
            string location_type = newdetail.Split(' ')[1];
            text += index.ToString() + " " + location_type.ToUpper()[0] + " " + newdetail + "\n\n";
            index++;
        }

        text += "Routes:\n\n";
        foreach (string detail in generate.RouteDetails())
        {
            string[] split = detail.Split('~');
            string newdetail = "";
            int i = 0;
            foreach (string s in split)
            {
                if (s.Length > 0)
                {
                    string cutoff = s.Substring(8);
                    newdetail += cutoff.Replace("toreplace", "");
                    if (newdetail[newdetail.Length-1] == '\n')
                        newdetail = newdetail.Substring(0, newdetail.Length - 1);
                    if (!s.Contains(":") && i >= 4)
                        newdetail += "\n";
                    i++;
                }
            }
            if (newdetail[newdetail.Length - 1] == '\n')
                newdetail = newdetail.Substring(0, newdetail.Length - 1);
            text += newdetail + "\n\n";
        }

#if !UNITY_EDITOR
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        DownloadFile(bytes, bytes.Length, "VaultsOfVaarnMap_Seed" + generate.ShareSeed().ToString() + ".txt");
#else
        Debug.Log(Application.persistentDataPath + "/VaultsOfVaarnMap_Seed" + generate.ShareSeed().ToString() + ".txt");
        File.WriteAllLines(Application.persistentDataPath + "/VaultsOfVaarnMap_Seed" + generate.ShareSeed().ToString() + ".txt",
            text.Split('\n'), Encoding.UTF8);
#endif
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf Click To Save A Textfile With Most Of The Map Information.");
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
