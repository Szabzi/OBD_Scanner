using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OBD.NET.Logging;


public class MyLogger : MonoBehaviour
{
    TextMeshProUGUI logTextField;
    public MyLogger( OBDLogLevel level = OBDLogLevel.Debug){
        logTextField = ScannerScript.debugLogger;
    }
    public void WriteLine( string message, OBDLogLevel level = OBDLogLevel.Debug){
        UnityEngine.Debug.Log( "MyLogger: " + message );
        logTextField.text += "\n" + message;
    }
}
