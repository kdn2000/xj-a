using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;

public class CreateRoad : MonoBehaviour
{
    public Material material;
    private int currLines = 0;
    private LineRenderer line;
    public GameObject block;
    public int width = 10;
    public int height = 4;
    private const string URL = "http://overpass-api.de/api/interpreter";
    
    public List<NodeStruct> nodes;
    public List<WayStruct> ways;
    
    void Start()
    {
        GenerateRequest();
    }

    void createLine()
    {
        line = new GameObject("Line" + currLines).AddComponent<LineRenderer>();
        line.material = material;
        line.startWidth = 100f;
        line.positionCount = 0;
        line.endWidth = 100f;
        line.useWorldSpace = false;
        line.numCapVertices = 50;
    }
    private void GenerateRoad() 
    {   
        float rightNode = -180;
        float leftNode = 180;
        float topNode = -180;
        float bottomNode = 180;

        foreach(var node in nodes)
        {
            float x = node.lat;
            float z = node.lon;

            rightNode = x > rightNode? x: rightNode;
            leftNode = x < leftNode? x: leftNode;
            topNode = z > topNode? z: topNode;
            bottomNode = z < bottomNode? z: bottomNode;
        }

        float scaleX = rightNode - leftNode;
        float scaleZ = topNode - bottomNode;

        // foreach(var node in nodes.elements)
        // {  
        //     float x = (node.lat - leftNode) / (rightNode - leftNode) * 10000 * scaleX;
        //     float z = (node.lon - topNode) / (bottomNode - topNode) * 10000 * scaleZ;
        //     Instantiate(block, new Vector3(x, 0, z), Quaternion.identity);
        // }

        foreach(var way in ways) 
        {   
            // Debug.Log(way.id);
            
            createLine();
            
            for(int index = 0; index < way.nodes.Length; index++)
            {   
                line.positionCount++;
                var nd1 = nodes.Find(item => item.id==way.nodes[index]);
                
                if(nd1.type != null) 
                {
                    float x1 = (nd1.lat - leftNode) / (rightNode - leftNode) * 1000000 * scaleX;
                    float z1 = (nd1.lon - topNode) / (bottomNode - topNode) * 1000000 * scaleZ;


                
                    line.SetPosition(index, new Vector3(x1, 0, z1));
                   
                }
        
                // var nd2 = nodes.Find(item => item.id==way.nodes[index + 1]);

                // float x2 = (nd2.lat - leftNode) / (rightNode - leftNode) * 100000 * scaleX;
                // float z2 = (nd2.lon - topNode) / (bottomNode - topNode) * 100000 * scaleZ;
                // Instantiate(block, new Vector3(x, 0, z), Quaternion.identity);
                

               
            }
        }
    }

    public void GenerateRequest()
    {
        StartCoroutine(routine: GetNodes());
    }

    private IEnumerator GetNodes()
    {
        string query = "[out:json];  area[name='Івано-Франківськ'];  (node(area););  out center;";
        UnityWebRequest request = UnityWebRequest.Get($"{URL}?data={query}");
        
        yield return request.SendWebRequest();

        Debug.Log(request.downloadHandler.text);

        Debug.Log(request.responseCode);

        Debug.Log("End");
        StartCoroutine(routine: GetWays());
    }

    private IEnumerator GetWays()
    {
        string query = "[out:json];  area[name='Івано-Франківськ'];  (way['highway'](area););  out center;";
        UnityWebRequest request = UnityWebRequest.Get($"{URL}?data={query}");
        
        yield return request.SendWebRequest();

        Debug.Log(request.downloadHandler.text);

        Debug.Log(request.responseCode);

        GenerateRoad();
    }
}