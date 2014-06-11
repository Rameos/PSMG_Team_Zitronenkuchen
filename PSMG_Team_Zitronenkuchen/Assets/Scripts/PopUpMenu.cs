﻿using UnityEngine;
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

    private ChangeFieldStateOnClick fieldScript;
    private MainController mainController;

    private Vector3 pos;

    #region ButtonActions
    // Action for Button_1: 
    public void button1_Action()
    {
        Debug.Log("Button1_Pressed");
        if (mainController.build("Military", selectedHexagon, pos))
        {
            GameObject milBuilding = Resources.Load("military-building", typeof(GameObject)) as GameObject;
            GameObject militaryBuilding = Instantiate(milBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
            militaryBuilding.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);
            Debug.Log(militaryBuilding.transform.position.ToString());
            Mesh militaryBuild = Resources.Load("military-building", typeof(Mesh)) as Mesh;
            selectedHexagon.renderer.material = Resources.Load("militaryMaterial", typeof(Material)) as Material;
            militaryBuilding.transform.parent = selectedHexagon.transform;
            fieldScript.fieldSet();
        }
        

    }

    // Action for Button_2: 
    public void button2_Action()
    {
        Debug.Log("Button2_Pressed");
        if (mainController.build("Research", selectedHexagon, pos))
        {
            GameObject resBuilding = Resources.Load("research-building", typeof(GameObject)) as GameObject;
            GameObject researchBuilding = Instantiate(resBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
            researchBuilding.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);
            selectedHexagon.renderer.material = Resources.Load("researchMaterial", typeof(Material)) as Material;
            researchBuilding.transform.parent = selectedHexagon.transform;
            fieldScript.fieldSet();
        }
    }
    // Action for Button_3: 
    public void button3_Action()
    {
        Debug.Log("Button3_Pressed");
        if (mainController.build("Economy", selectedHexagon, pos))
        {
            GameObject ecoBuilding = Resources.Load("economy-building 1", typeof(GameObject)) as GameObject;
            GameObject economyBuilding = Instantiate(ecoBuilding, selectedHexagon.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject; ;
            economyBuilding.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);
            selectedHexagon.renderer.material = Resources.Load("economyMaterial", typeof(Material)) as Material;
            economyBuilding.transform.parent = selectedHexagon.transform;
            fieldScript.fieldSet();
        }

    }
    #endregion

	// Use this for initialization
	void Start (){
        Debug.Log("StartPopup");
        mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }

    public void openMenu(Vector3 pos, GameObject hex, ChangeFieldStateOnClick script)
    {
        
        //Debug.Log(pos);
        //Set the Actions of the Buttons
        this.pos = pos;

        fieldScript = script;

        selectedHexagon = hex;
       
        buttonCallbackListener createMilitaryNodeButton = button1_Action;
        buttonCallbackListener createResearchNodeButton = button2_Action;
        buttonCallbackListener createEconomyNodeButton = button3_Action;

        //Create new Buttonelements and add them to the gazeUI
        gazeUI.Add(new GazeButton(new Rect(pos.x + 100, pos.y - 150, 300, 150), "Create Military Node", myStyle, createMilitaryNodeButton));
        gazeUI.Add(new GazeButton(new Rect(pos.x + 150, pos.y, 300, 150), "Create Research Node", myStyle, createResearchNodeButton));
        gazeUI.Add(new GazeButton(new Rect(pos.x + 100, pos.y + 150, 300, 150), "Create Economy Node", myStyle, createEconomyNodeButton));
        Debug.Log(gazeUI);
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
