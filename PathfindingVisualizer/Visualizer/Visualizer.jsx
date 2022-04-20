import React, {Component} from 'react';
import Node from './Node/Node';
import { astar } from '../Algorithm/Astar';
import { dijkstra } from '../Algorithm/Dijkstra';
import { getClassName } from './Node/Node';

import './Visualizer.css';

let START_NODE_ROW = 10;
let START_NODE_COL = 15;
let FINISH_NODE_ROW = 10;
let FINISH_NODE_COL = 35;

let is_moving_finish = false;
let is_moving_start = false;

let speed = 1;

let is_running = false;
let currentTimeout = 0;

let executionTime = 0;
let nodesVisited = 0;
let pathLength = 0;

let currentAlgorithm = 1;

export default class Visualizer extends Component {
    static Algorithm = Object.freeze({"Astar":1, "Dijkstra":2});

    constructor() {
      super();
      this.state = {
        grid: [],
        mouseIsPressed: false,
        currentTileType: Node.TileType.Wall
      };
    }

    componentDidMount() {
        const grid = getInitialGrid();
        this.setState({grid});

        this.changeCurrentTile(Node.TileType.Wall);

        window.addEventListener("mouseup", this.handleWindowMouseUp)
        window.addEventListener("mousedown", this.handleWindowMouseDown)
        window.addEventListener("dragstart", this.handleWindowMouseUp)
        // window.addEventListener('resize', this.getNewGrid);

        var slider = document.getElementById("myRange");
        slider.oninput = () => this.handleSpeedSlider(slider.value);

        var dropdown = document.getElementById("algorithmDropdown");
        dropdown.oninput = () => this.handleAlgorithmDropdown(dropdown.value);
    }

    componentWillUnmount(){
        window.removeEventListener("mouseup", this.handleWindowMouseUp)
        window.removeEventListener("mousedown", this.handleWindowMouseDown)
        window.removeEventListener("dragstart", this.handleWindowMouseUp)
        // window.removeEventListener('resize', this.getNewGrid);
    }

    clearResetGrid = () => {
        if (is_running){
            this.endVisualization();
        }
        this.clearGrid();
        const newGrid = getInitialGrid();
        this.setState({grid: newGrid});
    }

    getNewGrid = () => {
        if (is_running){
            this.endVisualization();
        }
        const newGrid = getInitialGrid();
        this.setState({grid: newGrid});
    }

    handleMouseDown(event, row, col) {
        event.preventDefault();
        const node = this.state.grid[row][col];
        if (node.Type === Node.TileType.Finish){
            is_moving_finish = true;
            return;
        }
        if (node.Type === Node.TileType.Start){
            is_moving_start = true;
            return;
        }
        const newGrid = changeTile(this.state.grid, row, col, this.state.currentTileType);
        this.setState({grid: newGrid});
    }
    
    handleMouseEnter(row, col) {
        if (!this.state.mouseIsPressed) return;

        if (is_moving_finish){
            if (row === START_NODE_ROW && col === START_NODE_COL){
                return;
            }

            changeTile(this.state.grid, FINISH_NODE_ROW, FINISH_NODE_COL, Node.TileType.Normal)
            changeTile(this.state.grid, row, col, Node.TileType.Finish)

            FINISH_NODE_COL = col;
            FINISH_NODE_ROW = row;

            this.setState({grid: this.state.grid});
            return;
        }
        if (is_moving_start){
            if (row === FINISH_NODE_ROW && col === FINISH_NODE_COL){
                return;
            }

            changeTile(this.state.grid, START_NODE_ROW, START_NODE_COL, Node.TileType.Normal)
            changeTile(this.state.grid, row, col, Node.TileType.Start)

            START_NODE_COL = col;
            START_NODE_ROW = row;

            this.setState({grid: this.state.grid});
            return;
        }
        changeTile(this.state.grid, row, col, this.state.currentTileType);
        this.setState({grid: this.state.grid});
    }   

    handleWindowMouseUp = (event) => {
        this.setState({mouseIsPressed: false});
        is_moving_finish = false;
        is_moving_start = false;
    }

    handleWindowMouseDown = (event) => {
        this.setState({mouseIsPressed: true});
    }

    changeCurrentTile = (type) => {
        const oldType = this.state.currentTileType;

        if (oldType === Node.TileType.Normal){
            document.getElementById('open-button').className = 'tile-button';
        } else if (oldType === Node.TileType.Wall){
            document.getElementById('wall-button').className = 'tile-button';
        } else if (oldType === Node.TileType.BigWeight){
            document.getElementById('big-weight-button').className = 'tile-button';
        } else if (oldType === Node.TileType.SmallWeight){
            document.getElementById('small-weight-button').className = 'tile-button';
        }

        this.setState({currentTileType: type});

        if (type === Node.TileType.Normal){
            document.getElementById('open-button').className = 'selected-tile-button';
        } else if (type === Node.TileType.Wall){
            document.getElementById('wall-button').className = 'selected-tile-button';
        } else if (type === Node.TileType.BigWeight){
            document.getElementById('big-weight-button').className = 'selected-tile-button';
        } else if (type === Node.TileType.SmallWeight){
            document.getElementById('small-weight-button').className = 'selected-tile-button';
        }
    }

