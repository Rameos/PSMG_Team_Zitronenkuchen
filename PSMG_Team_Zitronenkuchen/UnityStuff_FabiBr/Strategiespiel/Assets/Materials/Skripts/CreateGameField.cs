using UnityEngine;
using System.Collections;

public class CreateGameField : MonoBehaviour {


    public GameObject hexagon;
    public GameObject field;

    private const float GAP_SIZE = 1; // 0-5 seems reasonable
    private const float FIELD_SIZE = 40; // startmenu function to select the fieldsize?

    private Quaternion FIELD_ROTATION = new Quaternion(0, 0, 0, 0);
    private Vector3 HEX_SIZE = new Vector3(500, 500, 2000);
    private Vector3 FIRSTHEXAGON_POSITION = new Vector3(250, 0,-650);
    private Vector3 ROTATION = new Vector3(90, 30, 0);
    private Vector3 newHexPosition;
    public Material defaultMaterial;


	// Use this for initialization
	void Start () {

        initiateMaterial();

        for (int i = 0; i < FIELD_SIZE; i++)
                {       
                    for (int j = 0; j < FIELD_SIZE/2; j++)
                    {
                        newHexPosition = positionHexagons(i, j);
                        GameObject hex = Instantiate(hexagon, newHexPosition, FIELD_ROTATION) as GameObject;       // Instantiates hexagon prefabs as gameobjects
                        assignMaterialToObject(hex);
                        addComponentsAndScale(hex);
                    }
                }
            }

    private void assignMaterialToObject(GameObject obj)
    {
        obj.renderer.material = defaultMaterial;
    }

    private void initiateMaterial()
    {
        defaultMaterial = Resources.Load("DefaultGamefield", typeof(Material)) as Material;
        if (defaultMaterial == null)
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
        hex.transform.localScale = HEX_SIZE;
        hex.transform.Rotate(ROTATION);
        hex.AddComponent("ChangeFieldStateOnClick");
        hex.AddComponent("SphereCollider");
        hex.GetComponent<SphereCollider>().radius = 1000;
    }

    // Calculates the individual Position of every Hexagon to create a mesh
    private Vector3 positionHexagons(int i, int j)
    {
        Vector3 newPosition = FIRSTHEXAGON_POSITION;
        if (i % 2 == 0)
            {
                newPosition.x += i*(HEX_SIZE.x/13.3333f + GAP_SIZE);        // changes the position of every even numbered field dependent on the position of the first field
                newPosition.z -= j*(HEX_SIZE.x/11.7647f + GAP_SIZE);        //
            }
        else
            {
                newPosition.x += i * (HEX_SIZE.x / 13.3333f + GAP_SIZE);                          // changes the position of every odd numbered hexagon adding a slight offset so the field lines up properly
                newPosition.z -= j * (HEX_SIZE.x / 11.7647f + GAP_SIZE) + HEX_SIZE.x/23.5294f;    //
            }
        return newPosition;
    }
}
