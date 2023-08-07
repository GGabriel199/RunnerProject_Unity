using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private float y;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void Start()
    {
        offset = transform.position;
    }
    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        RaycastHit hit;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        if(Physics.Raycast(target.position, Vector3.down, out hit, 2.5f))
            y = Mathf.Lerp(y, hit.point.y, Time.deltaTime * smoothSpeed);
        else
            y = Mathf.Lerp(y, target.position.y, Time.deltaTime * smoothSpeed);
            desiredPosition.y = offset.y + y;
            transform.position = desiredPosition;        
    }
}
