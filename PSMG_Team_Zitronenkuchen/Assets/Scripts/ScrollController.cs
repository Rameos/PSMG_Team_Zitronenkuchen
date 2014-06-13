﻿using UnityEngine;
using System.Collections;
using iViewX;

public class ScrollController : MonoBehaviourWithGazeComponent
{

    private int framesSinceEntering = 0;
    private bool entered = false;

    private bool noGUIMode = true;

    private int xDirection = 1; //default 1, -1 if opposite direction, 0 if no movement
    private int zDirection = 1; //default 1, -1 if opposite direction, 0 if no movement

    private Vector3 minSize = new Vector3(10.0f, 10.0f, 10.0f);
    private Vector3 maxSize = new Vector3(12.0f, 12.0f, 12.0f);

    private float maxX = 21.1055f;
    private float minX = 0.0f;

    private float maxZ = 21.1055f;
    private float minZ = 0.0f;

	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        if (entered)
        {
            framesSinceEntering++;
            //camera moves if gaze stays an arrow for 6 frames
            if (framesSinceEntering == 2)
            {
                moveCamera();
                
            }
        }
        
       
	}

    void OnGUI()
    {

        //switch between gui and nogui
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.G)
        {
            if (!noGUIMode) {
                entered = false;
                noGUIMode = true;
                disableRenderers();
            }
            else
            {
                entered = false;
                noGUIMode = false;
                enabledRenderers();
            }

        }
    }

    private void enabledRenderers()
    {
        GameObject rightArrow = GameObject.FindGameObjectWithTag("RightArrow");
        GameObject upArrow = GameObject.FindGameObjectWithTag("UpArrow");
        GameObject leftArrow = GameObject.FindGameObjectWithTag("LeftArrow");
        GameObject downArrow = GameObject.FindGameObjectWithTag("DownArrow");

        rightArrow.renderer.enabled = true;
        upArrow.renderer.enabled = true;
        leftArrow.renderer.enabled = true;
        downArrow.renderer.enabled = true;
    }

    private void disableRenderers()
    {
        GameObject rightArrow = GameObject.FindGameObjectWithTag("RightArrow");
        GameObject upArrow = GameObject.FindGameObjectWithTag("UpArrow");
        GameObject leftArrow = GameObject.FindGameObjectWithTag("LeftArrow");
        GameObject downArrow = GameObject.FindGameObjectWithTag("DownArrow");
        GameObject upRightArrow = GameObject.FindGameObjectWithTag("UpperRightArrow");
        GameObject downRightArrow = GameObject.FindGameObjectWithTag("BottomRightArrow");
        GameObject upLeftArrow = GameObject.FindGameObjectWithTag("UpperLeftArrow");
        GameObject downLeftArrow = GameObject.FindGameObjectWithTag("BottomLeftArrow");

        rightArrow.renderer.enabled = false;
        upArrow.renderer.enabled = false;
        leftArrow.renderer.enabled = false;
        downArrow.renderer.enabled = false;

        upRightArrow.renderer.enabled = false;
        downRightArrow.renderer.enabled = false;
        upLeftArrow.renderer.enabled = false;
        downLeftArrow.renderer.enabled = false;
    }

    private void moveCamera()
    {

        determineDirection();
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        camera.transform.Translate(0.2f * xDirection * Time.deltaTime * 2, 0, 0.2f * zDirection * Time.deltaTime * 2, Space.World);

        if (camera.transform.position.x < minX) {
            xDirection = -1;
            camera.transform.Translate(0.2f * xDirection * Time.deltaTime * 2, 0, 0.2f * zDirection * Time.deltaTime * 2, Space.World);
        }

        if (camera.transform.position.x > maxX)
        {
            xDirection = 1;
            camera.transform.Translate(0.2f * xDirection * Time.deltaTime * 2, 0, 0.2f * zDirection * Time.deltaTime * 2, Space.World);
        }

        moveArrows();

        reEnableMovement();
    }

    private void reEnableMovement()
    {
        framesSinceEntering = 0;
    }

    private void moveArrows()
    {

        Vector3 movement = new Vector3(0.2f * xDirection, 0, 0.2f * zDirection);
        GameObject rightArrow = GameObject.FindGameObjectWithTag("RightArrowParent");
        GameObject upArrow = GameObject.FindGameObjectWithTag("UpArrowParent");
        GameObject leftArrow = GameObject.FindGameObjectWithTag("LeftArrowParent");
        GameObject downArrow = GameObject.FindGameObjectWithTag("DownArrowParent");
        GameObject upRightArrow = GameObject.FindGameObjectWithTag("UpperRightArrowParent");
        GameObject downRightArrow = GameObject.FindGameObjectWithTag("BottomRightArrowParent");
        GameObject downLeftArrow = GameObject.FindGameObjectWithTag("BottomLeftArrowParent");
        GameObject upLeftArrow = GameObject.FindGameObjectWithTag("UpperLeftArrowParent");


        rightArrow.transform.Translate(movement * Time.deltaTime * 2);
        leftArrow.transform.Translate(movement * Time.deltaTime * 2);
        upArrow.transform.Translate(movement * Time.deltaTime * 2);
        downArrow.transform.Translate(movement * Time.deltaTime * 2);
        upRightArrow.transform.Translate(movement * Time.deltaTime * 2);
        upLeftArrow.transform.Translate(movement * Time.deltaTime * 2);
        downLeftArrow.transform.Translate(movement * Time.deltaTime * 2);
        downRightArrow.transform.Translate(movement * Time.deltaTime * 2);
    }

    private void determineDirection()
    {
        switch (gameObject.tag)
        {
            case "LeftArrow":
                xDirection = -1;
                zDirection = 0;
                break;
            case "RightArrow":
                xDirection = 1;
                zDirection = 0;
                break;
            case "UpArrow":
                xDirection = 0;
                zDirection = 1;
                break;
            case "DownArrow":
                xDirection = 0;
                zDirection = -1;
                break;
            case "UpperRightArrow":
                xDirection = 1;
                zDirection = 1;
                break;
            case "BottomRightArrow":
                xDirection = 1;
                zDirection = -1;
                break;
            case "BottomLeftArrow":
                xDirection = -1;
                zDirection = -1;
                break;
            case "UpperLeftArrow":
                xDirection = -1;
                zDirection = 1;
                break;
        }
    }

    public override void OnGazeEnter(RaycastHit hit)
    {
        entered = true;
        highlightArrow();
    }

    public override void OnGazeStay(RaycastHit hit)
    {
    }

    public override void OnGazeExit()
    {
        entered = false;
        reEnableMovement();
        resetArrow();
    }

    private void highlightArrow()
    {
        if (!noGUIMode) {
            gameObject.renderer.enabled = true;
            gameObject.transform.localScale = maxSize;
            gameObject.transform.renderer.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
        }
        
    }


    private void resetArrow()
    {
        if (!noGUIMode)
        {
            gameObject.transform.localScale = minSize;
            gameObject.transform.renderer.material.shader = Shader.Find("Diffuse"); 
        }
    }


}
