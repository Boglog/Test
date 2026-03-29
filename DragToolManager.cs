using UnityEngine;

/// <summary>
/// Следит за тем, чтобы одновременно был активен только один инструмент.
/// Если игрок берёт новый инструмент, старый возвращается на место.
/// </summary>
public class DragToolManager : MonoBehaviour
{
    public static DragToolManager Instance { get; private set; }

    private DragTool currentActiveTool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Делает инструмент активным и сбрасывает предыдущий, если он был.
    /// </summary>
    public void SetActiveTool(DragTool tool)
    {
        if (tool == null)
            return;

        if (currentActiveTool != null && currentActiveTool != tool)
            currentActiveTool.ResetTool();

        currentActiveTool = tool;
    }

    /// <summary>
    /// Очищает ссылку на текущий активный инструмент.
    /// </summary>
    public void ClearActiveTool(DragTool tool)
    {
        if (currentActiveTool == tool)
            currentActiveTool = null;
    }
}