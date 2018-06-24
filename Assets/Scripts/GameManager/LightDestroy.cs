using UnityEngine;
using System.Collections;

public class LightDestroy : MonoBehaviour
{
    [SerializeField]
    private float destroyTime;
    private Light myLight;
    private float maxIntensity;

    void Start()
    {
        Destroy(gameObject, destroyTime);
        myLight = GetComponent<Light>();
        maxIntensity = myLight.intensity;
    }
    
    void FixedUpdate()
    {
        myLight.intensity -= (maxIntensity / destroyTime) * Time.deltaTime;
    }
}
