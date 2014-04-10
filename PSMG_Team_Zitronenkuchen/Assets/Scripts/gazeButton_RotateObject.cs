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

public class gazeButton_RotateObject : MonoBehaviour {

    private bool isRotating = false;
    private float sec = 2f;

    //Enable Rotation via GazeButton
    public void rotateByActivation()
    {
        isRotating = true;
        StartCoroutine(rotateAndWait());
    }

	// Update is called once per frame
	void Update () {
	
        if(isRotating)
        {
            transform.Rotate(Vector3.up,2f);
        }
	}

    //Stop Rotation after some Seconds (sec)
    IEnumerator rotateAndWait()
    {
        yield return new WaitForSeconds(sec);
        isRotating = false;
    }
}
