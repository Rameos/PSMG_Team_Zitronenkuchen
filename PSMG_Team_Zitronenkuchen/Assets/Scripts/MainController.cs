using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

    private int tirkid;
    private int researchPoints;
    private ArrayList ui = new ArrayList();
    private ArrayList spezialisedNodes = new ArrayList();
    private ArrayList labelUI = new ArrayList();

	// Use this for initialization
	void Start () {
        tirkid = 500;
        researchPoints = 0;
        Debug.Log("Start");
        InvokeRepeating("updateRessources", 1, 1);
	}

	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Application.LoadLevel("Main_Menu");
        }
	}

    void updateRessources()
    {
        earn(5);
        research(5);
        foreach (Specialisation node in spezialisedNodes)
        {
            if (node is EconomySpecialisation)
            {
                earn(node.Level * 5);
            }
            else if (node is ResearchSpecialisation)
            {
                research(node.Level * 5);
            }
            else if (node is MilitarySpecialisation)
            {
                ((MilitarySpecialisation)node).recruit();
                Debug.Log(((MilitarySpecialisation)node).Troops + " troops on " + node.Pos);
            }
        }
    }

    public bool build(string type, GameObject hex, Vector3 pos)
    {
        Specialisation newBuilt;
        switch (type)
        {
            case "Economy":
                newBuilt = new EconomySpecialisation(hex, pos);
                //hex.guiText.font = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GUIText>().font;
                //hex.guiText.material.color = new Color(1.0f, 1.0f, 1.0f);
                //hex.guiText.transform.position = pos;
                //hex.guiText.text = "Economy";
                //labelUI.Add(hex.guiText);
                break;
            case "Military":
                newBuilt = new MilitarySpecialisation(hex, pos);
                break;
            case "Research":
                newBuilt = new ResearchSpecialisation(hex, pos);
                break;
            default:
                newBuilt = null;
                break;
        }
        Debug.Log(newBuilt + ", " + newBuilt.Cost);
        if (newBuilt != null)
        {
            if (spend(newBuilt.Cost))
            {
                spezialisedNodes.Add(newBuilt);
                return true;
            }
        }
        return false;
    }

    public bool spend(int value)
    {
        if (tirkid >= value)
        {
            tirkid -= value;
            return true;
        }
        else return false;
    }

    public int earn(int value)
    {
        tirkid += value;
        return tirkid;
    }

    private void research(int value)
    {
        researchPoints += value;
    }

    public bool discover(int value)
    {
        if (researchPoints >= value)
        {
            researchPoints -= value;
            return true;
        }
        else return false;
    }

    void OnGUI()
    {

        GUI.Label(new Rect(0, 0, 300, 150), ("Tirkid: " + tirkid + "   Research: " + researchPoints));
        
        foreach (Specialisation node in spezialisedNodes)
        {
            if (node is MilitarySpecialisation)
            {
                //Vector3 pos = Camera.main.WorldToViewportPoint(node.Pos);
                //Debug.Log(pos);
            }
           
        }
        
    }
}
