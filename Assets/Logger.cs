using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OBD.NET.Logging;


public class Logger : MonoBehaviour
{
    TextMeshProUGUI logTextField;
    public Logger( OBDLogLevel level = OBDLogLevel.None){
        logTextField = ScannerScript.debugLogger;
    }
    public void WriteLine( string message, OBDLogLevel level = OBDLogLevel.Debug){
        UnityEngine.Debug.Log( "Logger: " + message );
        logTextField.text += "\n" + message;
    }
}
