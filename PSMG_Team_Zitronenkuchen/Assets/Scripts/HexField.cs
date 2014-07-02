using UnityEngine;
using System.Collections;

public class HexField : MonoBehaviour {

    public int owner;
    public string specialisation;
    public int upgradLevel;
    public Material ownedMaterial;
    public int xPos;
    public int yPos;
    public GameObject[,] hexArray = new GameObject[50,50];
    public bool isFilled;
    private bool set = false;

    public GameObject[] getSurroundingFields()
    {
        GameObject[] fields = new GameObject[18];
        fillHexArray();
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
                fields[6] = hexArray[xPos - 2, yPos - 1]; //links oben
                fields[7] = hexArray[xPos - 1, yPos + 1]; //oben
                fields[8] = hexArray[xPos - 2, yPos]; //LINKS UNTEN
                fields[9] = hexArray[xPos + 1, yPos + 1]; //rechts unten
                fields[10] = hexArray[xPos + 2, yPos - 1]; //unten
                fields[11] = hexArray[xPos + 2, yPos]; //links unten
                fields[12] = hexArray[xPos + 2, yPos + 1]; //links oben
                fields[13] = hexArray[xPos + 1, yPos - 2]; //oben
                fields[14] = hexArray[xPos, yPos + 2]; //LINKS UNTEN
                fields[15] = hexArray[xPos - 1, yPos - 2]; //rechts unten
                fields[16] = hexArray[xPos - 2, yPos + 1]; //unten
                fields[17] = hexArray[xPos, yPos - 2]; //links unten
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
                fields[6] = hexArray[xPos-2, yPos - 1]; //links oben
                fields[7] = hexArray[xPos - 1, yPos-1]; //oben
                fields[8] = hexArray[xPos - 2, yPos]; //LINKS UNTEN
                fields[9] = hexArray[xPos+1, yPos - 1]; //rechts unten
                fields[10] = hexArray[xPos + 2, yPos-1]; //unten
                fields[11] = hexArray[xPos + 2, yPos]; //links unten
                fields[12] = hexArray[xPos+2, yPos + 1]; //links oben
                fields[13] = hexArray[xPos + 1, yPos+2]; //oben
                fields[14] = hexArray[xPos, yPos + 2]; //LINKS UNTEN
                fields[15] = hexArray[xPos-1, yPos + 2]; //rechts unten
                fields[16] = hexArray[xPos - 2, yPos+1]; //unten
                fields[17] = hexArray[xPos, yPos-2]; //links unten
                
            }
            
        }
        Debug.Log("fields:" + fields);
        return fields;
    }

    
    private void fillHexArray()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("hex");
        foreach (GameObject obj in gameObjects){
            HexField hex = obj.GetComponent<HexField>();
            hexArray[hex.xPos, hex.yPos] = obj;
        }
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

    [RPC]
    void buildMilitary(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject milBuilding = Resources.Load("military-building2", typeof(GameObject)) as GameObject;
        GameObject militaryBuilding = Network.Instantiate(milBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f), 0) as GameObject;
        selectedHexagon.renderer.material = Resources.Load("militaryMaterial", typeof(Material)) as Material;
        militaryBuilding.transform.parent = selectedHexagon.transform;
        GameObject unitText = new GameObject();
        TextMesh text = unitText.AddComponent<TextMesh>();
        text.characterSize = 0.1f;
        Font font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        text.font = font;
        text.renderer.material = font.material;
        text.anchor = TextAnchor.MiddleCenter;
        unitText.transform.parent = selectedHexagon.transform;
        unitText.transform.position = selectedHexagon.transform.position;
        unitText.transform.Rotate(new Vector3(45, 0, 0));
    }

    [RPC]
    void buildResearch(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject resBuilding = Resources.Load("research-building2", typeof(GameObject)) as GameObject;
        GameObject researchBuilding = Instantiate(resBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        selectedHexagon.renderer.material = Resources.Load("researchMaterial", typeof(Material)) as Material;
        researchBuilding.transform.parent = selectedHexagon.transform;
    }

    [RPC]
    void buildEconomy(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject ecoBuilding = Resources.Load("economy-building2", typeof(GameObject)) as GameObject;
        GameObject economyBuilding = Instantiate(ecoBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject; ;
        selectedHexagon.renderer.material = Resources.Load("economyMaterial", typeof(Material)) as Material;
        economyBuilding.transform.parent = selectedHexagon.transform;
    }

    [RPC]
    void updateTroops(NetworkViewID id, int troops)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        hex.transform.GetComponentInChildren<TextMesh>().text = "" + troops;
    }

    [RPC]
    void addPos(NetworkViewID id, int x, int y)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        hex.GetComponent<HexField>().xPos = x;
        hex.GetComponent<HexField>().yPos = y;
    }

    
}
