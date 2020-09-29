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

lines = [
    {
        'coordinates': [
            [139.76451516151428, 35.68159659061569],
            [139.75964426994324, 35.682590062684206],
        ],
        'dates': [
            '2017-06-02T00:00:00',
            '2017-06-02T00:10:00'
        ],
        'color': 'red'
    },
    {
        'coordinates': [
            [139.75964426994324, 35.682590062684206],
            [139.7575843334198, 35.679505030038506],
        ],
        'dates': [
            '2017-06-02T00:10:00',
            '2017-06-02T00:20:00'
        ],
        'color': 'blue'
    },
    {
        'coordinates': [
            [139.7575843334198, 35.679505030038506],
            [139.76337790489197, 35.678040905014065],
        ],
        'dates': [
            '2017-06-02T00:20:00',
            '2017-06-02T00:30:00'
        ],
        'color': 'green',
        'weight': 15,
    },
    {
        'coordinates': [
            [139.76337790489197, 35.678040905014065],
            [139.76451516151428, 35.68159659061569],
        ],
        'dates': [
            '2017-06-02T00:30:00',
            '2017-06-02T00:40:00'
        ],
        'color': '#FFFFFF',
    },
]

coords = []
for i in range(len(edges)):
    x = nodes[edges[i][0]]['x']
    y = nodes[edges[i][0]]['y']
    coords.append([x, y])
    x = nodes[edges[i][1]]['x']
    y = nodes[edges[i][1]]['y']
    coords.append([x, y])

times = [1000 * i for i in range(len(coords))]

features = [
      {
        'type': 'Feature',
        'geometry': {
          'type': 'LineString',
          'coordinates': coords,
          },
        'properties': {
          'times': times
          }
        }
      ]

TimestampedGeoJson({
    'type': 'FeatureCollection',
    'features': features,
}, loop_button=True, period='PT1S').add_to(m)


m.save("Edge.html")

# fig, ax = ox.plot_graph(G, node_size=0, close=False)
