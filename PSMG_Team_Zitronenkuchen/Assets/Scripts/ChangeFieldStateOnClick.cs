using UnityEngine;
using System.Collections;
using iViewX;

public class ChangeFieldStateOnClick : MonoBehaviourWithGazeComponent
{
    public override void OnGazeEnter(RaycastHit hit)
    {
        Debug.Log("Enter");
    }

    //Rotate the Element if the Gaze stays on the Collider
    public override void OnGazeStay(RaycastHit hit)
    {
        //Debug.Log("Stay");
        highlightMaterial();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            showPopupMenu();
            Debug.Log("Hey Space");
        }
    }

    private void highlightMaterial()
    {
        gameObject.transform.renderer.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
    }

    //Reset the Element.Transform when the gaze leaves the Collider
    public override void OnGazeExit()
    {
        //Debug.Log("Exit");
        resetMaterial();
    }

    private void resetMaterial()
    {
        gameObject.transform.renderer.material.shader = Shader.Find("Diffuse");
    }

    // show the popup menu when a field was clicked
    private void showPopupMenu()
    {
        Debug.Log("showPopupMenu");
    }
}
