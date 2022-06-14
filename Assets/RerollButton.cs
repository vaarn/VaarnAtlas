using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RerollButton : MonoBehaviour
{
    public GenerateMap generate;
    public List<Sprite> randomdice;
    public SpriteRenderer dicerenderer;
    public CursorScript cursorscript;
    private ButtonAnimation btnanimation;

    // Start is called before the first frame update
    void Start()
    {
        btnanimation = GetComponent<ButtonAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnMouseDown();
            btnanimation.OnMouseDown();
        }
    }

    public void OnMouseDown()
    {
        dicerenderer.sprite = randomdice[Random.Range(0, randomdice.Count)];
        generate.GenerateRandomize();
    }

    public void OnMouseEnter()
    {
        cursorscript.SetButtonText("~#f5bccf Click To Randomize This Map.");
    }

    public void OnMouseExit()
    {
        cursorscript.SetButtonText("");
    }
}
