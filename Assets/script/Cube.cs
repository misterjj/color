using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Destroy(gameObject);
        Camera.main.GetComponent<Game>().win();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Camera.main.GetComponent<Game>().loss();
    }
    
}
