using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public Vector2Int from;
    public Vector2Int to;

    public List<Vector2Int> jumped = new List<Vector2Int>(0);

    public Move(Vector2Int _from, Vector2Int _to)
    {
        from = _from;
        to = _to;
    }

    public Move(Vector2Int _from, Vector2Int _to, List<Vector2Int> _jumped)
    {
        from = _from;
        to = _to;
        jumped = _jumped;
    }

    public void DebugMove()
    {
        Debug.Log("From " + from.ToString() + " to " + to.ToString() + " jumping " + jumped.Count + " pieces");
    }
}