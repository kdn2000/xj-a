from transport import Car
from folmap import CreateMap
import osmnx as ox
import folium
from random import uniform, randint


def main():
    ox.config(use_cache=True)

    place = "Ukraine, Ivano-Frankivsk"
    G = ox.graph_from_place(place, 'drive', simplify=False)
    nodes = G.nodes

    m = CreateMap()

    print("Введіть координати машини")
    # lon, lat = map(float, input().split())
    cars_c = 1
    for i in range(cars_c):
        lon, lat = uniform(48.888, 48.948), uniform(24.68, 24.752)
        car = Car(lon, lat)
        car.SelNearestNode(G)
        car.SetRoute(list(nodes)[randint(0, 100)], G)
        car.Draw(m)
        color = (randint(0, 256), randint(0, 256), randint(0, 256))
        car.DrawRoute(nodes.data(), m, color)
        car.Move(G, m)

    m.save("map.html")


if __name__ == '__main__':
    main()