using UnityEngine;

public class PenjagaLayarVR : MonoBehaviour
{
    void Start()
    {
        // Perintah sakti untuk mencegah layar HP redup atau mati
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        Debug.Log("<color=cyan>Penjaga Layar Aktif: HP tidak akan tertidur!</color>");
    }
}