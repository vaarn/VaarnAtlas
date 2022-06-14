using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessageBtn : MonoBehaviour
{
    public GameObject sendto;
    public CursorScript cursorscript;
    public string send;
    public string message;

    public void OnMouseDown()
    {
        if (send != "" && message != "")
        {
            sendto.SendMessage(send, SendMessageOptions.RequireReceiver);
            cursorscript.UnPlace();
        }
    }

    public void OnMouseEnter()
    {
        if (send != "" && message != "")
            cursorscript.SetButtonText(message);
    }

    public void OnMouseExit()
    {
        if (send != "" && message != "")
            cursorscript.SetButtonText("");
    }
}
