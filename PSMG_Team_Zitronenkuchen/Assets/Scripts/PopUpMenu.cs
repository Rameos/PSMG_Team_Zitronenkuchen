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
	public void Start () {

        Debug.Log("Reached PopUp");
        //Set the Actions of the Buttons
        buttonCallbackListener createMilitaryNodeButton = button1_Action;
        buttonCallbackListener createResearchNodeButton = button2_Action;
        buttonCallbackListener createEconomyNodeButton = button3_Action;

        //Create new Buttonelements and add them to the gazeUI
        gazeUI.Add(new GazeButton(new Rect(Screen.width * 0.35f, Screen.height * 0.05f, 200, 100), "Create Military Node", myStyle, createMilitaryNodeButton));
        gazeUI.Add(new GazeButton(new Rect(Screen.width * 0.35f, Screen.height * 0.35f, 200, 100), "Create Research Node", myStyle, createResearchNodeButton));
        gazeUI.Add(new GazeButton(new Rect(Screen.width * 0.35f, Screen.height * 0.65f, 200, 100), "Create Economy Node", myStyle, createEconomyNodeButton));
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

        // Important Note: Please create a "SelectGUI" input in the InputManager of Unity.
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
        }
	}
}