    clearGrid = () => {
        if (is_running){
            clearTimeout(currentTimeout);
        }

        const oldGrid = this.state.grid;
        const newGrid = [];
        for (let i = 0; i < oldGrid.length; i++){
            const currentRow = [];
            for (let j = 0; j < oldGrid[i].length; j++){
                const node = oldGrid[i][j];
                currentRow.push(createTypeNode(j, i, node.Type));
                document.getElementById(`node-${node.row}-${node.col}`).className = getClassName(node, '')         
            }
            newGrid.push(currentRow);
        }

        this.setState({grid: newGrid});
    }

    visualizeCurrentAlgorithm = () => {
        this.clearGrid();

        const {grid} = this.state;

        const startNode = grid[START_NODE_ROW][START_NODE_COL];
        const finishNode = grid[FINISH_NODE_ROW][FINISH_NODE_COL];

        let time = window.performance.now();
        let output;

        if (currentAlgorithm == Visualizer.Algorithm.Astar){
            output = astar(grid, startNode, finishNode);
        } else if (currentAlgorithm == Visualizer.Algorithm.Dijkstra){
            output = dijkstra(grid, startNode, finishNode);
        }

        time = window.performance.now() - time;
        
        this.updateStats(time, output.nodesInShortestPathOrder.length, output.visitedNodesInOrder.length);
        
        this.animate(output.visitedNodesInOrder, output.nodesInShortestPathOrder, 1);
    }

    updateStats = (time, newPathLength, newNodesVisited) =>{
        executionTime = time;
        pathLength = newPathLength;
        nodesVisited = newNodesVisited;

        document.getElementById('executionTime').innerHTML = `Execution Time: ${executionTime.toFixed(2)}ms`;
        document.getElementById('nodesVisited').innerHTML = `Nodes Searched: ${nodesVisited}`;
        document.getElementById('pathLength').innerHTML = `Path Length: ${pathLength}`;
    }

    beginVisualization = () => {
        if (is_running){
            clearTimeout(currentTimeout);
        }
        is_running = true;


        this.visualizeCurrentAlgorithm();
    }

    endVisualization(){
        clearTimeout(currentTimeout);
        is_running = false;
    }

    animate(visitedNodesInOrder, nodesInShortestPathOrder, index){
        if (!is_running){
            return;
        }
        if (index >= visitedNodesInOrder.length) {
            this.animateShortestPath(nodesInShortestPathOrder, 1);
            return;
        }
        const node = visitedNodesInOrder[index].node;
        if (visitedNodesInOrder[index].isVisit) {
            document.getElementById(`node-${node.row}-${node.col}`).className = getClassName(node, 'node-visited');
            currentTimeout = setTimeout(() => {
                this.animate(visitedNodesInOrder, nodesInShortestPathOrder, index + 1)  
            }, speed * 50);
        } else{
            if (node.Type !== Node.TileType.Finish) document.getElementById(`node-${node.row}-${node.col}`).className = getClassName(node, 'node-looked-at');
            this.animate(visitedNodesInOrder, nodesInShortestPathOrder, index + 1);
        }
    }

    animateShortestPath(nodesInShortestPathOrder, index) {
        if (index >= nodesInShortestPathOrder.length - 1){
            this.endVisualization();
            return;
        }

        const node = nodesInShortestPathOrder[index];
        document.getElementById(`node-${node.row}-${node.col}`).className = getClassName(node, 'node-shortest-path')
        currentTimeout = setTimeout(() => {
            this.animateShortestPath(nodesInShortestPathOrder, index + 1);
        }, speed * 50);
    }

    handleSpeedSlider(value) {
        speed = (200 - value)/100;
    }

    handleAlgorithmDropdown(value) {
        currentAlgorithm = value;
    }

