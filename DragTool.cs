using UnityEngine;

/// <summary>
/// Универсальный контроллер для всех инструментов макияжа.
/// Поддерживает инструменты из книжки и отдельные инструменты на сцене.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DragTool : MonoBehaviour
{
    [Header("Tool Type")]
    [SerializeField] private ToolType toolType;

    [Header("World Positions")]
    [Tooltip("Стартовая позиция для крема и спонжа.")]
    [SerializeField] private Transform startPosition;

    [Tooltip("Позиция под лицом.")]
    [SerializeField] private Transform readyPosition;

    [Header("Book Tab (optional)")]
    [Tooltip("Родитель вкладки. Нужен только для инструментов внутри книжки.")]
    [SerializeField] private Transform tabParent;

    [Header("Motion")]
    [SerializeField, Min(1f)] private float moveSpeed = 12f;
    [SerializeField, Min(1f)] private float followSpeed = 18f;

    private Collider2D cachedCollider;
    private Camera mainCamera;

    private bool isPicked;
    private bool isReady;
    private bool isMovingToTarget;

    private Vector3 targetPosition;
    private Vector3 homeLocalPosition;
    private Quaternion homeLocalRotation;
    private Vector3 homeLocalScale;
    private bool isBookTool;

    private ShadowColor selectedShadowColor = ShadowColor.None;

    private void Awake()
    {
        cachedCollider = GetComponent<Collider2D>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        isBookTool = tabParent != null;

        if (isBookTool)
        {
            homeLocalPosition = transform.localPosition;
            homeLocalRotation = transform.localRotation;
            homeLocalScale = transform.localScale;
        }
    }

    private void Update()
    {
        if (isMovingToTarget)
            MoveTowardsTarget();

        if (isPicked)
            FollowPointer();
    }

    private void OnMouseDown()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (DragToolManager.Instance != null)
            DragToolManager.Instance.SetActiveTool(this);

        if (toolType == ToolType.Shadow)
        {
            BeginDrag();
            return;
        }

        if (!isReady)
        {
            MoveToReadyPosition();
            return;
        }

        BeginDrag();
    }

    private void OnMouseUp()
    {
        if (!isPicked)
            return;

        Vector2 pointerWorld = GetPointerWorldPosition();
        Collider2D hit = Physics2D.OverlapPoint(pointerWorld);

        if (toolType == ToolType.Shadow)
        {
            HandleShadowRelease(hit);
            return;
        }

        HandleSimpleToolRelease(hit);
    }

    private void BeginDrag()
    {
        isPicked = true;
        isReady = false;
        isMovingToTarget = false;

        // Отвязываем от вкладки, чтобы инструмент корректно двигался в мировом пространстве.
        transform.SetParent(null, true);

        cachedCollider.enabled = false;
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.03f)
        {
            transform.position = targetPosition;
            isMovingToTarget = false;
        }
    }

    private void FollowPointer()
    {
        Vector3 pointerWorld = GetPointerWorldPosition();
        pointerWorld.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, pointerWorld, Time.deltaTime * followSpeed);
    }

    /// <summary>
    /// Для простых инструментов: первый клик отправляет в readyPosition.
    /// </summary>
    private void MoveToReadyPosition()
    {
        isReady = true;
        isPicked = false;
        isMovingToTarget = true;

        transform.SetParent(null, true);
        targetPosition = readyPosition != null ? readyPosition.position : transform.position;

        cachedCollider.enabled = true;
    }

    private void HandleShadowRelease(Collider2D hit)
    {
        isPicked = false;
        cachedCollider.enabled = true;

        if (hit != null)
        {
            if (hit.CompareTag("PaletteRed"))
            {
                selectedShadowColor = ShadowColor.Red;
                MoveToReadyPosition();
                return;
            }

            if (hit.CompareTag("PaletteBlue"))
            {
                selectedShadowColor = ShadowColor.Blue;
                MoveToReadyPosition();
                return;
            }

            if (hit.CompareTag("Face") && selectedShadowColor != ShadowColor.None && hit.TryGetComponent(out FaceController face))
            {
                face.PaintShadow(selectedShadowColor);
                ResetTool();
                return;
            }
        }
    }

    private void HandleSimpleToolRelease(Collider2D hit)
    {
        if (hit != null && hit.CompareTag("Face") && hit.TryGetComponent(out FaceController face))
        {
            switch (toolType)
            {
                case ToolType.Lipstick:
                    face.PaintLipsSimple();
                    break;

                case ToolType.Cream:
                    face.RemoveAcne();
                    break;

                case ToolType.Sponge:
                    face.ClearMakeup();
                    break;
            }

            ResetTool();
            return;
        }

        ResetTool();
    }

    /// <summary>
    /// Работает и в редакторе, и на Android.
    /// </summary>
    private Vector3 GetPointerWorldPosition()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return transform.position;

        Vector3 screenPos = Input.mousePosition;

        if (Input.touchCount > 0)
            screenPos = Input.GetTouch(0).position;

        screenPos.z = -mainCamera.transform.position.z;

        Vector3 world = mainCamera.ScreenToWorldPoint(screenPos);
        world.z = 0f;
        return world;
    }

    /// <summary>
    /// Возвращает инструмент в исходное состояние.
    /// Для вкладок — обратно в родителя вкладки.
    /// Для отдельных инструментов — на startPosition.
    /// </summary>
    public void ResetTool()
    {
        isPicked = false;
        isReady = false;
        isMovingToTarget = false;
        selectedShadowColor = ShadowColor.None;

        cachedCollider.enabled = true;

        if (isBookTool && tabParent != null)
        {
            transform.SetParent(tabParent, false);
            transform.localPosition = homeLocalPosition;
            transform.localRotation = homeLocalRotation;
            transform.localScale = homeLocalScale;
        }
        else if (startPosition != null)
        {
            transform.SetParent(null, true);
            transform.position = startPosition.position;
        }

        if (DragToolManager.Instance != null)
            DragToolManager.Instance.ClearActiveTool(this);
    }
}