using UnityEngine;
using System.Collections;

/**
 * This script creates the Map of Hexfields and initializes the startareas for both players. 
 **/
public class CreateGameField : MonoBehaviour
{
    // model for the hexagons to be created
    public GameObject hexagon;
    public Transform terrainPosition;

    // gap between the hexagons
    private const float GAP_SIZE = 0.01f; 
    // map will be FIELD_SIZExFIELD_SIZE fields big
    private const float FIELD_SIZE = 20;

    // field stats
    private Quaternion FIELD_ROTATION = new Quaternion(0, 0, 0, 0);
    private Vector3 HEX_SIZE = new Vector3(5, 5, 20);
    private Vector3 FIRSTHEXAGON_POSITION;
    private Vector3 ROTATION = new Vector3(90, 0, 0);
    private Vector3 newHexPosition;
    private Vector3 clientBasePos;
    private static Vector3 trueHexSize;

    // position of the players' bases(seen from each of their sides of the field)
    private int BASE_X = 4;
    private int BASE_Y = 6;

    private MainController mC;

    private int selectedRace = CustomGameProperties.alienRace;

    // Use this for initialization
    void Start()
    {
        trueHexSize = hexagon.renderer.bounds.size;
        mC = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
        if (!CustomGameProperties.usesMouse)
        {
            Screen.lockCursor = true;
        }
        if (Network.isServer)
        {
            // player is server. only server instantiates the hex fields via Network.Instantiate
            CustomGameProperties.cameraInUse = 1;
            FIELD_ROTATION.eulerAngles = ROTATION;
            hexagon.transform.localScale = HEX_SIZE;
            
            for (int i = 0; i < FIELD_SIZE; i++)
            {
                for (int j = 0; j < FIELD_SIZE; j++)
                {
                    newHexPosition = positionHexagons(i, j);
                    GameObject hex = Network.Instantiate(hexagon, newHexPosition, FIELD_ROTATION, 0) as GameObject;       // Instantiate clones of the hexagon prefab as gameobjects on both(!) players
                    // set the position of the hexfield in the HexField script on both players
                    NetworkView nview = hex.networkView;
                    NetworkViewID nviewId = nview.viewID;
                    nview.RPC("addPos", RPCMode.AllBuffered, nviewId, i, j);
                }
            }
            // build the start area on both players after(!) field is initialized. BASE_X/Y are startpositions for Server. (int)FIELD_SIZE - BASE_X/Y are startpositions for client
            gameObject.networkView.RPC("buildStartArea", RPCMode.AllBuffered, BASE_X, BASE_Y, (int)FIELD_SIZE - BASE_X, (int)FIELD_SIZE - BASE_Y);
        }
        else
        {
            // player is client. transform camera
            CustomGameProperties.cameraInUse = 2;
            GameObject.FindGameObjectWithTag("CameraWrapper").transform.position = new Vector3(6.202f, 1.0f, 6.16f);
            GameObject.FindGameObjectWithTag("CameraWrapper").transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            GameObject.FindGameObjectWithTag("CameraWrapperMiniMap").transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
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

    // Calculates the individual Position of every Hexagon to create a mesh
    private Vector3 positionHexagons(int i, int j)
    {
        Vector3 newPosition = FIRSTHEXAGON_POSITION;
        if (i % 2 == 0)
        {
            // changes the position of every even numbered field depending on the position of the first field
            newPosition.x += j * (trueHexSize.x + GAP_SIZE);              
            newPosition.z += i * (0.75f * trueHexSize.z + GAP_SIZE);      
        }
        else
        {
            // changes the position of every odd numbered hexagon adding a slight offset so the field lines up properly
            newPosition.x += (GAP_SIZE + trueHexSize.x) / 2 + j * (trueHexSize.x + GAP_SIZE);           
            newPosition.z += i * (0.75f * trueHexSize.z + +GAP_SIZE);                                  
        }
        return newPosition;
    }

    [RPC]
    void buildStartArea(int xPlayerOne, int yPlayerOne, int xPlayerTwo, int yPlayerTwo)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("hex");
        clientBasePos = new Vector3(xPlayerTwo, 1.0f, yPlayerTwo);
        bool oneSet = false;
        bool twoSet = false;
        foreach (GameObject baseField in gameObjects)
        {
            // iterate through all gameobjects
            HexField hex = baseField.GetComponent<HexField>();  // if gameobject is no hex -> hex=null
            if (hex != null)
            {
                if (Network.isServer)
                {
                    // server
                    if (hex.xPos == (int)xPlayerOne && hex.yPos == (int)yPlayerOne)
                    { 
                        // Server Start Area
                        baseField.GetComponent<HexField>().isFilled = true;

                        specialiseBase(baseField);
                        ArrayList influenceArea = baseField.GetComponent<HexField>().getSurroundingFields();

                        foreach (GameObject obj in influenceArea)
                        {
                            // color influence area around the server's base
                            if (Network.isServer)
                            {
                                obj.GetComponent<HexField>().owner = 1;
                                obj.GetComponent<HexField>().colorOwnedArea();
                            }
                        }
                        oneSet = true;
                    }
                }
                else
                {
                    // Client
                    if (hex.xPos == (int)xPlayerTwo && hex.yPos == (int)yPlayerTwo)
                    { 
                        // Client Start Area
                        baseField.GetComponent<HexField>().isFilled = true;

                        specialiseBase(baseField);
                        ArrayList influenceArea = baseField.GetComponent<HexField>().getSurroundingFields();

                        foreach (GameObject obj in influenceArea)
                        {
                            // color influence area around the client's base
                            if (Network.isClient)
                            {
                                obj.GetComponent<HexField>().owner = 2;
                                obj.GetComponent<HexField>().colorOwnedArea(); 
                            }

                        }
                        twoSet = true;
                    }
                }
            }
            
            if (oneSet || twoSet)
            {
                // bases for client and server already built -> break
                break;
            }

        }

    }

    // create a base specialisation on the given field
    private void specialiseBase(GameObject baseNode)
    {
        Specialisation spec = new BaseSpecialisation(baseNode, baseNode.GetComponent<HexField>().getPos());
        baseNode.GetComponent<HexField>().spec = spec;
        mC.specialisedNodes.Add(spec);

        int owner = Network.isServer ? 1 : 2;
        NetworkView nview = baseNode.networkView;
        NetworkViewID nviewId = nview.viewID;
        nview.RPC("setSpecialisation", RPCMode.AllBuffered, "Base");
        nview.RPC("buildBase", RPCMode.AllBuffered, nviewId, selectedRace, owner);
        nview.RPC("fieldSet", RPCMode.AllBuffered);
    }
}
