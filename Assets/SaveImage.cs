using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SaveImage : MonoBehaviour
{
    public AudioSource soundeffect;
    public GenerateMap genmap;
    public CursorScript cursorscript;

    public List<GameObject> hideall;
    private List<bool> wasactive;

    private double takeshot;


    [DllImport("__Internal")]
    public static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    private void Start()
    {
        takeshot = 0;
    }

    private void Update()
    {
        if (takeshot > 0)
            takeshot -= Time.deltaTime;
        if (takeshot < 0)
        {
            int i = 0;
            foreach (GameObject obj in hideall)
            {
                obj.SetActive(wasactive[i]);
                i++;
            }
            takeshot = 0;
        }
    }

    public void OnMouseDown()
    {
        soundeffect.Play();
        wasactive = new List<bool>();
        foreach(GameObject obj in hideall)
        {
            wasactive.Add(obj.activeInHierarchy);
            obj.SetActive(false);
        }
        takeshot = 1;
        StartCoroutine(UploadPNG());
        //ScreenCapture.CaptureScreenshot("ScreenshotsVaultsOfVaarn_Seed" + genmap.ShareSeed().ToString() + ".png");
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf Click To Save A Screenshot.");
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }

    IEnumerator UploadPNG()
    {
        // We should only read the screen after all rendering is complete
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

#if !UNITY_EDITOR
        DownloadFile(bytes, bytes.Length, "VaultsOfVaarnMap_Seed" + genmap.ShareSeed().ToString() + ".png");
#else
        ScreenCapture.CaptureScreenshot("VaultsOfVaarnMap_Seed" + genmap.ShareSeed().ToString() + ".png");
#endif
    }
}
