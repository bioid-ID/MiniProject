using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Zoom")]
    [SerializeField] private float defaultOrthographicSize = 5f;
    [SerializeField] private float minOrthographicSize = 3f;
    [SerializeField] private float maxOrthographicSize = 10f;
    [SerializeField] private float zoomSpeed = 0.35f;

    private Camera cachedCamera;
    private float shakeTimer;
    private float shakeMagnitude;
    private Vector2 shakeOffset;

    private void Awake()
    {
        cachedCamera = GetComponent<Camera>();
        if (cachedCamera != null && cachedCamera.orthographic && cachedCamera.orthographicSize < 2f)
            cachedCamera.orthographicSize = defaultOrthographicSize;
    }

    public void SetTarget(Transform followTarget, bool snapImmediate = false)
    {
        target = followTarget;

        if (snapImmediate && target != null)
            transform.position = target.position + offset;
    }

    public void PlayShake(float duration, float magnitude)
    {
        shakeTimer = Mathf.Max(shakeTimer, duration);
        shakeMagnitude = Mathf.Max(shakeMagnitude, magnitude);
    }

    private void Update()
    {
        HandleScrollZoom();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        UpdateShake();

        Vector3 desired = target.position + offset + new Vector3(shakeOffset.x, shakeOffset.y, 0f);
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }

    private void HandleScrollZoom()
    {
        if (cachedCamera == null || !cachedCamera.orthographic)
            return;

        if (Mouse.current == null)
            return;

        if (GameStateController.Instance != null && GameStateController.Instance.IsPaused)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) < 0.01f)
            return;

        cachedCamera.orthographicSize = Mathf.Clamp(
            cachedCamera.orthographicSize - scroll * zoomSpeed * 0.01f,
            minOrthographicSize,
            maxOrthographicSize);
    }

    private void UpdateShake()
    {
        if (shakeTimer <= 0f)
        {
            shakeOffset = Vector2.zero;
            return;
        }

        shakeTimer -= Time.deltaTime;
        float damp = Mathf.Clamp01(shakeTimer / 0.12f);
        shakeOffset = Random.insideUnitCircle * shakeMagnitude * damp;
    }
}
