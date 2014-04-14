// ----------------------------------------------------------------------------
//
// (C) Copyright 2014, Visual Interaction GmbH 
//
// All rights reserved. This work contains unpublished proprietary 
// information of Visual Interaction GmbH and is copy protected by law. 
// (see accompanying file eula.pdf)
//
//
// ----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace iViewX
{

    public abstract class MonoBehaviourWithGazeComponent : MonoBehaviour
    {
        /// <summary>
        /// Is the Element Selected via the gaze input? 
        /// </summary>
        private bool isSelected = false;

        /// <summary>
        /// Internal Function to get Informations from the MyGazeControlComponent.
        /// </summary>
        public void OnObjectHit(RaycastHit hit)
        {
            if (isSelected)
            {
                OnGazeStay(hit);
            }
            else
            {
                OnGazeEnter(hit);
                isSelected = true;
            }


        }

        /// <summary>
        /// Internal Function to get Informations from the MyGazeControlComponent.
        /// </summary>
        public void OnObjectExit()
        {
            isSelected = false;
            OnGazeExit();
        }

        /// <summary>
        /// Use this Function to descript what happend, if the gaze enters the ElementColliders
        /// </summary>
        public abstract void OnGazeEnter(RaycastHit hit);

        /// <summary>
        /// Use this Function to descript what happend, if the gaze stays in the ElementCollider
        /// </summary>
        public abstract void OnGazeStay(RaycastHit hit);

        /// <summary>
        /// Use this Function to descript what happend, if the gaze exits the ElementCollider
        /// </summary>
        public abstract void OnGazeExit();
    }
}