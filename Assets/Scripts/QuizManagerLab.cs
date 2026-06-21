using System.Collections;
using UnityEngine;

public class QuizManagerLab : MonoBehaviour
{
    [Header("Pengaturan Kunci Jawaban")]
    [Tooltip("Sesuai naskah Lab: A. Berkontraksi/Mengencang")]
    public string jawabanBenar = "A";

    [Header("Pengaturan Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip voBenar; // "Tepat sekali! Otot membantu tubuh..."
    public AudioClip voSalah;
    
    [Tooltip("Waktu ekstra tombol tetap terkunci setelah audio selesai (detik)")]
    public float jedaEkstraKunci = 1.0f; // <-- Tambahan UX

    [Header("Pengunci Gaze (UX)")]
    public Collider[] daftarTombolKuis;

    [Header("Transisi ke Scene 5")]
    [Tooltip("Masukkan objek Logika_Scene5 ke sini")]
    public GameObject sistemRangkaScene5;
    
    [Tooltip("Masukkan objek TV Video Otot agar layarnya mati setelah kuis selesai")]
    public GameObject tvVideoOtot;

    private bool sedangMemproses = false;

    public void PilihJawaban(string opsiDipilih)
    {
        if (sedangMemproses) return;
        StartCoroutine(ProsesJawaban(opsiDipilih));
    }

    private IEnumerator ProsesJawaban(string opsiDipilih)
    {
        sedangMemproses = true;
        AturInteraksiTombol(false); // 1. Tombol langsung dikunci (Collider mati)
        string pilihan = opsiDipilih.ToUpper();

        if (pilihan == jawabanBenar.ToUpper())
        {
            // --- JAWABAN BENAR ---
            Debug.Log("<color=green>JAWABAN BENAR DI LAB!</color>");

            float durasiVO = 0f;
            if (audioSource != null && voBenar != null)
            {
                audioSource.clip = voBenar;
                audioSource.Play();
                durasiVO = voBenar.length;
            }

            // Tunggu selama audio Benar berbicara (tidak perlu jeda ekstra karena akan pindah scene)
            yield return new WaitForSeconds(durasiVO);

            gameObject.SetActive(false);

            if (tvVideoOtot != null)
            {
                tvVideoOtot.SetActive(false);
            }

            if (sistemRangkaScene5 != null)
            {
                sistemRangkaScene5.SetActive(true);
            }
            
            Debug.Log("<color=yellow>Transisi ke Scene 5 dimulai! Rangka muncul di meja.</color>");
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

            // 2. Tunggu selama audio + Jeda Ekstra
            // Walaupun audio hanya 0.5 detik, tombol akan tetap terkunci selama (0.5 + 1.0 = 1.5 detik)
            yield return new WaitForSeconds(durasiVO + jedaEkstraKunci);

            sedangMemproses = false;
            AturInteraksiTombol(true); // 3. Tombol baru bisa ditatap lagi di sini
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