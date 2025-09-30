using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement Settings")]
    [SerializeField] private float dragSpeed = 2f;
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Camera Bounds")]
    [SerializeField] private Vector2 minCameraPos = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxCameraPos = new Vector2(10f, 10f);

    [Header("Touch Settings")]
    [SerializeField] private float minSwipeDistance = 20f;

    [Header("Camera Zoom Settings")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;


    private Camera mainCamera;
    private Vector3 touchStart;
    private Vector3 cameraStartPos;
    private Vector3 velocity = Vector3.zero;
    private bool isDragging = false;

    // Для обработки мультитач
    private int touchCount = 0;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }
    }

    void Update()
    {
        HandleInput();
        ApplyBounds();
        HandleZoom();
    }

    private void HandleInput()
    {
        // Обработка ввода на ПК (мышь)
//#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
//#endif

        // Обработка ввода на мобильных устройствах
//#if UNITY_IOS || UNITY_ANDROID
        HandleTouchInput();
//#endif
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag(Input.mousePosition);
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            ContinueDrag(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Игнорируем мультитач
            if (Input.touchCount != touchCount)
            {
                touchCount = Input.touchCount;
                if (Input.touchCount == 1)
                {
                    StartDrag(touch.position);
                }
                else
                {
                    EndDrag();
                }
            }

            if (touch.phase == TouchPhase.Moved && isDragging && Input.touchCount == 1)
            {
                ContinueDrag(touch.position);
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                EndDrag();
                touchCount = 0;
            }
        }
    }

    private void StartDrag(Vector3 inputPosition)
    {
        touchStart = mainCamera.ScreenToWorldPoint(inputPosition);
        cameraStartPos = transform.position;
        isDragging = true;
        velocity = Vector3.zero;
    }

    private void ContinueDrag(Vector3 inputPosition)
    {
        Vector3 touchCurrent = mainCamera.ScreenToWorldPoint(inputPosition);
        Vector3 difference = touchStart - touchCurrent;

        // Проверяем минимальное расстояние для свайпа (только для начала движения)
        if (!isDragging && difference.magnitude < minSwipeDistance)
            return;

        Vector3 targetPosition = cameraStartPos + difference;

        // Плавное перемещение камеры
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void EndDrag()
    {
        isDragging = false;
    }

    private void ApplyBounds()
    {
        Vector3 clampedPosition = transform.position;

        // Ограничиваем позицию камеры по заданным границам
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minCameraPos.x, maxCameraPos.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minCameraPos.y, maxCameraPos.y);

        transform.position = clampedPosition;
    }

    // Методы для изменения границ камеры во время выполнения
    public void SetCameraBounds(Vector2 minPos, Vector2 maxPos)
    {
        minCameraPos = minPos;
        maxCameraPos = maxPos;
        ApplyBounds();
    }

    public void SetCameraBounds(float minX, float minY, float maxX, float maxY)
    {
        minCameraPos = new Vector2(minX, minY);
        maxCameraPos = new Vector2(maxX, maxY);
        ApplyBounds();
    }

    // Визуализация границ камеры в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(
            (minCameraPos.x + maxCameraPos.x) * 0.5f,
            (minCameraPos.y + maxCameraPos.y) * 0.5f,
            transform.position.z
        );
        Vector3 size = new Vector3(
            maxCameraPos.x - minCameraPos.x,
            maxCameraPos.y - minCameraPos.y,
            0.1f
        );
        Gizmos.DrawWireCube(center, size);
    }

    private void HandleZoom()
    {
//#if UNITY_EDITOR || UNITY_STANDALONE
        // Зум колесом мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            mainCamera.orthographicSize -= scroll * (zoomSpeed * 100f);
        }
//#endif

//#if UNITY_IOS || UNITY_ANDROID
        // Зум жестом "щипок"
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 prevTouchZeroPos = touchZero.position - touchZero.deltaPosition;
            Vector2 prevTouchOnePos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (prevTouchZeroPos - prevTouchOnePos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            mainCamera.orthographicSize -= difference * zoomSpeed;
        }
//#endif

        // Ограничиваем значение зума
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
    }

}