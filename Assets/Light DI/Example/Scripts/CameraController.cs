using UnityEngine;

namespace LightDI.Examples
{
    public sealed class CameraController : SystemBase
    {
        [SerializeField][Min(0f)] private float cameraFollowSpeed = 8f;

        [Inject] private PlayerController _playerController;

        private Transform _cameraTransform;
        private Transform _playerTransform;

        private Vector3 _offset;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Vector3 GetWorldDirection(Vector3 viewportDirection)
        {
            return transform.TransformDirection(viewportDirection);
        }

        protected override void Start()
        {
            base.Start();
            _cameraTransform = Camera.main?.transform;
            _playerTransform = _playerController.transform;
            if (_cameraTransform != null)
            {
                _offset = _cameraTransform.position - _playerTransform.position;
            }
        }

        private void Update()
        {
            if (_playerTransform)
            {
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position,
                    _playerTransform.position + _offset,
                    cameraFollowSpeed * Time.deltaTime);
            }
        }
    }
}