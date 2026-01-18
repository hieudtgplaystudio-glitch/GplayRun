using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDestroyer : MonoBehaviour
{
    public GameObject destructionPoint;
    public Animator myAnimator;
    public Sprite[] random;
    public SpriteRenderer avata;
    // Start is called before the first frame update
    void Start()
    {
        int index = Random.Range(0, random.Length); // 0,1,2
        if (avata != null)
        {
            avata.sprite = random[index];

        }
        destructionPoint = GameObject.Find("PlatformDestructionPoint");
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < destructionPoint.transform.position.x )
        {

            //Destroy(gameObject);
           

            gameObject.SetActive(false);
        }
    }
}
