using UnityEngine;
using System.Collections;
using iViewX;

public class MilitaryMenu : MonoBehaviour {

    // Setup your style of the Buttons
    // Note: you must define your own style.
    public GUIStyle myStyle;
    public GUISkin mySkin;

    public AudioClip select;

    private static bool menuOpen = false;

    // Save all GazeButtonElements in an arrayList / List
    private ArrayList gazeUI = new ArrayList();
    // Set an Status for the Drawing of the Elements
    private bool isDrawing = false;

    private Vector3 positionOfHexagon;
    private GameObject militaryBuilding;
    private GameObject researchBuilding;
    private GameObject economyBuilding;

    private GameObject selectedHexagon;
    private GameObject attackingHex;

    private ChangeFieldStateOnClick fieldScript;
    private MainController mainController;

    private bool clicked = false;
    private bool showInfoPanel = false;

    private string type;
    private int troopSize;

    #region ButtonActions
    // Attack start action 
    public void button1_Action()
    {
        NetworkView view = attackingHex.networkView;
       
        bool attack = true;
        int troops = mainController.moveTroops(selectedHexagon);
        if (troops > 0)
        {
            view.RPC("prepareShip", RPCMode.AllBuffered);
            mainController.startTroopSend(troops, attack);
        }
        Debug.Log("Button1_Pressed");
    }

    // Move troops start action
    public void button2_Action()
    {
        NetworkView view = attackingHex.networkView;
        
        bool attack = false;
        int troops = mainController.moveTroops(selectedHexagon);
        //mainController.setRanges();
        if (troops > 0)
        {
            view.RPC("prepareShip", RPCMode.AllBuffered);
            mainController.startTroopSend(troops, attack);
        }
        Debug.Log("Button2_Pressed");
    }
    // Recruit action
    public void button3_Action()
    {

        if (!clicked)
        {
            if (mainController.buildTroops())
            {
                ((MilitarySpecialisation) selectedHexagon.GetComponent<HexField>().spec).RecruitCounter += 5;
               
            }
            clicked = true;
            Debug.Log("Button3_Pressed");
        }
        ChangeFieldStateOnClick.resetHighlighting(selectedHexagon);
        
    }
    // Move here action
    public void button4_Action()
    {
        NetworkView attackingView = attackingHex.networkView;
        NetworkViewID attackingViewId = attackingView.viewID;
        attackingView.RPC("unPrepareShip", RPCMode.AllBuffered);
        NetworkView selectedView = selectedHexagon.networkView;
        NetworkViewID selectedViewId = selectedView.viewID;
        attackingView.RPC("sendShip", RPCMode.AllBuffered, attackingViewId, selectedViewId);

        mainController.sendTroops(selectedHexagon);


        Debug.Log("Button4_Pressed");
        if (!clicked)
        {
            mainController.sendTroops(selectedHexagon);
            Debug.Log("Button4_Pressed");
            foreach (GameObject highlighter in GameObject.FindGameObjectsWithTag("Highlighter"))
            {
                Destroy(highlighter);
            }
            mainController.resetRanges();
            clicked = true;
        }
    }

