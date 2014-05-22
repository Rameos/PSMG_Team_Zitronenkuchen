using UnityEngine;
using System.Collections;
using iViewX;

public class ScrollController : MonoBehaviourWithGazeComponent
{

    private int framesSinceEntering = 0;
    private bool entered = false;

    private int xDirection = 1; //default 1, -1 if opposite direction, 0 if no movement
    private int zDirection = 1; //default 1, -1 if opposite direction, 0 if no movement

	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        if (entered)
        {
            framesSinceEntering++;
            if (framesSinceEntering == 6)
            {
                moveCamera();
                
            }
        }
        
       
	}

    private void moveCamera()
    {
        determineDirection();
        Vector3 movement = new Vector3(0.2f * xDirection, 0.0f,  0.2f * zDirection);

        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        camera.transform.Translate(movement*Time.deltaTime*2);
        moveArrows(movement);

        reEnableMovement();
    }

    private void reEnableMovement()
    {
        framesSinceEntering = 0;
    }

    private void moveArrows(Vector3 movement)
    {
        GameObject rightArrow = GameObject.FindGameObjectWithTag("RightArrowParent");
        GameObject upArrow = GameObject.FindGameObjectWithTag("UpArrowParent");
        GameObject leftArrow = GameObject.FindGameObjectWithTag("LeftArrowParent");
        GameObject downArrow = GameObject.FindGameObjectWithTag("DownArrowParent");

        rightArrow.transform.Translate(movement * Time.deltaTime * 2);
        upArrow.transform.Translate(movement * Time.deltaTime * 2);
        leftArrow.transform.Translate(movement * Time.deltaTime * 2);
        downArrow.transform.Translate(movement * Time.deltaTime * 2);
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
        }
    }

    public override void OnGazeEnter(RaycastHit hit)
    {
        entered = true;
        highlightMaterial();
    }

    public override void OnGazeStay(RaycastHit hit)
    {
    }

    public override void OnGazeExit()
    {
        entered = false;
        reEnableMovement();
        resetMaterial();
    }

    private void highlightMaterial()
    {
        gameObject.transform.renderer.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
    }


    private void resetMaterial()
    {
        gameObject.transform.renderer.material.shader = Shader.Find("Diffuse");
    }


}
