from random import randint
from math import atan, pi, ceil, floor

class Infrastructure:

    def __init__(self, G):
        self.__G = G
        self.__Lights = []
    
    
    """ Роширення доріг. Добавляє полоси у яких масиви з ід машинок"""
    def ExpansionRoads(self):
        for v, u, d in self.__G.edges(data=True):
            if not ('lanes' in d):
                lanes = 2
            else:    
                lanes = int(d['lanes'])
            if lanes == 1 or d['oneway'] == True:
                d['queue'] = {u: {0: {'cars': []}}, v: None}
                d['lanes'] = lanes
                continue
            d['lanes'] = lanes
            d['queue'] = {v: [{'cars': []} for i in range(ceil(lanes/2))], u: [{'cars': []} for i in range(floor(lanes/2))]}
    
    """ Створюєм світлофори в архітектурі osmnx. Обнова: тепер для доріг різні delays і відкрито/закрито"""
    def CreateIfTrafficLights(self):
        for n, d in self.__G.nodes(data=True):
            if len(list(self.__G.edges(n))) > 3:
                d['is_light'] = True
                d['delay'] = [randint(20, 100), randint(20, 100)]
                d['timer'] = 0
                d['for_open'] = {'first_group': [], 'second_group': []}
                d['is_open'] = [True, False]
                try:
                    Edges = [e for e in self.__G.edges(n)]
                    BaseSide1 = self.__G.nodes[Edges[0][1]]['x'] - self.__G.nodes[Edges[0][0]]['x']
                    BaseSide2 = self.__G.nodes[Edges[0][1]]['y'] - self.__G.nodes[Edges[0][0]]['y']
                    BaseAngle = atan(BaseSide2/BaseSide1)
                    Edges.pop(0)
                    for v, u in Edges:
                        Side1 = self.__G.nodes[u]['x'] - self.__G.nodes[v]['x']
                        Side2 = self.__G.nodes[u]['y'] - self.__G.nodes[v]['y']
                        Angle = atan(Side2/Side1) - BaseAngle
                        if Angle >= pi/2 and Angle <= 3*pi/2:
                            d['for_open']['first_group'].append((v, u))
                        else:
                            d['for_open']['second_group'].appen((v, u))
                            
                except:
                    pass
                
                self.__Lights.append({'osmid' : n})
            else:
                d['is_light'] = False
    
    # Розраховуєм чи зелене світло, чи червоне
    def Calc(self):
        Nodes = self.__G.nodes.data()
        for Light in self.__Lights:
            Node = Nodes[Light['osmid']]
            Node['timer'] += 1
            if Node['is_open'][0] is True:
                if Light['timer'] >= Node['delay'][1]:
                    Node['is_open'] = [False, True]
            if Node['is_open'][1] is True:
                if Light['timer'] >= Node['delay'][0]:
                    Node['is_open'] = [True, False]
            # if Light['timer'] >= Node['delay']:
            #     Node['is_open'] = not Node['is_open']
            #     Light['timer'] = 0

    # Створюєм Features для кожного світлофора
    def DrawTrafficLights(self, sec):
        Nodes = self.__G.nodes.data()
        Features = []
        for Light in self.__Lights:
            Feature = {
                'type': 'Feature',
                'properties': {
                    'time': sec, #sec * i * 1000
                    'icon': 'marker',
                    'iconstyle': {
                        'iconUrl': 'Assets/traffic_light_green.png' if Nodes[Light['osmid']]['is_open'][0] else 'Assets/traffic_light_red.png',
                        'iconSize': [40, 40]
                    }
                },
                'geometry': {
                    'type': 'Point',
                    'coordinates': [Nodes[Light['osmid']]['x'], Nodes[Light['osmid']]['y']]
                },
            }
            print(Feature)
            Features.append(Feature)
        return Features
