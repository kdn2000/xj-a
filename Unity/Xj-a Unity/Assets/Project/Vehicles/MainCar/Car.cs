using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json.Linq;


public class Car : MonoBehaviour, ITransport
{
    // Start is called before the first frame update
    private Camera camera;
    private Vector2 location;
    private int id;
    private double lat;
    private double lon;
    private int speed;
    private double dist = 0;
    private int acceleration = 3;
    private int maxSpeed;
    private List<string> routeNodes;
    private List<List<int>> routeLanes;
    private string from;
    private string to;
    private dynamic road;
    private int currentLane;
    private const double convert = 111.32;
    private bool slowDown = false;
    //self.Speed = Speed # km for hour
    //    self.Dist = 0
    //    self.__Id = Id
    //    self.__Type_ = Type_
    //    self.__Acceleration = 3 # m for s^2
    //    self.__MaxSpeed = MaxSpeed
    //    self.__RouteNodes = []
    //    self.__RouteLanes = []
    //    self.__From = None
    //    self.__To = None
    //    self.__Road = None
    //    self.__CurrentLane = None
    //    self.__Icon = CustomIcon('Assets/car.png', icon_size = (50, 50))
    //    self.__ConstLat = 111.32
    //    self.__ConstLon = 111.32 #* cos(round(Latitude)*pi/360) # Найбільша похибка 1 км (sin(x - PI/360) - sin(x))    
    //    self.__SlowDown = False

    void Start()
    {
        camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        var positionY = camera.transform.position.y;
        var translate = transform.position;
        camera.transform.position = new Vector3(translate.x, positionY, translate.z);
    }

    public int Id
    {
        get => id;
    }

    public double Dist { get => dist; }

    public List<int> SetRoad(dynamic graph, string from, string to, string next)
    {
        List<int> lines = new List<int>();
        dynamic road = graph["edges"][from][to];

        if (road["lanes"] == 1 || road["oneway"] == true)
        {
            lines.Add(0);
            return lines;
        }

        double side1 = graph["nodes"][to]["x"] - graph["nodes"][from]["x"];
        double side2 = graph["nodes"][to]["y"] - graph["nodes"][from]["y"];
        double angleCurrent = Mathd.Atan(side1 / side2);
        side1 = graph["nodes"][next]["x"] - graph["nodes"][to]["x"];
        side2 = graph["nodes"][next]["y"] - graph["nodes"][to]["y"];
        double angleFuture = Mathd.Atan(side1 / side2);
        angleCurrent = angleFuture - angleCurrent;

        if (angleCurrent <= Mathd.PI / 3 || graph["nodes"][next]["is_light"] == true)
        {
            lines.Add(road["queue"][to].Count - 1);
        }
        else if (angleCurrent >= 4 * Mathd.PI / 3 && graph["nodes"][next]["is_light"] == true)
        {
            lines.Add(0);
        }
        else
        {
            int lenght = road["queue"][to];
            if (lenght == 2)
            {
                lines.Add(0);
                lines.Add(1);
                return lines;
            }
            else
            {
                for (int i = 0; i < lenght; i++)
                {
                    lines.Add(i);
                }
            }
        }
        return lines;

        //if Angle <= pi / 3 and G.nodes[Next]['is_light'] is True:
        //    Lines = [len(Road['queue'][To]) - 1]
        //elif Angle >= 4 * pi / 3 and G.nodes[Next]['is_light'] is True:
        //    Lines = [0]
        //else:
        //    Lenght = len(Road['queue'][To])
        //    if Lenght == 2:
        //        Lines = [0, 1]
        //    else:
        //        Lines = [i for i in range(1, Lenght - 1)]

    }

