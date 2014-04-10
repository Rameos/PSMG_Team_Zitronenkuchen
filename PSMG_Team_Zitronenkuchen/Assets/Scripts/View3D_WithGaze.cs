// ----------------------------------------------------------------------------
//
// (C) Copyright 2014, Visual Interaction GmbH 
//
// All rights reserved. This work contains unpublished proprietary 
// information of Visual Interaction GmbH and is copy protected by law. 
// (see accompanying file eula.pdf)
//
//
// ----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class View3D_WithGaze : MonoBehaviour {

	
    // InputData
	public int HeadPositionStart =0; 
    public bool enableGaze = false; 
    public GameObject focusPoint;
    
    // SetupData
    public float sensivity = 15f;
    private float filter = 0.05f; 
    
    // TranformData
    private float rotationX = 0;
    private int xPosOffset = 5;
    private float differenceLastFrame; 

    // CameraData
    private Vector3 positionCamera;
    
    // GazeData
    private Vector3 headPos = Vector3.zero;
    private Vector3 posRightEye = Vector3.zero;
    private Vector3 posLeftEye = Vector3.zero;
	

    void Start () {

        transform.LookAt(focusPoint.transform.position);
	}
	
	void Update () {
        // Look the Camera to the Focuspoint
        transform.LookAt(focusPoint.transform.position);

        if (enableGaze)
        {
            #region checkGazedata

            posRightEye = gazeModel.posRightEye;
            posLeftEye = gazeModel.posLeftEye;

            headPos = (posRightEye + posLeftEye) * 0.5f;
            #endregion

            // Calculate the differenceX from the HeadPos and the HeadStartposition;
                float differenceX = (Mathf.RoundToInt(headPos.x - HeadPositionStart) * 0.5f) * 0.1f;
               
            // Only React, when the Difference is bigger than the filter
                if (Mathf.Abs(differenceX - differenceLastFrame) > filter)
                {
                    rotationX = differenceX + xPosOffset;
                    differenceLastFrame = differenceX;

                    //Move the Camera across the X-Axis
                    Vector3 position = new Vector3(rotationX-xPosOffset, 1, -10);
                    gameObject.transform.position = position;
                }
        }
	}

    private float clampPos(float pos, float min, float max)
    {
        if (pos < min)
            pos = min; 
        
        if (pos> max)
            pos = max;

        return Mathf.Clamp(pos, min, max); 
    }
}
