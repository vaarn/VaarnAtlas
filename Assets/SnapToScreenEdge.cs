using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToScreenEdge : MonoBehaviour
{
    public Vector2Int lockpos;
    public Vector2 offset;

    private void Update()
    {
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(Vector2.one);
        float width = edgeVector.x;
        float height = edgeVector.y;

        float x = (width * lockpos.x) + offset.x;
        x = (Mathf.Round(x * 10)) / 10f;
        float y = (height * lockpos.y) + offset.y;
        y = (Mathf.Round(y * 10)) / 10f;
        transform.position = new Vector3(x, y, transform.position.z);

        //Debug.Log("Char width = " + width.ToString());
        //Debug.Log("Char height = " + height.ToString());
    }
}
