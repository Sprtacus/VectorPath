using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorPath;

namespace VectorPathExamples {
    /// <summary>
    /// Spawns target asset at designated positions and updates navigation towards them.
    /// </summary>
    public class TargetSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject target;         // Prefab of the target to spawn

        [SerializeField] private Transform targetPos1;      // Transform of the first target position
        [SerializeField] private Transform targetPos2;      // Transform of the first target position
        [SerializeField] private Transform targetPos3;      // Transform of the third target position

        private GameObject _target;                         // Current instance of the spawned target

        private float _timeToReset = 2f;                    // Time interval between target spawns
        private float _timeLeft;                            // Time left until next target spawn

        void Update()
        {
            if(_timeLeft <= 0) {
                _timeLeft = _timeToReset;

                // Spawn or reposition the target
                if (_target == null) _target = Instantiate(target, targetPos1.position, Quaternion.identity);

                // Randomly choose one of the target positions
                switch(Random.Range(0, 3)) {
                    case 0:
                        _target.transform.position = targetPos1.position;
                        break;
                    case 1:
                        _target.transform.position = targetPos2.position;
                        break;
                    case 2:
                        _target.transform.position = targetPos3.position;
                        break;
                }
                
                // Update navigation towards the new target position
                NavigationManager.Instance.ForceSetTarget(_target.transform);
                NavigationManager.Instance.ForceUpdateNavMap();
            }
            else _timeLeft -= Time.deltaTime;
        }
    }
}
