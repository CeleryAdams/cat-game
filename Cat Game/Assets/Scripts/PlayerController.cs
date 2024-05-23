using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private bool isOnGround = true;

    [SerializeField] private Vector2 jumpForce;

    [SerializeField] private float walkDistance = 2.0f;
    [SerializeField] private float baseDuration = 2.5f;
    [SerializeField] private float minDuration = 0.01f;
    private bool isWalking = false;
    private float lastKeyPressTime;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && isOnGround && !isWalking)
        {
            //increase damping when jumping
            transposer.m_XDamping = 0.5f;
            //increase dead zone when jumping
            transposer.m_DeadZoneWidth = 0.8f;
            playerRb.AddForce(jumpForce, ForceMode2D.Impulse);
            isOnGround = false;
        }

        //call coroutine to move cat forward for a fixed distance over a fixed duration per keypress
        if (Input.GetKeyDown(KeyCode.F) && isOnGround) 
        {
            transposer.m_DeadZoneWidth = 0.29f;
            float timeSinceLastPress = Time.time - lastKeyPressTime;
            lastKeyPressTime = Time.time;

            StopAllCoroutines();
            StartCoroutine(MoveCatForward(CalculateMoveDuration(timeSinceLastPress)));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }

    IEnumerator MoveCatForward(float walkDuration)
    {
        isWalking = true;
        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition + Vector2.right * walkDistance;
        float elapsedTime = 0f;

        while (elapsedTime < walkDuration) 
        {
            //move cat from start position to end position over walk duration
            transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / walkDuration);

            //lerp virtual camera damping factor back to zero to avoid camera jolt when changing damping values
            if (transposer.m_XDamping > 0.0f) 
            {
                transposer.m_XDamping = Mathf.Lerp(0.5f, 0.0f, elapsedTime / walkDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transposer.m_XDamping = 0.0f;
        isWalking = false;
    }

    //reduce move duration if keypresses are closer together, so that the cat goes faster when presses are faster
    float CalculateMoveDuration(float timeSinceLastPress)
    {
        float moveDuration = baseDuration;

        if (timeSinceLastPress < baseDuration)
        {
            moveDuration = Mathf.Lerp(minDuration, baseDuration, timeSinceLastPress / baseDuration);
        }

        return moveDuration;
    }
}
