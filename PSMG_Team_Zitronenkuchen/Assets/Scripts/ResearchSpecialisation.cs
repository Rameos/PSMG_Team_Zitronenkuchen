using UnityEngine;
using System.Collections;

public class ResearchSpecialisation : Specialisation {

    public ResearchSpecialisation(Vector3 pos) : base(pos) { }
    
    public override int Cost
    {
        get
        {
            return 300;
        }
    }

}
