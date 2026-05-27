using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform Target;
    [SerializeField] private float _moveSpeed = 5f;

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

        targetPosition.z = -10f;

        transform.position = Vector3.Lerp(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
    }
}
