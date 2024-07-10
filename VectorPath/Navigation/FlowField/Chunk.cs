using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowField {

    /// <summary>
    /// A chunk is used to improve performance. It holds a number of NavNodes and adds some functionality over them. 
    /// It can have a minimum size of 1 and a maximum size of the flowfield itself. 
    /// Both extreme cases would not help performance, so you need to find a chunk size that is suitable for your application.
    /// </summary>
    public class Chunk
    {
        public bool chunkWasModified { get; private set; }
        private Vector2Int _size;
        private NavNode[,] _navNodes;
        // Bottom left corner of the chunk
        private Vector2Int _gridPosition;

        /// <summary>
        /// Constructor for a Chunk.
        /// A Chunk consists out of a size and a 2D-Array of NavNodes.
        /// Size will be set by the parameter, chunkWasModified to false and the Array will be initialized and filled with dummy NavNodes.
        /// </summary>
        /// <param name="size">Size of the Chunk. It has to be greater then 0.</param>
        /// <param name="gridPosition">Position of the Chunk. The position of the chunk is also the position of the bottom left NavNode in the chunk.</param>
        public Chunk(Vector2Int size, Vector2Int gridPosition) {
            if(size.x < 1 || size.y < 1) Debug.LogError("Out of Bounds!\nChunk size has to be greater then 0.");

            _size = size;
            _navNodes = new NavNode[size.x, size.y];
            _gridPosition = gridPosition;

            // Filling the chunk with dummy NavNodes.
            for(int i = 0; i < size.x; i++) {
                for(int j = 0; j < size.y; j++) {
                    SetNode(new Vector2Int(i, j), new NavNode(new Vector2Int(_gridPosition.x + i, gridPosition.y + j)));
                }
            }

            // Since this is the initial state, the modified flag should be false.
            ResetModified();
        }

        /// <summary>
        /// Returns the NavNode in the 2D-Array at the given index.
        /// </summary>
        /// <param name="index">Index in the array (0 is in the bottom left corner, 1st dimension in the x direction, 2nd dimension in the y direction)</param>
        /// <returns>NavNode</returns>
        public NavNode GetNode(Vector2Int index) {
            CheckForBounds(index);
            return _navNodes[index.x, index.y];
        }

        /// <summary>
        /// Sets the navNode in the 2D-Array at the given index.
        /// It will also mark the chunk as having been modified.
        /// </summary>
        /// <param name="index">Index in the array (0 is in the bottom left corner, 1st dimension in the x direction, 2nd dimension in the y direction)</param>
        /// <param name="navNode">NavNode that you want to put in the array.</param>
        public void SetNode(Vector2Int index, NavNode navNode) {
            CheckForBounds(index);
            _navNodes[index.x, index.y] = navNode;
            chunkWasModified = true;
        }

        /// <summary>
        /// Returns the size of the chunk.
        /// </summary>
        /// <returns>Size of the chunk.</returns>
        public Vector2Int GetSize() {
            return _size;
        }

        /// <summary>
        /// Resets chunkWasModified
        /// </summary>
        public void ResetModified() {
            chunkWasModified = false;
        }

        // Checks if index is Out of Bounds.
        private void CheckForBounds(Vector2Int index) {
            if(index.x < 0 || index.y < 0) Debug.LogError("Out of Bounds!\nTrying to access NavNode with Indices(" + index.x + ", " + index.y + ").\nBoth Indices has to be greater then 0.");
            if(index.x >= _size.x || index.y >= _size.y) Debug.LogError("Out of Bounds!\nTrying to access NavNode with Indices(" + index.x + ", " + index.y + ").\nBoth Indices has to be smaller then the ChunkSize (" + _size.x + ", " + _size.y + ").");
        }
    }

}
