using UnityEngine;
using UnityEngine.Rendering;

public class YSort : MonoBehaviour
{
    [SerializeField] private SortingGroup SortingGroup_Target;
    [SerializeField] private Transform SortPivot;
    [SerializeField] private int _sortingOffset;

    private void LateUpdate()
    {
        RefreshSortingOrder();
    }

    private void RefreshSortingOrder()
    {
        if (SortingGroup_Target == null)
        {
            return;
        }

        Transform targetTransform = SortPivot;

        if (targetTransform == null)
        {
            targetTransform = transform;
        }

        SortingGroup_Target.sortingOrder =
            Mathf.RoundToInt(-targetTransform.position.y * 100f) + _sortingOffset;
    }
}