using System.Collections;
using UnityEngine;

public class QuizManagerScene8 : MonoBehaviour
{
    [Header("Pengaturan Kunci Jawaban")]
    [Tooltip("Sesuai naskah Scene 8: B. Olahraga dan makan bergizi")]
    public string jawabanBenar = "B";

    [Header("Pengaturan Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip voBenar; 
    public AudioClip voSalah;

    [Header("Pengunci Gaze (UX)")]
    [Tooltip("Masukkan Collider dari tombol A, B, dan C agar tidak bisa ditatap dobel")]
    public Collider[] daftarTombolKuis;

    [Header("Transisi ke Scene Berikutnya")]
    [Tooltip("Masukkan objek/environment Scene 8 yang ingin di-hide")]
    public GameObject objekScene8Lama; // <-- Tambahan variabel baru
    [Tooltip("Masukkan Manager Scene 9 (Penutup) ke sini")]
    public GameObject logikaScene9Akhir;

    private bool sedangMemproses = false;

    public void PilihJawaban(string opsiDipilih)
    {
        if (sedangMemproses) return;
        StartCoroutine(ProsesJawaban(opsiDipilih));
    }

    private IEnumerator ProsesJawaban(string opsiDipilih)
    {
        sedangMemproses = true;
        
        // Kunci tombol agar anak tidak menatap jawaban lain saat sistem sedang memproses
        AturInteraksiTombol(false);
        
        string pilihan = opsiDipilih.ToUpper();

        if (pilihan == jawabanBenar.ToUpper())
        {
            // --- JAWABAN BENAR ---
            Debug.Log("<color=green>JAWABAN BENAR DI KUIS KESEHATAN!</color>");

            float durasiVO = 0f;
            if (audioSource != null && voBenar != null)
            {
                audioSource.clip = voBenar;
                audioSource.Play();
                durasiVO = voBenar.length;
            }

            // Tunggu persis selama audio Benar berbicara
            yield return new WaitForSeconds(durasiVO);

            // 1. Matikan Panel Kuis ini sendiri
            gameObject.SetActive(false);

            // 2. Matikan objek Scene 8 (Sesuai request) <-- Tambahan logika baru
            if (objekScene8Lama != null)
            {
                objekScene8Lama.SetActive(false);
            }

            // 3. Lanjut ke Scene 9 atau Penutup Game
            if (logikaScene9Akhir != null)
            {
                logikaScene9Akhir.SetActive(true);
            }
            
            Debug.Log("<color=yellow>Kuis Kesehatan Selesai! Scene 8 dimatikan, lanjut ke tahap akhir.</color>");
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

            yield return new WaitForSeconds(durasiVO);

            // Buka kembali kunci tombol agar anak bisa mencoba menjawab lagi
            sedangMemproses = false;
            AturInteraksiTombol(true);
        }
    }

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