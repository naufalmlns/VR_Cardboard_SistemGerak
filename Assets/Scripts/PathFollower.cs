using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [Header("Pengaturan Jalur")]
    [Tooltip("Masukkan titik-titik lintasan secara berurutan ke sini")]
    public Transform[] titikJalur;
    
    [Header("Pengaturan Karakter")]
    public float kecepatanJalan = 3f;
    public float kecepatanBelok = 5f;

    private int indeksTitikSekarang = 0;
    private bool sedangBerjalan = false; // Set ke false jika ingin menunggu trigger

   void Update()
    {
        if (titikJalur.Length == 0 || !sedangBerjalan) return;

        Transform targetTitik = titikJalur[indeksTitikSekarang];

        // 1. Kunci Ketinggian (Sumbu Y)
        // Kita buat kordinat tujuan baru, X dan Z ambil dari jalur, tapi Y tetap pakai Y objek ini
        Vector3 targetPosisi = new Vector3(targetTitik.position.x, transform.position.y, targetTitik.position.z);

        // 2. Arahkan pandangan (Rotasi)
        Vector3 arahTarget = targetPosisi - transform.position;
        arahTarget.y = 0; // Pastikan rotasi tidak menunduk/mendongak

        if (arahTarget != Vector3.zero)
        {
            Quaternion rotasiTarget = Quaternion.LookRotation(arahTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotasiTarget, kecepatanBelok * Time.deltaTime);
        }

        // 3. Gerakkan posisi menuju target yang Y-nya sudah dikunci
        transform.position = Vector3.MoveTowards(transform.position, targetPosisi, kecepatanJalan * Time.deltaTime);

        // 4. Cek jarak (gunakan targetPosisi, bukan targetTitik.position)
        if (Vector3.Distance(transform.position, targetPosisi) < 0.1f)
        {
            indeksTitikSekarang++;

            if (indeksTitikSekarang >= titikJalur.Length)
            {
                indeksTitikSekarang = 0; 
            }
        }
    }
    // Fungsi untuk menyalakan/mematikan jalan dari script lain
    public void SetBerjalan(bool status)
    {
        sedangBerjalan = status;
    }
}