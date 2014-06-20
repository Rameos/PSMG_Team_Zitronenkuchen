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



	// Use this for initialization
	void Start ()
    {
        if (Network.isServer)
        {
            FIELD_ROTATION.eulerAngles = ROTATION;
            hexagon.transform.localScale = HEX_SIZE;
            trueHexSize = hexagon.renderer.bounds.size;
            for (int i = 0; i < FIELD_SIZE; i++)
                {       
                    for (int j = 0; j < FIELD_SIZE; j++)
                    {
                        newHexPosition = positionHexagons(i, j);
                        GameObject hex = Network.Instantiate(hexagon, newHexPosition, FIELD_ROTATION, 0) as GameObject;       // Instantiates hexagon prefabs as gameobjects
                    }
                }
        }                     //soll Mittelpunkt von terrain und field übereinanderlegen funktioniert aber nicht
    }
	
	// Update is called once per frame
	void Update () {

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
