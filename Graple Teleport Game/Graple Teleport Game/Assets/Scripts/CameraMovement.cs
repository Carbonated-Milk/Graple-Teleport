using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject player;
    public float speedSizeBoost;

    private Rigidbody2D playerRb;
    private Camera cam;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position - new Vector3(0, 0, 10), .08f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 1 + (speedSizeBoost * player.GetComponent<Rigidbody2D>().velocity.magnitude), .02f);
    }
}
