using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuManagerVR : MonoBehaviour
{
    [Header("Pengaturan Main Menu")]
    public GameObject panelMainMenu;
    public VideoPlayer videoMenuLoop;

    // ==========================================
    // 1. TAMBAHKAN VARIABEL PANEL BARU DI SINI
    // ==========================================
    [Header("Pengaturan UI Tambahan")]
    public GameObject panelInfoPenyusun; 

    [Header("Pengaturan Transisi")]
    public Image layarTransisi;
    public float durasiFade = 1.0f;

    [Header("Sambungan ke Game Utama")]
    public GameObject logikaGameUtama;

    [Header("Pengunci Gaze (UX)")]
    public Collider[] daftarTombolMenu;

    private bool sedangLoading = false;

    void Awake()
    {
        if (logikaGameUtama != null) logikaGameUtama.SetActive(false);

        // Pastikan saat awal game, panel Main Menu yang nyala, dan Info mati
        if (panelMainMenu != null) panelMainMenu.SetActive(true);
        if (panelInfoPenyusun != null) panelInfoPenyusun.SetActive(false);

        if (videoMenuLoop != null)
        {
            videoMenuLoop.isLooping = true;
            videoMenuLoop.Play();
        }

        if (layarTransisi != null)
        {
            layarTransisi.gameObject.SetActive(true);
            Color c = layarTransisi.color;
            c.a = 1f; 
            layarTransisi.color = c;
            StartCoroutine(AnimasiFadeInAwal());
        }

        AturInteraksiTombol(true);
    }

    // ... (Fungsi AnimasiFadeInAwal dan ProsesMulaiGame biarkan tetap sama persis) ...

    private IEnumerator AnimasiFadeInAwal()
    {
        float t = 0;
        while (t < durasiFade)
        {
            t += Time.deltaTime;
            Color c = layarTransisi.color;
            c.a = Mathf.Lerp(1, 0, t / durasiFade); 
            layarTransisi.color = c;
            yield return null;
        }
        layarTransisi.gameObject.SetActive(false);
    }

    public void TekanTombolStart()
    {
        if (sedangLoading) return;
        StartCoroutine(ProsesMulaiGame());
    }

    private IEnumerator ProsesMulaiGame()
    {
        sedangLoading = true;
        AturInteraksiTombol(false);

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

        if (videoMenuLoop != null) videoMenuLoop.Stop();
        if (panelMainMenu != null) panelMainMenu.SetActive(false);
        if (panelInfoPenyusun != null) panelInfoPenyusun.SetActive(false); // Pastikan info juga mati saat main
        if (logikaGameUtama != null) logikaGameUtama.SetActive(true);

        if (layarTransisi != null)
        {
            float t = 0;
            while (t < durasiFade)
            {
                t += Time.deltaTime;
                Color c = layarTransisi.color;
                c.a = Mathf.Lerp(1, 0, t / durasiFade); 
                layarTransisi.color = c;
                yield return null;
            }
            layarTransisi.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    private void AturInteraksiTombol(bool status)
    {
        foreach (Collider tombol in daftarTombolMenu)
        {
            if (tombol != null) tombol.enabled = status;
        }
    }

    // ==========================================
    // 2. FUNGSI BARU UNTUK PINDAH UI TANPA FADE
    // ==========================================

    public void BukaInfoPenyusun()
    {
        if (panelMainMenu != null) panelMainMenu.SetActive(false);
        if (panelInfoPenyusun != null) panelInfoPenyusun.SetActive(true);
    }

    public void TutupInfoPenyusunKembaliKeMenu()
    {
        if (panelInfoPenyusun != null) panelInfoPenyusun.SetActive(false);
        if (panelMainMenu != null) panelMainMenu.SetActive(true);
    }
}