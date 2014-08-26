using UnityEngine;
using System.Collections;

public class HexField : MonoBehaviour {

    public int owner;
    public string specialisation;
    //public int upgradLevel; not needed yet
    public Material ownedMaterial;
    public Material defaultMaterial;
    public int xPos;
    public int yPos;
    public GameObject[,] hexArray = new GameObject[50,50];
    public bool isFilled;
    private bool set = false;
    public Specialisation spec;
    private bool inRange;

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
        { // left edge
            bottomLimit = true;
            startI = (-1) * xPos;
        }
        if (yPos <= 1)
        { // lower edge
            bottomLimit = true;
            startJ = (-1) * yPos;
        }
        if (xPos >= 48)
        { // right edge
            endI = 49 - xPos;
        }
        if (yPos >= 48)
        { // upper edge
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
        { // even row
            // Debug.Log("even");
            if (xPos != 0){
                list.Remove(hexArray[xPos - 1, yPos + 2]);
            }
            list.Remove(hexArray[xPos + 1, yPos + 2]);
        }
        else if (!bottomLimit)
        { // odd row
            // Debug.Log("odd");
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


    public bool InRange
    {
        get
        {
            return inRange;
        }
        set
        {
            inRange = value;
        }
    }

    private void fillHexArray()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("hex");
        foreach (GameObject obj in gameObjects){
            HexField hex = obj.GetComponent<HexField>();
            hexArray[hex.xPos, hex.yPos] = obj;
        }
    }

    public void colorOwnedArea()
    {
        ownedMaterial = Resources.Load("OwnedMaterial", typeof(Material)) as Material;
        if (ownedMaterial == null)
        {
            Debug.Log("loading failed, check existence of Resources folder in Assets");
        }
        //if (!isFilled)
        //{
            gameObject.renderer.material = ownedMaterial;
        //}       
    }

    public void decolorUnownedArea()
    {
        defaultMaterial = Resources.Load("DefaultMaterial", typeof(Material)) as Material;
        if (defaultMaterial == null)
        {
            Debug.Log("loading failed, check existence of Resources folder in Assets");
        }
        if (true)
        {
            gameObject.renderer.material = defaultMaterial;
            Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Debug.Log(Network.isServer);
            Debug.Log(gameObject);
        } 
    }

    [RPC]
    void buildBase(NetworkViewID id, int selectedRace)
    {
        Debug.Log("Build base for Race " + selectedRace);
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject milBuilding = Resources.Load("baseECO", typeof(GameObject)) as GameObject;
        GameObject militaryBuilding = Network.Instantiate(milBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f), 0) as GameObject;
        //selectedHexagon.renderer.material = Resources.Load("baseMaterial", typeof(Material)) as Material;
        GameObject baseBuilding = null;
        if (selectedRace == 1)
        {
            baseBuilding = Resources.Load("baseMIL", typeof(GameObject)) as GameObject;
        }
        else
        {
            baseBuilding = Resources.Load("base-building", typeof(GameObject)) as GameObject;
        } 
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
    void buildMilitary(NetworkViewID id, int selectedRace)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject milBuildingState1 = null;
        GameObject milBuildingState2 = null;
        GameObject milBuildingState3 = null;
        if (selectedRace == 1)
        {
            milBuildingState1 = Resources.Load("militaryMILState1", typeof(GameObject)) as GameObject;
            milBuildingState2 = Resources.Load("militaryMILState2", typeof(GameObject)) as GameObject;
            milBuildingState3 = Resources.Load("militaryMILState3", typeof(GameObject)) as GameObject;
        }
        else
        {
            milBuildingState1 = Resources.Load("militaryECOState1", typeof(GameObject)) as GameObject;
            milBuildingState2 = Resources.Load("militaryECOState2", typeof(GameObject)) as GameObject; 
            milBuildingState3 = Resources.Load("militaryECOState3", typeof(GameObject)) as GameObject;
        }       
        GameObject militaryBuildingState1 = Instantiate(milBuildingState1, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        GameObject militaryBuildingState2 = Instantiate(milBuildingState2, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        GameObject militaryBuildingState3 = Instantiate(milBuildingState3, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        //selectedHexagon.renderer.material = Resources.Load("militaryMaterial", typeof(Material)) as Material;
        militaryBuildingState1.transform.parent = selectedHexagon.transform;
        militaryBuildingState2.transform.parent = selectedHexagon.transform;
        militaryBuildingState3.transform.parent = selectedHexagon.transform;
        militaryBuildingState2.SetActive(false);
        militaryBuildingState3.SetActive(false);
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

    /*[RPC]
    void buildResearch(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject resBuilding = null;
        if (selectedRace == 1)
        {
            resBuilding = Resources.Load("economyMIL", typeof(GameObject)) as GameObject;
        }
        else
        {
            resBuilding = Resources.Load("economyECONOMY", typeof(GameObject)) as GameObject;
        }
        GameObject researchBuilding = Instantiate(resBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        //selectedHexagon.renderer.material = Resources.Load("researchMaterial", typeof(Material)) as Material;
        researchBuilding.transform.parent = selectedHexagon.transform;
    }*/

    [RPC]
    void buildEconomy(NetworkViewID id, int selectedRace)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject ecoBuildingState1 = null;
        GameObject ecoBuildingState2 = null;
        GameObject ecoBuildingState3 = null;
        if (selectedRace == 1)
        {
            ecoBuildingState1 = Resources.Load("economyMILState1", typeof(GameObject)) as GameObject;
            ecoBuildingState2 = Resources.Load("economyMILState2", typeof(GameObject)) as GameObject;
            ecoBuildingState3 = Resources.Load("economyMILState3", typeof(GameObject)) as GameObject;
        }
        else
        {
            ecoBuildingState1 = Resources.Load("economyECOState1", typeof(GameObject)) as GameObject;
            ecoBuildingState2 = Resources.Load("economyECOState2", typeof(GameObject)) as GameObject;
            ecoBuildingState3 = Resources.Load("economyECOState3", typeof(GameObject)) as GameObject;
        }
        GameObject economyBuildingState1 = Instantiate(ecoBuildingState1, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        GameObject economyBuildingState2 = Instantiate(ecoBuildingState2, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        GameObject economyBuildingState3 = Instantiate(ecoBuildingState3, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        //selectedHexagon.renderer.material = Resources.Load("economyMaterial", typeof(Material)) as Material;
        economyBuildingState1.transform.parent = selectedHexagon.transform;
        economyBuildingState2.transform.parent = selectedHexagon.transform;
        economyBuildingState3.transform.parent = selectedHexagon.transform;
        economyBuildingState2.SetActive(false);
        economyBuildingState3.SetActive(false);
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
        StartCoroutine(SuccessfulAttack(id, survivingTroops, pos, win));
    }

    private IEnumerator SuccessfulAttack(NetworkViewID id, int survivingTroops, Vector3 pos, bool win)
    {
        yield return new WaitForSeconds(3);
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>().attackSuccess(hex, survivingTroops, pos, win);
    }

    
    [RPC]
    void explobumm(NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        foreach (Transform child in hex.transform)
        {
            Object.Destroy(child.gameObject);
            //TODO: explobumm(Partikeleffekt)
        }
    }

    [RPC]
    void setOwner(NetworkViewID id, int owner)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        hex.GetComponent<HexField>().owner = owner;
    }

    [RPC]
    void toggleVisibility(NetworkViewID id, int state, string alienRaceBuilding)
    {
        Debug.Log(gameObject.name + "I am trying to toggle");
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        Transform state1 = null;
        Transform state2 = null;
        Transform state3 = null;
        foreach (Transform child in hex.transform)
        {
            Debug.Log("child name:"+ child.name);
            if (child.name == alienRaceBuilding + "State1(Clone)")
            {
                Debug.Log("child name:" + child.name);
                state1 = child;
            }
            if (child.name == alienRaceBuilding + "State2(Clone)")
            {
                state2 = child;
            }
            if (child.name == alienRaceBuilding + "State3(Clone)")
            {
                state3 = child;
            }     
        }
        switch (state)
        {
            case 1:
                state2.gameObject.SetActive(false);
                state3.gameObject.SetActive(false);
                break;
            case 2:
                state2.gameObject.SetActive(true);
                state1.gameObject.SetActive(false);
                state3.gameObject.SetActive(false);
                break;
            case 3:
                state3.gameObject.SetActive(true);
                state1.gameObject.SetActive(false);
                state2.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    [RPC]
    void destroyBuilding(NetworkViewID id, string alienRace)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        //hex.GetComponent<HexField>().owner = 0;
        //hex.GetComponent<HexField>().specialisation = null;
        foreach (Transform child in hex.transform)
        {
            if (child.name != alienRace+"State3(Clone)" && child.name != "New Game Object")
            {
                Object.Destroy(child.gameObject);
            }
        }
        
    }
    
}
