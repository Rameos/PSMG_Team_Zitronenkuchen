using UnityEngine;
using System.Collections;

public class HexField : MonoBehaviour {

    public int owner;
    public string specialisation;
    public int upgradLevel;

    public GameObject[] getSurroundingFields(int row, int col, GameObject[,] allFields)
    {
        GameObject[] fields = new GameObject[6];

        if (row != 0 && col != 0)
        {
            fields[0] = allFields[row, col - 1]; //links oben
            fields[1] = allFields[row - 1, col]; //oben
            fields[2] = allFields[row - 1, col + 1]; //rechts oben
            fields[3] = allFields[row, col + 1]; //rechts unten
            fields[4] = allFields[row + 1, col]; //unten
            fields[5] = allFields[row -1, col + 1]; //links unten
        }
        Debug.Log("fields:" + fields);
        return fields;
    }
}
