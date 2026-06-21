using System.Collections;
using UnityEngine;

[System.Serializable]
public class VOItem
{
    [Tooltip("Jeda diam (detik) SEBELUM audio ini mulai diputar")]
    public float jedaSebelumAudio = 0f;

    [Tooltip("File audio VO (.mp3/.wav)")]
    public AudioClip klipAudio;
    
    [Tooltip("Jeda diam (detik) SETELAH audio ini selesai diputar (opsional)")]
    public float jedaSetelahAudio = 0f;
}

public class SequenceVOPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    private Coroutine voCoroutine; 

    public void MulaiPutarVO(VOItem[] daftarVO)
    {
        HentikanVO();
        voCoroutine = StartCoroutine(PlayVOInSequence(daftarVO));
    }

    public void HentikanVO()
    {
        if (voCoroutine != null)
        {
            StopCoroutine(voCoroutine);
        }
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    private IEnumerator PlayVOInSequence(VOItem[] daftarVO)
    {
        foreach (VOItem vo in daftarVO)
        {
            // 1. Tunggu sejenak SEBELUM memutar suara (jika diatur lebih dari 0)
            if (vo.jedaSebelumAudio > 0)
            {
                yield return new WaitForSeconds(vo.jedaSebelumAudio);
            }

            // 2. Putar audio jika filenya ada
            if (vo.klipAudio != null && audioSource != null)
            {
                audioSource.clip = vo.klipAudio;
                audioSource.Play();

                // 3. Tunggu sampai audio ini selesai diputar sepenuhnya
                yield return new WaitForSeconds(vo.klipAudio.length);
            }

            // 4. Beri jeda tambahan SETELAH suara selesai (jika diatur lebih dari 0)
            if (vo.jedaSetelahAudio > 0)
            {
                yield return new WaitForSeconds(vo.jedaSetelahAudio);
            }
        }
    }
}