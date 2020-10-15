import folium
from random import randint

class TrafficLights:

    def __init__(self, G):
        self.__G = G

    def CreateIfTrafficLights(self):
        for n, d in self.__G.nodes(data=True):

            if len(list(self.__G.edges(n))) > 3:
                d['is_light'] = True
                d['delay'] = randint(10, 100)

            else:
                d['is_light'] = False
    
    def DrawTrafficLights(self, m):
        for n, d in self.__G.nodes(data=True):

            if d['is_light']:
                folium.Marker(
                    location=[d['y'], d['x']],
                    tooltip='Світло_курва_фор',
                ).add_to(m)