    // Attack here action
    public void button5_Action()
    {
        mainController.sendAttack(selectedHexagon);        
        
        NetworkView attackingView = attackingHex.networkView;
        NetworkViewID attackingViewId = attackingView.viewID;
        NetworkView selectedView = selectedHexagon.networkView;
        NetworkViewID selectedViewId = selectedView.viewID;
        attackingView.RPC("sendShip", RPCMode.AllBuffered, attackingViewId, selectedViewId);

        Debug.Log("Button5_Pressed");
        foreach (GameObject highlighter in GameObject.FindGameObjectsWithTag("Highlighter"))
        {
            Destroy(highlighter);
        }
        mainController.resetRanges();
    }
    // Cancel Troop send action
    public void button6_Action()
    {
        mainController.cancelTroopMovement();
        NetworkView view = attackingHex.networkView;
        view.RPC("unPrepareShip", RPCMode.AllBuffered);
        Debug.Log("Button6_Pressed");
        foreach (GameObject highlighter in GameObject.FindGameObjectsWithTag("Highlighter"))
        {
            Destroy(highlighter);
        }
        mainController.resetRanges();
    }
    // Specialise on Laser
    public void button7_Action()
    {
        Debug.Log("LASER");
        ((MilitarySpecialisation)selectedHexagon.GetComponent<HexField>().spec).WeaponType = MilitarySpecialisation.LASER;
        placeEmptySpaceShip();
        type = "LASER";
    }
    // Specialise on Protons
    public void button8_Action()
    {
        Debug.Log("PROTONS");
        ((MilitarySpecialisation)selectedHexagon.GetComponent<HexField>().spec).WeaponType = MilitarySpecialisation.PROTONS;
        placeEmptySpaceShip();
        type = "PROTONS";
    }
    // Specialise on EMP
    public void button9_Action()
    {
        Debug.Log("EMP");
        ((MilitarySpecialisation)selectedHexagon.GetComponent<HexField>().spec).WeaponType = MilitarySpecialisation.EMP;
        placeEmptySpaceShip();
        type = "EMP";
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
        buttonCallbackListener laser = button7_Action;
        buttonCallbackListener protons = button8_Action;
        buttonCallbackListener emp = button9_Action;

        bool isSending = mainController.isSending()>0;


        Debug.Log(isSending + ", " + hex.GetComponent<HexField>().InRange);
        //Create new Buttonelements and add them to the gazeUI
        if (!isSending && ((hex.GetComponent<HexField>().owner == 1 && Network.isServer) || (hex.GetComponent<HexField>().owner == 2 && Network.isClient)))
        {
            if (!(hex.GetComponent<HexField>().spec is BaseSpecialisation))
            {
                Debug.Log(((MilitarySpecialisation)hex.GetComponent<HexField>().spec).WeaponType);
                if (((MilitarySpecialisation)hex.GetComponent<HexField>().spec).WeaponType == 0)
                {
                    showInfoPanel = false;
                    gazeUI.Add(new GazeButton(new Rect(pos.x - 110, pos.y - 180, 220, 200), "LASER \n FLEET", myStyle, laser));
                    gazeUI.Add(new GazeButton(new Rect(pos.x + 20, pos.y, 220, 200), " PROTON \n TORPEDO FLEET", myStyle, protons));
                    gazeUI.Add(new GazeButton(new Rect(pos.x - 240, pos.y, 220, 200), " EMP \n FLEET", myStyle, emp));
                }
                else
                {
                    if (((MilitarySpecialisation)hex.GetComponent<HexField>().spec).Troops < 100)
                    {
                        attackingHex = selectedHexagon;
                        showInfoPanel = true;
                        if (((MilitarySpecialisation)hex.GetComponent<HexField>().spec).RecruitCounter == 0)
                        {
                            gazeUI.Add(new GazeButton(new Rect(pos.x - 110, pos.y + 50, 220, 200), "ATTACK", myStyle, attackButton));
                            gazeUI.Add(new GazeButton(new Rect(pos.x + 40, pos.y - 80, 220, 200), " \n MOVE TROOPS", myStyle, moveButton));
                            gazeUI.Add(new GazeButton(new Rect(pos.x - 260, pos.y - 80, 220, 200), "150 \n BUILD SHIPS", myStyle, buildButton));
                        }   
                    }
                    else
                    {
                        showInfoPanel = true;
                        gazeUI.Add(new GazeButton(new Rect(pos.x - 260, pos.y - 80, 220, 200), "ATTACK", myStyle, attackButton));
                        gazeUI.Add(new GazeButton(new Rect(pos.x + 40, pos.y - 80, 220, 200), " \n MOVE TROOPS", myStyle, moveButton));
                    }
                }
            }
            else showInfoPanel = true;
        }
        else if (isSending && hex.GetComponent<HexField>().InRange) // troops are being sent
        {
            Debug.Log("Owner: "+hex.GetComponent<HexField>().owner+", Is Server?"+Network.isServer);
            if ((hex.GetComponent<HexField>().owner == 2 && Network.isServer) || (hex.GetComponent<HexField>().owner == 1 && Network.isClient))
            {
                gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y - 150, 220, 200), "ATACK HERE", myStyle, attacking));
            }
            else
            {
                gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y - 150, 220, 200), "MOVE HERE", myStyle, moving));
            }
            showInfoPanel = false;
            gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y +  50, 220, 200), "CANCEL", myStyle, canceling));
                
        }
        //Debug.Log(gazeUI);
        menuOpen = true;
    }

    void closeMenu()
    {
        Debug.Log("closeMenu");
        GameObject field = GameObject.FindGameObjectWithTag("Field");
        int layerDef = LayerMask.NameToLayer("Default");
        moveToLayer(field.transform, layerDef);
        Debug.Log("Was is mit meinem hex passiert??" + selectedHexagon);
        //ChangeFieldStateOnClick.resetHighlighting(selectedHexagon);
        menuOpen = false;
        clicked = false;
        showInfoPanel = false;
    }

    private void placeEmptySpaceShip()
    {
        NetworkView view = selectedHexagon.networkView;
        NetworkViewID id = view.viewID;
        view.RPC("initiateTroopBuilding", RPCMode.AllBuffered, CustomGameProperties.alienRace, id);
       
    }

    public static bool isOpen()
    {
        return menuOpen;
    }

    void moveToLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            moveToLayer(child, layer);
    }

    void OnGUI()
    {
        GUI.skin = mySkin;
        //Draw every Button from the ArrayList gazeUI
        if (isDrawing && selectedHexagon != null)
        {
            if ((selectedHexagon.GetComponent<HexField>().owner == 1 && Network.isServer) || (selectedHexagon.GetComponent<HexField>().owner == 2 && Network.isClient))
            {
                if (showInfoPanel)
                {
                    if (selectedHexagon.GetComponent<HexField>().spec is MilitarySpecialisation)
                    {
                        troopSize = ((MilitarySpecialisation)selectedHexagon.GetComponent<HexField>().spec).Troops;
                        type = ((MilitarySpecialisation)selectedHexagon.GetComponent<HexField>().spec).WeaponName;
                    }
                    else
                    {
                        troopSize = ((BaseSpecialisation)selectedHexagon.GetComponent<HexField>().spec).Troops;
                        type = ((BaseSpecialisation)selectedHexagon.GetComponent<HexField>().spec).WeaponName;
                    }
                    GUI.Box(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 180, 250, 200), "FLEET TYPE: \n " + type + "\n FLEET SIZE: \n" + troopSize);
                }
            }
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