using UnityEngine;
using System.Collections;

public class CreateGameField : MonoBehaviour
{
    public GameObject hexagon;
    // public GameObject field;
    // public GameObject terrain;
    public Transform terrainPosition;

    private const float GAP_SIZE = 0.01f; // 0-5 seems reasonable
    private const float FIELD_SIZE = 20; // startmenu function to select the fieldsize?

    private Quaternion FIELD_ROTATION = new Quaternion(0, 0, 0, 0);
    private Vector3 HEX_SIZE = new Vector3(5, 5, 20);
    private Vector3 FIRSTHEXAGON_POSITION;
    private Vector3 ROTATION = new Vector3(90, 0, 0);
    private Vector3 newHexPosition;
    private Vector3 clientBasePos;
    private static Vector3 trueHexSize;


    private int BASE_X = 4;
    private int BASE_Y = 6;

    private MainController mC;

    private int selectedRace = CustomGameProperties.alienRace;

    // Use this for initialization
    void Start()
    {
        trueHexSize = hexagon.renderer.bounds.size;
        mC = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
        if (Network.isServer)
        {
            if (!CustomGameProperties.usesMouse)
            {
                Screen.lockCursor = true;
            }
            FIELD_ROTATION.eulerAngles = ROTATION;
            hexagon.transform.localScale = HEX_SIZE;
            
            for (int i = 0; i < FIELD_SIZE; i++)
            {
                for (int j = 0; j < FIELD_SIZE; j++)
                {
                    newHexPosition = positionHexagons(i, j);
                    GameObject hex = Network.Instantiate(hexagon, newHexPosition, FIELD_ROTATION, 0) as GameObject;       // Instantiates hexagon prefabs as gameobjects
                    NetworkView nview = hex.networkView;
                    NetworkViewID nviewId = nview.viewID;
                    nview.RPC("addPos", RPCMode.AllBuffered, nviewId, i, j);
                }
            }
            gameObject.networkView.RPC("buildStartArea", RPCMode.AllBuffered, BASE_X, BASE_Y, (int)FIELD_SIZE - BASE_X, (int)FIELD_SIZE - BASE_Y);
            //field.transform.renderer.bounds.center = new Vector3(100, 100, 100);                         //soll Mittelpunkt von terrain und field übereinanderlegen funktioniert aber nicht
        }
        else
        {
            GameObject.FindGameObjectWithTag("CameraWrapper").transform.position = new Vector3(6.3f, 1.0f, 5.5f);
            GameObject.FindGameObjectWithTag("CameraWrapper").transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }

        GameObject.FindGameObjectWithTag("CameraWrapper").AddComponent<ScrollController> ();
    }


    public static float getFieldWidth()
    {
        //20 times hexagonsize + 18 times gapsize - 1/2 of hexsize because of border
        return FIELD_SIZE * trueHexSize.x + GAP_SIZE*(FIELD_SIZE - 2) - 1/2*  trueHexSize.x;
    }

    public static float getFieldHeight()
    {

        return FIELD_SIZE * 0.7f * trueHexSize.z + GAP_SIZE * (FIELD_SIZE - 2) - 1 / 2 * trueHexSize.z;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Calculates the individual Position of every Hexagon to create a mesh
    private Vector3 positionHexagons(int i, int j)
    {
        Vector3 newPosition = FIRSTHEXAGON_POSITION;
        if (i % 2 == 0)
        {
            newPosition.x += j * (trueHexSize.x + GAP_SIZE);              // changes the position of every even numbered field dependent on the position of the first field
            newPosition.z += i * (0.75f * trueHexSize.z + GAP_SIZE);      //
        }
        else
        {
            newPosition.x += (GAP_SIZE + trueHexSize.x) / 2 + j * (trueHexSize.x + GAP_SIZE);                         // changes the position of every odd numbered hexagon adding a slight offset so the field lines up properly
            newPosition.z += i * (0.75f * trueHexSize.z + +GAP_SIZE);                                  //
        }
        return newPosition;
    }

    [RPC]
    void buildStartArea(int xPlayerOne, int yPlayerOne, int xPlayerTwo, int yPlayerTwo)
    {
        Debug.Log("buildStartArea at: " + xPlayerOne + ", " + yPlayerOne + " and: " + xPlayerTwo + ", " + yPlayerTwo);
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("hex");
        clientBasePos = new Vector3(xPlayerTwo, 1.0f, yPlayerTwo);
        bool oneSet = false;
        bool twoSet = false;
        foreach (GameObject baseField in gameObjects)
        {// iterate through all gameobjects
            HexField hex = baseField.GetComponent<HexField>();  // if gameobject is no hex -> null
            if (Network.isServer)
            {
                if (hex.xPos == (int)xPlayerOne && hex.yPos == (int)yPlayerOne)
                { // Player One Start Area
                    Debug.Log("got one");
                    Material baseMaterial = Resources.Load("BaseMaterial", typeof(Material)) as Material;
                    baseField.GetComponent<HexField>().isFilled = true;
                    baseField.renderer.material = baseMaterial;
                    specialiseBase(baseField);
                    ArrayList influenceArea = baseField.GetComponent<HexField>().getSurroundingFields();
                    foreach (GameObject obj in influenceArea)
                    {
                        if (Network.isServer)
                        {
                            obj.GetComponent<HexField>().owner = 1;
                            obj.GetComponent<HexField>().colorOwnedArea();
                            obj.GetComponent<HexField>().FinishedBuilding = true;
                        }
                    }
                    baseField.GetComponent<HexField>().owner = 1;
                    oneSet = true;
                }
            }
            else
            {
                if (hex.xPos == (int)xPlayerTwo && hex.yPos == (int)yPlayerTwo)
                { // Player Two Start Area
                    Debug.Log("got two");
                    Material baseMaterial = Resources.Load("BaseMaterial", typeof(Material)) as Material;
                    baseField.GetComponent<HexField>().isFilled = true;
                    baseField.renderer.material = baseMaterial;
                    specialiseBase(baseField);
                    ArrayList influenceArea = baseField.GetComponent<HexField>().getSurroundingFields();
                    foreach (GameObject obj in influenceArea)
                    {
                        if (Network.isClient)
                        {
                            obj.GetComponent<HexField>().owner = 2;
                            obj.GetComponent<HexField>().colorOwnedArea();
                            obj.GetComponent<HexField>().FinishedBuilding = true;   
                        }

                    }
                    baseField.GetComponent<HexField>().owner = 2;
                    twoSet = true;
                }
            }
            if (oneSet || twoSet)
            {
                break;
            }

        }

    }

    private void specialiseBase(GameObject baseNode)
    {
        Specialisation spec = new BaseSpecialisation(baseNode, baseNode.GetComponent<HexField>().getPos());
        baseNode.GetComponent<HexField>().spec = spec;
        mC.specialisedNodes.Add(spec);
        NetworkView nview = baseNode.networkView;
        NetworkViewID nviewId = nview.viewID;
        nview.RPC("setSpecialisation", RPCMode.AllBuffered, "Base");
        nview.RPC("buildBase", RPCMode.AllBuffered, nviewId, selectedRace);
        nview.RPC("fieldSet", RPCMode.AllBuffered);
        nview.RPC("showTroops", RPCMode.AllBuffered, ((BaseSpecialisation)spec).Troops);
    }
}
