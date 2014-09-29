using UnityEngine;
using System.Collections;

public class BackButtonBehaviour : MonoBehaviour {

	// Use this for initialization
    public AudioClip selectSound;

	void Start () {
	
	}


    void OnMouseExit()
    {
       
            gameObject.guiText.color = new Color32(87, 192, 195, 255); 
    }

    void OnMouseEnter()
    {
        audio.PlayOneShot(selectSound);
            gameObject.guiText.color = new Color32(218, 164, 59, 255); //orange

    }

    void OnMouseUp()
    {

        Application.LoadLevel("Alternate_Main_Menu");

    }

	// Update is called once per frame
	void Update () {
	
	}
}
