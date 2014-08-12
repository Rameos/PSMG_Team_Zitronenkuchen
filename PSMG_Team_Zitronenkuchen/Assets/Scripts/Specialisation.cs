using UnityEngine;
using System.Collections;

public abstract class Specialisation {

    private GameObject hex;
    private Vector3 pos;
    private int level = 1;
    private bool inRange;

    public Specialisation(GameObject hex, Vector3 pos)
    {
        this.hex = hex;
        this.pos = pos;
        Debug.Log("pos " + pos);
    }
    public bool InRange
    {
        get
        {
            return inRange;
        }
        set
        {
            inRange = value;
        }
    }


    abstract public int Cost
    {
        get;
    }

    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
        }
    }

    abstract public string type
    {
        get;
    }

    //abstract public int BuildCounter
    //{
    //    get;
    //    set;
    //}

    public Vector3 Pos
    {
        get
        {
            return pos;
        }
    }

    public GameObject Hex
    {
        get
        {
            return hex;
        }
    }


}
