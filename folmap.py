import osmnx as ox
import folium
import numpy as np
from folium.plugins import MousePosition, MarkerCluster, Fullscreen, TimestampedGeoJson

""" Тут все норм, потреби лізти сюда немає"""
def AddTools(map):
    Fullscreen(
        position='topright',
        title='Expand me',
        title_cancel='Exit me',
        force_separate_button=True).add_to(map)

    MousePosition().add_to(map)


def AddLayers(map):
    folium.TileLayer('OpenStreetMap').add_to(map)
    folium.TileLayer('Stamen Terrain').add_to(map)
    folium.TileLayer('Stamen Toner').add_to(map)
    folium.TileLayer('Stamen Water Color').add_to(map)
    folium.TileLayer('cartodbpositron').add_to(map)
    folium.LayerControl().add_to(map)


def CreateMap(G=None):
    if G is not None:
        nodes = G.nodes(data=True)
        edges = list(G.edges)

        m = ox.plot_graph_folium(G)
    else:
        m = folium.Map(location=[48.92, 24.71], zoom_start=14, tiles='cartodbdark_matter')

    # marker_cluster = MarkerCluster().add_to(m)

    AddLayers(m)

    # for i in range(0, 100, 10):
    #     x = nodes[edges[i][0]]['x']
    #     y = nodes[edges[i][0]]['y']
    #
    #     pop = '<h4 style="color:green;">Тип: <div style="color:orange">легковий автомобіль</div></h4>' \
    #           '<h4 style="color:green;">Швидкість: <div style="color:orange">45 км/год</div></h4>' \
    #           f'<h4 style="color:green;">Координати:  <div style="color:orange">{x} {y}</div></h4>'
    #     folium.Marker(location=[y, x],
    #                   popup=pop,
    #                   tooltip='Легковий атомобіль',
    #                   icon=folium.features.CustomIcon('Assets/car.png', icon_size=(50, 50))).add_to(marker_cluster)

    AddTools(m)

    # features = []
    #
    # for i in range(len(edges)):
    #     x = nodes[edges[i][0]]['x']
    #     y = nodes[edges[i][0]]['y']
    #     feature = {
    #         'type': 'Feature',
    #         'properties': {
    #             'time': i * 1000
    #         },
    #         'geometry': {
    #             'type': 'Point',
    #             'coordinates': [x, y],
    #         },
    #     }
    #     features.append(feature)
    #
    #     x = nodes[edges[i][1]]['x']
    #     y = nodes[edges[i][1]]['y']
    #     feature = {
    #         'type': 'Feature',
    #         'properties': {
    #             'time': i * 1000
    #         },
    #         'geometry': {
    #             'type': 'Point',
    #             'coordinates': [x, y],
    #         },
    #     }
    #     features.append(feature)
    #
    # TimestampedGeoJson({
    #     'type': 'FeatureCollection',
    #     'features': features,
    # }, loop_button=True, period='PT1S', duration='PT1S').add_to(m)

    return m
