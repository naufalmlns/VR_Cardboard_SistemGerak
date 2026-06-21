using UnityEngine;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;

    void Awake()
    {
        // Mencegah BGM menjadi ganda/dobel saat Scene di-reload
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // BGM akan kebal dari pembersihan Scene
        }
        else
        {
            Destroy(gameObject);
        }
    }
}