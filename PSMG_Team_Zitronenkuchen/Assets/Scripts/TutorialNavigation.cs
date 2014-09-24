using UnityEngine;
using System.Collections;

public class TutorialNavigation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI () {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
        {
            Destroy(GameObject.FindGameObjectWithTag("IntroText"));
            Destroy(GameObject.FindGameObjectWithTag("IntroStars"));
            Destroy(GameObject.FindGameObjectWithTag("SkipText"));
        }
    }
     
}
