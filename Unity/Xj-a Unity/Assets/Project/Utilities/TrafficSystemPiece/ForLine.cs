using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForLine : MonoBehaviour
{
    public string GetNameOfEndNode() // for debug
    {
        TrafficSystemPiece piece = gameObject.GetComponent<TrafficSystemPiece>();
        string endNode = MainSceneManager.Instance.edges[piece.m_nameNode]["end"].ToString();
        foreach (dynamic node in MainSceneManager.Instance.nodes)
        {
            if (node.Name == endNode)
            {
                return node.Value.GetValue("name_piece").ToString() + " " + node.Name;
            }
        }
        return "";
    }
    public void Start()
    {
        
    }

}
