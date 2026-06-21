using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // Wajib ditambahkan untuk memanipulasi UI Image

public class GazeUIButton : MonoBehaviour
{
    [Header("Pengaturan Waktu Gaze")]
    [Tooltip("Berapa lama user harus menatap tombol ini (dalam detik)")]
    public float gazeDuration = 2.0f;
    
    [Header("Visual Feedback (Loading Bar)")]
    [Tooltip("Masukkan objek UI Image (tipe Filled) ke sini")]
    public Image loadingBar; // Menampung gambar loading

    private float gazeTimer = 0f;
    private bool isGazing = false;

    [Header("Aksi Tombol")]
    public UnityEvent onGazeComplete;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        
        // Pastikan loading bar kosong saat game baru dimulai
        if (loadingBar != null)
        {
            loadingBar.fillAmount = 0f;
        }
    }

    void Update()
    {
        // 1. Tembakkan laser imajiner
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                isGazing = true;
            }
            else
            {
                isGazing = false;
            }
        }
        else
        {
            isGazing = false;
        }

        // 2. Logika Timer & Animasi Loading
        if (isGazing)
        {
            gazeTimer += Time.deltaTime;

            // --- ANIMASI LOADING BAR NAIK ---
            if (loadingBar != null)
            {
                // Menghitung persentase (0.0 sampai 1.0)
                loadingBar.fillAmount = gazeTimer / gazeDuration; 
            }

            // Jika timer penuh
            if (gazeTimer >= gazeDuration)
            {
                isGazing = false; 
                gazeTimer = 0f;   
                
                // Reset loading bar jadi 0 lagi setelah selesai
                if (loadingBar != null)
                {
                    loadingBar.fillAmount = 0f; 
                }
                
                onGazeComplete.Invoke(); 
            }
        }
        else
        {
            gazeTimer = 0f; 

            // --- RESET LOADING BAR JIKA BATAL MENATAP ---
            if (loadingBar != null)
            {
                loadingBar.fillAmount = 0f;
            }
        }
    }
}