using UnityEngine;
using System.Collections;

public class EconomySpecialisation : Specialisation {

    public EconomySpecialisation(Vector3 pos) : base(pos) { }

    public override int Cost
    {
        get
        {
            return 100;
        }
    }
	
}
