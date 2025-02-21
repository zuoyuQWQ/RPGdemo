using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parr : MonoBehaviour
{
    private GameObject cam;
    [SerializeField] private float parallaxEffect;

    private float startXPosition;
    private float startCamXPosition;
    private float length;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        // 记录初始位置
        startXPosition = transform.position.x;
        startCamXPosition = cam.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceMoved = cam.transform.position.x * (1- parallaxEffect);

        // 摄像机移动的距离
        float cameraDeltaX = cam.transform.position.x - startCamXPosition;

        // 对象的新位置
        float newXPosition = startXPosition + cameraDeltaX * parallaxEffect;

        // 更新对象位置
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);

        if(distanceMoved > startXPosition + length)
        {
            startXPosition += length;
        }
        else if(distanceMoved < startXPosition - length)
        {
            startXPosition -= length;
        }
    }
}
