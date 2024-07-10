using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VectorPathExamples {
    /// <summary>
    /// Spawns rats at two designated spawn points at regular intervals.
    /// </summary>
    public class RatSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject rat;        // Prefab of the rat to spawn

        [SerializeField] private Transform spawn1;      // Transform of the first spawn point
        [SerializeField] private Transform spawn2;      // Transform of the second spawn point

        [SerializeField] private float spawnsPerSecond; // Amount of Spawns per second
        private float _timeToReset;                     // Time interval between spawns
        private float _timeLeft;                        // Time left until next spawn

        void Awake() {
            _timeToReset = 1/spawnsPerSecond;
        }

        void Update()
        {
            if(_timeLeft <= 0) {
                _timeLeft = _timeToReset;
                GameObject ratInst = null;
                
                // Spawns a rat at a random spawn position
                switch(Random.Range(0, 2)) {
                    case 0:
                        ratInst = Instantiate(rat, spawn1.position, Quaternion.identity);
                        break;
                    case 1:
                        ratInst = Instantiate(rat, spawn2.position, Quaternion.identity);
                        break;
                }
            }
            else _timeLeft -= Time.deltaTime;
        }
    }
}