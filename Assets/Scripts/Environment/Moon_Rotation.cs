using UnityEngine;
using System.Collections;

public class Moon_Rotation : MonoBehaviour
{
    private float rotSpeed = 30;
    private Transform myTransform;

    void Start()
    {
        myTransform = transform;
    }

    void Update()
    {
        myTransform.Rotate(0, rotSpeed * Time.deltaTime, 0, Space.Self);
    }
}
