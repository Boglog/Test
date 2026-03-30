using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float targetAspect = 9f / 16f;

    void Start()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float scale = screenAspect / targetAspect;

        Camera cam = GetComponent<Camera>();

        if (scale < 1f)
            cam.orthographicSize /= scale;
    }
}