using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour
{
    private int score = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            Destroy(ball.gameObject);
            score++;
            Debug.Log($"Score: {score}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
