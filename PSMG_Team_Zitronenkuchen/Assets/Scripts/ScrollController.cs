using UnityEngine;
using System.Collections;
using iViewX;

public class ScrollController : MonoBehaviourWithGazeComponent
{

    private int framesSinceEntering = 0;
    private bool entered = false;

    private int xDirection = 1; //default 1, -1 if opposite direction, 0 if no movement
    private int zDirection = 1; //default 1, -1 if opposite direction, 0 if no movement

    private float maxX = 21.1055f;
    private float minX = 0.0f;

    private float maxZ = 21.1055f;
    private float minZ = 0.0f;

    private double left = 5;
    private double down = 5;
    private double right;
    private double up;

    private float speed = 0.5f;

    private string direction;

    private Vector3 movement;
    // Use this for initialization
    void Start()
    {
        right = Screen.width-5;
        up = Screen.height-5;
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

    private void moveCamera(string direction)
    {

        determineDirection(direction);
        movement = new Vector3(speed * xDirection, 0, speed * zDirection);
        GameObject camera = GameObject.FindGameObjectWithTag("CameraWrapper");
        camera.transform.Translate(movement * Time.deltaTime * 2);

        if (camera.transform.position.x < minX)
        {
            xDirection = -1;
            camera.transform.Translate(0.2f * xDirection * Time.deltaTime * 2, 0, 0.2f * zDirection * Time.deltaTime * 2, Space.World);
        }

        if (camera.transform.position.x > maxX)
        {
            xDirection = 1;
            camera.transform.Translate(0.2f * xDirection * Time.deltaTime * 2, 0, 0.2f * zDirection * Time.deltaTime * 2, Space.World);
        }
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
                xDirection = -1;
                zDirection = 0;
                break;
            case "Right":
                xDirection = 1;
                zDirection = 0;
                break;
            case "Up":
                xDirection = 0;
                zDirection = 1;
                break;
            case "Down":
                xDirection = 0;
                zDirection = -1;
                break;
            case "UpRight":
                xDirection = 1;
                zDirection = 1;
                break;
            case "DownRight":
                xDirection = 1;
                zDirection = -1;
                break;
            case "DownLeft":
                xDirection = -1;
                zDirection = -1;
                break;
            case "UpLeft":
                xDirection = -1;
                zDirection = 1;
                break;
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