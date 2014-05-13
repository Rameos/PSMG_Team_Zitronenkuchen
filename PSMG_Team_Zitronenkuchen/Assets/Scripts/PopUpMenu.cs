using UnityEngine;
using System.Collections;
using iViewX;

public class PopUpMenu : MonoBehaviour {

    // Setup your style of the Buttons
    // Note: you must define your own style.
    public GUIStyle myStyle;

    // Save all GazeButtonElements in an arrayList / List
    private ArrayList gazeUI = new ArrayList();
    // Set an Status for the Drawing of the Elements
    private bool isDrawing = false;


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
        Debug.Log("Button3_Pressed");
    }
    #endregion

	// Use this for initialization
	void Start (){

    }

    public void openMenu(Vector3 pos)
    {
        //Debug.Log(pos);
        //Set the Actions of the Buttons
       
        buttonCallbackListener createMilitaryNodeButton = button1_Action;
        buttonCallbackListener createResearchNodeButton = button2_Action;
        buttonCallbackListener createEconomyNodeButton = button3_Action;

        //Create new Buttonelements and add them to the gazeUI
        gazeUI.Add(new GazeButton(new Rect(pos.x + 100, pos.y - 150, 300, 150), "Create Military Node", myStyle, createMilitaryNodeButton));
        gazeUI.Add(new GazeButton(new Rect(pos.x + 150, pos.y, 300, 150), "Create Research Node", myStyle, createResearchNodeButton));
        gazeUI.Add(new GazeButton(new Rect(pos.x + 100, pos.y + 150, 300, 150), "Create Economy Node", myStyle, createEconomyNodeButton));

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
    }
	
	// Update is called once per frame
	void Update () {
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
                isDrawing = false;
            else
                isDrawing = true;

        }

        // Important Note: Please create a "SelectGUI" input in the InputManager of Unity.
        else if (Input.GetButtonUp("SelectGUI"))
        {
            isDrawing = false;
            gazeUI.Clear();
            closeMenu();
        }
	}
}
