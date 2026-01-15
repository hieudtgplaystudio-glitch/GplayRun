using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject thePlatform;
    public Transform generationPoint;
    public float distanceBetween;

    private float platformWidth;

    public float distanceBetweenMin;
    public float distanceBetweenMax;

    private int PlatformSelector;
    private float[] platformWidths;

    public ObjectPooler[] objPools;

    private float minHeight;
    public Transform minHeightPoint;
    public Transform maxHeightPoint;
    private float maxHeight;
    public float maxHeightChange;
    private float heightChange;

    public StarGenerator theStarGenerator;
    public float randomStarThreshold;

    // --- PHẦN GAI NHỌN (CŨ) ---
    public float randomSpikeThreshold;
    public ObjectPooler spikePool;

    // --- [MỚI] PHẦN BẪY TRỪ TIỀN ---
    public float randomMoneyTrapThreshold; // Tỉ lệ xuất hiện bẫy (0-100)
    public ObjectPooler moneyTrapPool;     // Bể chứa bẫy
    // -------------------------------

    public float powerUpHeight;

    void Start()
    {
        platformWidths = new float[objPools.Length];

        for (int i = 0; i < objPools.Length; i++)
        {
            platformWidths[i] = objPools[i].pooledObject.GetComponent<BoxCollider2D>().size.x;
        }

        minHeight = minHeightPoint.position.y;
        maxHeight = maxHeightPoint.position.y;

        theStarGenerator = FindObjectOfType<StarGenerator>();
    }

    void Update()
    {
        if (transform.position.x < generationPoint.position.x)
        {
            if (Random.Range(0f, 100f) < 30f) 
{
                distanceBetween = 0f; // Dính chặt vào nhau
            }
            else 
            {
                distanceBetween = Random.Range(distanceBetweenMin, distanceBetweenMax); 
            }

            PlatformSelector = Random.Range(0, objPools.Length);

            heightChange = transform.position.y + Random.Range(-maxHeightChange, maxHeightChange);

            if (heightChange > maxHeight)
            {
                heightChange = maxHeight;
            }
            else if (heightChange < minHeight)
            {
                heightChange = minHeight;
            }

            transform.position = new Vector3(transform.position.x + (platformWidths[PlatformSelector] / 2) + distanceBetween, heightChange, transform.position.z);

            // Sinh ra Platform từ Pool
            GameObject newPlatform = objPools[PlatformSelector].getPooledObject();
            newPlatform.transform.position = transform.position;
            newPlatform.transform.rotation = transform.rotation;
            newPlatform.SetActive(true);

            // 1. Sinh Sao
            if (Random.Range(0f, 100f) < randomStarThreshold)
            {
                theStarGenerator.spawnStars(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));
            }
            else if (Random.Range(0f, 100f) < randomSpikeThreshold)
            {
                float spikePositionX = Random.Range(-platformWidths[PlatformSelector] / 2 + 1f, platformWidths[PlatformSelector] / 2 - 1f);
                Vector3 spikePosition = new Vector3(spikePositionX, 0.5f, 0f);
                
                GameObject newSpike = spikePool.getPooledObject();
                newSpike.transform.position = transform.position + spikePosition;
                newSpike.transform.rotation = transform.rotation;
                newSpike.SetActive(true);
            }
            else if (Random.Range(0f, 100f) < randomMoneyTrapThreshold)
            {
                // Tính toán vị trí ngẫu nhiên trên mặt đất (giống hệt Gai)
                float trapX = Random.Range(-platformWidths[PlatformSelector] / 2 + 1f, platformWidths[PlatformSelector] / 2 - 1f);
                
                // Chỉnh độ cao Y (0.5f) cho phù hợp với Sprite của bẫy, bạn có thể sửa số này nếu bẫy bị chìm hoặc bay lơ lửng
                Vector3 trapPosition = new Vector3(trapX, 0.5f, 0f); 

                GameObject newTrap = moneyTrapPool.getPooledObject();
                newTrap.transform.position = transform.position + trapPosition;
                newTrap.transform.rotation = transform.rotation;
                newTrap.SetActive(true);
            }
            // -----------------------------------------------

            transform.position = new Vector3(transform.position.x + (platformWidths[PlatformSelector] / 2), transform.position.y, transform.position.z);
        }
    }
}