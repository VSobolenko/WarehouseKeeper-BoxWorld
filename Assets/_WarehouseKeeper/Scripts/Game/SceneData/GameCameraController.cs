using Game.Components;
using UnityEngine;

namespace WarehouseKeeper.Directors.Game.SceneData
{
public class GameCameraController : MonoBehaviour
{
    [SerializeField] private CameraOrthographicScale _camera;
    [SerializeField] private float _offsetY;
    [SerializeField] private float _offsetX;
    
    public void SetupCamera(int maxX, int maxY)
    {
        _camera.WidthOrHeight = 0;
        _camera.Size = maxX / 2f + _offsetX;

        var cameraPosition = new Vector3(maxX / 2f, _camera.transform.position.y, maxY / 2f + _offsetY);
        _camera.transform.position = cameraPosition;
        
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (_camera == null)
            _camera = GetComponent<CameraOrthographicScale>();
    }

#endif
}
}