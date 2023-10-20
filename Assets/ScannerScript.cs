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
    IEnumerator JustKeepTrying(string comPort, MyLogger logger){

        using SerialConnection connection = new (comPort);
        using ELM327 dev = new(connection, logger);

        dev.SubscribeDataReceived<EngineRPM>((_, data) => logger.WriteLine("EngineRPM: " + data.Data.Rpm));
        dev.SubscribeDataReceived<VehicleSpeed>((_, data) => logger.WriteLine("VehicleSpeed: " + data.Data));
        dev.SubscribeDataReceived<IOBDData>((_, data) => logger.WriteLine($"PID {data.Data.PID.ToHexString()}: {data.Data}"));

        logger.WriteLine("Attempting connection on COM port "+comPort);
        dev.Initialize();

        var wait = new WaitForSeconds(5);

        if(!dev.Connection.IsOpen){
            logger.WriteLine("Failed to connect on COM port "+comPort);
            dev.Connection.Dispose();
        }
        else{
            logger.WriteLine("Successful connection on COM port "+comPort);
        }
        return null;
    }
    public string comPort;

    public static TextMeshProUGUI debugLogger;
    MyLogger logger;
    bool permissionsGranted = false; 

    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName){
        logger.WriteLine("Cannot connect without Bluetooth permissions. Change app settings.");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName){
        permissionsGranted = true;
        logger.WriteLine("Permission granted on prompt.");
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName){
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
            }else{
                permissionsGranted = true;
                logger.WriteLine("Bluetooth permission already granted.");
            }
        }else{
            permissionsGranted = true;
            logger.WriteLine("Not running on Android.");
        }
    }
    public void ConnectIfPermitted(){
        if(permissionsGranted){
            ConnectToScanner();
        }else{
            logger.WriteLine("No connection can be made without app permissions.");
        }
    }
    private void ConnectToScanner()
    {
        logger.WriteLine( "Attempting connection on all available ports." );
        IEnumerable<string> availablePorts = SerialConnection.GetAvailablePorts();
        foreach (string port in availablePorts){
            StartCoroutine(JustKeepTrying(port,logger));
        }
        //StartCoroutine(JustKeepTrying("COM6",logger));
        logger.WriteLine("All connection attempts terminated.");
    }
}
