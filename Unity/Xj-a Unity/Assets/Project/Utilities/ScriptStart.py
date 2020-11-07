from infrastructure import Infrastructure
import osmnx as ox
from dataDriver import DataDriver
import sys
import os

def main():
    place = sys.argv[1]
    dataDir = sys.argv[2]
    G = ox.graph_from_place(place, 'drive', simplify=False)
    infrastructure = Infrastructure(G)
    dataDriver = DataDriver(os.path.dirname(os.path.realpath(__file__)) + dataDir)
    infrastructure.ExpansionRoads()
    infrastructure.CreateIfTrafficLights()
    dataDriver.GetPOIs(place, {'amenity': ['cafe', 'fast_food', 'bus_station', 'fuel', 'parking', 'bank', 'clinic', 'hospital', 'nightclub', 'casino', 'fire_station', 'police', 'townhall', '	restaurant', 'swingerclub', 'stripclub'],
                                'building': 'office'})
    dataDriver.Export(G)
        
if __name__ == "__main__":
    main()
    