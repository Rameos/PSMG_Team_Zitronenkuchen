using UnityEngine;
using System.Collections;

/**
 * This script is assigned to each EconomySpecialisation the player builds
 **/
public class EconomySpecialisation : Specialisation {

    private int buildCounter = 1;

    // call constructor of the base class
    public EconomySpecialisation(GameObject hex, Vector3 pos) : base(hex, pos) { }

    public override int Cost
    {
        get
        {
            return 100;
        }
    }

    public override string type
    {
        get { return "eco"; }
    }

    public override int BuildCounter
    {
        get
        {
            return buildCounter;
        }
        set
        {
            buildCounter = value;
        }
    }
	
}
