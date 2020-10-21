from random import randint

class TrafficLights:

    def __init__(self, G):
        self.__G = G
        self.__Lights = []
    
    # Створюєм світлофори в архітектурі osmnx
    def CreateIfTrafficLights(self):
        for n, d in self.__G.nodes(data=True):

            if len(list(self.__G.edges(n))) > 3:
                d['is_light'] = True
                d['delay'] = randint(10, 100)
                d['is_open'] = True
                self.__Lights.append({'osmid' : n, 'timer' : 0})
                
            else:
                d['is_light'] = False
    
    # Розраховуєм чи зелене світло, чи червоне
    def Calc(self):
        Nodes = self.__G.nodes.data()
        for Light in self.__Lights:
            Light['timer'] += 1
            Node = Nodes[Light['osmid']]
            if Light['timer'] >= Node['delay']:
                Node['is_open'] = not Node['is_open']
                Light['timer'] = 0

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
                        'iconUrl': 'Assets/traffic_light_green.png' if Nodes[Light['osmid']]['is_open'] else 'Assets/traffic_light_red.png',
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

