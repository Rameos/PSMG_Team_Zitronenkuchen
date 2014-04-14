// ----------------------------------------------------------------------------
//
// (C) Copyright 2014, Visual Interaction GmbH 
//
// All rights reserved. This work contains unpublished proprietary 
// information of Visual Interaction GmbH and is copy protected by law. 
// (see accompanying file eula.pdf)
//
// ----------------------------------------------------------------------------


using UnityEngine;
using System.Collections;
using iViewX;

public class ExampleGazeMenu : MonoBehaviour {

    // Setup your style of the Buttons
    // Note: you must define your own style.
    public GUIStyle myStyle;

    //Script for the Rotation of a cube in the Scene
    public gazeButton_RotateObject cubeRotationScript;

    // Save all GazeButtonElements in an arrayList / List
    private ArrayList gazeUI = new ArrayList();
    // Set an Status for the Drawing of the Elements
    private bool isDrawing = false;

   
    #region ButtonActions
    // Action for Button_1: Write a Line into the Code
    public void button1_Action()
    {
        cubeRotationScript.rotateByActivation();
    }

    // Action for Button_2: Finish the Game
    public void button2_Action()
    {
        Debug.Log("Button2_Pressed");
        Application.Quit();
    }
    #endregion

    void Start()
    {

        //Set the Actions of the Buttons
        buttonCallbackListener writeIntroConsoleButton = button1_Action;
        buttonCallbackListener quitApplicationButton = button2_Action;

        //Create new Buttonelements and add them to the gazeUI
        gazeUI.Add(new GazeButton(new Rect(Screen.width * 0.35f, Screen.height * 0.05f, 512, 256), "Rotate Cube", myStyle, writeIntroConsoleButton));
        gazeUI.Add(new GazeButton(new Rect(Screen.width * 0.35f, Screen.height * 0.65f, 512, 256), "Quit Application", myStyle, quitApplicationButton));


    }
    
    void OnGUI()
    {
        //Draw every Button from the ArrayList gazeUI
        if (isDrawing)
        {
            foreach (GazeButton button in gazeUI)
            {
                button.OnGUI();
            }
        }
    }

    void Update()
    {
        //Update only if the buttons are visible (Plea
        if (isDrawing)
        {
            foreach (GazeButton button in gazeUI)
            {
                button.Update();
            }
        }

       // Important Note: Please create a "SelectGUI" input in the InputManager of Unity.
        if (Input.GetButtonDown("SelectGUI"))
        {
            if (isDrawing)
                isDrawing = false;
            else
                isDrawing = true;

        }

        // Important Note: Please create a "SelectGUI" input in the InputManager of Unity.
        else if (Input.GetButtonUp("SelectGUI"))
        {
            isDrawing = false;
        }
    }
}
