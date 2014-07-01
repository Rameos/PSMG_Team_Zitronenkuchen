using UnityEngine;
using System.Collections;

public class EconomySpecialisation : Specialisation {

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
            return 0;
        }
        set
        {
            
        }
    }
	
}
