using System.Collections;
using UnityEngine;

public class TulangGaze : MonoBehaviour
{
    [Header("Identitas Tulang")]
    [Tooltip("Isi dengan angka: 0 untuk Kepala, 1 untuk Badan, 2 untuk Tangan, 3 untuk Kaki")]
    public int indeksTulang; // <-- TAMBAHAN BARU

    [Header("Pengaturan Target")]
    public Transform targetPosisi; 
    public float kecepatanTerbang = 3f;

    [Header("Pengaturan Gaze")]
    [Tooltip("Berapa lama (detik) ditatap sampai terbang?")]
    public float durasiMenatap = 1.5f;

    [Header("Manager")]
    public ManagerScene5Rangka managerRangka;

    private Vector3 posisiMeja;
    private Quaternion rotasiMeja;
    private bool sedangTerbang = false;
    private Collider col;

    public void SimpanPosisiMeja()
    {
        posisiMeja = transform.position;
        rotasiMeja = transform.rotation;
        col = GetComponent<Collider>();
    }

    public void SnapKePosisiUtuh()
    {
        transform.position = targetPosisi.position;
        transform.rotation = targetPosisi.rotation;
    }

    public void TerbangKeMeja(float delay)
    {
        StartCoroutine(ProsesTerbangKeMeja(delay));
    }

    private IEnumerator ProsesTerbangKeMeja(float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        if (col != null) col.enabled = false;

        float t = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * kecepatanTerbang;
            transform.position = Vector3.Lerp(startPos, posisiMeja, t);
            transform.rotation = Quaternion.Lerp(startRot, rotasiMeja, t);
            yield return null;
        }

        if (col != null) col.enabled = true;
    }

    public bool ApakahSedangTerbang()
    {
        return sedangTerbang;
    }

    public void PicuTerbang()
    {
        if (sedangTerbang) return;
        Debug.Log("<color=green>GAZE PENUH! " + gameObject.name + " Terbang!</color>");
        StartCoroutine(TerbangKeTarget());
    }

    private IEnumerator TerbangKeTarget()
    {
        sedangTerbang = true;
        if (col != null) col.enabled = false;

        float t = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * kecepatanTerbang;
            transform.position = Vector3.Lerp(startPos, targetPosisi.position, t);
            transform.rotation = Quaternion.Lerp(startRot, targetPosisi.rotation, t);
            yield return null;
        }

        transform.position = targetPosisi.position;
        transform.rotation = targetPosisi.rotation;

        // --- PEMANGGILAN YANG SUDAH DIPERBARUI ---
        if (managerRangka != null) managerRangka.LaporTulangTerpasang(indeksTulang);
    }
}