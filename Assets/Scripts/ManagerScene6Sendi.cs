using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class ManagerScene6Sendi : MonoBehaviour
{
    [Header("Pengaturan Layar TV")]
    [Tooltip("Masukkan objek Canvas TV/Video Player di sini")]
    public GameObject tvLayar;
    public VideoPlayer videoPlayer;
    
    [Header("Materi Video (Scene 6)")]
    [Tooltip("Masukkan 3 video sendi secara berurutan")]
    public VideoClip[] daftarVideoSendi;

    [Header("Kuis Scene 6")]
    [Tooltip("Masukkan Panel Kuis Sendi yang akan muncul setelah video selesai")]
    public GameObject panelKuisSendi;

    void OnEnable()
    {
        if (panelKuisSendi != null) panelKuisSendi.SetActive(false);
        
        // PENTING: Jangan nyalakan TV di sini! Biarkan TV dalam keadaan mati dulu.
        if (tvLayar != null) tvLayar.SetActive(false);

        StartCoroutine(PutarMateriSendi());
    }

    private IEnumerator PutarMateriSendi()
    {
        // Beri jeda 1 detik saat TV MASIH MATI agar pemain fokus melihat ke arah TV
        yield return new WaitForSeconds(1f);

        // Memutar 3 video secara berurutan
        for (int i = 0; i < daftarVideoSendi.Length; i++)
        {
            if (videoPlayer != null && daftarVideoSendi[i] != null)
            {
                // 1. Masukkan kaset baru dan perintahkan mesin untuk "bersiap" (loading)
                videoPlayer.clip = daftarVideoSendi[i];
                videoPlayer.Prepare();

                // 2. Tunggu sampai kaset benar-benar siap di belakang layar
                while (!videoPlayer.isPrepared)
                {
                    yield return null; // Tunggu ke frame berikutnya
                }

                // 3. Khusus untuk video pertama, nyalakan layar TV HANYA SETELAH videonya siap!
                if (i == 0 && tvLayar != null)
                {
                    tvLayar.SetActive(true);
                }

                // 4. Putar video (sekarang dijamin frame pertamanya adalah video Scene 6)
                videoPlayer.Play();

                // Tunggu persis selama durasi video tersebut, ditambah jeda transisi 0.5 detik
                yield return new WaitForSeconds((float)daftarVideoSendi[i].length + 0.5f);
            }
        }

        Debug.Log("<color=yellow>Materi Sendi Selesai! Memunculkan Kuis Scene 6.</color>");
        
        // Munculkan Kuis Interaktif
        if (panelKuisSendi != null) panelKuisSendi.SetActive(true);
    }
}