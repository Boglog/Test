using UnityEngine;

/// <summary>
/// Переключает вкладки книжки по кнопке.
/// При переключении инструменты текущей вкладки возвращаются в исходное состояние.
/// </summary>
public class BookTabs : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField] private GameObject tabLipstick;
    [SerializeField] private GameObject tabShadow;
    [SerializeField] private GameObject tabCare;

    private GameObject[] tabs;
    private int currentIndex;

    private void Awake()
    {
        tabs = new[]
        {
            tabLipstick,
            tabShadow,
            tabCare
        };
    }

    private void Start()
    {
        currentIndex = 0;
        ShowOnlyCurrentTab();
    }

    /// <summary>
    /// Вызывается кнопкой "Next".
    /// </summary>
    public void SwitchTab()
    {
        ResetToolsInTab(currentIndex);

        if (tabs[currentIndex] != null)
            tabs[currentIndex].SetActive(false);

        currentIndex = (currentIndex + 1) % tabs.Length;

        if (tabs[currentIndex] != null)
            tabs[currentIndex].SetActive(true);
    }

    private void ShowOnlyCurrentTab()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i] != null)
                tabs[i].SetActive(i == currentIndex);
        }
    }

    private void ResetToolsInTab(int tabIndex)
    {
        if (tabs == null || tabIndex < 0 || tabIndex >= tabs.Length || tabs[tabIndex] == null)
            return;

        DragTool[] tools = tabs[tabIndex].GetComponentsInChildren<DragTool>(true);

        foreach (DragTool tool in tools)
        {
            if (tool != null)
                tool.ResetTool();
        }
    }
}