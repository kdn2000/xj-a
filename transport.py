import math
import osmnx as ox
import folium
from folium.plugins import TimestampedGeoJson
from math import atan, cos, sin, fabs
from random import choice


def rgb_to_hex(rgb):
    return '%02x%02x%02x' % rgb


class Car:
    def __init__(self, Longitude, Latitude, MaxSpeed=100, Speed=0, Type_="car"):
        self.Lon = Longitude
        self.Lat = Latitude
        self.__Type_ = Type_
        self.__Speed = Speed
        self.__MaxSpeed = MaxSpeed
        self.__Route = []
        self.__From = None
        self.__To = None

    def SetRoute(self, Node, G):
        route = ox.shortest_path(G, self.__From, Node, weight='length')
        self.__To = route[1]
        self.__Route = route

    def SelNearestNode(self, G):
        Nodes = G.nodes.data()
        Point = (self.Lon, self.Lat)
        Node, Len = ox.distance.get_nearest_node(G, Point, method='haversine', return_dist=True)
        self.__From = Node
        self.Lon = Nodes[Node]['y']
        self.Lat = Nodes[Node]['x']

    def Move(self, G, m, sec=1):
        Nodes = G.nodes.data()
        Features = []
        for i in range(1000):
            Feature = {
                'type': 'Feature',
                'properties': {
                    'time': sec * i * 1000
                },
                'geometry': {
                    'type': 'Point',
                    'coordinates': [self.Lat, self.Lon],
                },
            }

            Side1 = Nodes[self.__To]['x'] - Nodes[self.__From]['x']
            Side2 = Nodes[self.__To]['y'] - Nodes[self.__From]['y']
            # Angle = atan(fabs(Side2 / Side1))
            self.Lat += 0.1 * Side1
            self.Lon += 0.1 * Side2
            Features.append(Feature)

            XRange = Nodes[self.__To]['x'] - 0.00000001 < self.Lat < Nodes[self.__To]['x'] + 0.00000001
            YRange = Nodes[self.__To]['y'] - 0.00000001 < self.Lon < Nodes[self.__To]['y'] + 0.00000001
            if XRange and YRange:
                self.__From = self.__To
                try:
                    Index = self.__Route.index(self.__To)
                    self.__To = self.__Route[Index + 1]
                except:
                    pass

        TimestampedGeoJson({
            'type': 'FeatureCollection',
            'features': Features,
        }, loop_button=True, period='PT1S', duration='PT1S').add_to(m)

    def Draw(self, m):
        folium.Marker(
            location=[self.Lon, self.Lat],
            tooltip='Легковий атомобіль',
            icon=folium.features.CustomIcon('Assets/icons8-car-48.png', icon_size=(50, 50))
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
