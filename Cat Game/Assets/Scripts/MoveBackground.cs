using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    private float spriteLength;
    private float startPosition;
    private GameObject camera;
    private GameObject defaultSprite;
    [SerializeField] private float parallaxAmount;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Virtual Camera");
        defaultSprite = GameObject.Find("trees");
        startPosition = transform.position.x;
        spriteLength = defaultSprite.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (camera.transform.position.x * (1 - parallaxAmount));
        float distance = (camera.transform.position.x * parallaxAmount);

        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        if (temp > startPosition + spriteLength * 0.5)
        {
            startPosition += spriteLength;
        }
        else if (temp < startPosition - spriteLength * 0.5)
        {
            startPosition -= spriteLength;
        }

    }
}
