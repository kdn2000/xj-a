import geopandas as gpd
from osmnx import save_graph_geopackage, plot_graph, pois
from networkx.readwrite import json_graph
import json
import os

class DataDriver:
    def __init__(self, dir_):
        self.__FeaturesData = []
        self.__POIsData = None
        self.__dir = dir_
        
    # Тут, по ідеї, мав бути пандас, але опитним путем було вияснено, що через списки швидше.
    # Це, напевно, через те, що пандас привик працювати з великою порцією данних.
    # А в нашій задачі використовується розрахунок данних не зразу. Тому проїзводітельность хромає.
    # Або я - дебільний хрущ - десь промахався ¯\_( :) )_/¯
    # begin
    def UpdateFeatures(self, Features):
        self.__FeaturesData.extend(Features)
                                                   
    def GetFeatures(self):
        return self.__FeaturesData
    # end
    
    
    def GetPOIs(self, place, tags):
        dir_ = self.__dir + '/pois.geojson'
        if os.path.exists(dir_) is False:
            self.__POIsData = pois.pois_from_place(place, tags)
            with open(dir_, 'w') as f:
                f.write(self.__POIsData.to_json())
        else:
            self.__POIsData = gpd.read_file(dir_)
        #print(self.__POIsData.head(8))
    
    # Спізділь. Експорт нашого графа
    def Export(self, G, format_='json'):
        if format_ == 'gpkg':
            save_graph_geopackage(G, filepath=self.__dir +'/map.gpkg')
        if format_ == 'svg':    
            fig, ax = plot_graph(G, show=False, save=True, close=True, filepath=self.__dir + '/map.svg')
        if format_ == 'json':
            nodes_ = {}
            edges_ = {}
            # json.dump(dict(nodes=[dict(n, G.nodes[n]] for n in G.nodes()],
            #        edges=[[u, v, G.get_edge_data(u, v)[0]] for u, v in G.edges()]),
            #open(self.__dir + '/map.json', 'w'), indent=2)
            for n, d in G.nodes(data=True):
                nodes_[n] = d
            for v, u, d in G.edges.data():
                edges_[v] = {u: d}
                print(edges_[v][u])
            json.dump({str('nodes'): nodes_, str('edges'): edges_}, open(self.__dir + '/map.json', 'w'), indent=2)