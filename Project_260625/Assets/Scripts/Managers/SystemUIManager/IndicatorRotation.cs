using UnityEngine;

public class IndicatorRotation : MonoBehaviour
{
    #region Inspector
    public float speed = 100f;
    #endregion Inspector

    void Update()
    {
        float time = Time.unscaledDeltaTime;

        transform.Rotate(0f, 0f, time * speed);
    }
}
