using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerValuesManager : PlayerValues
{
    public void SetPoints(float value)
    {
        points = value;
    }

    public float GetPoints()
    {
        return points;
    }

    public void PlusPoints(float value)
    {
        points += value;
        Debug.Log("Player points: " + points);
    }
}
