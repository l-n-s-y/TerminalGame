using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutworldPlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    private float moveSpeed = 5f;
    private float strafeSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forwardVelocity = transform.forward * (Input.GetAxis("Vertical") * moveSpeed);
        Vector3 strafeVelocity = transform.right * (Input.GetAxis("Horizontal") * strafeSpeed);
        if (forwardVelocity.magnitude != 0) {
            animator.SetBool("walking",true);
        } else {
            animator.SetBool("walking",false);
        }

        rb.velocity = forwardVelocity + strafeVelocity;
    }
}
