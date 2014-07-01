﻿using UnityEngine;
using System.Collections;

public class SelectionMenuNavigation : MonoBehaviour {

    public AudioClip selectSound;

    public AudioClip UFO;

	// Use this for initialization
	void Start () {
	    
	}

    void OnMouseEnter()
    {
        gameObject.guiText.color = Color.magenta; //orange
        audio.PlayOneShot(selectSound);
    }

    void OnMouseUp()
    {
        if (gameObject.tag == "StartGame")
        {
            audio.PlayOneShot(UFO);
            Application.LoadLevel("Create_Gamefield");
        }

        if (gameObject.tag == "BackToMenu") 
        {
            Application.LoadLevel("Alternate_Main_Menu");
        }
    }

    void OnMouseExit()
    {
        gameObject.guiText.color = Color.cyan; //initialcolor
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
