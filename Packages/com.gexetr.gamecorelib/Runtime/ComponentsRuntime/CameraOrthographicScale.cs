using UnityEngine;

namespace Game.Components
{
/// <summary>
/// Width or height stays constant when changing screen resolution
/// </summary>
[RequireComponent(typeof(Camera)), ExecuteInEditMode]
public class CameraOrthographicScale : MonoBehaviour
{
    [SerializeField, HideInInspector] private Camera cameraComponent;

    [SerializeField, Space] private Vector2 defaultResolution = new Vector2(720, 1280);
    [SerializeField, Range(0f, 1f)] private float widthOrHeight;
    [SerializeField] private float size = 4;
    [SerializeField, Space] private bool autoUpdate;

    public float Size
    {
        get => size;
        set
        {
            size = value;
            UpdateScale();
        }
    }

    public float WidthOrHeight
    {
        get => widthOrHeight;
        set
        {
            widthOrHeight = Mathf.Clamp01(value);
            UpdateScale();
        }
    }

    private void Update()
    {
        if (autoUpdate == false)
            return;

        UpdateScale();
    }

    public void UpdateScale()
    {
        if (cameraComponent.orthographic == false)
        {
            Log.Warning("Can't update scale in non orthographic camera");

            return;
        }

        var constantWidthSize = size * (defaultResolution.x / defaultResolution.y / cameraComponent.aspect);
        cameraComponent.orthographicSize = Mathf.Lerp(constantWidthSize, size, widthOrHeight);
    }

    private void Reset() => cameraComponent ??= GetComponent<Camera>();
}
}