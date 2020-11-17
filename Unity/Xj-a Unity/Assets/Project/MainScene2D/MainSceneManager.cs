using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using IronPython.Runtime;
using Assets.Project.Utilities;
using System.Net.Sockets;
using UnityEditor.Experimental.GraphView;
using System.Net;
using Mapbox.Unity.Map;
using System.Text;
using System.Threading;

public class MainSceneManager : Singleton<MainSceneManager>
{
    private SocketServer socketServer = null;
    static private int port = 65432;
    static private string address = "127.0.0.1";
    private bool afterInit = false;
    private dynamic graph;
    private dynamic nodes;
    private dynamic edges;



    void Start()
    { 
        Thread init = new Thread(Initialize);
        init.Start();
    }

    private void Initialize()
    {
        string json = @"";
        string cmd = "/K conda activate ox && python \"Assets\\Project\\Utilities\\PythonScriptManager.py\" \"Ivano - Frankivsk, Ukraine\" ";
        //string pathJSON = Environment.CurrentDirectory + @"\Assets\Project\Data\map.json";
        //string pathComm = Environment.CurrentDirectory + @"\Assets\Project\Data\isCommunicate.tmp";
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = cmd;
        process.StartInfo = startInfo;
        process.Start();

        socketServer = new SocketServer(address, port);

        json += @"[";
        json += socketServer.Receive();
        json += @"]";
        Debug.Log("The graph!");

        graph = JArray.Parse(json)[0];
        nodes = graph["nodes"];
        edges = graph["edges"];
        

        
        afterInit = true;
    }

    private void AfterInit()
    {
        System.Random random = new System.Random();
        for (int i = 0; i < 25; i++)
        {
            List<string> attr = new List<string>();
            double lat = random.NextDouble() * (-48.888 + 48.948) + 48.888;
            double lon = random.NextDouble() * (-24.68 + 24.752) + 24.68;
            attr.Add("_SelNearestNode");
            attr.Add(lat.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture));
            attr.Add(lon.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture));
            socketServer.SendStandardCommand(attr);
            string node = socketServer.Receive();
            CarFactory.Instance.CreateCar(Convert.ToSingle(nodes[node]["y"]), Convert.ToSingle(nodes[node]["x"]));
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (afterInit)
        {
            AfterInit();
            afterInit = false;
        }
    }

    private void OnApplicationQuit()
    {
        socketServer.SendCommand("comm:exit");
        socketServer.Destroy();
    }
}
