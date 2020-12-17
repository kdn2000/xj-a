using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface ITransport
{
    int Id { get; }
    double Dist { get; }
    List<int> SetRoad(dynamic graph, string from, string to, string next);
    void SetSpeed(dynamic graph, List<ITransport> cars, double time);
    void SetRoute(dynamic graph, List<string> nodes);
    void Move(dynamic graph, List<ITransport> cars, double sec);
}

