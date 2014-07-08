using System;
using UnityEngine;
using System.Collections.Generic;

namespace iViewX
{
    public class errorIDContainer {
        
        /// <summary>
        /// Container for the ErrorIDs and States of the Eye Tracker Controller
        /// </summary>
        
        public const int STATE_CONNECT = 0;
        public const int STATE_DISCONNET = 1;
        public const int STATE_SETUPCALIBRATION = 2;
        public const int STATE_CALIBRATE = 3;
        public const int STATE_SETUPVALIDATE = 4;
        public const int STATE_VALIDATE = 5;
        public const int STATE_PAUSE = 6;
        public const int STATE_CONTINUE = 7;
        public const int STATE_GAZEFILTER = 8;

        public const int ACTION_COMPLETE = 1;
        public const int ACTION_CRITICALERROR= 99;

        private static Dictionary<int, string> ErrorId = new Dictionary<int, string>()
            {
                {1, "Intended functionality has been fulfilled"},
                {2, "No new data available"},
                {3, "Calibration was aborted"},
                {100, "Failed to establish connection"},
                {101, "No connection established"},
                {102, "System is not calibrated"},
                {103, "System is not validated"},
                {104, "No SMI eye tracking application running"},
                {105, "Wrong port settings"},
                {111, "Eye tracking device required for this function is not connected"},
                {112, "Parameter out of range"},
                {113, "Eye tracking device required for this calibration method is not connected"},
                {121, "Failed to create sockets"},
                {122, "Failed to connect sockets"},
                {123, "Failed to bind sockets"},
                {124, "Failed to delete sockets"},
                {131, "No response from iViewX; check iViewX connection settings (IP addresses, ports) or last command"},
                {132, "iViewX version could not be resolved"},
                {133, "Wrong version of iViewX"},
                {171, "Failed to access log file"},
                {181, "Socket error during data transfer"},
                {191, "Recording buffer is empty"},
                {192, "Recording is activated"},
                {193, "Data buffer is full"},
                {194, "iViewX is not ready"},
            
                {201, "No installed iViewX detected"},
                {220, "Could not open port for TTL output"},
                {221, "Could not close port for TTL output"},
                {222, "Could not access AOI data"},
            };

        private static Dictionary<int, string> ErrorState = new Dictionary<int, string>()
            {
                {0, "Connect"},
                {1, "Disconnect"},
                {2, "Setup Calibration"},
                {3, "Calibration"},
                {4, "Setup Validation"},
                {5, "Validation"},
                {6, "Pause"},
                {7, "Continue"},
                {8, "GazeFilterSetup"}

            };

        public static string getErrorMessage(int id)
        {
            return ErrorId[id];
        }

        public static string getState(int id)
        {
            return ErrorState[id];
        }
    }
}