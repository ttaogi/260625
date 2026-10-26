using UnityEngine;

public class OrderSorter : MonoBehaviour
{
    #region Inspector
    public bool useUpdate = false;
    #endregion Inspector

    private SpriteRenderer _spriteRenderer;

    private const float PRECISION = 10f;

    public void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateSort();
    }

    public void Update()
    {
        if (useUpdate)
            UpdateSort();
    }

    private void UpdateSort()
    {
        if (_spriteRenderer == null) return;

        _spriteRenderer.sortingOrder = (int)(-transform.position.y * PRECISION);
    }
}
