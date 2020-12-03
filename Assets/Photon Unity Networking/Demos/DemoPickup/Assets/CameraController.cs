using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed_h = 2.0f;
    public float speed_v = 2.0f;

    private float yaw;
    private float pitch;

    // Get the mouse inputs and set them to their respective axes
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Update()
    {
        yaw += speed_h * Input.GetAxis("Mouse X");
        pitch -= speed_v * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -45, 45);
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    // Move camera with player
    
}
