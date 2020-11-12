from infrastructure import Infrastructure

import osmnx as ox
from dataDriver import DataDriver
import sys
import os
import socket
import threading

HOST = "127.0.0.1"
PORT = 65432

class PythonManager:
    def __init__(self, place):
        print(place)
        self._G = ox.graph_from_place("Ivano - Frankivsk, Ukraine", 'drive', simplify=False)
        self._infrastructure = Infrastructure(self._G)
        self._dataDriver = DataDriver(HOST, PORT)
        self._infrastructure.ExpansionRoads()
        self._infrastructure.CreateIfTrafficLights()
        # self._dataDriver.GetPOIs(place, {'amenity': ['cafe', 'fast_food', 'bus_station', 'fuel', 'parking', 'bank', 'clinic', 'hospital', 'nightclub', 'casino', 'fire_station', 'police', 'townhall', '	restaurant', 'swingerclub', 'stripclub'],
        #                            'building': 'office'})
        self._dataDriver.Export(self._G)
        self._StartCommunication()
    
    def _StartCommunication(self):
        self._event = threading.Event()
        self._event.set()
        self._listen = threading.Thread(target=self._Listening, args=(self._event,))
        self._listen.start()
        self._listen.join()
        
        

    def _Listening(self, event):
        while event.is_set():
            while True:
                data = self._dataDriver.Recieve() # buffer
                if not data:
                    break
                data = data.decode().split('#')
                if len(data) < 2:
                    comm = data[0].find('comm:')
                    if comm != -1:
                        comm = data[0][5:]
                        if comm == "exit":
                            self._Destroy()

                funcName = data[0].find('f:')
                argv = data[1].find('argv:')
                if funcName != -1 and argv != -1:
                    print("recive correct! ")
                    funcName = data[0][2:]
                    argv = data[1][5:].split(',')
                    argv.pop(len(argv) - 1)
                    print("funcName:", funcName, "argv:", argv)
                    func = getattr(self, funcName)
                    returnValue = func(argv)
                    self._dataDriver.Send(returnValue)
                else:
                    print("recive failed")
    
    def _Destroy(self):
        self._dataDriver.Destroy()
        self._event.clear()
        self._listen.join()
        

    def _SelNearestNode(self, attr):
        Point = (float(attr[0]), float(attr[1])) # Lon, lat
        Node, Len = ox.distance.get_nearest_node(self._G, Point, method='haversine', return_dist=True)
        return str(Node)
        


# def main():
#     place = sys.argv[1]
#     dataDir = sys.argv[2]
#     G = ox.graph_from_place(place, 'drive', simplify=False)
#     infrastructure = Infrastructure(G)
#     dataDriver = DataDriver(os.path.dirname(os.path.realpath(__file__)) + dataDir)
#     infrastructure.ExpansionRoads()
#     infrastructure.CreateIfTrafficLights()
#     dataDriver.GetPOIs(place, {'amenity': ['cafe', 'fast_food', 'bus_station', 'fuel', 'parking', 'bank', 'clinic', 'hospital', 'nightclub', 'casino', 'fire_station', 'police', 'townhall', '	restaurant', 'swingerclub', 'stripclub'],
#                                 'building': 'office'})
#     dataDriver.Export(G)
        
if __name__ == "__main__":
    place = sys.argv[1]
    pythonManager = PythonManager(place)
    