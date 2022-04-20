import React, {Component} from 'react';

import './Node.css';

export default class Node extends Component {
    static TileType = Object.freeze({"Normal":1, "Wall":2, "Start":3, "Finish":4, "SmallWeight":5, "BigWeight":6})

    constructor() {
      super();
      this.state = { };
    }

    render() {
        const {
            col,
            row,
            Type,
            onMouseDown,
            onMouseEnter
        } = this.props;

        let extraClassName = (Node.TileType.Finish === Type)
            ? 'node-finish'
            : (Node.TileType.Start === Type)
            ? 'node-start'
            : (Node.TileType.Wall === Type)
            ? 'node-wall'
            : (Node.TileType.SmallWeight === Type)
            ? 'node-small-weight'
            : (Node.TileType.BigWeight === Type)
            ? 'node-big-weight'
            : '';


        if (Type === Node.TileType.SmallWeight){
            return (
                <div
                draggable="false"
                id={`node-${row}-${col}`}
                className={`node ${extraClassName}`}      
                onMouseDown={(event) => onMouseDown(event, row, col)}
                onMouseEnter={() => onMouseEnter(row, col)}>
                    <i className='.node-small-weight'></i>
                </div>
            )
        }
        if (Type === Node.TileType.BigWeight){
            return (
                <div
                draggable="false"
                id={`node-${row}-${col}`}
                className={`node ${extraClassName}`}      
                onMouseDown={(event) => onMouseDown(event, row, col)}
                onMouseEnter={() => onMouseEnter(row, col)}>
                    <i className='.node-big-weight'></i>
                </div>
            )
        }

        return (
            <div
            draggable="false"
            id={`node-${row}-${col}`}
            className={`node ${extraClassName}`}      
            onMouseDown={(event) => onMouseDown(event, row, col)}
            onMouseEnter={() => onMouseEnter(row, col)}></div>
        );        
    }
}

export function getClassName(node, extra){
    let extraClassName = (Node.TileType.Finish === node.Type)
    ? 'node-finish '
    : (Node.TileType.Start === node.Type)
    ? 'node-start '
    : (Node.TileType.Wall === node.Type)
    ? 'node-wall '
    : (Node.TileType.SmallWeight === node.Type)
    ? 'node-small-weight '
    : (Node.TileType.BigWeight === node.Type)
    ? 'node-big-weight '
    : '';

    return 'node '+ extraClassName + extra;
}