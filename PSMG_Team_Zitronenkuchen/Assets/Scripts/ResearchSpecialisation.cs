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

}
