using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Wajib untuk manipulasi Image Layar Putih

public class QuizManagerSendi : MonoBehaviour
{
    [Header("Pengaturan Kunci Jawaban")]
    [Tooltip("Sesuai naskah Lab: B. Sendi")]
    public string jawabanBenar = "B";

    [Header("Pengaturan Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip voBenar; 
    public AudioClip voSalah;

    [Header("Pengunci Gaze (UX)")]
    public Collider[] daftarTombolKuis;

    [Header("Pengaturan Efek Transisi Lingkungan")]
    [Tooltip("Masukkan Layar_Putih dari Canvas_Transisi ke sini")]
    public Image layarCahayaPutih;
    public float durasiCahaya = 1f;
    
    [Tooltip("Lingkungan Lab (Scene 4-6) yang akan dimatikan")]
    public GameObject envLab;
    [Tooltip("Lingkungan Taman (Scene 7-9) yang akan dinyalakan")]
    public GameObject envTaman;
    
    [Tooltip("Objek akar/root dari Player VR yang akan dipindahkan posisinya")]
    public Transform playerRig; 
    [Tooltip("Titik referensi pemain berdiri di Taman (menghadap TV Taman)")]
    public Transform targetPosisiTaman;

    [Header("Transisi ke Scene Berikutnya")]
    [Tooltip("Masukkan Manager Scene 7 (Berlari) ke sini")]
    public GameObject logikaScene7;

    private bool sedangMemproses = false;

    public void PilihJawaban(string opsiDipilih)
    {
        if (sedangMemproses) return;
        StartCoroutine(ProsesJawaban(opsiDipilih));
    }

    private IEnumerator ProsesJawaban(string opsiDipilih)
    {
        sedangMemproses = true;
        AturInteraksiTombol(false);
        
        string pilihan = opsiDipilih.ToUpper();

        if (pilihan == jawabanBenar.ToUpper())
        {
            // --- JAWABAN BENAR ---
            Debug.Log("<color=green>JAWABAN BENAR DI KUIS SENDI!</color>");

            float durasiVO = 0f;
            if (audioSource != null && voBenar != null)
            {
                audioSource.clip = voBenar;
                audioSource.Play();
                durasiVO = voBenar.length;
            }

            // Tunggu persis selama audio Benar berbicara
            yield return new WaitForSeconds(durasiVO);

            // ==========================================
            // MULAI SEQUENCE TRANSISI LINGKUNGAN
            // ==========================================

            // 1. FADE OUT: LAYAR MENJADI PUTIH PEKAT
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

            // 2. TUKAR POSISI & NYALAKAN TAMAN 
            // (PENTING: envLab jangan dimatikan dulu agar script ini tetap hidup!)
            
            // Lepaskan ikatan Player Rig agar aman
            if (playerRig != null) playerRig.SetParent(null); 
            
            // Nyalakan lingkungan Taman
            if (envTaman != null) envTaman.SetActive(true);

            // Pindahkan posisi pemain ke depan TV Taman
            if (playerRig != null && targetPosisiTaman != null)
            {
                playerRig.position = targetPosisiTaman.position;
                playerRig.rotation = targetPosisiTaman.rotation;
            }

            // 3. BANGUNKAN SCENE 7 (Video Berlari)
            if (logikaScene7 != null)
            {
                logikaScene7.SetActive(true);
            }

            // 4. FADE IN: KEMBALIKAN LAYAR MENJADI BENING DI TAMAN
            if (layarCahayaPutih != null)
            {
                float t = 0;
                while (t < durasiCahaya)
                {
                    t += Time.deltaTime;
                    Color warnaLayar = layarCahayaPutih.color;
                    warnaLayar.a = Mathf.Lerp(1, 0, t / durasiCahaya); 
                    layarCahayaPutih.color = warnaLayar;
                    yield return null;
                }
                layarCahayaPutih.gameObject.SetActive(false);
            }

            // 5. BERSIH-BERSIH TERAKHIR
            // Baru matikan lingkungan Lab setelah layar sepenuhnya bening di Taman
            if (envLab != null) envLab.SetActive(false);

            Debug.Log("<color=yellow>Transisi ke Taman Selesai! Kuis Sendi dimatikan.</color>");
            
            // Matikan Panel Kuis ini sendiri
            gameObject.SetActive(false);
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