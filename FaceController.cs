using UnityEngine;

/// <summary>
/// Управляет визуальным состоянием лица:
/// прыщи, тени, помада и полный сброс макияжа.
/// </summary>
public class FaceController : MonoBehaviour
{
    [Header("Acne")]
    [SerializeField] private GameObject acneObject;

    [Header("Eyeshadow")]
    [SerializeField] private GameObject redShadow;
    [SerializeField] private GameObject blueShadow;

    [Header("Lipstick")]
    [SerializeField] private GameObject lipsObject;

    /// <summary>
    /// Убирает прыщи с лица.
    /// </summary>
    public void RemoveAcne()
    {
        SetActiveSafe(acneObject, false);
    }

    /// <summary>
    /// Включает нужный спрайт теней и выключает второй.
    /// </summary>
    public void PaintShadow(ShadowColor color)
    {
        SetActiveSafe(redShadow, false);
        SetActiveSafe(blueShadow, false);

        if (color == ShadowColor.Red)
            SetActiveSafe(redShadow, true);
        else if (color == ShadowColor.Blue)
            SetActiveSafe(blueShadow, true);
    }

    /// <summary>
    /// Включает единый спрайт помады.
    /// </summary>
    public void PaintLipsSimple()
    {
        SetActiveSafe(lipsObject, true);
    }

    /// <summary>
    /// Полностью убирает весь макияж.
    /// </summary>
    public void ClearMakeup()
    {
        SetActiveSafe(redShadow, false);
        SetActiveSafe(blueShadow, false);
        SetActiveSafe(lipsObject, false);
    }

    private static void SetActiveSafe(GameObject target, bool value)
    {
        if (target != null)
            target.SetActive(value);
    }
}