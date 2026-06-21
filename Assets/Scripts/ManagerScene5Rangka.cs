using System.Collections;
using UnityEngine;

public class ManagerScene5Rangka : MonoBehaviour
{
    [Header("Audio & VO Utama")]
    public AudioSource audioSource;
    public AudioClip[] daftarVOMateri; 
    public AudioClip efekTulangJatuh; 
    public AudioClip voInstruksi;
    public AudioClip voSelesai;    

    [Header("Grup Rangka")]
    public GameObject rangkaUtuh;
    public GameObject rangkaBerantakan;
    
    [Tooltip("Masukkan ke-4 objek tulang (Kepala, Badan, Tangan, Kaki) yang ada di meja ke sini")]
    public TulangGaze[] daftarTulang;

    [Header("VO Saat Tulang Berhasil Terpasang")]
    [Tooltip("Masukkan VO tulang. URUTAN HARUS SAMA dengan daftarTulang (misal: 0=Kepala, 1=Badan, dst)")]
    public AudioClip[] daftarVOBagianTulang;

    [Header("Transisi ke Scene 6")]
    [Tooltip("Masukkan objek Logika_Scene6 (yang mengatur TV Sendi) ke sini")]
    public GameObject logikaScene6Sendi;

    private int jumlahTulangTersusun = 0;
    private int totalTulang = 4; 

    [Header("Pengaturan Akhir Rangka")]
    [Tooltip("Titik tujuan kerangka bergeser (misal: di pojok meja)")]
    public Transform targetPosisiPojok;
    public float kecepatanGeser = 2f;

    void OnEnable()
    {
        jumlahTulangTersusun = 0;

        foreach (TulangGaze tulang in daftarTulang)
        {
            if (tulang != null) tulang.SimpanPosisiMeja();
        }
        
        if (rangkaUtuh != null) rangkaUtuh.SetActive(true);
        if (rangkaBerantakan != null) rangkaBerantakan.SetActive(false);

        StartCoroutine(SequenceAwalRangka());
    }

    private IEnumerator SequenceAwalRangka()
    {
        foreach (AudioClip vo in daftarVOMateri)
        {
            if (audioSource != null && vo != null)
            {
                audioSource.PlayOneShot(vo);
                yield return new WaitForSeconds(vo.length + 0.2f); 
            }
        }

        yield return new WaitForSeconds(0.5f);

        foreach (TulangGaze tulang in daftarTulang)
        {
            if (tulang != null) tulang.SnapKePosisiUtuh();
        }

        if (rangkaUtuh != null) rangkaUtuh.SetActive(false); 
        if (rangkaBerantakan != null) rangkaBerantakan.SetActive(true); 

        if (audioSource != null && efekTulangJatuh != null)
        {
            audioSource.PlayOneShot(efekTulangJatuh);
        }

        float delayTerbang = 0f;
        foreach (TulangGaze tulang in daftarTulang)
        {
            if (tulang != null) 
            {
                tulang.TerbangKeMeja(delayTerbang);
                delayTerbang += 0.2f; 
            }
        }

        yield return new WaitForSeconds(1.5f); 

        if (audioSource != null && voInstruksi != null)
        {
            audioSource.PlayOneShot(voInstruksi);
        }
    }

    // ==========================================
    // FUNGSI DIPANGGIL SAAT GAZE TULANG SELESAI
    // ==========================================
    public void LaporTulangTerpasang(int indeksTulang)
    {
        float durasiVO = 0f;

        // 1. Putar VO nama tulang saat terpasang
        if (indeksTulang >= 0 && indeksTulang < daftarVOBagianTulang.Length)
        {
            AudioClip klip = daftarVOBagianTulang[indeksTulang];
            if (klip != null && audioSource != null)
            {
                // Hentikan suara instruksi jika masih berjalan agar tidak bertumpuk
                if (audioSource.isPlaying) audioSource.Stop();
                
                audioSource.PlayOneShot(klip);
                durasiVO = klip.length;
            }
        }

        // 2. Tambah hitungan tulang
        jumlahTulangTersusun++;
        
        // 3. Cek apakah semua tulang sudah terpasang
        if (jumlahTulangTersusun >= totalTulang) 
        {
            StartCoroutine(SequenceSelesai(durasiVO));
        }
    }

    private IEnumerator SequenceSelesai(float jedaTungguVOAkhir)
    {
        // Tunggu suara nama tulang terakhir selesai diucapkan, ditambah jeda sedikit
        yield return new WaitForSeconds(jedaTungguVOAkhir + 0.5f); 
        
        if (audioSource != null && voSelesai != null)
        {
            audioSource.PlayOneShot(voSelesai);
            yield return new WaitForSeconds(voSelesai.length);
        }
        
        Debug.Log("<color=green>Rangka berhasil disusun! Menukar dan menggeser ke pojok...</color>");

        if (rangkaBerantakan != null) rangkaBerantakan.SetActive(false);
        
        if (rangkaUtuh != null && targetPosisiPojok != null) 
        {
            rangkaUtuh.SetActive(true);

            float t = 0;
            Vector3 posisiAwal = rangkaUtuh.transform.position;
            
            while (t < 1f)
            {
                t += Time.deltaTime * kecepatanGeser;
                rangkaUtuh.transform.position = Vector3.Lerp(posisiAwal, targetPosisiPojok.position, t);
                yield return null;
            }
        }

        if (logikaScene6Sendi != null)
        {
            Debug.Log("<color=yellow>Rangka sudah minggir. TV Scene 6 Menyala!</color>");
            logikaScene6Sendi.SetActive(true);
        }
    }
}