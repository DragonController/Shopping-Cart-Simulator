using System;
using UnityEngine;

public class OrderScores : MonoBehaviour
{
}

[Serializable]
public class OrderScore
{
    public int itemCount;
    public int mode;
    public float time;
}
