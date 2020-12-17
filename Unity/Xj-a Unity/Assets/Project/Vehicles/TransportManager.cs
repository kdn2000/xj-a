using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportManager : Singleton<TransportManager>
{
    // Start is called before the first frame update
    private List<ITransport> transports;

    void Start()
    {
        transports = new List<ITransport>();   
    }

    public void AddTransport(ITransport transport)
    {
        transports.Add(transport);
    }
}
