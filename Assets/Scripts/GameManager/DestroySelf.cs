using UnityEngine;
using System.Collections;

public class DestroySelf : MonoBehaviour
{
    [SerializeField]
    private float destroyTime;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

}
