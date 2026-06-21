using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement; // Wajib untuk mereset Scene

public class ManagerScene9Penutup : MonoBehaviour
{
    [Header("Pengaturan Layar TV & UI")]
    public GameObject tvLayar;
    public VideoPlayer videoPlayer;
    public GameObject panelTombolAkhir;

    [Header("Materi Video & Audio (Scene 9)")]
    public VideoClip videoPenutup;
    public AudioClip[] daftarVOPenutup;
    public AudioSource audioSource;

    [Header("Transisi Kembali ke Main Menu")]
    public Image layarTransisi;
    public float durasiFade = 1.0f;

    private bool sedangMemproses = false;

    void OnEnable()
    {
        sedangMemproses = false;
        
        if (panelTombolAkhir != null) panelTombolAkhir.SetActive(false);
        if (tvLayar != null) tvLayar.SetActive(false);

        StartCoroutine(PutarMateriPenutup());
    }

    private IEnumerator PutarMateriPenutup()
    {
        yield return new WaitForSeconds(1f);

        if (videoPlayer != null && videoPenutup != null)
        {
            videoPlayer.clip = videoPenutup;
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared) yield return null;

            if (tvLayar != null) tvLayar.SetActive(true);
            videoPlayer.Play();

            float totalWaktuAudio = 0f;
            if (audioSource != null && daftarVOPenutup.Length > 0)
            {
                foreach (AudioClip vo in daftarVOPenutup)
                {
                    if (vo != null)
                    {
                        audioSource.PlayOneShot(vo);
                        float jedaTunggu = vo.length + 0.3f;
                        yield return new WaitForSeconds(jedaTunggu);
                        totalWaktuAudio += jedaTunggu;
                    }
                }
            }

            float sisaWaktuVideo = (float)videoPenutup.length - totalWaktuAudio;
            if (sisaWaktuVideo > 0) yield return new WaitForSeconds(sisaWaktuVideo + 0.5f);
            else yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("<color=yellow>Materi Penutup Selesai! Memunculkan Tombol Akhir.</color>");
        if (panelTombolAkhir != null) panelTombolAkhir.SetActive(true);
    }

    public void TekanTombolSelesai()
    {
        if (sedangMemproses) return;
        StartCoroutine(ProsesKembaliKeMenu());
    }

    private IEnumerator ProsesKembaliKeMenu()
    {
        sedangMemproses = true;

        // 1. Lepaskan script ini dari Parent agar proses loading tidak terputus
        transform.SetParent(null);

        // 2. FADE OUT: Layar menjadi pekat
        if (layarTransisi != null)
        {
            layarTransisi.gameObject.SetActive(true);
            float t = 0;
            while (t < durasiFade)
            {
                t += Time.deltaTime;
                Color c = layarTransisi.color;
                c.a = Mathf.Lerp(0, 1, t / durasiFade); 
                layarTransisi.color = c;
                yield return null;
            }
        }

        // 3. CUCI BERSIH GAME! (Reload Scene)
        // Cara ini akan mengembalikan semua posisi kamera, status tulang, 
        // dan hierarchy kembali rapi seperti sedia kala!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}