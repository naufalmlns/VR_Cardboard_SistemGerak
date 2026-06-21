using System.Collections;
using UnityEngine;

public class QuizManagerTaman : MonoBehaviour
{
    [Header("Pengaturan Kunci Jawaban")]
    public string jawabanBenar = "B";

    [Header("Pengaturan Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip voBenar;
    public AudioClip voSalah;

    [Header("Transisi Environment")]
    public GameObject envTaman;
    public GameObject envLorong;
    public VRSequenceManager sequenceManager;

    // --- UNTUK KAMERA ---
    [Header("Pengaturan Pemain")]
    [Tooltip("Masukkan objek Main Camera / XR Rig pemain ke sini")]
    public Transform kameraPemain;
    [Tooltip("Masukkan objek Kamera_Kereta dari Env_Lorong ke sini")]
    public Transform gerbongKeretaLorong;
    // ----------------------------------    

    [Header("Pengunci Gaze (UX)")]
    [Tooltip("Masukkan semua objek tombol A, B, dan C ke sini agar bisa dimatikan saat suara bermain")]
    public Collider[] daftarTombolKuis;

    // Variabel gembok penanda apakah sistem sedang sibuk memutar audio
    private bool sedangMemproses = false;

    // --- TAMBAHAN BARU UNTUK TRANSISI CAHAYA ---
    [Header("Efek Cahaya Mengecil")]
    [Tooltip("Masukkan UI Image warna putih yang menutupi full layar Canvas")]
    public UnityEngine.UI.Image layarCahayaPutih;
    [Tooltip("Lama waktu layar memutih dan kembali bening (dalam detik)")]
    public float durasiCahaya = 1f;

    public void PilihJawaban(string opsiDipilih)
    {
        // Jika sedang memproses jawaban (audio sedang main), abaikan perintah baru sama sekali
        if (sedangMemproses) return;

        // Pindahkan logika ke Coroutine agar bisa menangani jeda waktu
        StartCoroutine(ProsesJawaban(opsiDipilih));
    }

    private IEnumerator ProsesJawaban(string opsiDipilih)
    {
        sedangMemproses = true;
        AturInteraksiTombol(false);
        string pilihan = opsiDipilih.ToUpper();

        if (pilihan == jawabanBenar.ToUpper())
        {
            Debug.Log("<color=green>JAWABAN BENAR!</color>");

            float durasiVO = 0f;
            if (audioSource != null && voBenar != null)
            {
                audioSource.clip = voBenar;
                audioSource.Play();
                durasiVO = voBenar.length;
            }

            // Tunggu persis selama audio Benar berbicara
            yield return new WaitForSeconds(durasiVO);

            // --- FADE OUT: LAYAR MENJADI PUTIH ---
            if (layarCahayaPutih != null)
            {
                layarCahayaPutih.gameObject.SetActive(true);
                float t = 0;
                while (t < durasiCahaya)
                {
                    t += Time.deltaTime;
                    Color warnaLayar = layarCahayaPutih.color;
                    warnaLayar.a = Mathf.Lerp(0, 1, t / durasiCahaya);
                    layarCahayaPutih.color = warnaLayar;
                    yield return null;
                }
            }
            // -------------------------------------

            // --- TELEPORTASI (Saat layar buta/putih) ---
            if (kameraPemain != null && gerbongKeretaLorong != null)
            {
                kameraPemain.SetParent(gerbongKeretaLorong);
                kameraPemain.localPosition = Vector3.zero;
                kameraPemain.localRotation = Quaternion.identity;
            }

            RenderSettings.fog = true;

            if (sequenceManager != null) sequenceManager.LanjutKeLangkahBerikutnya();

            // Tukar Dunia (Matikan taman & kuis ini)
            if (envLorong != null) envLorong.SetActive(true);

            // PERINGATAN: Setelah baris ini tereksekusi, script ini akan mati total!
            if (envTaman != null) envTaman.SetActive(false);
        }
        else
        {
            // --- JAWABAN SALAH ---
            Debug.Log("<color=red>JAWABAN SALAH!</color>");

            float durasiVO = 0f;
            if (audioSource != null && voSalah != null)
            {
                audioSource.clip = voSalah;
                audioSource.Play();
                durasiVO = voSalah.length;
            }

            // Tunggu persis selama audio Salah selesai mengingatkan pemain
            yield return new WaitForSeconds(durasiVO);

            // 2. BUKA KUNCI KUIS: Nyalakan kembali collider agar pemain bisa menjawab ulang
            sedangMemproses = false;
            AturInteraksiTombol(true);
        }
    }

    // Fungsi bantuan untuk mematikan/menyalakan interaksi collider sekaligus
    private void AturInteraksiTombol(bool status)
    {
        foreach (Collider tombol in daftarTombolKuis)
        {
            if (tombol != null)
            {
                tombol.enabled = status;
            }
        }
    }
}