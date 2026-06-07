using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform Target;
    [SerializeField] private float _moveSpeed = 5f;

    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private Vector2 _maxPosition;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (Target == null)
        {
            return;
        }

        Vector3 targetPosition = Target.position;
        targetPosition.z = transform.position.z;

        targetPosition = GetClampedPosition(targetPosition);

        transform.position = Vector3.Lerp(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
    }

    private Vector3 GetClampedPosition(Vector3 targetPosition)
    {
        if (_camera == null)
        {
            return targetPosition;
        }

        float cameraHeight = _camera.orthographicSize;
        float cameraWidth = cameraHeight * _camera.aspect;

        float minX = _minPosition.x + cameraWidth;
        float maxX = _maxPosition.x - cameraWidth;
        float minY = _minPosition.y + cameraHeight;
        float maxY = _maxPosition.y - cameraHeight;

        if (minX <= maxX)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        }

        if (minY <= maxY)
        {
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        return targetPosition;
    }
}