from transport import Car
from lights import TrafficLights
from dataDriver import DataDriver
from folmap import CreateMap
import osmnx as ox
from folium.plugins import TimestampedGeoJson
from random import uniform, randint


def main():
    ox.config(use_cache=True)

    place = "Ukraine, Ivano-Frankivsk"
    data_dir = './data'
    G = ox.graph_from_place(place, 'drive', simplify=False)
    nodes = G.nodes

    m = CreateMap()

    #print("Введіть координати машини")
    # lon, lat = map(float, input().split())
    
    data_driver = DataDriver(data_dir)
    data_driver.GetPOIs(place, {'amenity': ['cafe', 'fast_food', 'bus_station', 'fuel', 'parking', 'bank', 'clinic', 'hospital', 'nightclub', 'casino', 'fire_station', 'police', 'townhall', '	restaurant', 'swingerclub', 'stripclub'],
                                 'building': 'office'})
    
    Features = []
    
    traffic_lights = TrafficLights(G)
    traffic_lights.CreateIfTrafficLights()

    cars_c = 25
    cars = []
    # Створюєм машинки
    for i in range(cars_c):
        lon, lat = uniform(48.888, 48.948), uniform(24.68, 24.752)
        car = Car(lon, lat)
        car.SelNearestNode(G)
        car.SetRoute(list(nodes)[randint(0, 100)], G)
        # car.Draw(m)
        color = (randint(0, 256), randint(0, 256), randint(0, 256))
        car.DrawRoute(nodes.data(), m, color)
        cars.append(car)

    #  Main cycle 
    #  Сихронізація. i - це еквівалент часу ( його перетворення бачимо в time).
    for i in range(1000):
        time = i * 1.00001 * 1000
        traffic_lights.Calc()
        Features.extend(traffic_lights.DrawTrafficLights(time))
        for j in range(cars_c):
            Features.append(cars[j].Move(G, m, time)) 
        data_driver.UpdateFeatures(Features) # передаєм у клас DataDriver наші Features
        Features.clear()

    TimestampedGeoJson({
        'type': 'FeatureCollection',
        'features': data_driver.GetFeatures()
    },
        loop_button=True,
        period='PT1S',
        duration='PT1S',
        max_speed=60,
        min_speed=1,
        time_slider_drag_update=True,
        date_options='HH:mm:ss'
    ).add_to(m)
    

    m.save("map.html")
    
    data_driver.Export(G)

if __name__ == '__main__':
    main()