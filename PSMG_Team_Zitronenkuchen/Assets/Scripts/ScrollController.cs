using UnityEngine;
using System.Collections;
using iViewX;

public class ScrollController : MonoBehaviourWithGazeComponent
{

    private int selectionIndex; //0 for left, 1 for right, 2 for up, 3 for down

    private int framesSinceEntering = 0;
    private bool entered = false;

    private int xDirection = 1; //default 1, -1 if opposite direction
    private int yDirection = 1; //default 1, -1 if opposite direction

    private float enterTime;
	// Use this for initialization
	void Start () {
       
	}

    void moveToLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            moveToLayer(child, layer);
    }
	
	// Update is called once per frame
	void Update () {
        if (entered)
        {
            framesSinceEntering++;
            if (framesSinceEntering == 5)
            {
                moveCamera();
                
            }
        }
        
       
	}

    private void moveCamera()
    {
        determineDirection();
        Vector3 movement = new Vector3(0.2f * xDirection, 0, 0.05f * yDirection);

        if (Camera.current != null)
        {
            Camera.current.transform.Translate(movement);
        }

        moveArrows(movement);

    }

    private void moveArrows(Vector3 movement)
    {
        GameObject rightArrow = GameObject.FindGameObjectWithTag("RightArrowParent");
        GameObject upArrow = GameObject.FindGameObjectWithTag("UpArrowParent");
        GameObject leftArrow = GameObject.FindGameObjectWithTag("LeftArrowParent");
        GameObject downArrow = GameObject.FindGameObjectWithTag("DownArrowParent");

        rightArrow.transform.Translate(movement);
        upArrow.transform.Translate(movement);
        leftArrow.transform.Translate(movement);
        downArrow.transform.Translate(movement);
    }

    private void determineDirection()
    {
        switch (gameObject.tag)
        {
            case "LeftArrow":
                xDirection = -1;
                yDirection = 0;
                break;
            case "RightArrow":
                xDirection = 1;
                yDirection = 0;
                break;
            case "UpArrow":
                xDirection = 0;
                yDirection = 1;
                break;
            case "DownArrow":
                xDirection = 0;
                yDirection = -1;
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
        framesSinceEntering = 0;
        entered = false;
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
