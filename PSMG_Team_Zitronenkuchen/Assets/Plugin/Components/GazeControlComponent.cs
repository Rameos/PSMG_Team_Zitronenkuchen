// ----------------------------------------------------------------------------
//
// (C) Copyright 2014, Visual Interaction GmbH 
//
// All rights reserved. This work contains unpublished proprietary 
// information of Visual Interaction GmbH and is copy protected by law. 
// (see accompanying file eula.pdf)
//
// ----------------------------------------------------------------------------


using UnityEngine;
using System.Threading;
using System.Collections;
using iViewX;

namespace iViewX
{

    public class GazeControlComponent : MonoBehaviour
    {

        public bool useGazeFilter = true;
        public bool useLocalConnectionSettings = true; 
        /// <summary>
        /// Required designer variables.
        /// </summary>
        private static Thread eyeThread;
        private static GazeController gazeController;
        private MonoBehaviourWithGazeComponent oldSelection;

        private static GazeControlComponent instance;

        public static GazeControlComponent Instance
        {
            get
            {
                if (!instance)
                {
                    instance = (GazeControlComponent)FindObjectOfType(typeof(GazeControlComponent));
                    {
                        if (!instance)
                        {
                            GameObject gameObject = new GameObject();
                            gameObject.name = "EyeTrackingController";
                            instance = gameObject.AddComponent(typeof(GazeControlComponent)) as GazeControlComponent;
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Pause the EyeTracker
        /// </summary>
        public void PauseEyeTracker()
        {
            try
            {
                gazeController.pauseEyeTracking();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Continue the EyeTracker
        /// </summary>
        public void ContinueEyeTracker()
        {
            try
            {
                gazeController.continueEyeTracking();
            }

            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }


        /// <summary>
        /// Start a Calibartion
        /// Use this function in your Scripts to start a Calibration: Set True, if the System must start a Calibration
        /// </summary>
        public void StartCalibration()
        {
            if (!instance.enabled)
                instance.enabled = true;
            gazeModel.isCalibrationRunning = true;
        }

        /// <summary>
        /// Start a Validation
        /// Use this function in your Scripts to start a Validation: Set True, if the System must start a Validation
        /// </summary>
        public void StartValidation()
        {
            if (!instance.enabled)
                instance.enabled = true;
            gazeModel.isValidationRunning = true;
        }

        public void UseGazeFilter()
        {
           if(useGazeFilter)
           {
               useGazeFilter = false;
               gazeController.disableGazeFilter();
           }
           else
           {
               useGazeFilter = true;
               gazeController.enableGazeFilter();
           }
        }

        void Awake()
        {
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                gazeController = new GazeController();

                instance.initEyeThread();
                instance.startEyeThread();


            }
            else
            {
                Destroy(gameObject);
            }


        }


        /// <summary>
        /// Mono Behaviour Update Function:
        /// Start the Coroutine if, the System must start a Validation/Calibration
        /// </summary>
        void Update()
        {

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (gazeModel.isEyeTrackerRunning)
            {
                startCalibrationRoutine();
                startValidationRoutine();

            }

            if (enableGazeReactionOfObjects)
            {
                rayCastGazeRay();
            }
#else
            
            Debug.LogError("You need Windows as operation system.");
#endif
        }


        /// <summary>
        /// Mono Behaviour OnApplicationQuit Function:
        /// Stops the EyeTrackingController and Joins the Thread
        /// </summary>
        void OnApplicationQuit()
        {
            Debug.Log("Finish");
            instance.joinEyeThread();
        }


        /// <summary>
        /// Enable Raycasts from the Gazeposition everyFrame 
        /// (use this Feature to integrate GazeReactionScripts)
        /// </summary>
        public bool enableGazeReactionOfObjects = true;

        /// <summary>
        /// Init a new Thread for the Eye Tracking Input
        /// </summary>
        private void initEyeThread()
        {
            eyeThread = new Thread(gazeController.initEyeThread);
        }

        /// <summary>
        /// Start the EyeTrackingThread
        /// </summary>
        private void startEyeThread()
        {
            eyeThread.Start();
            UseGazeFilter();
        }

        /// <summary>
        /// Stop the Datastream from the Eye Tracker and join the Thread.
        /// </summary>
        private void joinEyeThread()
        {
            gazeController.finish();
            eyeThread.Join();
        }

        /// <summary>
        /// Start a Coroutine to hide the gameView and open a CalibrationScreen from the Server
        /// </summary>
        private void startCalibrationRoutine()
        {
            if (gazeModel.isCalibrationRunning)
            {
                gazeModel.isCalibrationRunning = false;
                StartCoroutine(IEnumerator_StartCalibration());
            }
        }


        /// <summary>
        /// Start a Coroutine to hide the gameView and open a ValidationScreen from the Server
        /// </summary>
        private void startValidationRoutine()
        {
            if (gazeModel.isValidationRunning)
            {
                gazeModel.isValidationRunning = false;
                StartCoroutine(IEnuerator_Start_Validation());
            }
        }


        /// <summary>
        /// Load a CalibrationProfile from the Server
        /// </summary>
        /// <param name="nameOfProfile"></param>
        private void getCalibrationDataFromDriver(string nameOfProfile)
        {
            gazeController.loadCalibration(nameOfProfile);
        }


        /// <summary>
        /// Cast a Ray from the Gazepoint into the Scene
        /// Call the OnObjectHit(), if a Object was in the Gaze of the User. 
        /// (Use MonoBehaviorWithGazeComponent to integrate your own GazeInteraction) 
        /// </summary>
        private void rayCastGazeRay()
        {
            Ray raygaze;
            RaycastHit hit;


            //CheckInput
            if (gazeModel.isEyeTrackerRunning)
            {
                Vector2 gazePos = (gazeModel.posGazeLeft + gazeModel.posGazeRight) * 0.5f;
                gazePos.y = Screen.height - gazePos.y;
                raygaze = Camera.main.ScreenPointToRay(gazePos);
            }

            //Usee the MouseInput as a DebugInput, if no EyeTracker runs
            else
            {
                raygaze = Camera.main.ScreenPointToRay(Input.mousePosition);
            }


            //Cast a Ray from the Gazepoint into the scene
            if (Physics.Raycast(raygaze, out hit))
            {
                //Only React when a Object has a Monobehabvior with GazeComponent
                MonoBehaviourWithGazeComponent hitMono = hit.collider.gameObject.GetComponent<MonoBehaviourWithGazeComponent>();
                if (hitMono != null)
                {
                    // Save the OldSelection
                    if (oldSelection == null)
                        oldSelection = hitMono;

                    // Call from the oldSelection the GazeExit() Function
                    else if (hitMono != oldSelection)
                    {
                        oldSelection.OnObjectExit();
                        oldSelection = hitMono;
                    }
                    // Invoke Start and Update of the GazeEvent
                    hitMono.OnObjectHit(hit);

                }

            }

            else
            {
                // Call from the oldSelection the GazeExit() Function
                if (oldSelection != null)
                {
                    oldSelection.OnObjectExit();
                    oldSelection = null;
                }
            }
        }

        /// <summary>
        /// Disable Fullscreen
        /// Wait one Frame
        /// Start the Calibration from the Server
        /// </summary>
        IEnumerator IEnumerator_StartCalibration()
        {
            if (Screen.fullScreen == true)
                Screen.fullScreen = false;

            yield return new WaitForFixedUpdate();
            //gazeController.loadCalibration("new profile");
            gazeController.startCalibration();
            Screen.fullScreen = true;
            yield return null;
        }


        /// <summary>
        /// Disable Fullscreen
        /// Wait one Frame
        /// Start the Validation from the Server
        /// </summary>
        IEnumerator IEnuerator_Start_Validation()
        {
            if (Screen.fullScreen == true)
                Screen.fullScreen = false;

            yield return new WaitForFixedUpdate();
            gazeController.startValidation();
            Screen.fullScreen = true;
            yield return null;
        }
    }
}