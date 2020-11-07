using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using IronPython.Runtime;
using Assets.Project.Utilities;

public class MainSceneManager : Singleton<MainSceneManager>
{    
    private CarFactory carFactory = CarFactory.instance;
    private dynamic graph;
    private dynamic nodes;
    private dynamic edges;



    void Start()
    {
        
        StartCoroutine("GetGraph");
        for(int i = 0;i < 25; i++)
        {
            
        }
    }

    private IEnumerator GetGraph()
    {
        string json = @"";
        string cmd = "/C conda activate ox && /C ..\\Utilities\\ScriptStart.py \"Ivano - Frankivsk, Ukraine\" \\..\\Data";
        string path = Environment.CurrentDirectory + @"\..\Data\map.json";
        try
        {

            // Create the file, or overwrite if the file exists.
            // Open the stream and read it back.
            System.Diagnostics.Process.Start("CMD.exe", cmd);

            using (StreamReader sr = File.OpenText(path))
            {
                string s = @"[";
                json += s;
                while ((s = sr.ReadLine()) != null)
                {
                    json += s;
                }
                json += @"]";
            }
        }

        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }

        graph = JArray.Parse(json)[0];
        nodes = graph["nodes"];
        edges = graph["edges"];

        yield return null;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
