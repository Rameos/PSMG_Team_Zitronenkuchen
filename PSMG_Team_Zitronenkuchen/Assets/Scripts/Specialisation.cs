using UnityEngine;
using System.Collections;

public abstract class Specialisation {

    private int level = 1;

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

}
