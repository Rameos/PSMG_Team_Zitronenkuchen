using UnityEngine;
using System.Collections;

public abstract class Specialisation {

    private Vector3 pos;
    private int level = 1;

    public Specialisation(Vector3 pos)
    {
        this.pos = pos;
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

    public Vector3 Pos
    {
        get
        {
            return pos;
        }
    }


}
