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
    public static float scaleKoefficient = 6f; // Бажано [5.5 ; 6]

    private SocketServer socketServer = null;
    static private int port = 65432;
    static private string address = "127.0.0.1";
    static public string place = "Ivano - Frankivsk, Ukraine";
    private bool afterInit = false;
    private bool completeFlag = false;
    public JObject nodes;
    public JObject edges;



    void Start()
    {
        TrafficSystemBuilder.Instance.SetRoadsScale(scaleKoefficient - 2f);
        //Thread init = new Thread(Initialize);
        //init.Start();
        TrafficSystemBuilder.Instance.TestOrDebug();
        StartCoroutine(routine: SerializeJSON.Instance.Generate("Івано-Франківськ"));
    }

    private void Initialize()
    {
        //string json = @"";
        //string cmd = "/K conda activate ox && python \"Assets\\Project\\Utilities\\PythonScriptManager.py\" \"Ivano - Frankivsk, Ukraine\" \"/../Data\" ";
        ////string pathJSON = Environment.CurrentDirectory + @"\Assets\Project\Data\map.json";
        ////string pathComm = Environment.CurrentDirectory + @"\Assets\Project\Data\isCommunicate.tmp";
        //System.Diagnostics.Process process = new System.Diagnostics.Process();
        //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        ////startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        //startInfo.FileName = "cmd.exe";
        //startInfo.Arguments = cmd;
        //process.StartInfo = startInfo;
        //process.Start();

        //socketServer = new SocketServer(address, port);
        //cmd = socketServer.Receive();
        //if(File.Exists(Directory.GetCurrentDirectory() + "\\Assets\\Project\\Data\\" + place + ".json") && cmd == "Send json")
        //{
        //    string[] jsonBuffer = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\Assets\\Project\\Data\\" + place + ".json");
        //    json += @"[";
        //    foreach(string line in jsonBuffer)
        //    {
        //        json += line;
        //    }
        //    json += "]";
        //}
        
        Debug.Log("The graph!");

        while (!SerializeJSON.Instance.CompleteFlag) { }

        nodes = SerializeJSON.Instance.Nodes;
        edges = SerializeJSON.Instance.Edges;
        
        
        afterInit = true;
    }

    private void AfterInit()
    {
        //System.Random random = new System.Random();
        //for (int i = 0; i < 25; i++)
        //{
        //    List<string> attr = new List<string>();
        //    double lat = random.NextDouble() * (-48.888 + 48.948) + 48.888;
        //    double lon = random.NextDouble() * (-24.68 + 24.752) + 24.68;
        //    attr.Add("_SelNearestNode");
        //    attr.Add(lat.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture));
        //    attr.Add(lon.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture));
        //    socketServer.SendStandardCommand(attr);
        //    string node = socketServer.Receive();
        //    CarFactory.Instance.CreateCar(Convert.ToSingle(nodes[node]["y"]), Convert.ToSingle(nodes[node]["x"]));
        //}

        TrafficSystemBuilder.Instance.GenerateTrafficSystem(nodes, edges);

    }
    // Update is called once per frame
    void Update()
    {
        if (SerializeJSON.Instance.CompleteFlag)
        {
            Debug.Log("The graph!");

            nodes = SerializeJSON.Instance.Nodes;
            edges = SerializeJSON.Instance.Edges;


            TrafficSystemBuilder.Instance.GenerateTrafficSystem(nodes, edges);
        }
    }

    //private void OnApplicationQuit()
    //{
    //    socketServer.SendCommand("comm:exit");
    //    socketServer.Destroy();
    //}
}
