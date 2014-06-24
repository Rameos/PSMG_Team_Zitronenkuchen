using UnityEngine;
using System.Collections;

public class HexField : MonoBehaviour {

    public int owner;
    public string specialisation;
    public int upgradLevel;
    public Material ownedMaterial;
    public int xPos;
    public int yPos;
    public GameObject[,] hexArray;
    public bool isFilled;

    public GameObject[] getSurroundingFields()
    {
        GameObject[] fields = new GameObject[6];

        if (xPos != 0 && yPos != 0)
        {
            if (xPos % 2 == 0)
            {
                Debug.Log("even");
                fields[0] = hexArray[xPos, yPos - 1]; //links oben
                fields[1] = hexArray[xPos - 1, yPos]; //oben
                fields[2] = hexArray[xPos - 1, yPos - 1]; //LINKS UNTEN
                fields[3] = hexArray[xPos, yPos + 1]; //rechts unten
                fields[4] = hexArray[xPos + 1, yPos]; //unten
                fields[5] = hexArray[xPos + 1, yPos - 1]; //links unten
            }
            else
            {
                Debug.Log("odd"); 
                fields[0] = hexArray[xPos, yPos - 1]; //links oben
                fields[1] = hexArray[xPos - 1, yPos]; //oben
                fields[2] = hexArray[xPos - 1, yPos + 1]; //rechts oben
                fields[3] = hexArray[xPos, yPos + 1]; //rechts unten
                fields[4] = hexArray[xPos + 1, yPos]; //unten
                fields[5] = hexArray[xPos + 1, yPos + 1]; //links unten
                
            }
            
        }
        Debug.Log("fields:" + fields);
        return fields;
    }

    public void colorOwnedArea(GameObject obj)
    {
        ownedMaterial = Resources.Load("OwnedMaterial", typeof(Material)) as Material;
        if (ownedMaterial == null)
        {
            Debug.Log("loading failed, check existence of Resources folder in Assets");
        }
        if (!isFilled)
        {
            obj.renderer.material = ownedMaterial;
        }       
    }
}
