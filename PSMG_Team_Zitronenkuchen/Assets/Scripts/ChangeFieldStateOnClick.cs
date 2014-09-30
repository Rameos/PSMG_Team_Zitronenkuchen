using UnityEngine;
using System.Collections;
using iViewX;

/**
 * This script is assigned to every Hexfield
 * It manages highlighting the hexfield on gaze and the opening of popupmenues when spacebar is clicked
 **/
public class ChangeFieldStateOnClick : MonoBehaviourWithGazeComponent
{
    // if field is already specialised(built on) or not
    private bool set = false;
    

    public override void OnGazeEnter(RaycastHit hit)
    {
        // nothing to do here
    }

    public override void OnGazeStay(RaycastHit hit)
    {
        highlightMaterial();


        if (Input.GetKeyDown(KeyCode.Space))
        {
            // user gazing at the field and pressing space -> open menu
            Vector3 pos = new Vector3(Screen.width / 2, Screen.height / 2, 0);

          
            
            if (!set)
            {
                // field has no specialisation -> show normal popup(build) menu
                showPopupMenu(pos);
            }

            else if ((hit.transform.gameObject.GetComponent<HexField>().specialisation == "Military" || hit.transform.gameObject.GetComponent<HexField>().specialisation == "Base") && hit.transform.gameObject.GetComponent<HexField>().FinishedBuilding == true)
            {                
                // field has military or base specialisation
                showMilitaryMenu(pos);
            }
            else
            {
                // if user hits already set field that is nit military(own or opponent economy) -> nothing happens
            }
            
                
        }
    }

    public static void resetHighlighting(GameObject currentHex)
    {
        currentHex.transform.renderer.material.shader = Shader.Find("Diffuse");
    }

    private void highlightMaterial()
    {
        if (!PopUpMenu.isOpen() && !MilitaryMenu.isOpen())
        {
            // only highlight the hexfield the player gazes on when no menu is opened
            gameObject.transform.renderer.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
        }
    }

    //Reset the Element.Transform when the gaze leaves the Collider
    public override void OnGazeExit()
    {
        resetMaterial();
    }


    private void resetMaterial()
    {
        if (!PopUpMenu.isOpen() && !MilitaryMenu.isOpen())
        {
            // only reset the material on the hexfield the player gazes on when no menu is opened
            gameObject.transform.renderer.material.shader = Shader.Find("Diffuse"); 
        }
    }



    // show the standard popup menu(build menu)
    private void showPopupMenu(Vector3 pos)
    {
        PopUpMenu popUpMenu = GameObject.FindWithTag("PointLight").GetComponent<PopUpMenu>();
        popUpMenu.openMenu(pos, gameObject, this);
    }

    // show military menu
    private void showMilitaryMenu(Vector3 pos)
    {
        MilitaryMenu milMenu = GameObject.FindWithTag("PointLight").GetComponent<MilitaryMenu>();
        milMenu.openMenu(pos, gameObject, this);
    }

    // mark field as set
    [RPC]
    public void fieldSet()
    {
        set = true;
    }
    

}
