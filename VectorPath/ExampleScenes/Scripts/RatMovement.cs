using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorPath;

namespace VectorPathExamples {

    /// <summary>
    /// Controls the movement and rotation of a rat in 2D space.
    /// </summary>
    [RequireComponent(typeof(Navigator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class RatMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;   // Speed at which the rat moves
        [SerializeField] private float maxSpeed;        // Maximum speed the rat can achieve
        [SerializeField] private float steeringSpeed;   // Maximum speed the rat can achieve    

        private Navigator _navigator;                   // Reference to the Navigator component
        private Rigidbody2D _rb;                        // Reference to the Rigidbody2D component

        private Vector2 _vel;                           // Target velocity
        private Quaternion rotation;                    // Target rotation for smooth rotation

        void Awake()
        {
            _navigator = GetComponent<Navigator>();
            _rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            Move(_navigator.GetMoveDirection(transform.position));
        }

        /// <summary>
        /// Moves the rat towards the specified direction.
        /// </summary>
        /// <param name="newDirection">The direction vector in which the rat should move.</param>
        public void Move(Vector2 newDirection)
        {
            _rb.velocity = Vector2.Lerp(_rb.velocity, newDirection * movementSpeed, steeringSpeed * Time.deltaTime);
            _rb.velocity = Vector2.ClampMagnitude(_rb.velocity, maxSpeed);
            
            RotateByMovement();
        }

        /// <summary>
        /// Rotates the rat to align with its movement direction.
        /// </summary>
        private void RotateByMovement()
        {
            if (_rb.velocity.normalized != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _rb.velocity.normalized);
                rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 500 * Time.deltaTime);

                _rb.MoveRotation(targetRotation);
            }
            else
            {
                _rb.MoveRotation(rotation);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Target")) {
                Destroy(gameObject);
            }
        }
    }
}
