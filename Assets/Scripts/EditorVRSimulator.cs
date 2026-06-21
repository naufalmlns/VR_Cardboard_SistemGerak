using UnityEngine;

public class EditorVRSimulator : MonoBehaviour
{
// Kode ini hanya akan berjalan di dalam Unity Editor laptopmu.
// Saat dibuild ke HP Android, kode ini otomatis diabaikan.
#if UNITY_EDITOR
    private Vector3 lastMousePosition;
    private float xRotation = 0f;
    private float yRotation = 0f;
    public float sensitivity = 0.2f;

    void Update()
    {
        // Saat klik kanan mouse ditekan pertama kali
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        // Saat klik kanan mouse ditahan dan digeser
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            yRotation += delta.x * sensitivity;
            xRotation -= delta.y * sensitivity;
            
            // Membatasi agar tidak bisa menengok sampai kayang (terbalik)
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
#endif
}   