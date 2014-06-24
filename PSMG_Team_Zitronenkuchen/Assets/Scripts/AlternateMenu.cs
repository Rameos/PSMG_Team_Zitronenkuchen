﻿using UnityEngine;
using System.Collections;
using iViewX;

public class AlternateMenu : MonoBehaviour {

    public bool isQuit = false;
    public bool isStart = false;
    public bool isCalibrate = false;
    public bool isMouse = false;
    public bool isEyetracker = false;

    public float newPos1;
    public float newPos2;

    public AudioClip selectSound;

    void OnMouseEnter()
    {
        renderer.material.color = Color.cyan;
        audio.PlayOneShot(selectSound);
    }

    void OnMouseExit()
    {
        renderer.material.color = Color.white;
    }

    void OnMouseUp()
    {
        if (isQuit == true)
        {
            Application.Quit();
        }
        else if (isStart == true)
        {
            
            transform.position = new Vector3(-75, transform.position.y, transform.position.z);
            GameObject useMouseBtn = GameObject.FindGameObjectWithTag("UseMouse");
            GameObject useEyetrackerBtn = GameObject.FindGameObjectWithTag("UseEyetracker");
            useMouseBtn.transform.position = new Vector3(-22, transform.position.y, transform.position.z);
            useEyetrackerBtn.transform.position = new Vector3(-5, transform.position.y, transform.position.z);
        }
        else if (isMouse)
        {
            CustomGameProperties.usesMouse = true;
            Application.LoadLevel("Create_Gamefield");
            //TODO trigger that mouse is used in game
        }
        else if (isEyetracker)
        {
            Application.LoadLevel("Create_Gamefield");
            //TODO trigger that eyetracker is used in game
        }
        else if (isCalibrate == true)
        {
            GazeControlComponent.Instance.StartCalibration(); 
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}

/*
 * var isQuit=false;

function OnMouseEnter(){
//change text color
renderer.material.color=Color.red;
}

function OnMouseExit(){
//change text color
renderer.material.color=Color.white;
}

function OnMouseUp(){
//is this quit
if (isQuit==true) {
//quit the game
Application.Quit();
}
else {
//load level
Application.LoadLevel(1);
}
}

function Update(){
//quit game if escape key is pressed
if (Input.GetKey(KeyCode.Escape)) { Application.Quit();
}
}*/

