using UnityEngine;
using System.Collections;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform target;
    public float sTime = 0.3f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 tPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, tPosition, ref velocity, sTime);
        }
    }
}
