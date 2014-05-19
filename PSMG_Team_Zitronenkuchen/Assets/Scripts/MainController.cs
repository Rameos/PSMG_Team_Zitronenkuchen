using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

    private int tirkid;
    private int researchPoints;
    private ArrayList ui = new ArrayList();
    private ArrayList spezialisedNodes = new ArrayList();

	// Use this for initialization
	void Start () {
        tirkid = 100;
        researchPoints = 0;
        Debug.Log("Start");
        InvokeRepeating("updateRessources", 1, 1);
	}

	
	// Update is called once per frame
	void Update () {
	}

    void updateRessources()
    {
        earn(1);
        research(1);
        foreach(Specialisation node in spezialisedNodes){
            if (node is EconomySpecialisation)
            {
                earn(node.Level);
            }
            else if (node is ResearchSpecialisation)
            {
                research(node.Level);
            }
        }

       // Debug.Log("Tirkid: " + tirkid);
        // Debug.Log("Researchpoints: " + researchPoints);
    }

    public bool build(string type)
    {
        Specialisation newBuilt;
        switch (type)
        {
            case "Economy":
                newBuilt = new EconomySpecialisation();
                break;
            case "Military":
                newBuilt = new MilitarySpecialisation();
                break;
            case "Research":
                newBuilt = new ResearchSpecialisation();
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
}
