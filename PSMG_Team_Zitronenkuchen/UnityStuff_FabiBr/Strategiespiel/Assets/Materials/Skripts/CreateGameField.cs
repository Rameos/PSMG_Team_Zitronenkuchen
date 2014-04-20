using UnityEngine;
using System.Collections;

public class CreateGameField : MonoBehaviour {


    public GameObject hexagon;

    private const float GAP_SIZE = 3; // 0-5 seems reasonable
    private const float FIELD_SIZE = 20; // startmenu function to select the fieldsize?

    private Quaternion FIELD_ROTATION = new Quaternion(0, 0, 0, 0);
    private Vector3 HEX_SIZE = new Vector3(990, 990, 3990);
    private Vector3 FIRSTFIELD_POSITION = new Vector3(250, 0,-650);
    private Vector3 ROTATION = new Vector3(90, 30, 0);


	// Use this for initialization
	void Start () {
        for (int i = 0; i < FIELD_SIZE; i++)
                {       
                    for (int j = 0; j < FIELD_SIZE/2; j++)
                    {
                        Vector3 newPosition = FIRSTFIELD_POSITION;
                        
                        if (i % 2 == 0)
                            {
                                newPosition.x += i*(75+ GAP_SIZE);      // changes the position of every even numbered field dependent on the position of the first field
                                newPosition.z -= j*(85+GAP_SIZE);       //  
                            }
                        else
                            {
                                newPosition.x += i*(75+ GAP_SIZE);          // changes the position of every odd numbered hexagon adding a slight offset so the field lines up properly
                                newPosition.z -= j * (85+GAP_SIZE) + 43;    //
                            }
                            GameObject hex = Instantiate(hexagon, newPosition, FIELD_ROTATION) as GameObject;       // Instantiates hexagon prefabs as gameobjects
                            hex.transform.localScale += HEX_SIZE;
                            hex.transform.Rotate(ROTATION);
                            hex.AddComponent("ChangeFieldStateOnClick");
                        }
                    }
                }
	
	// Update is called once per frame
	void Update () {

        
	}
}
