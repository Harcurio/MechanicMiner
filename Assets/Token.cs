 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    // Start is called before the first frame update
    public float OriginalX;
    public float OriginalY;
    public CircleCollider2D circle;

    
    void Start()
    {
        circle = GetComponent<CircleCollider2D>();
        OriginalX = circle.transform.position.x;
        OriginalY = circle.transform.position.y;
    }



    // Update is called once per frame
   
}
