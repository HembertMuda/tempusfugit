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

    private float moveHorizontal;
    private float moveVertical;

    private float mousePanX;
    private float mousePanY;

    private float camRotXInit;
    private float currentcamRotX;

    private void Start()
    {
        camRotXInit = camTransform.localEulerAngles.x;
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

        //Debug.Log($"moveHorizontal = {moveHorizontal} / moveVertical = {moveVertical}");
    }
}
