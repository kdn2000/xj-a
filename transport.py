#import pandas
import networkx as nx
import math

class Transport:
    def __init__(self, longitude, latitude, max_speed, id, start_speed=0, acceleration = 5):
        self.__longitude = longitude
        self.__latitude = latitude
        self.__current_road = {"from": None, "to": None, "lenght": None, "angle": None, "id": None}  
        self.__speed = start_speed
        self.__maxSpeed = max_speed
        self.__acceleration = acceleration
        self.__id = id
        self.__s = 0.00 # s - пройдений шлях
        self.__direction_roads = None
        self.__is_delete = False

    def setDirection(self, G, node_from, node_to): # Дістає шлях
        self.__direction_roads = nx.dijkstra_path(G, node_from, node_to, "lenght")
        self.__setRoad(self.__direction_roads[0], self.__direction_roads[1], 1)

    def __setRoad(self, node_from, node_to, id): # Записує поточну дорогу, на якій вона знаходиться
        self.__current_road["to"] = node_to
        self.__current_road["from"] = node_from
        self.__current_road["id"] = id
        self.__current_road["lenght"] = math.sqrt(math.pow(node_from['x'] - node_to['x'], 2) + pow(node_from['y'] - node_to['y'], 2))
        self.__current_road["angle"] = math.atan((node_from['x'] - node_to['x'])/(node_from['y'] - node_to['y']))
        self.__s = 0.00
    
    def __setInBase(self, delete=False):
        """ Зробіть, так аби я зміг зробити тут запис до бази даних параметрів (id, longitude, latitude, speed, і т. п.) для візуалізації.
            Для оптимізації (щоб кожна машинка не діставала базу раз за разом) зробіть клас для доступу до бази. 
        """
        pass

    def __delete(self): # Записує параметер __is_delete. 
                        # Тобто, якщо машинка прибула до останньої ноди, то вона знищується (ну ви її знищете)
        self.__is_delete = True

    def __move(self, t): # Рух. Можливі помилки, так як треба її дебажити. Зробите анімацію + доступ до базу я її продебажу :))))

        dx = self.__speed * math.cos(self.__current_road["angle"]) * t
        dy = self.__speed * math.sin(self.__current_road["angle"]) * t
        ds = math.sqrt(dx ** 2 + dy ** 2)

        if self.__s + ds < self.__current_road["lenght"]:
            self.__s += ds
            self.__longitude += dx
            self.__latitude += dy
            self.__setInBase()

        else:
            id = self.__current_road["id"]

            if id is not len(self.__direction_roads) - 1:
                self.__s = 0.00
                self.__setRoad(self.__direction_roads[id], self.__direction_roads[id + 1], id + 1)
                cords = self.__current_road["to"]
                self.__longitude = cords['x'] # можлива помилка unsubscriptable, але потім пітону похер
                self.__latitude = cords['y']  # default'е значення - None, а не multidigraph.node
                self.__setInBase()

            else:
                self.__setInBase(True)
                self.__delete()
