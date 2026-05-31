using UnityEngine;
using UnityEngine.Rendering;

public class YSort : MonoBehaviour
{
    [SerializeField] private SortingGroup SortingGroup_Target;
    [SerializeField] private int _sortingOffset = 0;

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

        SortingGroup_Target.sortingOrder =
            Mathf.RoundToInt(-transform.position.y * 100) + _sortingOffset;
    }
}