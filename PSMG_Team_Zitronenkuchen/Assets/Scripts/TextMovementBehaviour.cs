using UnityEngine;
using System.Collections;

/**
 * This class handles Text Movement in the Intro scene
 **/
public class TextMovementBehaviour : MonoBehaviour {

    Vector3 destination;
    Vector3 start;
    private float startTime;
    private float movementLength;

	// set up destination and time for abimaruonen
	void Start () {
      destination = new Vector3(0.0216f, 1.4106f, -9.531f);
      start = gameObject.transform.position;

      startTime = Time.time;
      movementLength = Vector3.Distance(start, destination);
	}
	
	// Update is called once per frame, text is animated star wars style
	void Update () {

        float distCovered = (Time.time - startTime) * 0.05f;
        float fracJourney = distCovered / movementLength;
        gameObject.transform.position = Vector3.Lerp(start, destination, fracJourney);
	}
}