    render() {
        const {grid, mouseIsPressed} = this.state;
    
        return (
            <>
                <div className='toolbar-background' id='toolbar'>
                    <div className='toolbar'>
                        <div className='visual-button-panel'>
                            <select className="algorithm-dropdown" id="algorithmDropdown" defaultValue={Visualizer.Algorithm.Astar}>
                                <option value={Visualizer.Algorithm.Astar}>Astar</option>
                                <option value={Visualizer.Algorithm.Dijkstra}>Dijkstra</option>
                            </select>
                            <button className="block" onClick={() => this.beginVisualization()}>
                                Run
                            </button>
                            <button className="block" onClick={() => this.clearGrid()}>
                                Clear
                            </button>
                            <button className="block" onClick={() => this.clearResetGrid()}>
                                Reset
                            </button>
                            <div className='slider-panel'>
                                <div className='slider-label'>
                                    Speed
                                </div>
                                <div className="slidecontainer">
                                    <input type="range" min="1" max="200" defaultValue="100" className="slider" id="myRange"/>
                                </div>
                            </div>
                        </div>
                        <div className='tile-button-panel'>
                            <div>
                                <button id='wall-button' className='tile-button' onClick={() => this.changeCurrentTile(Node.TileType.Wall)}>
                                    <i className='wall-icon'></i>
                                </button>
                                <button id='open-button' className='tile-button' onClick={() => this.changeCurrentTile(Node.TileType.Normal)}>
                                    <i className='open-icon'></i>
                                </button>
                            </div>
                            <div>
                                <button id='small-weight-button' className='tile-button' onClick={() => this.changeCurrentTile(Node.TileType.SmallWeight)}>
                                    <i className='small-weight-icon'></i>
                                </button>
                                <button id='big-weight-button' className='tile-button' onClick={() => this.changeCurrentTile(Node.TileType.BigWeight)}>
                                    <i className='big-weight-icon'></i>
                                </button>
                            </div>
                        </div>
                        <div className='info-panel'>
                            <div id='executionTime'>
                                Execution Time: 0ms
                            </div>
                            <div id='pathLength'>
                                Path Length: 0
                            </div>
                            <div id='nodesVisited'>
                                Nodes Searched: 0
                            </div>
                        </div>
                    </div>
            </div>
                <div className="grid"> 
                    {grid.map((row, rowIdx) => {
                        return (
                            <div key={rowIdx} className="row">
                                {row.map((node, nodeIdx) => {
                                    const {row, col, Type} = node;
                                        return (
                                            <Node
                                                key={nodeIdx}
                                                col={col}
                                                row={row}
                                                Type={Type}
                                                mouseIsPressed={mouseIsPressed}
                                                onMouseDown={(event, row, col) => this.handleMouseDown(event, row, col)}
                                                onMouseEnter={(row, col) => this.handleMouseEnter(row, col)}></Node>
                                        );
                                })}
                            </div>
                        );
                    })}
                </div>
            </>
        );
    }
}

const getInitialGrid = () => {
    const grid = [];
    const height = Math.floor((window.innerHeight - document.getElementById(`toolbar`).offsetHeight) / 25); 
    const width = Math.floor(window.innerWidth / 25);

    FINISH_NODE_ROW = Math.floor(height * Math.random());
    FINISH_NODE_COL = Math.floor(width * Math.random());
    START_NODE_ROW = Math.floor(height * Math.random());
    START_NODE_COL = Math.floor(width * Math.random());

    for (let row = 0; row < height; row++) {
      const currentRow = [];
      for (let col = 0; col < width; col++) {
        currentRow.push(createNode(col, row));
      }
      grid.push(currentRow);
    }
    return grid;
};

const resizeGrid = () => {
    const grid = [];
    const height = Math.floor((window.innerHeight - document.getElementById(`toolbar`).offsetHeight) / 25); 
    const width = Math.floor(window.innerWidth / 25);

    const oldGrid = this.state.grid;

    for (let row = 0; row < height; row++) {
        const currentRow = [];
        for (let col = 0; col < width; col++) {
            if (row < oldGrid.length && col < oldGrid[row].length){
                currentRow.push(createTypeNode(col, row, oldGrid[row][col].Type));
            } else {
                currentRow.push(createNode(col, row));
            }
        }
        grid.push(currentRow);
      }
      return grid;
}
  
const createNode = (col, row) => {

    let Type = Node.TileType.Normal;

    if (col === START_NODE_COL && row === START_NODE_ROW){
        Type = Node.TileType.Start;
    }

    if (col === FINISH_NODE_COL && row === FINISH_NODE_ROW){
        Type = Node.TileType.Finish;
    }

    return {
      col,
      row,
      Type: Type,
      distance: Infinity,
      distanceFromGoal: distance(row, col, FINISH_NODE_ROW, FINISH_NODE_COL),
      isVisited: false,
      previousNode: null,
    };
};

const createTypeNode = (col, row, type) => {
    return {
      col,
      row,
      Type: type,
      distance: Infinity,
      distanceFromGoal: distance(row, col, FINISH_NODE_ROW, FINISH_NODE_COL),
      isVisited: false,
      previousNode: null,
    };
};

const distance = (x1, y1, x2, y2) => {
    return Math.abs(x1 - x2) + Math.abs(y1 - y2);
}

const changeTile = (grid, row, col, type) => {
    if (grid[row][col].Type === Node.TileType.Finish && !is_moving_finish){
        return grid;
    }
    if (grid[row][col].Type === Node.TileType.Start && !is_moving_start){
        return grid;
    }
    delete grid[row][col];
    const node = createTypeNode(col, row, type);
    grid[row][col] = node;
    return grid;
};