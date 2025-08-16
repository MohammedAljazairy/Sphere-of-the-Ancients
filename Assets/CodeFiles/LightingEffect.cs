using System.Collections;
using UnityEngine;
//https://discussions.unity.com/t/how-to-make-light-fade-in-3-seconds-after-scene-begins/196806
public class LightingEffect : MonoBehaviour
{
    public Light PLight;        
    public float speed = 2f;      
    public float maxIntensity = 8f;

    private float timer = 0f;

    void Update()
    {
            timer += Time.deltaTime * speed;
            PLight.intensity = (Mathf.Sin(timer) + 1) / 2 * maxIntensity;
        
    }
}
