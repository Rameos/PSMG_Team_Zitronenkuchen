using UnityEngine;
using System.Collections;
using iViewX;

/**
 * This script is responsible for showing a popup menu for fields that have a military specialisation and handel its own button's actions like recruiting moving and attacking.
 **/
public class MilitaryMenu : MonoBehaviour {

    // style of the Buttons
    public GUIStyle myStyle;
    public GUISkin mySkin;

    // sound to be played when a button action is selected
    public AudioClip select;

    private static bool menuOpen = false;
 
    private ArrayList gazeUI = new ArrayList();
    private bool isDrawing = false;

    private GameObject selectedHexagon;
    private GameObject attackingHex;

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
            // only continue if there are troops to attack with
            view.RPC("prepareShip", RPCMode.AllBuffered);
            mainController.startTroopSend(troops, attack);
        }
    }

    // Move troops start action
    public void button2_Action()
    {
        if (!clicked) 
        {
            NetworkView view = attackingHex.networkView;

            bool attack = false;
            int troops = mainController.moveTroops(selectedHexagon);

            if (troops > 0)
            {
                // only continue if there are troops to move
                view.RPC("prepareShip", RPCMode.AllBuffered);
                mainController.startTroopSend(troops, attack);
            }            

            clicked = true;
        }
       
    }

    // Recruit action
    public void button3_Action()
    {

        if (!clicked)
        {
            if (mainController.buildTroops())
            {
                // only increase recruitcounter if the player can afford it
                ((MilitarySpecialisation) selectedHexagon.GetComponent<HexField>().spec).RecruitCounter += 25;
               
            }
            clicked = true;
        }
        ChangeFieldStateOnClick.resetHighlighting(selectedHexagon);
        
    }

    // Move here action
    public void button4_Action()
    {        
        if (!clicked)
        {
             
            NetworkView attackingView = attackingHex.networkView;
            NetworkViewID attackingViewId = attackingView.viewID;
            attackingView.RPC("unPrepareShip", RPCMode.AllBuffered);
            NetworkView selectedView = selectedHexagon.networkView;
            NetworkViewID selectedViewId = selectedView.viewID;
            attackingView.RPC("sendShip", RPCMode.AllBuffered, attackingViewId, selectedViewId);

            mainController.sendTroops(selectedHexagon);

            // reset Highlighting of possible targets
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

        // reset Highlighting of possible targets
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

        // reset Highlighting of possible targets
        foreach (GameObject highlighter in GameObject.FindGameObjectsWithTag("Highlighter"))
        {
            Destroy(highlighter);
        }
        mainController.resetRanges();
    }

    // Specialise on Laser
    public void button7_Action()
    {
        ((MilitarySpecialisation)selectedHexagon.GetComponent<HexField>().spec).WeaponType = MilitarySpecialisation.LASER;
        placeEmptySpaceShip();
        type = "LASER";
    }
        
    // Specialise on Protons
    public void button8_Action()
    {
        ((MilitarySpecialisation)selectedHexagon.GetComponent<HexField>().spec).WeaponType = MilitarySpecialisation.PROTONS;
        placeEmptySpaceShip();
        type = "PROTONS";
    }
    // Specialise on EMP
    public void button9_Action()
    {
        ((MilitarySpecialisation)selectedHexagon.GetComponent<HexField>().spec).WeaponType = MilitarySpecialisation.EMP;
        placeEmptySpaceShip();
        type = "EMP";
    }

    #endregion

    // Use this for initialization
    void Start()
    {
        mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }

    // called from changefieldstateonclick when space is pressed on a military or base node
    public void openMenu(Vector3 pos, GameObject hex, ChangeFieldStateOnClick script)
    { 
        selectedHexagon = hex;
        //Set the Actions of the Buttons 
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

        //Create new Buttonelements and add them to the gazeUI
        if (!isSending && ((hex.GetComponent<HexField>().owner == 1 && Network.isServer) || (hex.GetComponent<HexField>().owner == 2 && Network.isClient)))
        {
            // troops are not sent and player tries to open popupmenu on his own military or base node
            if (!(hex.GetComponent<HexField>().spec is BaseSpecialisation))
            {
                // military node
                if (((MilitarySpecialisation)hex.GetComponent<HexField>().spec).WeaponType == 0)
                {
                    // Military node not specialised yet. show specialisation options
                    showInfoPanel = false;
                    gazeUI.Add(new GazeButton(new Rect(pos.x - 110, pos.y - 180, 220, 200), "LASER \n FLEET", myStyle, laser));
                    gazeUI.Add(new GazeButton(new Rect(pos.x + 20, pos.y, 220, 200), " PROTON \n TORPEDO FLEET", myStyle, protons));
                    gazeUI.Add(new GazeButton(new Rect(pos.x - 240, pos.y, 220, 200), " EMP \n FLEET", myStyle, emp));
                }
                else
                {
                    // military node is already specialised. show info panel
                    showInfoPanel = true;
                    if (((MilitarySpecialisation)hex.GetComponent<HexField>().spec).Troops < 100)
                    {
                        // troop max not reached. recruiting is possible
                        attackingHex = selectedHexagon;
                        if (((MilitarySpecialisation)hex.GetComponent<HexField>().spec).RecruitCounter == 0)
                        {
                            gazeUI.Add(new GazeButton(new Rect(pos.x - 110, pos.y + 50, 220, 200), "ATTACK", myStyle, attackButton));
                            gazeUI.Add(new GazeButton(new Rect(pos.x + 40, pos.y - 80, 220, 200), " \n MOVE TROOPS", myStyle, moveButton));
                            gazeUI.Add(new GazeButton(new Rect(pos.x - 260, pos.y - 80, 220, 200), "150 \n BUILD SHIPS", myStyle, buildButton));
                        }   
                    }
                    else
                    {
                        gazeUI.Add(new GazeButton(new Rect(pos.x - 260, pos.y - 80, 220, 200), "ATTACK", myStyle, attackButton));
                        gazeUI.Add(new GazeButton(new Rect(pos.x + 40, pos.y - 80, 220, 200), " \n MOVE TROOPS", myStyle, moveButton));
                    }
                }
            }
            // Base node. only info panel is shown
            else showInfoPanel = true;
        }
        else if (isSending && hex.GetComponent<HexField>().InRange) 
        {
            // troops are being sent
            if ((hex.GetComponent<HexField>().owner == 2 && Network.isServer) || (hex.GetComponent<HexField>().owner == 1 && Network.isClient))
            {
                // player opened menu on enemy node
                gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y - 150, 220, 200), "ATACK HERE", myStyle, attacking));
            }
            else
            {
                // player opened menu on own node
                if (!(hex == attackingHex))
                gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y - 150, 220, 200), "MOVE HERE", myStyle, moving));
            }
            showInfoPanel = false;
            gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y +  50, 220, 200), "CANCEL", myStyle, canceling));
                
        } if (isSending && hex == attackingHex)
        {
            // plaver opened menu on the node from which he is moving/attacking. enable cancel
            showInfoPanel = false;
            gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y + 50, 220, 200), "CANCEL", myStyle, canceling));

        }
        menuOpen = true;
    }

    void closeMenu()
    {
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
                    // show weaponname and troopcount in the infopanel
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