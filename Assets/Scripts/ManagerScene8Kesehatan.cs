using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class ManagerScene8Kesehatan : MonoBehaviour
{
    [Header("Pengaturan Layar TV & UI")]
    public GameObject tvLayar;
    public VideoPlayer videoPlayer;
    [Tooltip("Panel UI yang berisi 5 tombol gambar kebiasaan")]
    public GameObject panelMenuKebiasaan;
    [Tooltip("Panel Kuis untuk Scene 8")]
    public GameObject panelKuisScene8;

    [Header("Audio Pembuka")]
    public AudioSource audioSource;
    public AudioClip voIntro; 

    [Header("Materi 5 Kebiasaan")]
    [Tooltip("Masukkan 5 video kebiasaan. URUTAN HARUS SAMA dengan tombolnya.")]
    public VideoClip[] daftarVideoKebiasaan;
    [Tooltip("Masukkan 5 VO kebiasaan. URUTAN HARUS SAMA dengan videonya.")]
    public AudioClip[] daftarAudioKebiasaan;

    [Header("Pengunci Gaze (UX)")]
    [Tooltip("Masukkan Collider (misal BoxCollider) dari ke-5 tombol kebiasaan di sini")]
    public Collider[] daftarColliderTombol;

    private bool sedangMemutarMateri = false;

    void OnEnable()
    {
        sedangMemutarMateri = false;
        
        // 1. Tampilkan Menu, Sembunyikan Kuis
        if (panelMenuKebiasaan != null) panelMenuKebiasaan.SetActive(true);
        if (panelKuisScene8 != null) panelKuisScene8.SetActive(false);
        if (tvLayar != null) tvLayar.SetActive(true);

        // 2. Kunci semua tombol agar tidak bisa di-gaze dulu
        AturInteraksiTombol(false);

        // 3. Mulai memutar VO dan menunggu
        StartCoroutine(SequenceIntro());
    }

    private IEnumerator SequenceIntro()
    {
        float durasiVO = 0f;

        if (audioSource != null && voIntro != null)
        {
            audioSource.PlayOneShot(voIntro);
            durasiVO = voIntro.length; 
        }

        // Tunggu persis selama VO berbicara
        yield return new WaitForSeconds(durasiVO);

        // 4. VO Selesai! Buka kunci tombol agar bisa ditatap
        AturInteraksiTombol(true);
        Debug.Log("<color=green>VO Intro Selesai. Tombol Kebiasaan sekarang bisa di-gaze!</color>");
    }

    // Fungsi tambahan untuk mematikan/menyalakan interaksi
    private void AturInteraksiTombol(bool status)
    {
        foreach (Collider tombol in daftarColliderTombol)
        {
            if (tombol != null)
            {
                tombol.enabled = status;
            }
        }
    }

    // Fungsi ini yang akan dipanggil oleh script GazeUIButton milikmu
    public void PilihKebiasaan(int indeksKebiasaan)
    {
        if (sedangMemutarMateri) return; 

        if (indeksKebiasaan >= 0 && indeksKebiasaan < daftarVideoKebiasaan.Length)
        {
            StartCoroutine(PutarMateriDanLanjutKuis(indeksKebiasaan));
        }
    }

    private IEnumerator PutarMateriDanLanjutKuis(int indeks)
    {
        sedangMemutarMateri = true;

        if (audioSource != null) audioSource.Stop();
        if (panelMenuKebiasaan != null) panelMenuKebiasaan.SetActive(false);

        if (videoPlayer != null && daftarVideoKebiasaan[indeks] != null)
        {
            videoPlayer.clip = daftarVideoKebiasaan[indeks];
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }

            videoPlayer.Play();
            
            float durasiTunggu = (float)daftarVideoKebiasaan[indeks].length;

            if (audioSource != null && daftarAudioKebiasaan.Length > indeks && daftarAudioKebiasaan[indeks] != null)
            {
                audioSource.PlayOneShot(daftarAudioKebiasaan[indeks]);
                
                if (daftarAudioKebiasaan[indeks].length > durasiTunggu)
                {
                    durasiTunggu = daftarAudioKebiasaan[indeks].length;
                }
            }

            yield return new WaitForSeconds(durasiTunggu + 0.5f);
        }

        Debug.Log("<color=yellow>Materi Kebiasaan Selesai! Memunculkan Kuis Scene 8.</color>");

        if (panelKuisScene8 != null) panelKuisScene8.SetActive(true);
    }
}