import requests
from bs4 import BeautifulSoup
from urllib.parse import urlparse, urljoin
from tqdm import tqdm
import networkx as nx
import matplotlib.pyplot as plt
from collections import deque


def get_links(url):
    response = requests.get(url)
    soup = BeautifulSoup(response.text, 'html.parser')
    return [urljoin(url, a['href']) for a in soup.find_all('a', href=True)]


def bfs_traversal(start_url, num_layers, graph):
    visited = set()
    queue = deque([(start_url, 0)])
    
    plt.ion()  # Turn on interactive mode
    
    while queue:
        url, layer = queue.popleft()
        if layer > num_layers:
            break
        if url not in visited:
            visited.add(url)
            connections = get_links(url)
            for x in connections:
                graph.add_edge(url, x)
                queue.append((x, layer + 1))
                plt.clf()
                try:
                    nx.draw_networkx(graph, pos=pos, with_labels=False, node_size=1)
                except:
                    nx.draw_networkx(graph, with_labels=False, node_size=1)
                plt.pause(0.001)  # Add a short pause for visualization

    plt.ioff()  # Turn off interactive mode after loop
                
                


scan_layers = 1
entrance = "https://www.youtube.com/"


graph = nx.Graph()
seed = 0
pos = nx.spring_layout(graph, seed=seed)


bfs_traversal(entrance, scan_layers, graph)





nx.draw_networkx(graph, with_labels=False, node_size=1)
plt.show()