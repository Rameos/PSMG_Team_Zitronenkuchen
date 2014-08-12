using UnityEngine;
using System.Collections;

public class EconomySpecialisation : Specialisation {

    private int buildCounter = 1;
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

    // maybe needed later for not building instant
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
