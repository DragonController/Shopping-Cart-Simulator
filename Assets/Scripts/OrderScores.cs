using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderScores : MonoBehaviour
{
    void Start()
    {
        // OrderScore orderScore = new OrderScore();
        // orderScore.itemCount = 6;
        // orderScore.mode = 1;
        // orderScore.time = 12.3;

        // string json = JsonUtility.ToJson(orderScore);
        // System.IO.File.WriteAllText(Application.persistentDataPath + "/OrderScores.json", json);
    }
}

[Serializable]
public class OrderScore
{
    public int itemCount;
    public int mode;
    public float time;
}
