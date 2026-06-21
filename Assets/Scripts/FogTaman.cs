using UnityEngine;

public class FogTaman : MonoBehaviour
{
    void Start()
    {
        // Mematikan kabut secepat kilat di frame pertama saat game dimulai
        RenderSettings.fog = false;
    }
}