using UnityEngine;

public class TransisiDunia : MonoBehaviour
{
    [Header("Environment")]
    public GameObject envTaman;
    public GameObject envLorong;

    public void PindahKeLorongSaraf()
    {
        // 1. Ganti Environment secara instan
        envTaman.SetActive(false);
        envLorong.SetActive(true);

        // 2. Nyalakan Fog (Kabut) untuk menyembunyikan ujung lorong
        RenderSettings.fog = true;
        
        // (Opsional) Jika kamu mau memastikan warna kabutnya merah lewat script
        // RenderSettings.fogColor = new Color(0.5f, 0f, 0f); 
    }
}