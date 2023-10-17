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
using UnityEngine.Android;

public class ScannerScript : MonoBehaviour
{
    public string comPort;

    public static TextMeshProUGUI debugLogger;
    [SerializeField] TMP_InputField comPortIF;
    MyLogger logger;
    bool permissionsGranted = false; 

    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        logger.WriteLine("Cannot connect without Bluetooth permissions. Change app settings.");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        permissionsGranted = true;
        logger.WriteLine("Permission granted on prompt.");
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        logger.WriteLine("Cannot connect without Bluetooth permissions. Restart app to be prompted again.");
    }

    void Start(){
        debugLogger = GameObject.Find("Logger").GetComponent<TextMeshProUGUI>();
        logger = new MyLogger();

        if (Application.platform == RuntimePlatform.Android){
            if ( !Permission.HasUserAuthorizedPermission( "android.permission.BLUETOOTH" ) ){
                logger.WriteLine("Requesting Bluetooth permission.");
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission("android.permission.BLUETOOTH", callbacks);
                Permission.RequestUserPermission("android.permission.BLUETOOTH_ADMIN", callbacks);
            }
            else{
                permissionsGranted = true;
                logger.WriteLine("Bluetooth permission already granted.");
            }
        }
        else{
            permissionsGranted = true;
            logger.WriteLine("Not running on Android.");
            comPortIF.text = "COM3";
        }
    }
    public void ConnectIfPermitted(){
        if(permissionsGranted){
            ConnectToScanner();
        }
        else{
            logger.WriteLine("No connection can be made without app permissions.");
        }
    }
    private void ConnectToScanner()
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
            Debug.Log("using comport");
            logger.WriteLine( $"Using ComPort : {comPort}" );

            using SerialConnection connection = new (comPort);
            logger.WriteLine( "using SerialConnection connection = new (comPort); line passed." );

            using ELM327 dev = new(connection, logger);
            logger.WriteLine( "using ELM327 dev = new(connection, logger); line passed" );

            dev.SubscribeDataReceived<EngineRPM>((_, data) => logger.WriteLine("EngineRPM: " + data.Data.Rpm));
            logger.WriteLine( "dev.SubscribeDataReceived<EngineRPM>... line passed" );

            dev.SubscribeDataReceived<VehicleSpeed>((_, data) => logger.WriteLine("VehicleSpeed: " + data.Data));
            logger.WriteLine( "dev.SubscribeDataReceived<VehicleSpeed>... line passed" );

            dev.SubscribeDataReceived<IOBDData>((_, data) => logger.WriteLine($"PID {data.Data.PID.ToHexString()}: {data.Data}"));
            logger.WriteLine( "dev.SubscribeDataReceived<IOBDData>... line passed" );

            dev.Initialize();
            logger.WriteLine("dev.Initialize(); line passed");//huha

            dev.RequestData<FuelType>();
            logger.WriteLine("Fuel type requested from device.");
        }
    }
}
