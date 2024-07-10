using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowField {

    /// <summary>
    /// Represents a grid structure composed of chunks, allowing retrieval and modification of navigation nodes within defined grid indices.
    /// The grid is initialized with specified dimensions and chunk sizes, and supports operations to get and set navigation nodes at specific indices.
    /// Additional functionality includes checking for out-of-bounds indices and calculating neighboring nodes optionally including diagonals.
    /// </summary>
    public class Grid
    {
        private Chunk[,] _chunks;
        private Vector2Int _size;
        private Vector2Int _chunkSize;
        
        /// <summary>
        /// Initializes a grid structure with specified dimensions and chunk sizes.
        /// </summary>
        /// <param name="size">The overall size of the grid in grid units (tiles).</param>
        /// <param name="chunkSize">The size of each chunk in grid units.</param>
        public Grid(Vector2Int size, Vector2Int chunkSize) {
            _size = size;
            _chunkSize = chunkSize;
            _chunks = new Chunk[Mathf.CeilToInt(_size.x / _chunkSize.x), Mathf.CeilToInt(_size.y / _chunkSize.y)];

            for( int i = 0; i < _chunks.GetLength(0); i++) {
                for( int j = 0; j < _chunks.GetLength(1); j++) {
                    Vector2Int tempChunkSize = new Vector2Int(Mathf.Min(_chunkSize.x, _size.x - i * _chunkSize.x), Mathf.Min(_chunkSize.y, _size.y - j * _chunkSize.y));
                    _chunks[i,j] = new Chunk(tempChunkSize, new Vector2Int(i * tempChunkSize.x, j * tempChunkSize.y));
                }
            }
        }

        private NavNode[] GetNeighbors(Vector2Int positionInt, bool getDiagonals)
        {
            NavNode[] neighbors;
            if(getDiagonals) neighbors = new NavNode[8];
            else neighbors = new NavNode[4];

            // Add Top
            if (positionInt.x >= 0 && positionInt.x < _size.x && positionInt.y + 1 >= 0 && positionInt.y + 1 < _size.y)
            {
                neighbors[0] = GetNavNode(new Vector2Int(positionInt.x, positionInt.y + 1));
            }
            // Add Bottom
            if (positionInt.x >= 0 && positionInt.x < _size.x && positionInt.y - 1 >= 0 && positionInt.y - 1 < _size.y)
            {
                neighbors[1] = GetNavNode(new Vector2Int(positionInt.x, positionInt.y - 1));
            }
            // Add Right
            if (positionInt.x + 1 >= 0 && positionInt.x + 1 < _size.x && positionInt.y >= 0 && positionInt.y < _size.y)
            {
                neighbors[2] = GetNavNode(new Vector2Int(positionInt.x + 1, positionInt.y));
            }
            // Add Left
            if (positionInt.x - 1 >= 0 && positionInt.x - 1 < _size.x && positionInt.y >= 0 && positionInt.y < _size.y)
            {
                neighbors[3] = GetNavNode(new Vector2Int(positionInt.x - 1, positionInt.y));
            }
            
            // If only the direct neighbors are wanted, return here
            if(!getDiagonals) return neighbors;

            // Add Top Right
            if (positionInt.x + 1 >= 0 && positionInt.x + 1 < _size.x && positionInt.y + 1 >= 0 && positionInt.y + 1< _size.y)
            {
                neighbors[4] = GetNavNode(new Vector2Int(positionInt.x + 1, positionInt.y + 1));
            }
            // Add Bottom Right
            if (positionInt.x + 1 >= 0 && positionInt.x + 1 < _size.x && positionInt.y - 1 >= 0 && positionInt.y - 1< _size.y)
            {
                neighbors[5] = GetNavNode(new Vector2Int(positionInt.x + 1, positionInt.y - 1));
            }
            // Add Top Left
            if (positionInt.x - 1 >= 0 && positionInt.x - 1 < _size.x && positionInt.y + 1 >= 0 && positionInt.y + 1< _size.y)
            {
                neighbors[6] = GetNavNode(new Vector2Int(positionInt.x - 1, positionInt.y + 1));
            }
            // Add Bottom Left
            if (positionInt.x - 1 >= 0 && positionInt.x - 1 < _size.x && positionInt.y - 1 >= 0 && positionInt.y - 1< _size.y)
            {
                neighbors[7] = GetNavNode(new Vector2Int(positionInt.x - 1, positionInt.y - 1));
            }

            return neighbors;
        }

        /// <summary>
        /// Retrieves the navigation node at the specified grid index.
        /// </summary>
        /// <param name="index">The grid index of the navigation node to retrieve.</param>
        /// <returns>The navigation node at the specified index.</returns>
        public NavNode GetNavNode(Vector2Int index) {
            CheckOutOfBounds(index);

            Chunk chunk = _chunks[Mathf.FloorToInt(index.x / _chunkSize.x), Mathf.FloorToInt(index.y / _chunkSize.y)];
            Vector2Int minChunkIndex = new Vector2Int(Mathf.FloorToInt(index.x / _chunkSize.x) * _chunkSize.x, Mathf.FloorToInt(index.y / _chunkSize.y) * _chunkSize.y);
            return chunk.GetNode(new Vector2Int(index.x - minChunkIndex.x, index.y - minChunkIndex.y));
        }

        /// <summary>
        /// Sets the navigation node at the specified grid index.
        /// </summary>
        /// <param name="navNode">The navigation node to set.</param>
        /// <param name="index">The grid index where the navigation node should be set.</param>
        public void SetNavNode(NavNode navNode, Vector2Int index) {
            if(navNode == null) Debug.LogWarning("Attempting to add null to the grid, which may cause unexpected behavior.");
            CheckOutOfBounds(index);

            Chunk chunk = _chunks[Mathf.FloorToInt(index.x / _chunkSize.x), Mathf.FloorToInt(index.y / _chunkSize.y)];
            Vector2Int minChunkIndex = new Vector2Int(Mathf.FloorToInt(index.x / _chunkSize.x) * _chunkSize.x, Mathf.FloorToInt(index.y / _chunkSize.y) * _chunkSize.y);
            chunk.SetNode(new Vector2Int(index.x - minChunkIndex.x, index.y - minChunkIndex.y), navNode);
        }

        private void CheckOutOfBounds(Vector2Int index) {
            if(index.x >= _size.x) {
                Debug.LogError("Index " + index.x + " out of bounds for grid.");
            }
            if(index.y >= _size.y) {
                Debug.LogError("Index " + index.y + " out of bounds for grid.");
            }
        }
    }

}
 