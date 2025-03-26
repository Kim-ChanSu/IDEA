using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSensor : MonoBehaviour
{
    private float curTime;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ball(Clone)")
        {
            curTime = 0f;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ball(Clone)")
        {
            curTime = curTime + Time.deltaTime;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ball(Clone)")
        {
            Debug.Log(curTime);
        }
    }
}
