using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTriggerControl : MonoBehaviour
{
    private GameObject touching;
    private bool enter;
    private bool exit;

    private void Update()
    {
        if (touching == null)
        {
            enter = false;
            exit = false;
        }
    }

    public void UpdateCursorTrigger()
    {
        if (touching)
        {
            if (enter)
            {
                if (touching != null && touching.activeInHierarchy)
                    touching.SendMessage("OnMouseEnter", SendMessageOptions.RequireReceiver);
                enter = false;
            }

            if (exit)
            {
                if (touching != null && touching.activeInHierarchy)
                    touching.SendMessage("OnMouseExit", SendMessageOptions.RequireReceiver);
                exit = false;
                touching = null;
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (touching != null && touching.activeInHierarchy)
                    touching.SendMessage("OnMouseDown", SendMessageOptions.RequireReceiver);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (touching)
        {
            exit = true;
            UpdateCursorTrigger();
        }
        touching = collision.gameObject;
        enter = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        exit = true;
    }
}
