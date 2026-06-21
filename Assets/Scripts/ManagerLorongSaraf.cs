using System.Collections;
using UnityEngine;

[System.Serializable]
public class VOLorongItem
{
    public float jedaSebelumAudio = 0f;
    public AudioClip klipAudio;
    public float jedaSetelahAudio = 0.5f;
}

public class ManagerLorongSaraf : MonoBehaviour
{
    [Header("Pengaturan Efek Transisi")]
    [Tooltip("Masukkan Layar_Putih dari Canvas_Transisi ke sini")]
    public UnityEngine.UI.Image layarCahayaPutih;
    public float durasiCahaya = 1f;

    [Header("Pengaturan Audio VO Dinamis")]
    public AudioSource audioSource;
    public VOLorongItem[] daftarVO;

    [Header("Pengaturan Aktor Terhubung")]
    public PathFollower[] daftarAktor;

    [Header("Transisi ke Scene 4 (Lab Otot)")]
    [Tooltip("Berapa lama pemain tetap berjalan setelah semua VO selesai")]
    public float waktuJalanEkstra = 3f; 
    public GameObject envLorongSaraf;
    public GameObject envLab;
    
    [Tooltip("Objek akar/root dari Player VR yang akan dipindahkan posisinya")]
    public Transform playerRig; 
    [Tooltip("Titik referensi di mana pemain harus berdiri saat berada di Lab")]
    public Transform targetPosisiLab;
    public GameObject logikaScene4Otot;

    void OnEnable()
    {
        StartCoroutine(MulaiSequenceLorong());
    }

    private IEnumerator MulaiSequenceLorong()
    {
        // 1. FADE IN: KEMBALIKAN LAYAR MENJADI BENING DI LORONG
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

        // 2. Jeda adaptasi
        yield return new WaitForSeconds(0.5f);

        // 3. Suruh aktor melangkah
        AturJalanAktor(true);

        // 4. Mainkan antrean VO
        foreach (VOLorongItem vo in daftarVO)
        {
            if (vo.jedaSebelumAudio > 0) yield return new WaitForSeconds(vo.jedaSebelumAudio);

            if (vo.klipAudio != null && audioSource != null)
            {
                audioSource.clip = vo.klipAudio;
                audioSource.Play();
                yield return new WaitForSeconds(vo.klipAudio.length);
            }

            if (vo.jedaSetelahAudio > 0) yield return new WaitForSeconds(vo.jedaSetelahAudio);
        }

        // 5. JEDA JALAN EKSTRA
        Debug.Log("<color=yellow>VO habis. Melanjutkan sisa perjalanan...</color>");
        yield return new WaitForSeconds(waktuJalanEkstra);

        // ==========================================
        // PROSES TRANSISI KE SCENE 4 (LAB)
        // ==========================================
        
        // 6. FADE OUT: LAYAR MENJADI PUTIH PEKAT
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

        // 7. Hentikan perjalanan aktor saat layar sudah tertutup
        AturJalanAktor(false);

        // 8. EVAKUASI KAMERA DAN SCRIPT AGAR TIDAK IKUT MATI
        if (playerRig != null)
        {
            playerRig.SetParent(null);
        }
        transform.SetParent(null); 

        // 9. MATIKAN FOG SECARA GLOBAL (DI BALIK LAYAR PUTIH)
        RenderSettings.fog = false;

        // 10. Tukar Lingkungan
        if (envLorongSaraf != null) envLorongSaraf.SetActive(false);
        if (envLab != null) envLab.SetActive(true);

        // 11. Pindahkan Posisi Kamera/Pemain ke dalam Lab
        if (playerRig != null && targetPosisiLab != null)
        {
            playerRig.position = targetPosisiLab.position;
            playerRig.rotation = targetPosisiLab.rotation; 
        }

        // 12. Bangunkan Manager Scene 4
        if (logikaScene4Otot != null) logikaScene4Otot.SetActive(true);

        // 13. FADE IN: KEMBALIKAN LAYAR MENJADI BENING DI LAB
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

        Debug.Log("<color=green>Tiba di Lab (tanpa Fog) dan transisi selesai! Scene 4 dimulai.</color>");
    }

    private void AturJalanAktor(bool statusJalan)
    {
        foreach (PathFollower aktor in daftarAktor)
        {
            if (aktor != null) aktor.SetBerjalan(statusJalan);
        }
    }
}