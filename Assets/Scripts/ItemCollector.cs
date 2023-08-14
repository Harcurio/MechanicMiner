using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    public bool collected = false;

    private PlayerMovement player;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("tokenItem"))
        {
            Destroy(collision.gameObject);
            collected = true;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {

            //collision.gameObject.;
            //Debug.Log(collision.);
            //Destroy(collision.gameObject.);
            Destroy(collision.gameObject);
            //overlapPoint
        }
    }
}
