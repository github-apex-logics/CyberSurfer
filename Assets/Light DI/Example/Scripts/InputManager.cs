using UnityEngine;

namespace LightDI.Examples
{
    public sealed class InputManager : SystemBase, IInputManager
    {
        private Vector3 _movementDirection;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Vector3 GetMovementDirection()
        {
            return _movementDirection;
        }

        private void Update()
        {
            _movementDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                _movementDirection += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                _movementDirection -= Vector3.forward;
            }
            if (Input.GetKey(KeyCode.D))
            {
                _movementDirection += Vector3.right;
            }
            if (Input.GetKey(KeyCode.A))
            {
                _movementDirection -= Vector3.right;
            }

            _movementDirection.Normalize();
        }
    }
}