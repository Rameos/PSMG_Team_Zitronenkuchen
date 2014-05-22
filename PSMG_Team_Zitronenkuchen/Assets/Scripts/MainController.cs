using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

    private int tirkid;
    private int researchPoints;
    private ArrayList ui = new ArrayList();
    private ArrayList spezialisedNodes = new ArrayList();

	// Use this for initialization
	void Start () {
        tirkid = 500;
        researchPoints = 0;
        Debug.Log("Start");
        InvokeRepeating("updateRessources", 1, 1);
	}

	
	// Update is called once per frame
	void Update () {
	}

    void updateRessources()
    {
        earn(5);
        research(5);
        foreach(Specialisation node in spezialisedNodes){
            if (node is EconomySpecialisation)
            {
                earn(node.Level*5);
            }
            else if (node is ResearchSpecialisation)
            {
                research(node.Level*5);
            }
            else if (node is MilitarySpecialisation)
            {
                ((MilitarySpecialisation) node).recruit();
                Debug.Log(((MilitarySpecialisation) node).Troops + " troops on " + node.Pos);
            }
        }

<<<<<<< HEAD
       Debug.Log("Tirkid: " + tirkid);
       Debug.Log("Researchpoints: " + researchPoints);
=======
        Debug.Log("Researchpoints: " + researchPoints);
        Debug.Log("Tirkid: " + tirkid);
>>>>>>> d8a63d14cc91033259b69e8d1bf61ddb6a9c6f88
    }

    public bool build(string type, Vector3 pos)
    {
        Specialisation newBuilt;
        switch (type)
        {
            case "Economy":
                newBuilt = new EconomySpecialisation(pos);
                break;
            case "Military":
                newBuilt = new MilitarySpecialisation(pos);
                break;
            case "Research":
                newBuilt = new ResearchSpecialisation(pos);
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
