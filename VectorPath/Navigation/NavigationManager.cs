using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VectorPath {

    /// <summary>
    /// Singleton manager responsible for coordinating navigation flow field operations and target management.
    /// </summary>
    public class NavigationManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the NavigationManager.
        /// </summary>
        public static NavigationManager Instance { get; private set; }

        /// <summary>
        /// The navigation flow field used for pathfinding calculations.
        /// </summary>
        public NavigationFlowField navigationFlowField;

        /// <summary>
        /// The current target for navigation purposes.
        /// </summary>
        public Transform Target;

        private void Awake() {
            if(Instance != null) Debug.LogError("There is more then one NavigationManager. This can lead to unexpected behaviour!");
            Instance = this;
            if(navigationFlowField == null) Debug.LogError("There is no navigationFlowField set in the NavigationManager!");
        }

        /// <summary>
        /// Sets the navigation target and updates the target position in the navigation flow field.
        /// </summary>
        /// <param name="target">The new target transform.</param>
        public void ForceSetTarget(Transform target) {
            Target = target;
            navigationFlowField.targetPosition = target.position;
        }

        /// <summary>
        /// Forces an update of the navigation map (NavMap) using the navigation flow field.
        /// </summary>
        public void ForceUpdateNavMap() {
            navigationFlowField.CalculateNavMap();
        }

    }
    
}
