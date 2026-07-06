using UnityEngine;

public class IndicatorRotation : MonoBehaviour
{
    void Update()
    {
        float time = Time.unscaledDeltaTime;

        transform.Rotate(0f, 0f, time * 100f);
    }
}
