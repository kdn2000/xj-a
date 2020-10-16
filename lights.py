import folium
from random import randint

class TrafficLights:

    def __init__(self, G):
        self.__G = G
        self.__Lights = []
        self.__LightsTimers = []

    def CreateIfTrafficLights(self):
        for n, d in self.__G.nodes(data=True):

            if len(list(self.__G.edges(n))) > 3:
                d['is_light'] = True
                d['delay'] = randint(10, 100)
                d['is_open'] = True
                self.__Lights.append({'osmid' : n, 'timer' : 0})
                
            else:
                d['is_light'] = False
    
    def Calc(self):
        Nodes = self.__G.nodes.data()
        for Light in self.__Lights:
            Light['timer'] += 1
            Node = Nodes[Light['osmid']]
            if Light['timer'] >= Node['delay']:
                Node['is_open'] = not Node['is_open']
                Light['timer'] = 0

    def DrawTrafficLights(self, m):
        for n, d in self.__G.nodes(data=True):

            if d['is_light']:
                folium.Marker(
                    location=[d['y'], d['x']],
                    tooltip='Світло_курва_фор',
                ).add_to(m)

