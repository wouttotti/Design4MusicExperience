using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDegreesFromPoint : MonoBehaviour {

    private void Start()
    {
        Debug.Log(FindDegree(0, 1));
    }

    public static float FindDegree(int x, int y)
    {
        float value = (float)((Mathf.Atan2(x, y) / Mathf.PI) * 180f);
        if (value < 0) value += 360f;

        return value;
    }
}
