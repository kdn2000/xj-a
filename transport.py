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
    def __init__(self, Id, Longitude, Latitude, MaxSpeed=100, Speed=0, Type_="car"):
        self.Lon = Longitude
        self.Lat = Latitude
        self.__Id = Id
        self.__Type_ = Type_
        self.__Speed = 40 # km for hour
        self.__MaxSpeed = MaxSpeed
        self.__Route = []
        self.__From = None
        self.__To = None
        self.__Road = None
        self.__CurrentLine = None
        self.__Icon = CustomIcon('Assets/car.png', icon_size=(50, 50))
        self.__ConstLat = 111.32
        self.__ConstLon = 111.32 * cos(round(Latitude)*pi/360) # Найбільша похибка 1 км (sin(x - PI/360) - sin(x))    
        
    def GetId(self):
        return self.__Id
    
    """Вибирає полосу на якій їздити. Це для того, аби вірішити питання з перестройкою на полосу. Beta"""
    def ChangeRoad(self, G):
        self.__Road = G.get_edge_data(self.__From, self.__To)[0]
        if self.__Road['lanes'] == 1 or self.__Road['oneway'] == True:
            self.__Road['queue'][self.__To][0]['cars'].append(self.__Id)
            pass
        
        Index = self.__Route.index(self.__To) + 1
        # try:
        Side1 = G.nodes[self.__To]['x'] - G.nodes[self.__From]['x']
        Side2 = G.nodes[self.__To]['y'] - G.nodes[self.__From]['y']
        AngleCurrent = atan(Side2/Side1)
        Side1 = G.nodes[self.__Route[Index]]['x'] - G.nodes[self.__To]['x']
        Side2 = G.nodes[self.__Route[Index]]['y'] - G.nodes[self.__To]['y']
        AngleFuture = atan(Side2/Side1)
        Angle = AngleFuture - AngleCurrent
        # except:
        #     pass
        
        # try:
        if Angle <= pi/3:
            self.__CurrentLine = len(self.__Road['queue'][self.__To]) - 1
        elif Angle >= 3*pi/2:
            self.__CurrentLine = 0
        else:
            Lenght = len(self.__Road['queue'][self.__To])
            Index = None
            Min = 1000
            if Lenght == 2:
                for i in [0, 1]:
                    if len(self.__Road['queue'][self.__To][i]['cars'])  < Min:
                        Min = len(self.__Road['queue'][self.__To][i]['cars'])
                        Index = i
            else:
                for i in range(1, Lenght - 1):
                    if len(self.__Road['queue'][self.__To][i]['cars'])  < Min:
                        Min = len(self.__Road['queue'][self.__To][i]['cars'])
                        Index = i
            self.__CurrentLine = Index
        try:
            self.__Road['queue'][self.__To][self.__CurrentLine]['cars'].append(self.__Id)
        except:
            pass

    def SetRoute(self, Node, G):
        route = nx.dijkstra_path(G, self.__From, Node, weight='length')
        self.__To = route[1]
        self.__Route = route
        self.ChangeRoad(G)

    def SelNearestNode(self, G):
        Nodes = G.nodes.data()
        Point = (self.Lon, self.Lat)
        Node, Len = ox.distance.get_nearest_node(G, Point, method='haversine', return_dist=True)
        self.__From = Node
        self.Lon = Nodes[Node]['y']
        self.Lat = Nodes[Node]['x']

    def Move(self, G, m, sec): # sec=1.00001
        #g = 0
        Nodes = G.nodes.data()
        SpeedLon = self.__Speed / (3600 * self.__ConstLon)
        SpeedLat = self.__Speed / (3600 * self.__ConstLat)
        Feature = {
            'type': 'Feature',
            'properties': {
                'time': sec, #sec * i * 1000
                'icon': 'marker',
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
        try:
            self.Lat += SpeedLat * (Side1 / MainSide)
        except:
            pass
        try:
            self.Lon += SpeedLon * (Side2 / MainSide)
        except:
            pass
        
        
        
        XRange = Nodes[self.__To]['x'] - SpeedLat < self.Lat < Nodes[self.__To]['x'] + SpeedLat
        YRange = Nodes[self.__To]['y'] - SpeedLon < self.Lon < Nodes[self.__To]['y'] + SpeedLon
        try:
            if XRange and YRange:
                #g += 1
                self.__Road['queue'][self.__To][self.__CurrentLine]['cars'].remove(self.__Id) # видаляє машинку з полоси
                self.__From = self.__To
                self.Lat = Nodes[self.__From]['x']
                self.Lon = Nodes[self.__From]['y']
                Index = self.__Route.index(self.__To)
                self.__To = self.__Route[Index + 1]
                self.ChangeRoad(G) # вибрати полосу
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
        for i in range(len(self.__Route) - 1):
            x1 = Nodes[self.__Route[i]]['x']
            y1 = Nodes[self.__Route[i]]['y']
            x2 = Nodes[self.__Route[i + 1]]['x']
            y2 = Nodes[self.__Route[i + 1]]['y']
            locations.append([(y1, x1), (y2, x2)])

        print(locations)
        color = rgb_to_hex(col)
        folium.PolyLine(
            locations=locations,
            color='#' + color,
            weight=5,
            opacity=1
        ).add_to(m)
