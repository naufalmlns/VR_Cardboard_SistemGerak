using UnityEngine;
using UnityEngine.UI; // Wajib ditambahkan untuk memanggil Image UI

public class KameraGazeRaycaster : MonoBehaviour
{
    [Header("Pengaturan Laser")]
    [Tooltip("Jarak maksimal mata bisa menatap objek (meter)")]
    public float jarakGaze = 15f;

    [Header("UI Reticle (Gaze)")]
    [Tooltip("Masukkan objek Image DonatLoading yang ada di Canvas kamera")]
    public Image donatLoading;

    private TulangGaze tulangTerakhir;
    private float timerGaze = 0f;

    void Start()
    {
        // Pastikan donat tidak terlihat di awal
        if (donatLoading != null)
        {
            donatLoading.fillAmount = 0f;
            donatLoading.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, jarakGaze))
        {
            TulangGaze tulang = hit.collider.GetComponent<TulangGaze>();

            // Jika menatap tulang DAN tulang itu belum terbang
            if (tulang != null && !tulang.ApakahSedangTerbang())
            {
                // Jika ini adalah tulang baru (baru pindah tatapan)
                if (tulang != tulangTerakhir)
                {
                    ResetGaze();
                    tulangTerakhir = tulang;
                    
                    if (donatLoading != null) donatLoading.gameObject.SetActive(true);
                }

                // --- PROSES LOADING DONAT ---
                timerGaze += Time.deltaTime; // Waktu terus berjalan
                
                if (donatLoading != null)
                {
                    // Animasi mengisi donat dari 0.0 ke 1.0
                    donatLoading.fillAmount = timerGaze / tulang.durasiMenatap;
                }

                // Jika waktu sudah penuh
                if (timerGaze >= tulang.durasiMenatap)
                {
                    tulang.PicuTerbang(); // Perintahkan tulang untuk melayang!
                    ResetGaze(); // Sembunyikan donat
                }
            }
            else
            {
                // Menatap tulang yang sudah terbang
                ResetGaze();
            }
        }
        else
        {
            // Menatap tembok / ruang kosong
            ResetGaze();
        }
    }

    // Fungsi untuk mengosongkan dan menyembunyikan donat
    private void ResetGaze()
    {
        tulangTerakhir = null;
        timerGaze = 0f;
        if (donatLoading != null)
        {
            donatLoading.fillAmount = 0f;
            donatLoading.gameObject.SetActive(false);
        }
    }
}