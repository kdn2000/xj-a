import geopandas as gpd
from osmnx import save_graph_geopackage, plot_graph, pois
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
    def Export(self, G, format_='gpkg'):
        if format_ == 'gpkg':
            save_graph_geopackage(G, filepath=self.__dir +'/map.gpkg')
        if format_ == 'svg':    
            fig, ax = plot_graph(G, show=False, save=True, close=True, filepath=self.__dir + '/map.svg')
            
        
        