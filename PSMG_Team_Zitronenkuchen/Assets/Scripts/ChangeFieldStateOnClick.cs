using UnityEngine;
using System.Collections;
using iViewX;

public class ChangeFieldStateOnClick : MonoBehaviourWithGazeComponent
{
    private Material defaultMaterial = Resources.Load("DefaultMaterial", typeof(Material)) as Material;
    private Material highlightedMaterial = Resources.Load("HighlightedMaterial", typeof(Material)) as Material;

    PopUpMenu popUpMenu;

    private bool menuIsOpen = false;

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
        gameObject.transform.renderer.material = defaultMaterial;
        
    }

    // show the popup menu when a field was clicked
    private void showPopupMenu(Vector3 pos)
    {
        Debug.Log("showPopupMenu");
        menuIsOpen = true;
        popUpMenu = GameObject.FindWithTag("PointLight").GetComponent<PopUpMenu>();
        popUpMenu.openMenu(pos);
    }

}
