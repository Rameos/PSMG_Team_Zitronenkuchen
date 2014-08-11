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

    #region ButtonActions
    // Attack start action 
    public void button1_Action()
    {
        bool attack = true;
        int troops = mainController.moveTroops(selectedHexagon);
        if (troops > 0)
        {
            mainController.startTroopSend(troops, attack);
        }
        Debug.Log("Button1_Pressed");
    }

    // Move troops start action
    public void button2_Action()
    {
        bool attack = false;
        int troops = mainController.moveTroops(selectedHexagon);
        if (troops > 0)
        {
            mainController.startTroopSend(troops, attack);
        }
        Debug.Log("Button2_Pressed");
    }
    // Recruit action
    public void button3_Action()
    {
        if (mainController.buildTroops())
        {
            ((MilitarySpecialisation) selectedHexagon.GetComponent<HexField>().spec).RecruitCounter += 5;
        }
        Debug.Log("Button3_Pressed");
    }
    // Move here action
    public void button4_Action()
    {
        mainController.sendTroops(selectedHexagon);
        Debug.Log("Button4_Pressed");
    }
    // Attack here action
    public void button5_Action()
    {
        mainController.sendAttack(selectedHexagon);
        Debug.Log("Button5_Pressed");
    }
    // Cancel Troop send action
    public void button6_Action()
    {
        mainController.cancelTroopMovement();
        Debug.Log("Button6_Pressed");
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
        //Debug.Log(hex.GetComponent<HexField>().owner);

        //Set the Actions of the Buttons
        

        fieldScript = script;

        selectedHexagon = hex;

        buttonCallbackListener attackButton = button1_Action;
        buttonCallbackListener moveButton = button2_Action;
        buttonCallbackListener buildButton = button3_Action;
        buttonCallbackListener moving = button4_Action;
        buttonCallbackListener attacking = button5_Action;
        buttonCallbackListener canceling = button6_Action;
        bool isSending = mainController.isSending()>0;

        //Debug.Log(isSending);
        //Create new Buttonelements and add them to the gazeUI
        if (!isSending && ((hex.GetComponent<HexField>().owner == 1 && Network.isServer) || (hex.GetComponent<HexField>().owner == 2 && Network.isClient)))
        {
            if (!(hex.GetComponent<HexField>().spec is BaseSpecialisation))
            {
                gazeUI.Add(new GazeButton(new Rect(pos.x - 110, pos.y - 180, 220, 200), "ATTACK", myStyle, attackButton));
                gazeUI.Add(new GazeButton(new Rect(pos.x + 20, pos.y, 220, 200), " \n MOVE TROOPS", myStyle, moveButton));
                gazeUI.Add(new GazeButton(new Rect(pos.x - 240, pos.y, 220, 200), "150 \n BUILD TROPPS", myStyle, buildButton));
            } 
        }
        else if (isSending) // troops are being sent
        {
            Debug.Log("Owner: "+hex.GetComponent<HexField>().owner+", Is Server?"+Network.isServer);
            if ((hex.GetComponent<HexField>().owner == 2 && Network.isServer) || (hex.GetComponent<HexField>().owner == 1 && Network.isClient))
            {
                gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y - 150, 220, 200), "ATACK HERE", myStyle, attacking));
                gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y +  50, 220, 200), "CANCEL", myStyle, canceling));
            }
            else
            {
                gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y - 150, 220, 200), "MOVE HERE", myStyle, moving));
                gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y +  50, 220, 200), "CANCEL", myStyle, canceling));
            }
                
        }
        //Debug.Log(gazeUI);
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
            //Debug.Log("You pressed Space");

        }
    }

    // Update is called once per frame
    void Update()
    {
        //Update only if the buttons are visible
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