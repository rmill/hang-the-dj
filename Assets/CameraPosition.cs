using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public Vector3 cameraOffset;
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3 (player.position.x + cameraOffset.x, player.position.y + cameraOffset.y, cameraOffset.z);
    }
}
