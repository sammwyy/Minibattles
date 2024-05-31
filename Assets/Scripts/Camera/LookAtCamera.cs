using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main; // Obtener la cámara principal
    }

    void Update()
    {
        if (_camera != null)
        {
            // Hacer que el objeto mire hacia la cámara
            transform.LookAt(_camera.transform);

            // Corregir la orientación para que no se vea como un espejo
            transform.Rotate(0, 180, 0);
        }
    }
}
