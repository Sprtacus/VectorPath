using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowField {

    /// <summary>
    /// The NavNode is the atomic structure used to hold the data and functions for each point in the flow field.
    /// I used it like this: I overlaid a grid of the same size as the tilemap grid and added a NavNode to each entry in my grid.
    /// But you could scale this factor up or down. You could use more or less tiles for the NavNode.
    /// </summary>
    public class NavNode
    {
        /// <summary>
        /// Cost to traverse this Node. It could be used to make it less likely for agents to traverse ground types like mud.
        /// You would need to implement the behaviour of setting the travers cost.
        /// </summary>
        public int CostToTraverse;

        /// <summary>
        /// The direction has to be calculated by the kernal before you can use it.
        /// After calculation it will have the direction a agent has to take to get to the target location.
        /// </summary>
        public Vector2 Direction;
        
        /// <summary>
        /// This is the position of the NavNode in the overall FlowField
        /// </summary>
        public readonly Vector2Int GridPosition;

        private int _cost;

        /// <summary>
        /// Constructor for NavNode.
        /// The costs will be set to Int32-MaxValue, which represents an obstacle.
        /// The direction is set to a zero-vector, since it should be changed once calculated.
        /// </summary>
        /// <param name="gridPosition"> Position of the NavNode in the FlowField Grid (x, y Coordinats). </param>
        /// <param name="costToTraverse"> Costs to traverse the NavNode (default set to 1). It has to be a positiv value. (0 is not positiv)</param>
        public NavNode(Vector2Int gridPosition, int costToTraverse = 1)
        {
            if(costToTraverse <= 0) Debug.LogError("Out of Bounds!\nTraverse cost needs to be positiv.");
            CostToTraverse = costToTraverse;
            GridPosition = gridPosition;

            _cost = Int32.MaxValue;

            Direction = Vector2.zero;
        }

        /// <summary>
        /// Getter Method to check if NavNode2D is an obstacle.
        /// </summary>
        /// <returns> Returns true if NavNode2D is an obstacle. </returns>
        public bool isObstacle() {
            return _cost == Int32.MaxValue;
        }

        /// <summary>
        /// Return the cost of the NavNode
        /// </summary>
        /// <returns>Cost of the NavNode</returns>
        public int GetCost() {
            return _cost;
        }

        /// <summary>
        /// Sets the cost of the NavNode
        /// </summary>
        /// <param name="cost">Cost to set to</param>
        /// <param name="useCostToTraverse"></param>
        public void SetCost(int cost, bool useCostToTraverse = true) {
            if(useCostToTraverse) _cost = cost + CostToTraverse;
            else _cost = cost;
        }
    }

}
