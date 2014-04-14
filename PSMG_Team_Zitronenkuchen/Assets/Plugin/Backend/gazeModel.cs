using UnityEngine;
using System.Collections;
using iViewX;

    public class gazeModel
    {
        /// <summary>
        /// Container for the Gaze Data from the Server
        /// </summary>
        #region gazeData

        // Left Eye
        public static Vector3 posLeftEye { get; set; }
        public static Vector2 posGazeLeft { get; set; }
        public static float diamLeftEye { get; set; }

        // Right Eye
        public static Vector3 posRightEye { get; set; }
        public static Vector2 posGazeRight { get; set; }
        public static float diamRightEye { get; set; }

        //Head Position
        public static Vector3 posHead { get; set; }

        #endregion

        #region accuracyData
        //Left deviatiation
        public static Vector2 deviationLeft { get; set; }
        public static Vector2 deviationRight { get; set; }
        #endregion

        #region systemData
        public static bool isEyeTrackerRunning { get; set; }
        public static bool isCalibrationRunning { get; set; }
        public static bool isValidationRunning { get; set; }
        public static long timeStamp { get; set; }
        public static EyeTrackingController.SystemInfoStruct systemInfo { get; set; }
        public static Vector2 gameScreenPosition { get; set; }

        #endregion
    }
