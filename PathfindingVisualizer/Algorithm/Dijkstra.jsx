import Node from '../Visualizer/Node/Node';
import Heap from 'heap';

export function dijkstra(grid, startNode, finishNode) {
    const visitedNodesInOrder = [];
    startNode.distance = 0;
    var openHeap = new Heap(function(nodeA, nodeB) {
        const difference = nodeA.distance - nodeB.distance;
        return difference;
    });
    var openSet = new Set();

    openHeap.push(startNode);
    openSet.add(startNode);

    while (openHeap.size() > 0) {
      const closestNode = openHeap.pop();
      if (closestNode === finishNode){
        return  {
          'visitedNodesInOrder': visitedNodesInOrder,
          'nodesInShortestPathOrder': constructPath(closestNode)
        };
      }

      visitedNodesInOrder.push({"node": closestNode, "isVisit": true});

      getNeighbors(closestNode, grid).forEach(neighbor => addNeighbor(neighbor, closestNode, openSet, openHeap, visitedNodesInOrder));
    }

    return {
      'visitedNodesInOrder': visitedNodesInOrder,
      'nodesInShortestPathOrder': null
    };
  }

  function addNeighbor(neighbor, closestNode, openSet, openHeap, visitedNodesInOrder){
    if (neighbor.Type === Node.TileType.Wall){
        return;
    }

    let weight = 1;
    if (neighbor.Type === Node.TileType.SmallWeight){
        weight = 3;
    }
    if (neighbor.Type === Node.TileType.BigWeight){
        weight = 5;
    }

    const tentativeScore = closestNode.distance + weight;
    if (tentativeScore < neighbor.distance){
        neighbor.previousNode = closestNode;
        neighbor.distance = tentativeScore;

        if (!openSet.has(neighbor)){
            visitedNodesInOrder.push({"node": neighbor, "isVisit": false});
            openSet.add(neighbor);
            openHeap.push(neighbor);
        }
    }
  }
  
  function getNeighbors(node, grid) {
      const neighbors = [];

      if (node.row > 0){
        neighbors.push(grid[node.row - 1][node.col])
      }
      if (node.col > 0){
        neighbors.push(grid[node.row][node.col - 1])
      }
      if (node.row < grid.length - 1){
        neighbors.push(grid[node.row + 1][node.col])
      }
      if (node.col < grid[node.row].length - 1){
        neighbors.push(grid[node.row][node.col + 1])
      }

      return neighbors;
  }
  
  export function constructPath(finishNode) {
    const nodesInShortestPathOrder = [];
    let currentNode = finishNode;
    
    while (currentNode !== null) { 
      nodesInShortestPathOrder.unshift(currentNode);
      currentNode = currentNode.previousNode;
    }
    return nodesInShortestPathOrder;
  }