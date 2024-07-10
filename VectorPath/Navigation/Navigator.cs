using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VectorPath {

    /// <summary>
    /// Helper class for obtaining movement directions from the NavigationFlowField managed by the NavigationManager.
    /// </summary>
    public class Navigator : MonoBehaviour
    {
        /// <summary>
        /// Retrieves the movement direction based on the given position in the navigation grid.
        /// </summary>
        /// <param name="position">The position for which to retrieve the movement direction.</param>
        /// <returns>The movement direction vector.</returns>
        public Vector2 GetMoveDirection(Vector2 position) {
            NavigationFlowField nav = NavigationManager.Instance.navigationFlowField;
            Vector2Int posInGrid = nav.GetPositionInNavMap(position);
            if(nav.PositionIsInNavMap(posInGrid)) {
                return nav.NavMap[posInGrid.x, posInGrid.y].Direction;
            }
            return Vector2.zero;
        }
    }

}
