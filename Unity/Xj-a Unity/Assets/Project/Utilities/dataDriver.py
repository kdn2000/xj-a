from osmnx import save_graph_geopackage, plot_graph, pois
from networkx.readwrite import json_graph
import json
import os
import socket

class DataDriver:
    def __init__(self, host, port, dir_):
        self.__FeaturesData = []
        self.__POIsData = None
        self.__socket  = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.__dir = dir_
        self.Connect(host, port)
    
    def Connect(self, host, port):
        try:
            self.__socket.connect((host, port))
        except:
            print("Coonection failed")
            self.Connect(host, port)
        print("connection succefull!")

    # Тут, по ідеї, мав бути пандас, але опитним путем було вияснено, що через списки швидше.
    # Це, напевно, через те, що пандас привик працювати з великою порцією данних.
    # А в нашій задачі використовується розрахунок данних не зразу. Тому проїзводітельность хромає.
    # Або я - дебільний хрущ - десь промахався ¯\_( :) )_/¯
    # begin
    # end
    
    # def SetIsBeganCommunicate(self):
    #     dir_ = self.__dir + '\\isCommunicate.tmp'
    #     with open(dir_, 'r+') as f:
    #         f.truncate(0)
    #         f.write("True")
    # def SetIsEndCommunicate(self):
    #     dir_ = self.__dir + '\\isCommunicate.tmp'
    #     with open(dir_, 'r+') as f:
    #         f.truncate(0)
    #         f.write

    # def GetPOIs(self, place, tags):
    #     dir_ = self.__dir + '\\pois.geojson'
    #     if os.path.exists(dir_) is False:
    #         self.__POIsData = pois.pois_from_place(place, tags)
    #         with open(dir_, 'w') as f:
    #             f.write(self.__POIsData.to_json())
    #     else:
    #         self.__POIsData = gpd.read_file(dir_)
    #     #print(self.__POIsData.head(8))
    
    def Send(self, command):
        length = len(command)
        tmp = ""
        g = 0
        self.__socket.sendall(str(length).encode())
        for char in command:
            g += 1
            tmp += char
            if g == 1024:
                g = 0
                self.__socket.sendall(tmp.encode())
                tmp = ""
        if tmp != "":
            self.__socket.sendall(tmp.encode())
        print("send")

    def Recieve(self):
        value = self.__socket.recv(1024)
        return value
    
    def Destroy(self):
        self.__socket.close()

    # Спізділь. Експорт нашого графа
    def Export(self, G, name, format_='json'):
        # if format_ == 'gpkg':
        #     save_graph_geopackage(G, filepath=self.__dir +'\\map.gpkg')
        # if format_ == 'svg':    
        #     fig, ax = plot_graph(G, show=False, save=True, close=True, filepath=self.__dir + '\\map.svg')
        if format_ == 'json':
            nodes_ = {}
            edges_ = {}
            # json.dump(dict(nodes=[dict(n, G.nodes[n]] for n in G.nodes()],
            #        edges=[[u, v, G.get_edge_data(u, v)[0]] for u, v in G.edges()]),
            #open(self.__dir + '/map.json', 'w'), indent=2)
            for n, d in G.nodes(data=True):
                nodes_[n] = d
            for v, u, d in G.edges.data():
                d['end'] = str(u)
                edges_[v] = d
                #print(edges_[v][u])
            with open(self.__dir + '/{0}.json'.format(name), 'w') as file:
                json.dump({str('nodes'): nodes_, str('edges'): edges_}, file)
            self.Send("Send json")
            print("Send json")

            