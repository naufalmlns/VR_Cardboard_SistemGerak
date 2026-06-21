using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class LangkahSequence
{
    [Header("Pengaturan Video")]
    public VideoClip klipVideo;

    [Header("Pengaturan UI (Opsional)")]
    public GameObject uiSetelahVideo;
}

public class VRSequenceManager : MonoBehaviour
{
    [Header("Komponen Utama")]
    public VideoPlayer videoPlayer; 
    
    public AudioSource sumberAudioVO; 

    [Header("Daftar Alur VR (Dinamis)")]
    public LangkahSequence[] daftarLangkah;

    private int indeksSekarang = 0;
    private bool sedangLoadingVideo = false;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoTamat;
        }

        MulaiLangkah(0);
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
            Debug.Log("Semua sequence telah selesai!");
            videoPlayer.gameObject.SetActive(false);
            sedangLoadingVideo = false;
            yield break; 
        }

        indeksSekarang = indeksTujuan;
        VideoClip klipBaru = daftarLangkah[indeksSekarang].klipVideo;
        
        if (klipBaru != null)
        {
            videoPlayer.gameObject.SetActive(true);
            
            // 1. Matikan video lama dan masukkan kaset baru
            videoPlayer.Stop();
            videoPlayer.clip = klipBaru;
            
            // =========================================================
            // KUNCI PERBAIKAN: COLOK KABEL SEBELUM "PREPARE"!
            // =========================================================
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.controlledAudioTrackCount = 1; // Kita minta izin pakai 1 track
            videoPlayer.EnableAudioTrack(0, true);
            
            if (sumberAudioVO != null)
            {
                videoPlayer.SetTargetAudioSource(0, sumberAudioVO);
            }

            // 2. Baru suruh Unity membaca kasetnya
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }

            // 3. Setelah selesai baca, cek: Apakah kasetnya benar-benar ada suaranya?
            if (videoPlayer.audioTrackCount == 0)
            {
                // Jika ternyata bisu (seperti Element 1), matikan track-nya agar Unity tidak error
                videoPlayer.EnableAudioTrack(0, false);
            }

            // 4. Play! Suara Element 0 sekarang pasti langsung keluar!
            videoPlayer.Play();
        }
        
        sedangLoadingVideo = false;
    }

    void OnVideoTamat(VideoPlayer vp)
    {
        LangkahSequence langkahIni = daftarLangkah[indeksSekarang];

        if (langkahIni.uiSetelahVideo != null)
        {
            langkahIni.uiSetelahVideo.SetActive(true);
        }
        else
        {
            LanjutKeLangkahBerikutnya();
        }
    }

    public void LanjutKeLangkahBerikutnya()
    {
        MulaiLangkah(indeksSekarang + 1);
    }

    void SembunyikanSemuaUI()
    {
        foreach (LangkahSequence langkah in daftarLangkah)
        {
            if (langkah.uiSetelahVideo != null)
            {
                langkah.uiSetelahVideo.SetActive(false);
            }
        }
    }
}