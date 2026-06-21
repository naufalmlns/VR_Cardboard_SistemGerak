using System.Collections;
using UnityEngine;
using UnityEngine.Video;

// Struct untuk daftar langkah di Lab
[System.Serializable]
public class LangkahLab
{
    [Header("Pengaturan Video")]
    [Tooltip("Video yang akan diputar pada langkah ini")]
    public VideoClip klipVideo;

    [Header("Pengaturan UI (Opsional)")]
    [Tooltip("UI yang muncul setelah video tamat (Misal: Tombol Barbel / Panel Kuis).")]
    public GameObject uiSetelahVideo;
}

public class ManagerSequenceLab : MonoBehaviour
{
    [Header("Komponen Utama")]
    public VideoPlayer videoPlayer; 
    
    [Tooltip("Masukkan objek MainAudio ke sini untuk menyalurkan suara VO dan memicu Ducking BGM")]
    public AudioSource sumberAudioVO; // <-- Tambahan Slot Audio

    [Header("Daftar Alur Lab (Dinamis)")]
    [Tooltip("Tambahkan jumlah langkah video di sini (+/-)")]
    public LangkahLab[] daftarLangkah;

    private int indeksSekarang = 0;
    private bool sedangLoadingVideo = false;

    void OnEnable()
    {
        // Terpanggil otomatis saat pemain teleport ke Lab dan Lab menyala
        if (videoPlayer != null)
        {
            // Cegah error event bertumpuk
            videoPlayer.loopPointReached -= OnVideoTamat; 
            videoPlayer.loopPointReached += OnVideoTamat;
        }

        // Mulai mainkan langkah pertama
        MulaiLangkah(0);
    }

    void OnDisable()
    {
        // Bersihkan event saat Lab dimatikan agar memori aman
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoTamat;
        }
    }

    public void MulaiLangkah(int indeksTujuan)
    {
        if (sedangLoadingVideo) return;
        StartCoroutine(ProsesMulaiLangkah(indeksTujuan));
    }

    private IEnumerator ProsesMulaiLangkah(int indeksTujuan)
    {
        sedangLoadingVideo = true;
        SembunyikanSemuaUI();

        if (indeksTujuan >= daftarLangkah.Length)
        {
            Debug.Log("<color=cyan>Semua sequence di Lab telah selesai!</color>");
            videoPlayer.gameObject.SetActive(false);
            sedangLoadingVideo = false;
            yield break;
        }

        indeksSekarang = indeksTujuan;
        VideoClip klipBaru = daftarLangkah[indeksSekarang].klipVideo;
        
        if (klipBaru != null)
        {
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.Stop();
            videoPlayer.clip = klipBaru;

            // =========================================================
            // SETUP AUDIO SEBELUM "PREPARE"
            // =========================================================
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.controlledAudioTrackCount = 1; 
            videoPlayer.EnableAudioTrack(0, true);
            
            if (sumberAudioVO != null)
            {
                videoPlayer.SetTargetAudioSource(0, sumberAudioVO);
            }

            // Baru suruh Unity membaca kasetnya
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }

            // Cek apakah kasetnya benar-benar ada suaranya
            if (videoPlayer.audioTrackCount == 0)
            {
                videoPlayer.EnableAudioTrack(0, false);
            }

            videoPlayer.Play();
        }
        
        sedangLoadingVideo = false;
    }

    // Terpanggil otomatis saat 1 video selesai
    void OnVideoTamat(VideoPlayer vp)
    {
        LangkahLab langkahIni = daftarLangkah[indeksSekarang];

        if (langkahIni.uiSetelahVideo != null)
        {
            // Munculkan Barbel / Panel Kuis
            langkahIni.uiSetelahVideo.SetActive(true);
        }
        else
        {
            // Langsung lanjut jika dikosongkan
            LanjutKeLangkahBerikutnya();
        }
    }

    // Panggil fungsi ini dari Event Trigger Gaze di Barbel
    public void LanjutKeLangkahBerikutnya()
    {
        MulaiLangkah(indeksSekarang + 1);
    }

    void SembunyikanSemuaUI()
    {
        foreach (LangkahLab langkah in daftarLangkah)
        {
            if (langkah.uiSetelahVideo != null)
            {
                langkah.uiSetelahVideo.SetActive(false);
            }
        }
    }
}