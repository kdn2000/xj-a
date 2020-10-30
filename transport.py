import osmnx as ox
import networkx as nx
import folium
from folium.features import CustomIcon
#from folium.plugins import TimestampedGeoJson
from math import sqrt, cos, pi, atan # cos, sin, fabs,

""" З назви пон, що це робить"""
def rgb_to_hex(rgb):
    return '%02x%02x%02x' % rgb

""" Клас машини.  SetRoute - встановлює шлях. 
    SelNearestNode - використовується при запуску, запусує координати найближчої ноди.
    Move - розрахунок наступного кроку, Бібічка рухається по шляху.
    Draw - не використовується
    DrawRoute - малює шлях"""
class Car:
    def __init__(self, Id, Longitude, Latitude, MaxSpeed=60, Speed=0, Type_="car"):
        self.Lon = Longitude
        self.Lat = Latitude
        self.Speed = Speed # km for hour
        self.Dist = 0
        self.__Id = Id
        self.__Type_ = Type_
        self.__Acceleration = 3 # m for s^2
        self.__MaxSpeed = MaxSpeed
        self.__RouteNodes = []
        self.__RouteLanes = []
        self.__From = None
        self.__To = None
        self.__Road = None
        self.__CurrentLane = None
        self.__Icon = CustomIcon('Assets/car.png', icon_size=(50, 50))
        self.__ConstLat = 111.32
        self.__ConstLon = 111.32 #* cos(round(Latitude)*pi/360) # Найбільша похибка 1 км (sin(x - PI/360) - sin(x))    
        self.__SlowDown = False
        
        
    def GetId(self):
        return self.__Id
    
    """Вибирає полосу на якій їздити. Це ініціалізація. Це для того, аби вірішити питання з перестройкою на полосу. Beta"""
    def SetRoad(self, G, From, To, Next):            
        Road = G.get_edge_data(From, To)[0]
        
        if Road['lanes'] == 1 or Road['oneway'] == True:
            Lines = [0]
            pass
        
        Side1 = G.nodes[To]['x'] - G.nodes[From]['x']
        Side2 = G.nodes[To]['y'] - G.nodes[From]['y']
        AngleCurrent = atan(Side2/Side1)
        Side1 = G.nodes[Next]['x'] - G.nodes[To]['x']
        Side2 = G.nodes[Next]['y'] - G.nodes[To]['y']
        AngleFuture = atan(Side2/Side1)
        Angle = AngleFuture - AngleCurrent
        
        if Angle <= pi/3 and G.nodes[Next]['is_light'] is True:
            Lines = [len(Road['queue'][To]) - 1]
        elif Angle >= 4*pi/3 and G.nodes[Next]['is_light'] is True:
            Lines = [0]
        else:
            Lenght = len(Road['queue'][To])
            if Lenght == 2:
                Lines = [0, 1]
            else:
                Lines = [i for i in range(1, Lenght - 1)]
        
        return Lines

    def SetSpeed(self, G, Cars, Time):
        Nodes = G.nodes.data()
        Detected = False
        
        if 'maxspeed' in self.__Road:
            MaxSpeed = int(self.__Road['maxspeed'])
        else:
            MaxSpeed = self.__MaxSpeed
        
        """Знайти дороги по яким можуть пересуватися потенційні об'єкти для зіткнення."""
        FutureLenght = ((self.Speed + 20) / (3600 * self.__ConstLon)) * Time
        FutureRoads = []
        Lights = []
        Side1 = self.Lat - Nodes[self.__To]['x']
        Side2 = self.Lon - Nodes[self.__To]['y']
        Lenght = sqrt(Side1 ** 2 + Side2 ** 2)
        FutureRoads.append({'road': self.__Road, 'lenght': Lenght})
        Index = self.__RouteNodes.index(self.__To) + 1
        try:
            while FutureLenght - Lenght > 0:
                if 'is_light' in Nodes[self.__RouteNodes[Index - 1]]:
                    if Nodes[self.__RouteNodes[Index - 1]]['is_light'] is True:
                        Node = Nodes[self.__RouteNodes[Index - 1]]
                        Node1 = Nodes[self.__RouteNodes[Index]]
                        Group = 2 if Node['is_open'][0] else 1 
                        if Group == 1:
                            if [int(Node['osmid']), int(Node1['osmid'])] in Node['for_open']['first_group']:
                                Lights.append({'light': Node, 'index_road': len(FutureRoads) - 1})
                        else:
                            if [int(Node['osmid']), int(Node1['osmid'])] in Node['for_open']['second_group']:
                                Lights.append({'light': Node, 'index_road': len(FutureRoads) - 1})
                        
                Side1 = Nodes[self.__RouteNodes[Index - 1]]['x'] - Nodes[self.__RouteNodes[Index]]['x']
                Side2 = Nodes[self.__RouteNodes[Index - 1]]['y'] - Nodes[self.__RouteNodes[Index]]['y']
                Lenght += sqrt(Side1 ** 2 + Side2 ** 2)
                FutureRoads.append({'road': G.get_edge_data(self.__RouteNodes[Index - 1], self.__RouteNodes[Index]), 'lenght': Lenght})
                Index += 1
        except Exception as e:
            print(e)
        
        L = self.Speed ** 2 / (2 * self.__Acceleration * (3.6 ** 3) * 1000 * self.__ConstLon)
        Index = self.__RouteNodes.index(self.__To)
        """Чи є зіткнення з машинками. Машинки стоять на полосі. Машинки знають на яку їм потрібно їхати полосу"""
        try:
            for Road in FutureRoads:
                for Lane in self.__RouteLanes[Index - 1]:
                    if 'queue' in Road['road']:
                        Vehicles = Road['road']['queue'][self.__RouteNodes[Index]][Lane]['cars']
                        for Vehicle in Vehicles:
                            RoadIndex = FutureRoads.index(Road) - 1
                            if RoadIndex < 0:
                                S = Cars[Vehicle].Dist
                            else:
                                S = FutureRoads[RoadIndex]['lenght'] + Cars[Vehicle].Dist 
                            if S - (5 / (1000 * self.__ConstLon)) > L:
                                Detected = True
                            else:
                                if self.__SlowDown is True and self.Speed == 0:
                                    Detected = True
            
        except:
            pass
                            
        """ Чи є зіткнення з світлофорами."""
        for Light in Lights:
            S = FutureRoads[Light['index_road']]['lenght']
            if S - (5 / (1000 * self.__ConstLon)) > L:
                Detected = True
            else:
                if self.__SlowDown is True and self.Speed == 0:
                    Detected = True
                
        
        if Detected is True:
            self.__SlowDown = True
        else:
            self.__SlowDown = False
        
        if self.__SlowDown is True:
            self.Speed -= int(self.__Acceleration * 3.6)
            if self.Speed <= 10:
                self.Speed = 0 
        else:
            self.Speed += int(self.__Acceleration * 3.6)
            if self.Speed > MaxSpeed:
                self.Speed = MaxSpeed
      
    def SetRoute(self, Node, G):
        route = nx.dijkstra_path(G, self.__From, Node, weight='length')
        self.__To = route[1]
        self.__RouteNodes = route
        """RouteLines - це на які полоси можна заходити під час руху."""
        try:
            for i in range(len(route)):
                self.__RouteLanes.append(self.SetRoad(G, self.__RouteNodes[route[i]], self.__RouteNodes[route[i + 1]], self.__RouteNodes[route[i + 2]]))
        except:
            self.__RouteLanes.append([0])
        
        self.__Road = G.get_edge_data(self.__From, self.__To)[0]
        self.__CurrentLane = 0

    def SelNearestNode(self, G):
        Nodes = G.nodes.data()
        Point = (self.Lon, self.Lat)
        Node, Len = ox.distance.get_nearest_node(G, Point, method='haversine', return_dist=True)
        self.__From = Node
        self.Lon = Nodes[Node]['y']
        self.Lat = Nodes[Node]['x']

    def Move(self, G, Cars, sec): # sec=1.00001
        #g = 0
        Nodes = G.nodes.data()
        SpeedLon = self.Speed / (3600 * self.__ConstLon)
        SpeedLat = self.Speed / (3600 * self.__ConstLat)
        Feature = {
            'type': 'Feature',
            'properties': {
                'time': sec, #sec * i * 1000
                'icon': 'marker',
                'popup': '<h1>{}</h1>'.format(self.Speed),
                'iconstyle': {
                    'iconUrl': 'Assets/car.png',
                    'iconSize': [40, 40]
                }
            },
            'geometry': {
                'type': 'Point',
                'coordinates': [self.Lat, self.Lon]
            },
        }
        

        Side1 = Nodes[self.__To]['x'] - Nodes[self.__From]['x']
        Side2 = Nodes[self.__To]['y'] - Nodes[self.__From]['y']
        MainSide = sqrt(Side2 ** 2 + Side1 ** 2)
        self.Dist += self.Speed / (3600 * self.__ConstLon)
        try:
            self.Lat += SpeedLat * (Side1 / MainSide)
        except:
            pass
        try:
            self.Lon += SpeedLon * (Side2 / MainSide)
        except:
            pass
        
        self.SetSpeed(G, Cars, 5)
        
        XRange = Nodes[self.__To]['x'] - SpeedLat < self.Lat < Nodes[self.__To]['x'] + SpeedLat
        YRange = Nodes[self.__To]['y'] - SpeedLon < self.Lon < Nodes[self.__To]['y'] + SpeedLon
        try:
            if XRange and YRange:
                #g += 1
                self.Dist = 0
                try:
                    self.__Road['queue'][self.__To][self.__CurrentLane]['cars'].remove(self.__Id) # видаляє машинку з полоси
                except:
                    pass
                self.__From = self.__To
                self.Lat = Nodes[self.__From]['x']
                self.Lon = Nodes[self.__From]['y']
                Index = self.__RouteNodes.index(self.__To)
                self.__To = self.__RouteNodes[Index + 1]
                self.__Road = G.get_edge_data(self.__From, self.__To)[0]
                Min = 100
                for Lanes in self.__RouteLanes[Index]:
                    for Lane in Lanes:
                        if len(Lane) <= Min:
                            self.__CurrentLane = Lane 
                self.__Road['queue'][self.__To][self.__CurrentLane]['cars'].append(self.__Id)
                    # if Index + 2 == len(self.__Route):
                    #     self.Draw(m)
                    #     print(Index, g, i)
                    #     print(len(self.__Route))
                    #     break
        except:
            pass

        return Feature      

    def Draw(self, m):
        folium.Marker(
            location=[self.Lon, self.Lat],
            tooltip='Легковий атомобіль',
            icon=self.__Icon
        ).add_to(m)

    def DrawRoute(self, Nodes, m, col):
        locations = []
        for i in range(len(self.__RouteNodes) - 1):
            x1 = Nodes[self.__RouteNodes[i]]['x']
            y1 = Nodes[self.__RouteNodes[i]]['y']
            x2 = Nodes[self.__RouteNodes[i + 1]]['x']
            y2 = Nodes[self.__RouteNodes[i + 1]]['y']
            locations.append([(y1, x1), (y2, x2)])

        print(locations)
        color = rgb_to_hex(col)
        folium.PolyLine(
            locations=locations,
            color='#' + color,
            weight=5,
            opacity=1
        ).add_to(m)
