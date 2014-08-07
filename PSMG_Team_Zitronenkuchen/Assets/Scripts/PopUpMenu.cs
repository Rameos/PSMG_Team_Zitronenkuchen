using UnityEngine;
using System.Collections;
using iViewX;

public class PopUpMenu : MonoBehaviour {

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

    private MainController mainController;

    private Vector3 pos;

    #region ButtonActions
    // Build Military: 
    public void button1_Action()
    {
        Debug.Log("Button1_Pressed");
        if (mainController.build("Military", selectedHexagon, pos))
        {
            NetworkView nview = selectedHexagon.networkView;
            NetworkViewID nviewId = nview.viewID;
            nview.RPC("buildMilitary", RPCMode.AllBuffered, nviewId);
            nview.RPC("fieldSet", RPCMode.AllBuffered);
        }


    }

    // Build Research: 
    //public void button2_Action()
    //{
    //    Debug.Log("Button2_Pressed");
    //    if (mainController.build("Research", selectedHexagon, pos))
    //    {
    //        NetworkView nview = selectedHexagon.networkView;
    //        NetworkViewID nviewId = nview.viewID;
    //        nview.RPC("buildResearch", RPCMode.AllBuffered, nviewId);
    //        nview.RPC("fieldSet", RPCMode.AllBuffered);
    //    }
    //}

    // Build Economy: 
    public void button3_Action()
    {
        Debug.Log("Button3_Pressed");
        if (mainController.build("Economy", selectedHexagon, pos))
        {
            NetworkView nview = selectedHexagon.networkView;
            NetworkViewID nviewId = nview.viewID;
            nview.RPC("buildEconomy", RPCMode.AllBuffered, nviewId);
            nview.RPC("fieldSet", RPCMode.AllBuffered);
        }

    }
    #endregion


	// Use this for initialization
	void Start (){
        Debug.Log("StartPopup");
        mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }

    public void openMenu(Vector3 posx, GameObject hex, ChangeFieldStateOnClick script)
    {
        Debug.Log(hex.GetComponent<HexField>().owner);
        if ((hex.GetComponent<HexField>().owner == 1 && Network.isServer) || (hex.GetComponent<HexField>().owner == 2 && Network.isClient))
        {
            //this.pos = pos;
            pos = new Vector3(Screen.width / 2, Screen.height/2, 0);

            selectedHexagon = hex;

            buttonCallbackListener createMilitaryNodeButton = button1_Action;
            // buttonCallbackListener createResearchNodeButton = button2_Action;
            buttonCallbackListener createEconomyNodeButton = button3_Action;

            //Create new Buttonelements and add them to the gazeUI
            gazeUI.Add(new GazeButton(new Rect(pos.x - 100, pos.y - 150, 220, 200), "150 \n CREATE \n MILITARY NODE", myStyle, createMilitaryNodeButton));
            // gazeUI.Add(new GazeButton(new Rect(pos.x, pos.y-50, 300, 150), "Create Research Node", myStyle, createResearchNodeButton));
            gazeUI.Add(new GazeButton(new Rect(pos.x - 100 , pos.y + 50, 220, 200), "100 \n CREATE \n ECONOMY NODE", myStyle, createEconomyNodeButton));
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
