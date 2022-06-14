using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkovChain : MonoBehaviour
{
    public string textinput;

    private List<char> init_char;
    private List<char> a_char;
    private List<char> b_char;
    private bool next_is_init;
    private bool trained;

    private void Start()
    {
        trained = false;
    }

    private void Train()
    {
        init_char = new List<char>();
        a_char = new List<char>();
        b_char = new List<char>();
        next_is_init = true;
        for (int i = 0; i < textinput.Length - 1; i++)
        {
            if (next_is_init)
            {
                init_char.Add(textinput[i]);
                next_is_init = false;
            }
            a_char.Add(textinput[i]);
            b_char.Add(textinput[i + 1]);
            if (textinput[i + 1] == ' ')
            {
                i++;
                next_is_init = true;
            }
        }
        trained = true;
    }

    public string NewWord(int sizelimit)
    {
        if (!trained)
            Train();

        string newstring = init_char[Random.Range(0, init_char.Count)].ToString();
        bool wordend = false;
        sizelimit--;
        while (!wordend && sizelimit > 0)
        {
            char nextchar = RandomNextChar(newstring[newstring.Length - 1]);
            if (nextchar == ' ')
                wordend = true;
            else
                newstring += nextchar;
            sizelimit--;
        }
        return newstring;
    }

    public char RandomNextChar(char c)
    {
        List<char> possibilities = new List<char>();
        for(int i = 0; i < a_char.Count; i++)
        {
            if (a_char[i] == c)
                possibilities.Add(b_char[i]);
        }
        return possibilities[Random.Range(0, possibilities.Count)];
    }
}
