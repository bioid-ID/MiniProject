using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.85f;
    [SerializeField] private float floatSpeed = 1.2f;

    private TextMeshPro textMesh;
    private float timer;
    private Color startColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
            startColor = textMesh.color;
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        float t = timer / lifetime;

        if (textMesh != null)
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
