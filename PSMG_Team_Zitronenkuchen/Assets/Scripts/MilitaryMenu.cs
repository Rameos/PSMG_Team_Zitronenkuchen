using UnityEngine;
using System.Collections;
using iViewX;

public class MilitaryMenu : MonoBehaviour {

    // Setup your style of the Buttons
    // Note: you must define your own style.
    public GUIStyle myStyle;

    public AudioClip select;

    // Save all GazeButtonElements in an arrayList / List
    private ArrayList gazeUI = new ArrayList();
    // Set an Status for the Drawing of the Elements
    private bool isDrawing = false;

    private Vector3 positionOfHexagon;
    private GameObject militaryBuilding;
    private GameObject researchBuilding;
    private GameObject economyBuilding;

    private GameObject selectedHexagon;

    private ChangeFieldStateOnClick fieldScript;
    private MainController mainController;

    private Vector3 pos;

    #region ButtonActions
    // Action for Button_1: 
    public void button1_Action()
    {
        Debug.Log("Button1_Pressed");
    }

    // Action for Button_2: 
    public void button2_Action()
    {
        Debug.Log("Button2_Pressed");
    }
    // Action for Button_3: 
    public void button3_Action()
    {
        if (mainController.buildTroops())
        {
            selectedHexagon.GetComponent<HexField>().spec.BuildCounter += 5;
        }
        Debug.Log("Button3_Pressed");
    }
    public void button4_Action()
    {

    }

    public void button5_Action()
    {

    }

    #endregion

    // Use this for initialization
    void Start()
    {
        Debug.Log("StartPopup");
        mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }

    public void openMenu(Vector3 pos, GameObject hex, ChangeFieldStateOnClick script)
    {
        Debug.Log(hex.GetComponent<HexField>().owner);
        if (hex.GetComponent<HexField>().owner == 1)
        {

            //Set the Actions of the Buttons
            this.pos.x = Screen.width/2;
            this.pos.y = Screen.height / 2;

            fieldScript = script;

            selectedHexagon = hex;

            buttonCallbackListener attackButton = button1_Action;
            buttonCallbackListener moveButton = button2_Action;
            buttonCallbackListener buildButton = button3_Action;
            buttonCallbackListener moving = button4_Action;
            buttonCallbackListener attacking = button5_Action;

            //Create new Buttonelements and add them to the gazeUI
            if (true)
            {
                gazeUI.Add(new GazeButton(new Rect(pos.x + 100, pos.y - 150, 300, 150), "Attack", myStyle, attackButton));
                gazeUI.Add(new GazeButton(new Rect(pos.x + 150, pos.y, 300, 150), "Move Troops", myStyle, moveButton));
                gazeUI.Add(new GazeButton(new Rect(pos.x + 100, pos.y + 150, 300, 150), "Build Troops" + "\n" + "150", myStyle, buildButton));
            }
            else if (true)
            {

            }
            else if (true)
            {

            }
            Debug.Log(gazeUI);
        }

    }

    void closeMenu()
    {
        Debug.Log("closeMenu");
        GameObject field = GameObject.FindGameObjectWithTag("Field");
        int layerDef = LayerMask.NameToLayer("Default");
        moveToLayer(field.transform, layerDef);
    }

    void moveToLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            moveToLayer(child, layer);
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

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
        {
            Debug.Log("You pressed Space");

        }
    }

    // Update is called once per frame
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

        if (Input.GetButtonDown("SelectGUI"))
        {
            if (isDrawing)
            {
                isDrawing = false;
            }
            else
                isDrawing = true;
        }
        else if (Input.GetButtonUp("SelectGUI"))
        {
            isDrawing = false;
            collapseMenu();
        }
    }

    private void collapseMenu()
    {
        gazeUI.Clear();
        closeMenu();
    }

}