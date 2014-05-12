using UnityEngine;
using System.Collections;
using iViewX;

public class ChangeFieldStateOnClick : MonoBehaviourWithGazeComponent
{
    private Material defaultMaterial = Resources.Load("DefaultMaterial", typeof(Material)) as Material;
    private Material highlightedMaterial = Resources.Load("HighlightedMaterial", typeof(Material)) as Material;

    private bool showPopUp = false;
    PopUpMenu popUpMenu;

    public override void OnGazeEnter(RaycastHit hit)
    {
        //Debug.Log("Enter");
    }

    //Rotate the Element if the Gaze stays on the Collider
    public override void OnGazeStay(RaycastHit hit)
    {
        //Debug.Log("Stay");
        gameObject.transform.renderer.material = highlightedMaterial;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 posGaze = (gazeModel.posGazeLeft + gazeModel.posGazeRight) * 0.5f;
            showPopupMenu(posGaze);
        }
    }

    //Reset the Element.Transform when the gaze leaves the Collider
    public override void OnGazeExit()
    {
        //Debug.Log("Exit");
        if (showPopUp == false)
        {
            gameObject.transform.renderer.material = defaultMaterial;
        }
        
    }

    // show the popup menu when a field was clicked
    private void showPopupMenu(Vector3 pos)
    {
        Debug.Log("showPopupMenu");
        popUpMenu = GameObject.FindWithTag("PointLight").GetComponent<PopUpMenu>();
        popUpMenu.openMenu(pos);
        showPopUp = true;
    }

    void OnGUI()
    {
        /*if (showPopUp == true)
        {
            GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 50, 200, 100), "Create Military Node");
            GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), "Create Research Node");
            GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 150, 200, 100), "Create Economy Node");
        }*/
    }
}
