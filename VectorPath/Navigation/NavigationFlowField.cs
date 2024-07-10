using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using FlowField;

namespace VectorPath {

    /// <summary>
    /// Manages navigation flow field generation and visualization for AI pathfinding on a grid-based map.
    /// The flow field is computed based on walkable tilemaps and target positions within the defined grid dimensions.
    /// Features include generating and updating navigation costs, determining walkable neighbors, and calculating movement directions.
    /// Various debugging options are available to visualize grid boundaries, target positions, traversal costs, chunk updates, and flow directions.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Grid))]
    public class NavigationFlowField : MonoBehaviour
    {
        [Header("Walkable Tilemaps")]
        [Tooltip("A walkable tilemap should have tiles at positions where the ai could walk. There should be no offset between them if you add more than one.")]
        [SerializeField] private Tilemap[] groundTilemaps;
        
        [Header("NavMap Settings")]
        [Tooltip("Performance is greatly affected by the size of the NavMap! Choose the size wisely.")]
        [SerializeField] private Vector2Int size;
        [SerializeField] private Vector2Int offset;
        [SerializeField] private Vector2Int chunkSize;

        [Header("Navigation Settings")]
        public Vector3 targetPosition;

        public Chunk chunk;

        [Header("Debug")]
        [SerializeField] private bool drawBounds;
        [SerializeField] private bool drawTargetPosition;
        [SerializeField] private bool drawTraverseCost;
        [SerializeField] private bool drawCostMap;
        [SerializeField] private bool drawChunks;
        [SerializeField] private bool drawFlowDirections;
        [SerializeField] private bool displayCalculationTime;

        [HideInInspector] public NavNode[,] NavMap;
        public bool PreComputed = false;

        private UnityEngine.Grid grid;
        private bool[,] chunkUpdate;

        private static readonly Color GIZMO_ACCEPT_COLOR = Color.blue;
        private static readonly Color GIZMO_DECLINE_COLOR = Color.red;
        private static readonly Color GIZMO_NEUTRAL_COLOR = Color.white;
        private static readonly Color GIZMO_TARGET_COLOR = Color.yellow;

        private void Awake() {
            Instantiate();
        }

        void Start()
        {
            CalculateNavMap();
        }

