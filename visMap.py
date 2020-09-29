import osmnx as ox
import folium
import numpy as np
from folium.plugins import MousePosition, TimestampedGeoJson, MarkerCluster, Fullscreen

ox.config(use_cache=True)

place = "Ukraine, Ivano-Frankivsk"
G = ox.graph_from_place(place, 'drive', simplify=True)

nodes = G.nodes(data=True)
edges = list(G.edges)


m = ox.plot_graph_folium(G)
m = folium.Map(location=[48.92, 24.71], zoom_start=14)

marker_cluster = MarkerCluster().add_to(m)

folium.TileLayer('OpenStreetMap').add_to(m)
folium.TileLayer('Stamen Terrain').add_to(m)
folium.TileLayer('Stamen Toner').add_to(m)
folium.TileLayer('Stamen Water Color').add_to(m)
folium.TileLayer('cartodbpositron').add_to(m)
folium.TileLayer('cartodbdark_matter').add_to(m)
folium.LayerControl().add_to(m)

for i in range(0, 100, 10):
    x = nodes[edges[i][0]]['x']
    y = nodes[edges[i][0]]['y']

    pop = '<h4 style="color:green;">Тип: <div style="color:orange">легковий автомобіль</div></h4>' \
          '<h4 style="color:green;">Швидкість: <div style="color:orange">45 км/год</div></h4>' \
          f'<h4 style="color:green;">Координати:  <div style="color:orange">{x} {y}</div></h4>'
    folium.Marker(location=[y, x],
                  popup=pop,
                  tooltip='Легковий атомобіль',
                  icon=folium.features.CustomIcon('Assets/car.png', icon_size=(50, 50))).add_to(marker_cluster)

# g = TimeSliderChoropleth().add_to(m)

Fullscreen(
    position='topright',
    title='Expand me',
    title_cancel='Exit me',
    force_separate_button=True).add_to(m)

MousePosition().add_to(m)


features = []

for i in range(len(edges)):
    x = nodes[edges[i][0]]['x']
    y = nodes[edges[i][0]]['y']
    feature = {
        'type': 'Feature',
        'properties': {
            'time': i * 1000
        },
        'geometry': {
          'type': 'Point',
          'coordinates': [x, y],
          },
        }
    features.append(feature)

    x = nodes[edges[i][1]]['x']
    y = nodes[edges[i][1]]['y']
    feature = {
        'type': 'Feature',
        'properties': {
            'time': i * 1000
        },
        'geometry': {
            'type': 'Point',
            'coordinates': [x, y],
        },
    }
    features.append(feature)

TimestampedGeoJson({
    'type': 'FeatureCollection',
    'features': features,
}, loop_button=True, period='PT1S').add_to(m)


m.save("Edge.html")

# fig, ax = ox.plot_graph(G, node_size=0, close=False)
