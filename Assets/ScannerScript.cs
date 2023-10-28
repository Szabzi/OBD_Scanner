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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ScannerScript : MonoBehaviour
{
    List<string> logQueue = new ();
    void JustKeepTrying(object data){
        Tuple<string, MyLogger> parameters = (Tuple<string, MyLogger>)data;
        string comPort = parameters.Item1;
        MyLogger logger = parameters.Item2;

        using SerialConnection connection = new (comPort);
        using ELM327 dev = new(connection, logger);

        dev.SubscribeDataReceived<EngineRPM>((_, data) => logger.WriteLine("EngineRPM: " + data.Data.Rpm));
        dev.SubscribeDataReceived<VehicleSpeed>((_, data) => logger.WriteLine("VehicleSpeed: " + data.Data));
        dev.SubscribeDataReceived<IOBDData>((_, data) => logger.WriteLine($"PID {data.Data.PID.ToHexString()}: {data.Data}"));

        logQueue.Add("Thread started for connection to port "+comPort);
        dev.Initialize();

        var wait = new WaitForSeconds(5);

        if(!dev.Connection.IsOpen){
            logQueue.Add("Failed to connect on port "+comPort);
            dev.Connection.Dispose();
        }
        else{
            logQueue.Add("Successful connection on port "+comPort);
        }
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

    void Update(){
        int logsAm = logQueue.Count();
        if(logsAm != 0){
            for ( int i = 0; i < logsAm; i++){
                logger.WriteLine("Queued log: "+logQueue[0]);
                logQueue.RemoveAt(0);
            }
        }
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
        List<string> availablePorts = SerialConnection.GetAvailablePorts().ToList();
        int portsAm = availablePorts.Count();
        logger.WriteLine( $"Attempting connection on {portsAm} available ports." );

        Thread[] threads = new Thread[portsAm];
        for (int i = 0; i < portsAm; i++){
            logger.WriteLine("Attempting connection to port "+availablePorts[i]);
            Thread thread = new Thread(() => JustKeepTrying(new Tuple<string, MyLogger>(availablePorts[i], logger)));
            thread.Start();
            threads[i] = thread;
        }
        foreach (Thread thread in threads){
            thread.Join();
        }
        logger.WriteLine("Done.");

        // Thread thread = new Thread(() => JustKeepTrying(new Tuple<string, MyLogger>("COM6", logger)));
        // thread.Start();
        // thread.Join();
    }
}
