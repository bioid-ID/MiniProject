using System.Collections;
using UnityEngine;

public static class CameraShake
{
    public static void Shake(float duration, float magnitude)
    {
        Camera camera = Camera.main;
        if (camera == null)
            return;

        SimpleCameraFollow follow = camera.GetComponent<SimpleCameraFollow>();
        if (follow != null)
        {
            follow.PlayShake(duration, magnitude);
            return;
        }

        SimpleTweenRunner.Instance.Play(ShakeRoutine(camera.transform, duration, magnitude));
    }

    private static IEnumerator ShakeRoutine(Transform cameraTransform, float duration, float magnitude)
    {
        Vector3 original = cameraTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float damp = 1f - Mathf.Clamp01(elapsed / duration);
            Vector2 offset = Random.insideUnitCircle * magnitude * damp;
            cameraTransform.localPosition = original + new Vector3(offset.x, offset.y, 0f);
            yield return null;
        }

        cameraTransform.localPosition = original;
    }
}
