using UnityEngine;

namespace LightDI.Examples
{
    public sealed class PlayerController : MonoBehaviour, IInjectable
    {
        [SerializeField][Min(0f)] private float moveSpeed = 5f;

        [Inject] private CameraController _cameraController;
        [Inject] private IInputManager _inputManager;

        private CharacterController _characterController;

        public void PostInject()
        {
        }

        private void Start()
        {
            InjectionManager.RegisterObject(this);
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            _characterController.Move(moveSpeed * Time.deltaTime * _cameraController.GetWorldDirection(_inputManager.GetMovementDirection()));
        }
    }
}