using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private LayerMask interactionLayerMask = 0;

    [SerializeField]
    private float checkGroundDistance = 0.1f;

    [SerializeField]
    private float checkInteractionDistance = 1f;

    [SerializeField]
    public List<string> playerMemoriesNames = new List<string>();

    [SerializeField]
    private List<AudioClip> footstepClips = new List<AudioClip>();

    private float moveHorizontal;
    private float moveVertical;

    private float mousePanX;
    private float mousePanY;

    private float camRotXInit;
    private float currentcamRotX;

    private int interactionLayer;
    private Vector3 camLocalPosInit;

    private AudioSource audioSource;

    private Coroutine footstepCor;

    private UIManager uIManager;

    public Action<int> onInteractionLayerChanged;

    private void Start()
    {
        camRotXInit = camTransform.localEulerAngles.x;
        camLocalPosInit = camTransform.localPosition;
        GameManager.Instance.onGameStateChanged += OnGameStateChanged;
        audioSource = GetComponent<AudioSource>();
        uIManager = FindObjectOfType<UIManager>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
        if (moveDirection.magnitude > 0.8f)
        {
            if (footstepCor == null)
                footstepCor = StartCoroutine(PlayFootstep());
        }
        else
        {
            if (footstepCor != null)
            {
                StopCoroutine(footstepCor);
                footstepCor = null;
            }
        }

        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Walking)
        {
            if (moveDirection.magnitude > 0.1f)
            {
                moveDirection = new Vector3(moveDirection.x * moveSpeed, playerRigidbody.velocity.y, moveDirection.z * moveSpeed);
                moveDirection = transform.TransformDirection(moveDirection);

                playerRigidbody.MovePosition(playerRigidbody.position + moveDirection);
            }
        }

        if (moveDirection.magnitude <= 0.1f)
        {
            playerRigidbody.velocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Walking)
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
                playerRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }

            //if (GameManager.Instance.CurrentGameState == GameManager.GameState.Walking)
            //{
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
                onInteractionLayerChanged?.Invoke(interactionLayer);
            }

            if (Input.GetMouseButtonDown(0) && raycastInteraction)
            {
                Door doorRaycast = raycastHitInteraction.collider.GetComponent<Door>();
                if (doorRaycast != null)
                {
                    uIManager.HideTuto();

                    if (!doorRaycast.isOpened)
                        doorRaycast.Open();
                    else
                        doorRaycast.Close();
                }

                TalkableCharacter talkableCharacter = raycastHitInteraction.collider.GetComponentInParent<TalkableCharacter>();
                if (talkableCharacter != null)
                {
                    TalkToCharacter(talkableCharacter);
                }
            }
        }
    }

    public void TalkToCharacter(TalkableCharacter characterToTalk)
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Talking);
        AdaptCamToTalkableCharacter(characterToTalk);
        uIManager.CurrentTalkableCharacter = characterToTalk;
        characterToTalk.LetsTalk();
        playerRigidbody.velocity = Vector3.zero;
        moveHorizontal = 0f;
        moveVertical = 0f;
    }

    public void AdaptCamToTalkableCharacter(TalkableCharacter talkableCharacter)
    {
        camTransform.DOMove(talkableCharacter.camFramingPoint.position, 1f).SetEase(Ease.OutCubic);
        camTransform.DORotate(talkableCharacter.camFramingPoint.eulerAngles, 1f).SetEase(Ease.OutCubic);
    }

    IEnumerator PlayFootstep()
    {
        while (true)
        {
            audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Count)]);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnGameStateChanged(GameManager.GameState newGameState)
    {
        if (newGameState == GameManager.GameState.Walking)
        {
            camTransform.localPosition = camLocalPosInit;
            camTransform.localEulerAngles = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TriggerForceTalk triggerForceTalk = other.GetComponentInParent<TriggerForceTalk>();
        if (triggerForceTalk != null)
        {
            TalkToCharacter(triggerForceTalk.forceTalkableCharacter);
            triggerForceTalk.Disable();
        }

        StartMusicTrigger startMusicTrigger = other.GetComponentInParent<StartMusicTrigger>();
        if (startMusicTrigger != null)
        {
            startMusicTrigger.StartMusic();
        }
    }
}
