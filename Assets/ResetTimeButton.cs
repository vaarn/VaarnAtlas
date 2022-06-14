using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTimeButton : MonoBehaviour
{
    public GenerateKronos kronos;
    public GenerateMap generatemap;
    public AudioSource soundeffect;

    public void OnMouseDown()
    {
        kronos.DestroyKronos();
        generatemap.GetComponent<GenerateGnomon>().Destroy();
        generatemap.GenerateRandomize();
        soundeffect.Play();
    }
}