        /// <summary>
        /// Initializes the Navigation Flow Field by validating and processing input parameters such as walkable tilemaps, grid size, chunk size, and target position.
        /// This method ensures that necessary components are set up, initializes data structures for navigation nodes, and checks the validity of the target position within the grid.
        /// </summary>
        public void Instantiate() {
            if(groundTilemaps.Length == 0) {
                Debug.LogError("No walkable tilemaps have been provided. Cannot calculate the FlowField.");
            }
            if(groundTilemaps.Length > 0) {
                groundTilemaps = FilterOutNullTilemaps(groundTilemaps);
                CheckForOffset(groundTilemaps);
            }

            CheckForTargetReachable();

            grid = GetComponent<UnityEngine.Grid>();

            chunkUpdate = new bool[Mathf.CeilToInt((float)size.x / chunkSize.x), Mathf.CeilToInt((float)size.y / chunkSize.y)];
            NavMap = new NavNode[size.x, size.y];
        
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    NavMap[i, j] = new NavNode(new Vector2Int(i, j));
                }
            }
        }

        private int CountNeighbors(Vector2Int positionInGrid) {
            NavNode[] navNodes = GetNeighbors(positionInGrid, false);
            int amount = 0;
            foreach (var node in navNodes)
            {
                if (node != null && IsWalkable(node.GridPosition)) amount++;
            }
            return amount;
        }

        /// <summary>
        /// Calculates the navigation map (NavMap) including generating the cost map and calculating flow directions.
        /// </summary>
        public void CalculateNavMap() {
            Instantiate();
            GenerateCostMap();
            CalculateDirections();
        }

        private void GenerateCostMap() {
            List<NavNode> openList = new List<NavNode>();
            Vector2Int targetPositionInt = GetPositionInNavMap(targetPosition);

            NavMap[targetPositionInt.x, targetPositionInt.y].SetCost(0, false);
            openList.Add(NavMap[targetPositionInt.x, targetPositionInt.y]);

            while (openList.Count > 0)
            {
                CalculateNode(ref openList);
            }
        }

        private void CalculateNode(ref List<NavNode> openList) {
            NavNode node = openList[0];
            openList.RemoveAt(0);

            List<NavNode> neighbors = new List<NavNode>();
            neighbors.AddRange(GetNeighbors(node.GridPosition, false));

            for (int i = 0; i < neighbors.Count; i++)
            {
                CalculateNeighbor(ref openList, neighbors[i], node);
            }
        }

        private void CalculateNeighbor(ref List<NavNode> openList, NavNode neighbor, NavNode node) {
            if(neighbor == null) return;
            if(!IsWalkable(neighbor.GridPosition)) return;

            if (neighbor.GetCost() > node.GetCost() + 1)
            {
                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                    SetChunkUpToUpdate(neighbor.GridPosition, true);
                }
                neighbor.SetCost(node.GetCost());
            }
        }

        private void SetChunkUpToUpdate(Vector2Int position, bool hasToUpdate) {
            chunkUpdate[Mathf.FloorToInt((float)position.x / chunkSize.x), Mathf.FloorToInt((float)position.y / chunkSize.y)] = hasToUpdate;
        }

        private bool IsWalkable(Vector2Int position) {
            for (int i = 0; i < groundTilemaps.Length; i++)
            {
                if(groundTilemaps[i].HasTile(new Vector3Int(position.x, position.y, 0))) return true;
            }
            return false;
        }

        private void CalculateDirections() {
            for (int i = 0; i < chunkUpdate.GetLength(0); i++)
            {
                for (int j = 0; j < chunkUpdate.GetLength(1); j++)
                {
                    if(!chunkUpdate[i, j]) continue;

                    EvaluateChunk(new Vector2Int(i, j));
                    chunkUpdate[i, j] = false;
                }
            }
        }

        private void EvaluateChunk(Vector2Int chunkIndex) {
            for (int i = 0; i < chunkSize.x; i++)
            {
                for (int j = 0; j < chunkSize.y; j++)
                {
                    if(chunkIndex.x * chunkSize.x + i >= size.x) continue;
                    if(chunkIndex.y * chunkSize.y + j >= size.y) continue;
                    NavNode node = NavMap[chunkIndex.x * chunkSize.x + i, chunkIndex.y * chunkSize.y + j];

                    if (node.isObstacle()) continue;

                    List<NavNode> neighbors = new List<NavNode>();
                    neighbors.AddRange(GetNeighbors(node.GridPosition, false));

                    int location = 8;
                    for (int k = 0; k < neighbors.Count; k++)
                    {
                        if (neighbors[k] == null || neighbors[k].GetCost() == Int32.MaxValue) continue;

                        if (neighbors[k].GetCost() <= node.GetCost())
                        {
                            node = neighbors[k];
                            location = k;
                        }
                    }

                    NavMap[chunkIndex.x * chunkSize.x + i, chunkIndex.y * chunkSize.y + j].Direction = EvaluateDirection(location);
                }
            }
        }

        private Vector2 EvaluateDirection(int x)
        {
            switch (x)
            {
                case 0:
                    return new Vector2(0, 1).normalized;
                case 1:
                    return new Vector2(0, -1).normalized;
                case 2:
                    return new Vector2(1, 0).normalized;
                case 3:
                    return new Vector2(-1, 0).normalized;
                case 4:
                    return new Vector2(1, 1).normalized;
                case 5:
                    return new Vector2(1, -1).normalized;
                case 6:
                    return new Vector2(-1, 1).normalized;
                case 7:
                    return new Vector2(-1, -1).normalized;
                default:
                    return Vector2.zero;
            }
        }

        /// <summary>
        /// Retrieves the normalized direction vector at the specified grid position from the Navigation Flow Field.
        /// </summary>
        /// <param name="position">The grid position for which to retrieve the direction vector.</param>
        /// <returns>The normalized direction vector at the specified grid position.</returns>
        /// <remarks>
        /// This method ensures that the provided position is within the valid range of the Navigation Flow Field (NavMap). 
        /// If the position is out of bounds, it logs an error and returns a zero vector.
        /// </remarks>
        public Vector2 GetDirection(Vector2Int position) {
            if(position.x < 0 || position.y < 0) {
                Debug.LogError("Positions can not be negativ!");
                return Vector2.zero;
            }
            if(position.x >= size.x || position.y >= size.y) {
                Debug.LogError("Positions can not be bigger then NavMap size!");
                return Vector2.zero;
            }

            return NavMap[position.x, position.y].Direction;
        }

        /// <summary>
        /// Converts a world position into a grid position in the navigation map (NavMap).
        /// </summary>
        /// <param name="position">The world position to convert.</param>
        /// <returns>The grid position in the NavMap corresponding to the world position.</returns>
        public Vector2Int GetPositionInNavMap(Vector2 position) {
            Vector2Int positionInt = new Vector2Int(Mathf.FloorToInt(position.x / grid.cellSize.x), Mathf.FloorToInt(position.y / grid.cellSize.y));
            return positionInt;
        }

        /// <summary>
        /// Checks if a grid position is within the bounds of the navigation map (NavMap).
        /// </summary>
        /// <param name="position">The grid position to check.</param>
        /// <returns>True if the position is within the bounds of the NavMap; otherwise, false.</returns>
        public bool PositionIsInNavMap(Vector2Int position) {
            return position.x >= 0 && position.x < size.x && position.y >= 0 && position.y < size.y;
        }

        private bool CheckForTargetReachable(bool suppressError = false) {
            if(targetPosition.x < 0 || targetPosition.x >= size.x || targetPosition.y < 0 || targetPosition.y >= size.y) {
                if(!suppressError) Debug.LogError("Target is out of bounds!");
                return false;
            }
            return true;
        }

        private void CheckForOffset(Tilemap[] tilemaps) {
            for(int i=0; i<tilemaps.Length; i++) {
                if(tilemaps[i].transform.position != Vector3.zero) {
                    Debug.LogError("The " + (i+1) + " tilemap should be at (0|0|0) and not have an offset. This will lead to incorrect calculations and pathfinding!");
                }
            }
        }

        private Tilemap[] FilterOutNullTilemaps(Tilemap[] tilemaps) {
            Tilemap[] correctedTilemaps = new Tilemap[tilemaps.Length];
            int counter = 0;
            foreach(var tilemap in tilemaps) {
                if(tilemap == null) Debug.LogWarning("Empty slots are not allowed in the groundmaps! Will remove it from calculation.");
                else {
                    correctedTilemaps[counter] = tilemap;
                    counter++;
                }
            }
            return correctedTilemaps;
        }

        private NavNode[] GetNeighbors(Vector2Int positionInt, bool getDiagonals)
        {
            NavNode[] neighbors;
            if(getDiagonals) neighbors = new NavNode[8];
            else neighbors = new NavNode[4];

            // Add Top
            if (positionInt.x >= 0 && positionInt.x < size.x && positionInt.y + 1 >= 0 && positionInt.y + 1 < size.y)
            {
                neighbors[0] = NavMap[positionInt.x, positionInt.y + 1];
            }
            // Add Bottom
            if (positionInt.x >= 0 && positionInt.x < size.x && positionInt.y - 1 >= 0 && positionInt.y - 1 < size.y)
            {
                neighbors[1] = NavMap[positionInt.x, positionInt.y - 1];
            }
            // Add Right
            if (positionInt.x + 1 >= 0 && positionInt.x + 1 < size.x && positionInt.y >= 0 && positionInt.y < size.y)
            {
                neighbors[2] = NavMap[positionInt.x + 1, positionInt.y];
            }
            // Add Left
            if (positionInt.x - 1 >= 0 && positionInt.x - 1 < size.x && positionInt.y >= 0 && positionInt.y < size.y)
            {
                neighbors[3] = NavMap[positionInt.x - 1, positionInt.y];
            }
            
            // If only the direct neighbors are wanted, return here
            if(!getDiagonals) return neighbors;

            // Add Top Right
            if (positionInt.x + 1 >= 0 && positionInt.x + 1 < size.x && positionInt.y + 1 >= 0 && positionInt.y + 1< size.y)
            {
                neighbors[4] = NavMap[positionInt.x + 1, positionInt.y + 1];
            }
            // Add Bottom Right
            if (positionInt.x + 1 >= 0 && positionInt.x + 1 < size.x && positionInt.y - 1 >= 0 && positionInt.y - 1< size.y)
            {
                neighbors[5] = NavMap[positionInt.x + 1, positionInt.y - 1];
            }
            // Add Top Left
            if (positionInt.x - 1 >= 0 && positionInt.x - 1 < size.x && positionInt.y + 1 >= 0 && positionInt.y + 1< size.y)
            {
                neighbors[6] = NavMap[positionInt.x - 1, positionInt.y + 1];
            }
            // Add Bottom Left
            if (positionInt.x - 1 >= 0 && positionInt.x - 1 < size.x && positionInt.y - 1 >= 0 && positionInt.y - 1< size.y)
            {
                neighbors[7] = NavMap[positionInt.x - 1, positionInt.y - 1];
            }

            return neighbors;
        }

        private void OnDrawGizmos() {
            if(Application.isEditor && grid == null) grid = GetComponent<UnityEngine.Grid>();

            if(drawBounds) Gizmos.DrawWireCube(new Vector3(size.x * grid.cellSize.x / 2, size.y * grid.cellSize.y / 2, 0), new Vector3(size.x * grid.cellSize.x, size.y * grid.cellSize.y, 0));
            
            if(drawTargetPosition) {
                Gizmos.color = GIZMO_TARGET_COLOR;
                Gizmos.DrawWireCube(targetPosition, Vector3.one * 0.3f);
                if(CheckForTargetReachable(true)) Gizmos.color = GIZMO_ACCEPT_COLOR;
                else Gizmos.color = GIZMO_DECLINE_COLOR;
                Vector2Int targetInGrid = GetPositionInNavMap(targetPosition);
                Gizmos.DrawWireCube(new Vector3(targetInGrid.x * grid.cellSize.x + grid.cellSize.x / 2, targetInGrid.y * grid.cellSize.y + grid.cellSize.y / 2), grid.cellSize);
            }

            if(!Application.isPlaying && NavMap == null) return;

            if(drawTraverseCost) {
                for (int i = 0; i < NavMap.GetLength(0); i++) {
                    for (int j = 0; j < NavMap.GetLength(1); j++) {
                        if(NavMap[i,j].isObstacle()) continue;
                        ExtraGizmos.DrawNumber(new Vector3(i * grid.cellSize.x + grid.cellSize.x / 2, j * grid.cellSize.y + grid.cellSize.y / 2, 0), 0.3f, NavMap[i,j].GetCost(), GIZMO_ACCEPT_COLOR);
                    }
                }
            }

            if(drawCostMap) {
                for (int i = 0; i < NavMap.GetLength(0); i++) {
                    for (int j = 0; j < NavMap.GetLength(1); j++) {
                        if(NavMap[i,j].isObstacle()) continue;
                        ExtraGizmos.DrawNumber(new Vector3(i * grid.cellSize.x + grid.cellSize.x / 2, j * grid.cellSize.y + grid.cellSize.y / 2, 0), 0.3f, NavMap[i,j].GetCost(), GIZMO_ACCEPT_COLOR);
                    }
                }
            }

            if(drawChunks) {
                for (int i = 0; i < chunkUpdate.GetLength(0); i++) {
                    for (int j = 0; j < chunkUpdate.GetLength(1); j++) {
                        if(chunkUpdate[i,j]) Gizmos.color = GIZMO_ACCEPT_COLOR;
                        else Gizmos.color = GIZMO_NEUTRAL_COLOR;
                        Vector3 chunkPosition = new Vector3(i * chunkSize.x, j * chunkSize.y, 0);
                        chunkPosition += new Vector3(chunkSize.x / 2, chunkSize.y / 2, 0);
                        chunkPosition += new Vector3(chunkSize.x % 2 == 0 ? 0 : grid.cellSize.x / 2, chunkSize.y % 2 == 0 ? 0 : grid.cellSize.y / 2);
                        Gizmos.DrawWireCube(chunkPosition, new Vector3(chunkSize.x, chunkSize.y, 0));
                    }
                }
            }

            if(drawFlowDirections) {
                for (int i = 0; i < NavMap.GetLength(0); i++) {
                    for (int j = 0; j < NavMap.GetLength(1); j++) {
                        if(NavMap[i,j].Direction == Vector2.zero) continue;
                        ExtraGizmos.DrawArrow2DCentered(new Vector2(i * grid.cellSize.x + grid.cellSize.x / 2, j * grid.cellSize.y + grid.cellSize.y / 2), NavMap[i,j].Direction, GIZMO_NEUTRAL_COLOR, 0.6f, 0.2f, 25);
                    }
                }
            }
        }
    }
}
