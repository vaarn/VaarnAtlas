using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRegions : MonoBehaviour
{
    public SpriteRenderer selector;

    private List<Vector2Int> positions;
    private List<Vector2Int> toplefts;
    private List<Vector2Int> bottomrights;

    public void ResetRegions()
    {
        positions = new List<Vector2Int>();
        toplefts = new List<Vector2Int>();
        bottomrights = new List<Vector2Int>();
    }

    public void AddArea(Vector2Int position, Vector2Int topleft, Vector2Int bottomright)
    {
        positions.Add(position);
        toplefts.Add(topleft);
        bottomrights.Add(bottomright);
    }

    public bool CheckPosition(Vector2Int position)
    {
        bool found = false;
        int i = 0;
        foreach (Vector2Int pos in positions)
        {
            if (pos.x == position.x && pos.y == position.y)
            {
                found = true;
                selector.gameObject.SetActive(true);
                selector.GetComponent<Animator>().SetBool("Blinking", true);
                float width = Mathf.Abs(bottomrights[i].x - toplefts[i].x) + 1;
                float height = Mathf.Abs(bottomrights[i].y - toplefts[i].y) + 1;
                selector.transform.localPosition = new Vector3(toplefts[i].x + (width / 2f) - 0.5f, toplefts[i].y - (height / 2f) + 0.5f);
                selector.size = new Vector2(width, height);
            }
            i++;
        }
        if (!found)
        {
            selector.gameObject.SetActive(false);
        }
        return found;
    }
}
