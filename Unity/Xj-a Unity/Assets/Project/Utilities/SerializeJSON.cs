using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class SerializeJSON : Singleton<SerializeJSON>
{
    private const string URL = "http://overpass-api.de/api/interpreter";
    private JObject nodes = new JObject();
    private JObject edges = new JObject();
    private bool complete = false;

    public JObject Nodes
    {
        get { return nodes; }
    }
    public JObject Edges
    {
        get { return edges; }
    }
    public bool CompleteFlag
    {
        get { return complete; }
    }

    public IEnumerator Generate(string place)
    {
        //string query = System.String.Format("[out:json];  area[name='Івано-Франківськ'];  (node(area););  out center;", place);
        string query = $"[out:json];  area[name='{place}'];  (node(area););  out center;";
        UnityWebRequest request = UnityWebRequest.Get($"{URL}?data={query}");

        yield return request.SendWebRequest();

        //Debug.Log(request.downloadHandler.text);

        
            JObject elements = JObject.Parse(request.downloadHandler.text);
            foreach (dynamic element in elements["elements"])
            {
                nodes[element.GetValue("id").ToString()] = element;
            }
            complete = GetWays(place);
        
    }

    private bool GetWays(string place)
    {
        string query = $"[out:json];  area[name='{place}'];  (way['highway'](area););  out center;";
        UnityWebRequest request = UnityWebRequest.Get($"{URL}?data={query}");

        request.SendWebRequest();

        Debug.Log(request.downloadHandler.text);


        //Debug.Log(request.responseCode);
        
       
        JObject elements = JObject.Parse(request.downloadHandler.text);

        List<string> potentialNodes = new List<string>();
        float side1, side2;
        int countMain = 1;
        int countNodes = 0;
        JObject edge;

        foreach (var element in elements["elements"])
        {
            foreach (var node in element["nodes"])
            {
                potentialNodes.Add(node.ToString());
                countNodes++;
            }
            for (int i = 1; i < countNodes; i++)
            {
                edge = new JObject();
                edge["begin"] = potentialNodes[i - 1];
                edge["end"] = potentialNodes[i];
                try
                {
                    side1 = nodes[edge["begin"].ToString()]["lat"].ToObject<float>() - nodes[edge["end"].ToString()]["lat"].ToObject<float>();
                    side2 = nodes[edge["begin"].ToString()]["lon"].ToObject<float>() - nodes[edge["end"].ToString()]["lon"].ToObject<float>();

                    if (side2 == 0) edge["angle"] = "0.00";
                    else edge["angle"] = Mathf.Atan(side1 / side2).ToString();
                }
                catch { edge["angle"] = "0.00"; }

                edges[countMain.ToString()] = edge;
                countMain++;
            }
            countNodes = 0;
            potentialNodes.Clear();
            Debug.Log("xdddd - " + countMain.ToString());
        }

        return true;
        
        //catch { return false; }
    }
}