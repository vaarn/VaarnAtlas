using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    private double waittime;

    // Update is called once per frame
    void Update()
    {
        if (waittime > 0)
            waittime -= Time.deltaTime;
        if ((Input.GetMouseButtonDown(0) || Input.anyKeyDown) && waittime <= 0)
        {
            this.enabled = false;
            this.gameObject.SetActive(false);
        }
    }

    public void Activate()
    {
        waittime = 0.5;
    }
}
