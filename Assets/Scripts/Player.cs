﻿using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private float mouseSpeed = 1f;

    [SerializeField]
    private Vector2 clampCamRotX = new Vector2(-40f, 40f);

    [SerializeField]
    private Rigidbody playerRigidbody = null;

    [SerializeField]
    private Transform playerTransform = null;

    [SerializeField]
    private Transform camTransform = null;

    [SerializeField]
    private LayerMask interactionLayerMask;

    [SerializeField]
    private float checkGroundDistance = 0.1f;

    [SerializeField]
    private float checkInteractionDistance = 1f;

    private float moveHorizontal;
    private float moveVertical;

    private float mousePanX;
    private float mousePanY;

    private float camRotXInit;
    private float currentcamRotX;

    private int interactionLayer;

    public Action<int> onInteractionLayerChanged;

    private void Start()
    {
        camRotXInit = camTransform.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection = new Vector3(moveDirection.x * moveSpeed, playerRigidbody.velocity.y, moveDirection.z * moveSpeed);
            moveDirection = transform.TransformDirection(moveDirection);

            playerRigidbody.MovePosition(playerRigidbody.position + moveDirection);
        }
    }

    private void Update()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        mousePanX = Input.GetAxis("Mouse X");
        mousePanY = Input.GetAxis("Mouse Y");

        currentcamRotX -= mousePanY * mouseSpeed * Time.deltaTime;

        playerTransform.Rotate(Vector3.up, mousePanX * mouseSpeed * Time.deltaTime);
        camTransform.localEulerAngles = new Vector3(Mathf.Clamp(camRotXInit + currentcamRotX, clampCamRotX.x, clampCamRotX.y), camTransform.localEulerAngles.y, camTransform.localEulerAngles.z);

        Ray checkGroundRay = new Ray(transform.position - Vector3.up * 0.02f, -Vector3.up);

        if (Physics.Raycast(checkGroundRay, out RaycastHit raycastHitGround, checkGroundDistance))
        {
            playerRigidbody.useGravity = false;
        }

        Ray interactionRay = new Ray(camTransform.position, camTransform.forward);

        bool raycastInteraction = Physics.Raycast(interactionRay, out RaycastHit raycastHitInteraction, checkInteractionDistance, interactionLayerMask);

        if (raycastInteraction)
        {
            if (raycastHitInteraction.collider != null && raycastHitInteraction.collider.gameObject.layer != interactionLayer)
            {
                interactionLayer = raycastHitInteraction.collider.gameObject.layer;
                if (onInteractionLayerChanged != null)
                {
                    onInteractionLayerChanged(interactionLayer);
                }
            }
        }
        else if (interactionLayer != 0)
        {
            interactionLayer = 0;
            if (onInteractionLayerChanged != null)
            {
                onInteractionLayerChanged(interactionLayer);
            }
        }

        if (Input.GetMouseButtonDown(0) && raycastInteraction)
        {
            Door doorRaycast = raycastHitInteraction.collider.GetComponent<Door>();
            if (doorRaycast != null)
            {
                if (!doorRaycast.isOpened)
                    doorRaycast.Open();
                else
                    doorRaycast.Close();
            }
        }
    }
}
