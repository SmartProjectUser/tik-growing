using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    private float quotient;
    private float delta;
    Vector2 mousePosition;
    public float mass;
    void Start()
    {
        mass = 10;
        delta = 3;
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition -= (Vector2)transform.position;
        quotient = Mathf.Sqrt(mousePosition.x * mousePosition.x + mousePosition.y * mousePosition.y) / delta;
        mousePosition /= quotient;
        transform.Translate(mousePosition * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Food")
        {
            mass++;
            Destroy(collision.gameObject);
        }
    }
}
