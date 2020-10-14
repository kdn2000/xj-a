from transport import Car
from folmap import CreateMap
import osmnx as ox
import folium
from folium.plugins import TimestampedGeoJson
from random import uniform, randint


def main():
    ox.config(use_cache=True)

    place = "Ukraine, Ivano-Frankivsk"
    G = ox.graph_from_place(place, 'drive', simplify=False)
    nodes = G.nodes

    m = CreateMap()

    print("Введіть координати машини")
    # lon, lat = map(float, input().split())
    Features = []
    cars_c = 25
    for i in range(cars_c):
        lon, lat = uniform(48.888, 48.948), uniform(24.68, 24.752)
        car = Car(lon, lat)
        car.SelNearestNode(G)
        car.SetRoute(list(nodes)[randint(0, 100)], G)
        # car.Draw(m)
        color = (randint(0, 256), randint(0, 256), randint(0, 256))
        car.DrawRoute(nodes.data(), m, color)
        for F in car.Move(G, m):
            Features.append(F)

    TimestampedGeoJson({
        'type': 'FeatureCollection',
        'features': Features,
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


if __name__ == '__main__':
    main()