    public void SetSpeed(dynamic graph, List<ITransport> cars, double time)
    {
        dynamic nodes = graph["nodes"];
        bool detected = false;
        int maxSpeed;

        if (road["maxspeed"] != null)
        {
            maxSpeed = road["maxspeed"];
        }
        else
        {
            maxSpeed = this.maxSpeed;
        }

        double futureLenght = ((speed + 20) / (3600 * convert)) * time;
        var futureRoads = new JArray();
        var lights = new JArray();
        double side1 = lat - nodes[to]["x"];
        double side2 = lon - nodes[to]["y"];
        double lenght = Mathd.Sqrt(Mathd.Pow(side1, 2) + Mathd.Pow(side2, 2));
        futureRoads.Add(new JObject { { "road", road }, { "lenght", lenght } });
        int index = routeNodes.IndexOf(to) + 1;
        while(futureLenght - lenght > 0)
        {
            if (nodes[routeNodes[index - 1]]["is_light"] != null)
            {

                if (nodes[routeNodes[index - 1]]["is_light"] == true)
                {
                    dynamic node = nodes[routeNodes[index - 1]];
                    dynamic node1 = nodes[routeNodes[index]];
                    int group = node["is_open"][0] ? 2 : 1;
                    if (group == 1)
                    {
                        if (node["osmid"] == node["for_open"]["first_group"] && node1["osmid"] == node["for_open"]["first_group"])
                        {
                            lights.Add(new JObject { { "light", node }, { "index_road", futureRoads.Count } });
                        }
                    }
                    else
                    {
                        if (node["osmid"] == node["for_open"]["first_group"] && node1["osmid"] == node["for_open"]["second_group"])
                        {
                            lights.Add(new JObject { { "light", node }, { "index_road", futureRoads.Count } });
                        }
                    }
                }
            }
                //        Side1 = Nodes[self.__RouteNodes[Index - 1]]['x'] - Nodes[self.__RouteNodes[Index]]['x']
                //        Side2 = Nodes[self.__RouteNodes[Index - 1]]['y'] - Nodes[self.__RouteNodes[Index]]['y']
                //        Lenght += sqrt(Side1 * *2 + Side2 * *2)
                //        FutureRoads.append({ 'road': G.get_edge_data(self.__RouteNodes[Index - 1], self.__RouteNodes[Index]), 'lenght': Lenght})
                //        Index += 1
            side1 = nodes[routeNodes[index - 1]]["x"] = nodes[routeNodes[index]]["x"];
            side2 = nodes[routeNodes[index - 1]]["y"] = nodes[routeNodes[index]]["y"];
            lenght += Mathd.Sqrt(Mathd.Pow(side1, 2) + Mathd.Pow(side2, 2));
            futureRoads.Add(new JObject { { "road", graph["edges"][routeNodes[index - 1]][routeNodes[index]] }, { "lenght", lenght } });
            index += 1;
        }
        
        lenght = Mathd.Pow(speed, 2) / (2 * acceleration * (Mathd.Pow(3.6, 3)) * 1000 * convert);
        double s;
        index = routeNodes.IndexOf(to);
        foreach(var futureRoad in futureRoads.Children())
        {
            foreach(var lane in routeLanes[index - 1])
            {
                if(futureRoad["road"]["queue"] != null)
                {
                    dynamic vehicles = futureRoad["road"]["queue"][routeNodes[index]][lane]["cars"];
                    foreach (dynamic vehicle in vehicles)
                    {
                        int roadIndex = futureRoads.IndexOf(futureRoad) - 1;
                        if (roadIndex < 0)
                        {
                            s = cars[System.Int32.Parse(vehicle)];
                        }
                        else
                        {
                            s = futureRoads[roadIndex]["lenght"] + cars[System.Int32.Parse(vehicle)].dist;
                        }
                        if(s - (5 / (1000 * convert)) > lenght)
                        {
                            detected = true;
                        }
                        else
                        {
                            if(slowDown == true && speed == 0)
                            {
                                detected = true;
                            }
                        }
                    }
                }
            }
        }

        //""" Чи є зіткнення з світлофорами."""
        //for Light in Lights:
        //    S = FutureRoads[Light['index_road']]['lenght']
        //    if S - (5 / (1000 * self.__ConstLon)) > L:
        //        Detected = True
        //    else:
        //        if self.__SlowDown is True and self.Speed == 0:
        //            Detected = True



        //if Detected is True:
        //    self.__SlowDown = True
        //else:
        //    self.__SlowDown = False


        //if self.__SlowDown is True:
        //    self.Speed -= int(self.__Acceleration * 3.6)
        //    if self.Speed <= 10:
        //        self.Speed = 0
        //else:
        //    self.Speed += int(self.__Acceleration * 3.6)
        //    if self.Speed > MaxSpeed:
        //        self.Speed = MaxSpeed


    }

    public void SetRoute(dynamic graph, List<string> nodes)
    {
        to = nodes[1];
        routeNodes = nodes;
        try
        {
            for(int i = 0; i < routeNodes.Count; i++)
            {
                routeLanes.Add(SetRoad(graph, routeNodes[i], routeNodes[i + 1], routeNodes[i + 2]));
            }
        
        }
        catch
        {
            routeLanes.Add(new List<int> { 0 });
        }

        road = graph["edges"][from][to];
        currentLane = 0;
        //route = nx.dijkstra_path(G, self.__From, Node, weight = 'length')
        //self.__To = route[1]
        //self.__RouteNodes = route
        //"""RouteLines - це на які полоси можна заходити під час руху."""
        //try:
        //    for i in range(len(route)):
        //        self.__RouteLanes.append(self.SetRoad(G, self.__RouteNodes[route[i]], self.__RouteNodes[route[i + 1]], self.__RouteNodes[route[i + 2]]))
        //except:
        //    self.__RouteLanes.append([0])

        //    self.__Road = G.get_edge_data(self.__From, self.__To)[0]
        //self.__CurrentLane = 0
    }

    public void Move(dynamic graph, List<ITransport> cars, double sec)
    {
        throw new System.NotImplementedException();
    }
}
