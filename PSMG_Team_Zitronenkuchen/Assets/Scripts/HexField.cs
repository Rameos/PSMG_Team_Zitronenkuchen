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
    public Specialisation spec;

    public ArrayList getSurroundingFields()
    {
        ArrayList list = new ArrayList();
        
        fillHexArray();
        int startI = -2;
        int startJ = -2;
        int endI = 2;
        int endJ = 2;
        bool bottomLimit = false;
        if (xPos <= 1)
        {
            bottomLimit = true;
            startI = (-1) * xPos;
        }
        if (yPos <= 1)
        {
            bottomLimit = true;
            startJ = (-1) * yPos;
        }
        if (xPos >= 48)
        {
            endI = 49 - xPos;
        }
        if (yPos >= 48)
        {
            endJ = 49 - yPos;
        }
        for (int i = startI; i <= endI; i++)
        {
            for (int j = startJ; j <= endJ; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) != 4)
                {
                    list.Add(hexArray[xPos + i, yPos + j]);
                }
            }
        }
        if (xPos % 2 == 0 && yPos <= 47)
        {
            Debug.Log("even");
            if (xPos != 0){
                list.Remove(hexArray[xPos - 1, yPos + 2]);
            }
            list.Remove(hexArray[xPos + 1, yPos + 2]);
        }
        else if (!bottomLimit)
        {
            Debug.Log("odd");
            if (xPos != 0)
            {
                list.Remove(hexArray[xPos - 1, yPos - 2]);
            }
            if (xPos != 49)
            {
                list.Remove(hexArray[xPos + 1, yPos - 2]);
            }
        }
        return list;
    }

    public Vector3 getPos(){
        return new Vector3(xPos, yPos);
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
    void buildBase(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject milBuilding = Resources.Load("base-building", typeof(GameObject)) as GameObject;
        GameObject militaryBuilding = Network.Instantiate(milBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f), 0) as GameObject;
        selectedHexagon.renderer.material = Resources.Load("baseMaterial", typeof(Material)) as Material;
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
    void buildMilitary(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject milBuilding = Resources.Load("militaryECONOMY", typeof(GameObject)) as GameObject;
        GameObject militaryBuilding = Network.Instantiate(milBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f), 0) as GameObject;
        //selectedHexagon.renderer.material = Resources.Load("militaryMaterial", typeof(Material)) as Material;
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
        GameObject resBuilding = Resources.Load("economyECONOMY", typeof(GameObject)) as GameObject;
        GameObject researchBuilding = Instantiate(resBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        //selectedHexagon.renderer.material = Resources.Load("researchMaterial", typeof(Material)) as Material;
        researchBuilding.transform.parent = selectedHexagon.transform;
    }

    [RPC]
    void buildEconomy(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject ecoBuilding = Resources.Load("economyECONOMY", typeof(GameObject)) as GameObject;
        GameObject economyBuilding = Instantiate(ecoBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject; ;
        //selectedHexagon.renderer.material = Resources.Load("economyMaterial", typeof(Material)) as Material;
        economyBuilding.transform.parent = selectedHexagon.transform;
    }

    [RPC]
    void updateTroops(NetworkViewID id, int troops)
    {
        /*NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        hex.transform.GetComponentInChildren<TextMesh>().text = "" + troops;*/
    }

    [RPC]
    void addPos(NetworkViewID id, int x, int y)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        hex.GetComponent<HexField>().xPos = x;
        hex.GetComponent<HexField>().yPos = y;
    }

    [RPC]
    void showTroops(int troops)
    {
        gameObject.transform.GetComponentInChildren<TextMesh>().text = "" + troops;
    }

    [RPC]
    void setSpecialisation(string type)
    {
        specialisation = type;
    }

    [RPC]
    void processAttack(NetworkViewID id, int sendingTroops)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>().receiveAttack(hex, sendingTroops);
    }

    [RPC]
    void successfulAttack(NetworkViewID id, int survivingTroops, Vector3 pos, bool win)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>().attackSuccess(hex, survivingTroops, pos, win);
    }


    
}
