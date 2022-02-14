using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadbob : MonoBehaviour
{
    [Header("Headbob Settings")]
    [SerializeField] PlayerController playerController;

    [SerializeField] float walkingBobbingSpeed = 14f;
    [SerializeField] float sprintingBobbingSpeed = 16f;
    [SerializeField] float walkBobbingAmount = 0.05f;
    [SerializeField] float sprintBobbingAmount = 0.05f;

    [SerializeField] Rigidbody rb;

    [Header("Player GroundCheck")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.1f;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded = true;

    float defaultPosY = 0;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer, QueryTriggerInteraction.Ignore);

        if (Mathf.Abs(playerController.curMovementInput.x) > 0.1f || Mathf.Abs(playerController.curMovementInput.y) > 0.1f && isGrounded && !playerController.isSprinting)
        {
            timer += Time.deltaTime * walkingBobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * walkBobbingAmount, transform.localPosition.z);
        }
        else if (Mathf.Abs(playerController.curMovementInput.x) > 0.1f || Mathf.Abs(playerController.curMovementInput.y) > 0.1f && isGrounded && playerController.isSprinting)
        {
            timer += Time.deltaTime * sprintingBobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * sprintBobbingAmount, transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed), transform.localPosition.z);
        }
    }
}
