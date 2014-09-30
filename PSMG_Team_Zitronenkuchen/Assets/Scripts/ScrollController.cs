using UnityEngine;
using System.Collections;

/**
 * This class handles the scrolling of the camera along the map(including checking that the camera does not move outside the map borders)
 **/
public class ScrollController : MonoBehaviour
{



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
        setUpBorder();
        
    }

    //client camera has different rotation -> borders are opposed
    private void setUpBorder()
    {
      
       if (Network.isServer) {
           borderTop = CreateGameField.getFieldHeight() - 0.7f;
           borderBottom = gameField.transform.position.z;
           borderLeft = gameField.transform.position.x + 1.0f;
           borderRight = CreateGameField.getFieldWidth() - 0.7f;

       }
       else
       {
           borderBottom = CreateGameField.getFieldHeight();
           borderTop = gameField.transform.position.z;
           borderLeft = CreateGameField.getFieldWidth() - 0.7f;
           borderRight = gameField.transform.position.x;
       }
    }

    // Update is called once per frame
    void Update()
    {

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


    //move camera into desired direction
    private void moveCamera(string direction)
    {

        determineDirection(direction);
        movement = new Vector3(speed * xDirection, 0, speed * yDirection);
        camera.transform.Translate(movement * Time.deltaTime * 2);
        

       
    }

    //check for camera collision with bottom border
    private bool hitBottomBorder()
    {
        if (Network.isServer)
        {
            if (camera.transform.position.z <= borderBottom)
            {
                return true;
            }
        }
        else
        {
            if (camera.transform.position.z >= borderBottom)
            {
                return true;
            }
        }
        return false;
    }

    //check for camera collision with top border
    private bool hitTopBorder()
    {
        if (Network.isServer)
        {
            if (camera.transform.position.z >= borderTop)
            {
                return true;
            }
        }
        else
        {
            if (camera.transform.position.z <= borderTop)
            {
                return true;
            }
        }
        return false;
    }

    //check for camera collision with left border
    private bool hitLeftBorder()
    {
        if (Network.isServer)
        {
            if (camera.transform.position.x <= borderLeft)
            {
                return true;
            }
        }
        else
        {
            if (camera.transform.position.x >= borderLeft)
            {
                return true;
            }
        }
        return false;
    }

    //check for camera collision with right border
    private bool hitRightBorder()
    {
        if (Network.isServer)
        {
            if (camera.transform.position.x >= borderRight)
            {
                return true;
            }
        }
        else
        {
            if (camera.transform.position.x <= borderRight)
            {
                return true;
            }
        }
        return false;
    }

    //camera movement direction is determined
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
        }
    }



}