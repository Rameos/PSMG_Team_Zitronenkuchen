using UnityEngine;
using System.Collections;
using iViewX;

/**
 * This script is responsible for showing a popup menu for fields that are in the players's influence area, but do not have a specialisation yet. It also handels its own button actions(building)
 **/
public class PopUpMenu : MonoBehaviour {

    // style of the Buttons
    public GUIStyle myStyle;

    // sound to be played when a button action is selected
    public AudioClip select;

    private static bool menuOpen = false;

    private ArrayList gazeUI = new ArrayList();
    private bool isDrawing = false;

    private GameObject selectedHexagon;

    private MainController mainController;

    private Vector3 pos;

    private int selectedRace = CustomGameProperties.alienRace;

    #region ButtonActions

    // Build Military: 
    public void button1_Action()
    {
        if (mainController.build("Military", selectedHexagon, pos))
        {
            NetworkView nview = selectedHexagon.networkView;
            NetworkViewID nviewId = nview.viewID;
            int builder = Network.isServer ? 1 : 2;
            nview.RPC("buildMilitary", RPCMode.AllBuffered, nviewId, selectedRace, builder);
            nview.RPC("fieldSet", RPCMode.AllBuffered);
        }


    }

    // Build Economy: 
    public void button3_Action()
    {
        if (mainController.build("Economy", selectedHexagon, pos))
        {
            NetworkView nview = selectedHexagon.networkView;
            NetworkViewID nviewId = nview.viewID;
            nview.RPC("buildEconomy", RPCMode.AllBuffered, nviewId, selectedRace);
            nview.RPC("fieldSet", RPCMode.AllBuffered);
        }

    }
    #endregion


	// Use this for initialization
	void Start (){
        mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }

    // called from changefieldstateonclick when space is pressed on a field that is in the players's influence area, but does not have a specialisation yet.
    public void openMenu(Vector3 pos, GameObject hex, ChangeFieldStateOnClick script)
    {
        if ((hex.GetComponent<HexField>().owner == 1 && Network.isServer) || (hex.GetComponent<HexField>().owner == 2 && Network.isClient))
        {
            this.pos = pos;

            selectedHexagon = hex;

            buttonCallbackListener createMilitaryNodeButton = button1_Action;
            buttonCallbackListener createEconomyNodeButton = button3_Action;

            //Create new Buttonelements and add them to the gazeUI
            gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y - 150, 220, 200), "150 \n CREATE \n MILITARY NODE", myStyle, createMilitaryNodeButton));
            gazeUI.Add(new GazeButton(new Rect(pos.x - 100 , pos.y + 50, 220, 200), "100 \n CREATE \n ECONOMY NODE", myStyle, createEconomyNodeButton));
        }
        menuOpen = true;
        
    }

    void closeMenu()
    {
        ChangeFieldStateOnClick.resetHighlighting(selectedHexagon);
        menuOpen = false;

    }

    public static bool isOpen()
    {
        return menuOpen;
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
