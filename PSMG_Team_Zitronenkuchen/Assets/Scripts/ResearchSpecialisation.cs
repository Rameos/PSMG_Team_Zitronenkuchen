using UnityEngine;
using System.Collections;

public class ResearchSpecialisation : Specialisation {

    public ResearchSpecialisation(GameObject hex, Vector3 pos) : base(hex, pos) { }
    
    public override int Cost
    {
        get
        {
            return 300;
        }
    }

    public override string type
    {
        get { return "res"; }
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
