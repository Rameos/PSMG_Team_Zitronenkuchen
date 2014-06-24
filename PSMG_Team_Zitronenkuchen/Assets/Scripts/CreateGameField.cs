using UnityEngine;
using System.Collections;

public class CreateGameField : MonoBehaviour {


    public GameObject hexagon;
    public GameObject field;
    public GameObject terrain;
    public Transform terrainPosition;

    private const float GAP_SIZE = 0.01f; // 0-5 seems reasonable
    private const float FIELD_SIZE = 50; // startmenu function to select the fieldsize?

    private Quaternion FIELD_ROTATION = new Quaternion(0, 0, 0, 0);
    private Vector3 HEX_SIZE = new Vector3(5, 5, 20);
    private Vector3 FIRSTHEXAGON_POSITION;
    private Vector3 ROTATION = new Vector3(90, 0, 0);
    private Vector3 newHexPosition;
    private Vector3 trueHexSize;
    
    public Material defaultMaterial;
    public Material baseMaterial;
    

    private int BASE_X = 4;
    private int BASE_Y = 6;

    public GameObject[,] hexArray {get; set;}

	// Use this for initialization
	void Start () {
        if (!CustomGameProperties.usesMouse)
        {
            Screen.lockCursor = true;
        }
        initiateMaterial();
        FIELD_ROTATION.eulerAngles = ROTATION;
        hexagon.transform.localScale = HEX_SIZE;
        trueHexSize = hexagon.renderer.bounds.size;
        hexArray = new GameObject[(int)FIELD_SIZE, (int) FIELD_SIZE];
        for (int i = 0; i < FIELD_SIZE; i++)
            {       
                for (int j = 0; j < FIELD_SIZE; j++)
                {
                    newHexPosition = positionHexagons(i, j);
                    GameObject hex = Instantiate(hexagon, newHexPosition, FIELD_ROTATION) as GameObject;       // Instantiates hexagon prefabs as gameobjects
                    hex.AddComponent<HexField>();
                    hex.GetComponent<HexField>().xPos = i;
                    hex.GetComponent<HexField>().yPos = j;
                    hexArray[i, j] = hex;
                    assignMaterialToObject(hex);
                    addComponentsAndScale(hex);
                }
            }
        foreach (GameObject obj in hexArray)
        {
            obj.GetComponent<HexField>().hexArray = hexArray;
        }
        buildStartArea();
        //field.transform.renderer.bounds.center = new Vector3(100, 100, 100);                         //soll Mittelpunkt von terrain und field übereinanderlegen funktioniert aber nicht
        }

    private void buildStartArea()
    {
        GameObject baseField = hexArray[BASE_X, BASE_Y];
        baseField.GetComponent<HexField>().isFilled = true;
        baseField.renderer.material = baseMaterial;
        GameObject[] influenceArea = baseField.GetComponent<HexField>().getSurroundingFields();
        foreach (GameObject obj in influenceArea)
        {
            obj.GetComponent<HexField>().owner = 1;
            obj.GetComponent<HexField>().colorOwnedArea(obj);
        }
    }
    private void assignMaterialToObject(GameObject obj)
    {
        obj.renderer.material = defaultMaterial;
    }

    private void initiateMaterial()
    {
        defaultMaterial = Resources.Load("DefaultMaterial", typeof(Material)) as Material;
        baseMaterial = Resources.Load("BaseMaterial", typeof(Material)) as Material;

        if (defaultMaterial == null || baseMaterial == null)
        {
            Debug.Log("loading failed, check existence of Resources folder in Assets");
        }
    }
	
	// Update is called once per frame
	void Update () {

	}

    // Adds all Components needed to interact with the individual Hexagons and scales them
    private void addComponentsAndScale(GameObject hex)
    {
        hex.transform.parent = field.transform;
        hex.AddComponent("ChangeFieldStateOnClick");
        hex.AddComponent("MeshCollider");
        hex.GetComponent<MeshCollider>().sharedMesh = Resources.Load("hexagonbig", typeof(Mesh)) as Mesh;
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
                newPosition.z += i * (0.75f *trueHexSize.z +  + GAP_SIZE);                                  //
            }
        return newPosition;
    }
}
