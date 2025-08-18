using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerMovementIsometric : MonoBehaviour
{
   
    public float moveSpeed = 5f;
    
    public Rigidbody2D rb;
    
    Vector2 movement;


    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
        
    }
}
