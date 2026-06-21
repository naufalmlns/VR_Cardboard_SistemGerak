using UnityEngine;
using UnityEngine.Video;

// Struct ini agar kita bisa membuat list berpasangan (Waktu & Suara) di Inspector
[System.Serializable]
public class VOSequence
{
    [Tooltip("Di detik ke berapa suara ini akan diputar?")]
    public float waktuMulai; 
    
    [Tooltip("Masukkan file audio (mp3/wav) untuk kalimat ini")]
    public AudioClip klipSuara;
    
    // Penanda agar suara tidak diputar berulang-ulang setiap frame
    [HideInInspector] 
    public bool sudahDiputar = false; 
}

public class VOManager : MonoBehaviour
{
    [Header("Komponen")]
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;

    [Header("Daftar Voice Over")]
    [Tooltip("Tambahkan jumlah VO dan atur detiknya di sini")]
    public VOSequence[] daftarVO;

    void Update()
    {
        // Hanya cek waktu jika video sedang berjalan
        if (videoPlayer.isPlaying)
        {
            // Ambil waktu saat ini dari video (dalam hitungan detik)
            float waktuSekarang = (float)videoPlayer.time;

            // Cek setiap VO di dalam daftar kita
            for (int i = 0; i < daftarVO.Length; i++)
            {
                // Jika waktu video sudah melewati waktu mulai VO, DAN VO belum diputar
                if (waktuSekarang >= daftarVO[i].waktuMulai && !daftarVO[i].sudahDiputar)
                {
                    // Putar suaranya
                    if (daftarVO[i].klipSuara != null)
                    {
                        audioSource.PlayOneShot(daftarVO[i].klipSuara);
                    }
                    
                    // Tandai bahwa VO ini sudah diputar agar tidak bocor
                    daftarVO[i].sudahDiputar = true;
                    
                    Debug.Log("Memutar VO ke-" + (i+1) + " pada detik: " + waktuSekarang);
                }
            }
        }
    }

    // Fungsi tambahan jika kamu ingin mereset state saat video diulang
    public void ResetVO()
    {
        for (int i = 0; i < daftarVO.Length; i++)
        {
            daftarVO[i].sudahDiputar = false;
        }
    }
}