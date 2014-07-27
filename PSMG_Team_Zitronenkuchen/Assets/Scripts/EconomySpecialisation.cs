using UnityEngine;
using System.Collections;

public class EconomySpecialisation : Specialisation {

    // call base constructor
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
    //public override int BuildCounter
    //{
    //    get
    //    {
    //        return 0;
    //    }
    //    set
    //    {

    //    }
    //}
	
}
