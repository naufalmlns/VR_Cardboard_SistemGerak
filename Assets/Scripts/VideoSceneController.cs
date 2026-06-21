using UnityEngine;
using UnityEngine.Video;

public class VideoSceneController : MonoBehaviour
{
    [Header("Pengaturan Video")]
    public VideoPlayer videoPlayer; // Layar (Quad) tempat video diputar
    [Tooltip("Masukkan file video ke-2 (.mp4) ke sini")]
    public VideoClip klipVideo2;    // Kaset video ke-2

    [Header("Pengaturan UI")]
    public GameObject tombolMulai;

    void Start()
    {
        // 1. Sembunyikan tombol saat video 1 baru mulai
        if (tombolMulai != null)
        {
            tombolMulai.SetActive(false);
        }

        // 2. Daftarkan event untuk mendeteksi kapan video tamat
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += VideoSelesai;
        }
    }

    // Terpanggil otomatis saat video mentok/tamat
    void VideoSelesai(VideoPlayer vp)
    {
        // Cek agar tombol hanya muncul saat Video 1 yang selesai, 
        // bukan saat Video 2 selesai (agar tidak looping berantakan)
        if (vp.clip != klipVideo2) 
        {
            if (tombolMulai != null)
            {
                tombolMulai.SetActive(true);
            }
        }
    }

    // --- FUNGSI BARU UNTUK DIPANGGIL OLEH TOMBOL GAZE ---
    public void PutarVideoDua()
    {
        // 1. Sembunyikan tombol lagi
        if (tombolMulai != null)
        {
            tombolMulai.SetActive(false);
        }

        // 2. Ganti 'kaset' ke video 2 dan langsung putar
        if (videoPlayer != null && klipVideo2 != null)
        {
            videoPlayer.clip = klipVideo2;
            videoPlayer.Play();
        }
    }
}