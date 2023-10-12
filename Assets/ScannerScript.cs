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
using System.Threading.Tasks;

public class ScannerScript : MonoBehaviour
{
    public string comPort;

    public static TextMeshProUGUI debugLogger;
    [SerializeField] TMP_InputField comPortIF;
    Logger logger;
    void Start(){
        debugLogger = GameObject.Find("Logger").GetComponent<TextMeshProUGUI>();
        logger = new Logger();
    }
    public void ConnectToScanner()
    {
        comPort = comPortIF.text;
        logger.WriteLine( "Attempting connection..." );
        if ( comPort == "" ){
            logger.WriteLine( "Parameter ComPort needed." );
            IEnumerable<string> availablePorts = SerialConnection.GetAvailablePorts();
            logger.WriteLine( "Available ports:" );
            foreach (string port in availablePorts){
                logger.WriteLine(port);
            }
        }
        else{
            logger.WriteLine( $"Using ComPort : {comPort}" );
            using SerialConnection connection = new (comPort);
            using ELM327 dev = new(connection, logger);
            logger.WriteLine( "Serial connection to ELM327 device established." );
            // dev.SubscribeDataReceived<EngineRPM>((_, data) => Logger("EngineRPM: " + data.Data.Rpm));
            // dev.SubscribeDataReceived<VehicleSpeed>((_, data) => Logger("VehicleSpeed: " + data.Data));
            // dev.SubscribeDataReceived<IOBDData>((_, data) => Logger($"PID {data.Data.PID.ToHexString()}: {data.Data}"));
            // dev.Initialize();
        }
    }
}
