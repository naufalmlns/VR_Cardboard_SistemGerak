using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class ManagerScene7Berlari : MonoBehaviour
{
    [Header("Pengaturan Layar TV")]
    [Tooltip("Masukkan objek Canvas TV/Video Player di sini")]
    public GameObject tvLayar;
    public VideoPlayer videoPlayer;
    
    [Header("Materi Video & Audio (Scene 7)")]
    [Tooltip("Masukkan 1 video tubuh berlari (visual saja)")]
    public VideoClip videoBerlari;
    
    [Tooltip("Masukkan deretan file audio/suara VO berlari di sini agar diputar bergantian")]
    public AudioClip[] daftarAudioBerlari;
    public AudioSource audioSource;

    [Header("Transisi ke Scene Berikutnya")]
    [Tooltip("Masukkan objek Logika_Scene8 (Kesehatan) ke sini")]
    public GameObject logikaScene8Kesehatan;

    void OnEnable()
    {
        // Matikan TV sementara untuk loading kaset di belakang layar
        if (tvLayar != null) tvLayar.SetActive(false);

        StartCoroutine(PutarMateriBerlari());
    }

    private IEnumerator PutarMateriBerlari()
    {
        // Jeda sebentar setelah kuis sebelumnya selesai
        yield return new WaitForSeconds(1f);

        if (videoPlayer != null && videoBerlari != null)
        {
            // 1. Loading video baru
            videoPlayer.clip = videoBerlari;
            videoPlayer.Prepare();

            // 2. Tunggu sampai kaset siap
            while (!videoPlayer.isPrepared)
            {
                yield return null; 
            }

            // 3. Buka tirai TV dan putar videonya
            if (tvLayar != null) tvLayar.SetActive(true);
            videoPlayer.Play();

            // 4. PUTAR DERETAN SUARA BERSAMAAN DENGAN VIDEO
            float totalWaktuAudio = 0f;

            if (audioSource != null && daftarAudioBerlari.Length > 0)
            {
                foreach (AudioClip vo in daftarAudioBerlari)
                {
                    if (vo != null)
                    {
                        audioSource.PlayOneShot(vo);
                        
                        // Jeda persis sepanjang durasi suara + 0.3 detik untuk ambil napas
                        float jedaTunggu = vo.length + 0.3f;
                        yield return new WaitForSeconds(jedaTunggu);
                        
                        totalWaktuAudio += jedaTunggu; // Catat berapa lama waktu yang sudah dihabiskan audio
                    }
                }
            }

            // 5. PENGAMAN: Cek apakah video masih sisa walau audio sudah habis
            float sisaWaktuVideo = (float)videoBerlari.length - totalWaktuAudio;
            
            if (sisaWaktuVideo > 0)
            {
                // Jika video lebih panjang dari audio, tunggu sisa waktu videonya
                yield return new WaitForSeconds(sisaWaktuVideo + 0.5f);
            }
            else
            {
                // Jika audio lebih panjang/sama dengan video, beri jeda transisi sedikit saja
                yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            Debug.LogWarning("Video Player atau Video Berlari belum dimasukkan di Inspector!");
        }

        Debug.Log("<color=yellow>Materi Lari Selesai! Transisi ke Scene 8...</color>");

        // ==========================================
        // TRANSISI KE SCENE 8: KESEHATAN
        // ==========================================
        if (logikaScene8Kesehatan != null)
        {
            logikaScene8Kesehatan.SetActive(true);
        }
    }
}