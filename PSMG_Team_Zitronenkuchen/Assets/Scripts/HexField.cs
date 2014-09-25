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
    private bool finishedBuilding = false;
    private int troopsOnField = 0;

    private GameObject spaceship;
    private GameObject tempSpaceship;
    private GameObject destinationHex;
    private Vector3 direction;
    private Quaternion spaceshipQuaternion;
    GameObject spaceshipOrig;
    bool destinationNeedsShip = true;
    public AudioClip spaceShipRising = Resources.Load("UFO2", typeof(AudioClip)) as AudioClip;

    private bool isMovedOnField;
    private float startTime;
    private float distance;
    private Vector3 startPos;
    private Vector3 desiredPos;

    private float sendingStartTime;
    private float sendingDist;
    private Vector3 hexStartPos;
    private Vector3 hexDestPos;

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



    public bool FinishedBuilding
    {
        get
        {
            return finishedBuilding;
        }
        set
        {
            finishedBuilding = value;
        }
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
        defaultMaterial = Resources.Load("HexLava", typeof(Material)) as Material;
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
    void buildBase(NetworkViewID id, int selectedRace, int callingOwner)
    {
        
        ArrayList highlights = new ArrayList();
        string resource = "";
        ArrayList highlights2 = new ArrayList();
        string resource2 = "";
        Debug.Log("Build base for Race " + selectedRace);
        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject baseBuilding = null;
        if (selectedRace == 1)
        {
            baseBuilding = Resources.Load("baseMIL", typeof(GameObject)) as GameObject;
            highlights.Add(1);
            resource = "baseMIL(Clone)/Cylinder";
        }
        else
        {
            baseBuilding = Resources.Load("baseECO", typeof(GameObject)) as GameObject;
            highlights.Add(2);
            highlights.Add(3);
            highlights.Add(4);
            resource = "baseECO(Clone)/Silo";
            //spaceship
            highlights2.Add(1);
            highlights2.Add(2);
            resource2 = "baseECO(Clone)/Spaceship";
        }
        GameObject basicBuilding = Instantiate(baseBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
        selectedHexagon.GetComponent<HexField>().owner = callingOwner;
        FinishedBuilding = true;
        basicBuilding.transform.parent = selectedHexagon.transform;
        setColor(resource, highlights);
        setColor(resource2, highlights2);
        highlights.Clear();
        highlights2.Clear();
    }

    private void setColor(string resource, ArrayList highlights)
    {
        Material highlightServer = Resources.Load("Materials/Red", typeof(Material)) as Material;
        Material highlightClient = Resources.Load("Materials/Blue", typeof(Material)) as Material;
        Transform element = transform.Find(resource);

        // element can be null because temporary spaceships are not children of hexfields
        if (element == null)
        {
            if (CustomGameProperties.alienRace == 1)
            {
                element = tempSpaceship.transform.FindChild("Sphere_001").gameObject.transform;
            }
            else
            {
                element = tempSpaceship.transform;
            }
            
        }

        Material[] mats;
        mats = element.renderer.materials;
        foreach (int num in highlights)
        {
            if (owner == 1)
            {
                mats[num] = highlightServer;
            }
            else mats[num] = highlightClient;
        }
        element.renderer.materials = mats;
    }

    [RPC]
    public void initiateTroopBuilding(int selectedRace, NetworkViewID id)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hexagon = view.gameObject;

        spaceshipQuaternion = (Network.isServer) ? Quaternion.Euler (0.0f, 0.0f, 0.0f) : Quaternion.Euler(0.0f, 180.0f, 0.0f);

        bool alreadyBuilt = false;
        foreach (Transform child in hexagon.transform)
        {
            if (child.name == "spaceshipECO(Clone)" || child.name == "spaceshipMIL(Clone)")
                alreadyBuilt = true;
        }

        if (!alreadyBuilt) {
            string resource = "";
            ArrayList highlights = new ArrayList();
            
            spaceshipOrig = (selectedRace == 1) ? Resources.Load("spaceshipMIL", typeof(GameObject)) as GameObject : Resources.Load("spaceshipECO", typeof(GameObject)) as GameObject;
            spaceship = Instantiate(spaceshipOrig, hexagon.transform.position, spaceshipQuaternion) as GameObject;
           
            spaceship.transform.parent = hexagon.transform;

            if (selectedRace == 1)
            {
                resource = "spaceshipMIL(Clone)/Sphere_001";
                highlights = new ArrayList() { 1 };
            }
            else
            {
                resource = "spaceshipECO(Clone)";
                highlights = new ArrayList() { 1, 2 };
            }
            setColor(resource, highlights);
            highlights.Clear();
        }
        ChangeFieldStateOnClick.resetHighlighting(hexagon);
        
    }

    
    [RPC]
    public void prepareShip()
    {
        //does not work yet
        gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot(spaceShipRising);

        Vector3 elevate = new Vector3 (0.0f, 0.08f, 0.0f);
        if (spaceship != null)
        {
            isMovedOnField = true;
            startPos = spaceship.transform.position;
            desiredPos = spaceship.transform.position + elevate;
            distance = Vector3.Distance(startPos, desiredPos);
            startTime = Time.time;
            
        }
            
    }

    void Update()
    {
        if (tempSpaceship != null)
        {

            string resource = "";
            ArrayList highlights = new ArrayList();
            if (CustomGameProperties.alienRace == 1)
            {

                resource = "spaceshipMIL(Clone)/Sphere_001";
                highlights = new ArrayList() { 1 };
            }
            else
            {
                resource = "spaceshipECO(Clone)";
                highlights = new ArrayList() { 1, 2 };
            }
            setColor(resource, highlights);
            highlights.Clear();

            float distCovered = (Time.time - sendingStartTime) * 0.22f;
            float fracJourney = distCovered / sendingDist;
            tempSpaceship.transform.position = Vector3.Lerp(hexStartPos, hexDestPos, fracJourney);

            if (tempSpaceship.transform.position.x == hexDestPos.x && tempSpaceship.transform.position.z == hexDestPos.z)
            {
                
                Destroy(tempSpaceship);


               

                if (destinationNeedsShip)
                {
                    NetworkView view = destinationHex.networkView;
                    NetworkViewID id = view.viewID;
                    view.RPC("initiateTroopBuilding", RPCMode.AllBuffered, CustomGameProperties.alienRace, id);

                }
            }

        }

        if (isMovedOnField)
        {
            float distCovered = (Time.time - startTime) * 0.08f;
            float fracJourney = distCovered / distance;
            spaceship.transform.position = Vector3.Lerp(startPos, desiredPos, fracJourney);
        }
    }

    private void animateFlyingShip(GameObject origin, GameObject destination)
    {

        audio.PlayOneShot(spaceShipRising);

        destinationHex = destination; 
        tempSpaceship = Instantiate(spaceshipOrig, spaceship.transform.position, spaceshipQuaternion) as GameObject;
        tempSpaceship.tag = "temporaryship";

     

        hexDestPos = destination.transform.position;
        hexStartPos = gameObject.transform.position;
        sendingStartTime = Time.time;
        sendingDist = Vector3.Distance(hexStartPos, hexDestPos);
      
      

        

        GameObject [] buggyShips = GameObject.FindGameObjectsWithTag("temporaryship");

        foreach (GameObject ship in buggyShips)
        {
            if (ship != tempSpaceship)
            {
                Destroy(ship);
            }
        }
        //direction = (Network.isServer) ? (destination.transform.position - origin.transform.position).normalized * 0.04f : (destination.transform.position - origin.transform.position).normalized * 0.04f*(-1);
        
        
       

    }

    [RPC]
    public void unPrepareShip()
    {
        Vector3 lower = new Vector3 (0.0f, - 0.08f, 0.0f);

        if (spaceship != null)
        {
            isMovedOnField = true;
            startPos = spaceship.transform.position;
            desiredPos = spaceship.transform.position + lower;
            distance = Vector3.Distance(startPos, desiredPos);
            startTime = Time.time;
        }

       
            
    }

    [RPC]
    public void sendShip(NetworkViewID originId, NetworkViewID destinationId)
    {
        NetworkView originView = NetworkView.Find(originId);
        GameObject origin = originView.gameObject;
        NetworkView destinationView = NetworkView.Find(destinationId);
        GameObject destination = destinationView.gameObject;

        Vector3 destinationLoc = destination.transform.position;
        Vector3 movement = origin.transform.position - destinationLoc;

        
        foreach (Transform child in destination.transform)
        {
            if (child.name == "spaceshipECO(Clone)" || child.name == "spaceshipMIL(Clone)")
               destinationNeedsShip = false;

             if (child.name == "baseECO(Clone)")
               destinationNeedsShip = false;
        }

        Destroy(spaceship);
        isMovedOnField = false;
        animateFlyingShip(origin, destination);
        ChangeFieldStateOnClick.resetHighlighting(origin);

     
       
    }

    [RPC]
    void buildMilitary(NetworkViewID id, int selectedRace, int builder)
    {
        ArrayList highlights = new ArrayList();
        string resource = "";

        NetworkView view = NetworkView.Find(id);
        GameObject selectedHexagon = view.gameObject;
        GameObject milBuildingState1 = null;
        GameObject milBuildingState2 = null;
        GameObject milBuildingState3 = null;

        Quaternion buildingQuaternion = (Network.isServer) ? Quaternion.Euler(0.0f, 0.0f, 0.0f) : Quaternion.Euler(0.0f, 180.0f, 0.0f);
        
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

            highlights.Add(1);
            resource = "militaryECOState3(Clone)";
        }       
        GameObject militaryBuildingState1 = Instantiate(milBuildingState1, selectedHexagon.transform.position, buildingQuaternion) as GameObject;
        GameObject militaryBuildingState2 = Instantiate(milBuildingState2, selectedHexagon.transform.position, buildingQuaternion) as GameObject;
        GameObject militaryBuildingState3 = Instantiate(milBuildingState3, selectedHexagon.transform.position,buildingQuaternion) as GameObject;
        //selectedHexagon.renderer.material = Resources.Load("militaryMaterial", typeof(Material)) as Material;

        militaryBuildingState1.transform.parent = selectedHexagon.transform;
        militaryBuildingState2.transform.parent = selectedHexagon.transform;
        militaryBuildingState3.transform.parent = selectedHexagon.transform;

        militaryBuildingState2.SetActive(false);
        militaryBuildingState3.SetActive(false);

        if ((builder == 2 && Network.isServer) || builder == 1 && Network.isClient)
        {
            decolorUnownedArea();
        }
        /*GameObject unitText = new GameObject();
        TextMesh text = unitText.AddComponent<TextMesh>();
        text.characterSize = 0.1f;
        Font font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        text.font = font;
        text.renderer.material = font.material;
        text.anchor = TextAnchor.MiddleCenter;
        unitText.transform.parent = selectedHexagon.transform;
        unitText.transform.position = selectedHexagon.transform.position;
        unitText.transform.Rotate(new Vector3(45, 0, 0));*/

        setColor(resource, highlights);
        highlights.Clear();
        ChangeFieldStateOnClick.resetHighlighting(selectedHexagon);
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
        ArrayList highlights = new ArrayList();
        ArrayList highlights3 = new ArrayList();
        ArrayList highlights4 = new ArrayList();
        ArrayList highlights5 = new ArrayList();
        string resource = "";
        string resource2 = "";
        string resource3 = "";
        string resource4 = "";
        string resource5 = "";

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
            highlights.Add(2);
            highlights.Add(4);
            resource = "economyMILState2(Clone)/Cylinder_002";
            resource2 = "economyMILState3(Clone)/Cylinder_001";
        }
        else
        {
            ecoBuildingState1 = Resources.Load("economyECOState1", typeof(GameObject)) as GameObject;
            ecoBuildingState2 = Resources.Load("economyECOState2", typeof(GameObject)) as GameObject;
            ecoBuildingState3 = Resources.Load("economyECOState3", typeof(GameObject)) as GameObject;
            resource3 = "economyECOState1(Clone)";
            highlights3.Add(1);
            resource4 = "economyECOState2(Clone)";
            highlights4.Add(1);
            highlights4.Add(4);
            resource5 = "economyECOState3(Clone)";
            highlights5.Add(1);
            highlights5.Add(3);
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

        setColor(resource, highlights);
        setColor(resource2, highlights);
        setColor(resource3, highlights3);
        setColor(resource4, highlights4);
        setColor(resource5, highlights5);
        highlights.Clear();
        highlights3.Clear();
        highlights4.Clear();
        highlights5.Clear();
        ChangeFieldStateOnClick.resetHighlighting(selectedHexagon);
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

        //gameObject.transform.GetComponentInChildren<TextMesh>().text = "" + troops;

    }

    [RPC]
    void setSpecialisation(string type)
    {
        specialisation = type;
    }

    [RPC]
    void setBuildingStatus(int state)
    {
        if (state == 1)
        {
            finishedBuilding = true;
        }
        else if (state == 0)
        {
            finishedBuilding = false;
        }
    }
    [RPC]
    void processAttack(NetworkViewID id, int sendingTroops, int attackerWeaponType)
    {
        NetworkView view = NetworkView.Find(id);
        GameObject hex = view.gameObject;
        GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>().receiveAttack(hex, sendingTroops, attackerWeaponType);
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
            Destroy(tempSpaceship);
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
