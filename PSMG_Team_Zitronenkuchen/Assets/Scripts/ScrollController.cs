using UnityEngine;
using System.Collections;
using iViewX;

public class ScrollController : MonoBehaviourWithGazeComponent
{

    private int framesSinceEntering = 0;
    private bool entered = false;

    private int xDirection = 1; //default 1, -1 if opposite direction, 0 if no movement
    private int yDirection = 1; //default 1, -1 if opposite direction, 0 if no movement

    private float maxX = 21.1055f;
    private float minX = 0.0f;

    private float maxZ = 21.1055f;
    private float minZ = 0.0f;

    private float left = 5.0f;
    private float down = 5.0f;
    private float right;
    private float up;

    private GameObject camera;

    private GameObject gameField;
    private float borderLeft;
    private float borderRight;
    private float borderTop;
    private float borderBottom;

    private float speed = 0.5f;

    private string direction;

    private float viewPortX;
    private float viewPortY;

    private Vector3 movement;
    // Use this for initialization
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("CameraWrapper");


        right = Screen.width-5.0f;
        up = Screen.height-5.0f;

        gameField = GameObject.FindGameObjectWithTag("gameTerrain");
        borderLeft = gameField.transform.position.x + 1.0f;
        borderBottom = gameField.transform.position.z;
        borderTop = CreateGameField.getFieldHeight() - 0.7f;
        borderRight = CreateGameField.getFieldWidth() - 0.7f;
    }

    // Update is called once per frame
    void Update()
    {

        //diagonal movement already possible

        if (Input.mousePosition.x <= left)
        {
            moveCamera("Left");
        }

        if (Input.mousePosition.x >= right)
        {
            moveCamera("Right");
        }

        if (Input.mousePosition.y <= down)
        {
            moveCamera("Down");
        }

        if (Input.mousePosition.y >= up)
        {
            moveCamera("Up");
        }

    }

    void OnGUI()
    {


    }

    private void moveCamera(string direction)
    {

        determineDirection(direction);
        movement = new Vector3(speed * xDirection, 0, speed * yDirection);
        camera.transform.Translate(movement * Time.deltaTime * 2, Space.World);

       
    }

    private bool hitBottomBorder()
    {
        if (camera.transform.position.z <= borderBottom)
        {
            return true;
        }
        return false;
    }

    private bool hitTopBorder()
    {

        Debug.Log("this is the fieldsize" + borderTop);
        if (camera.transform.position.z >= borderTop)
        {
            return true;
        }
        return false;
    }

    private bool hitLeftBorder()
    {
        if (camera.transform.position.x <= borderLeft)
        {
            return true;
        }
        return false;
    }
    private bool hitRightBorder()
    {
        if (camera.transform.position.x >= borderRight)
        {
            return true;
        }
        return false;
    }

    private void reEnableMovement()
    {
        framesSinceEntering = 0;
    }

    private void determineDirection(string direction)
    {
        switch (direction)
        {
            case "Left":
                xDirection = (hitLeftBorder())?0:-1;
                yDirection = 0;
                break;
            case "Right":
                xDirection = (hitRightBorder()) ? 0 :1;
                yDirection = 0;
                break;
            case "Up":
                xDirection = 0;
                yDirection = (hitTopBorder()) ? 0 :1;
                break;
            case "Down":
                xDirection = 0;
                yDirection = (hitBottomBorder()) ? 0 : -1;
                break;
            /* might use later, when eyetrackerscrolling returns :(
            case "UpRight":
                xDirection = 1;
                yDirection = 1;
                break;
            case "DownRight":
                xDirection = 1;
                yDirection = -1;
                break;
            case "DownLeft":
                xDirection = -1;
                yDirection = -1;
                break;
            case "UpLeft":
                xDirection = -1;
                yDirection = 1;
                break; */
        }
    }

    public override void OnGazeEnter(RaycastHit hit)
    {
        entered = true;
    }

    public override void OnGazeStay(RaycastHit hit)
    {
    }

    public override void OnGazeExit()
    {
        entered = false;
    }


}