using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBD.NET.Communication;
using OBD.NET.Devices;
using OBD.NET.Extensions;
using OBD.NET.Logging;
using OBD.NET.OBDData;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;//ghj,

public class ScannerScript : MonoBehaviour
{
    public string comPort;
    [SerializeField] TextMeshProUGUI debugLogger;
    [SerializeField] TMP_InputField comPortIF;
    private void Logger( string message ){
        UnityEngine.Debug.Log( "Logger: " + message );
        debugLogger.text += "\n" + message;
    }
    public void ConnectToScanner()
    {
        comPort = comPortIF.text;
        Logger( "Attempting connection..." );
        if ( comPort == "" ){
            Logger( "Parameter ComPort needed." );
            IEnumerable<string> availablePorts = SerialConnection.GetAvailablePorts();
            Logger( "Available ports:" );
            foreach (string port in availablePorts){
                Logger(port);
            }
        }
        else{
            Logger( $"Using ComPort : {comPort}" );
            System.IO.Ports.SerialPort port = new SerialPort();
            // using SerialConnection connection = new (comPort);
            // using ELM327 dev = new(connection, new OBDConsoleLogger(OBDLogLevel.Debug));
            // dev.SubscribeDataReceived<EngineRPM>((_, data) => Logger("EngineRPM: " + data.Data.Rpm));
            // dev.SubscribeDataReceived<VehicleSpeed>((_, data) => Logger("VehicleSpeed: " + data.Data));
            // dev.SubscribeDataReceived<IOBDData>((_, data) => Logger($"PID {data.Data.PID.ToHexString()}: {data.Data}"));
            // dev.Initialize();
        }
    }
}
