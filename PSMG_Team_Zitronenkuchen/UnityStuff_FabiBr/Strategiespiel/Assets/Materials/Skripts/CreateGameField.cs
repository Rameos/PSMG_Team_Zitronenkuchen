using UnityEngine;
using System.Collections;

public class CreateGameField : MonoBehaviour {


    public GameObject hexagon;
    public Transform firstField;

    private const float gapSize = 5;


	// Use this for initialization
	void Start () {
        for (int i = 0; i < 20; i++)
                {       
                    for (int j = 0; j < 10; j++)
                    {
                        Vector3 newposition = firstField.position;
                        if(!(i == 0 && j == 0)) {
                        if (i % 2 == 0)
                            {
                                newposition.x += i*(75+ gapSize);
                                newposition.z -= j*(85+gapSize);
                            }
                        else
                            {
                                newposition.x += i*(75+ gapSize);
                                newposition.z -= j * (85+gapSize) + 43;
                            }
                            Instantiate(hexagon,newposition,firstField.rotation);
                        }
                    }
                }
	        }
	
	// Update is called once per frame
	void Update () {

        
	}
}
