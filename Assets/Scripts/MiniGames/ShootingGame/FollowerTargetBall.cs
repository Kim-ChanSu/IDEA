using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerTargetBall : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject = null;

    private void Awake()
    {
        FollwerTargetBallInit();
    }

    void Update()
    {
        FollowerTarget();
    }

    public void FollwerTargetBallInit()
    {
        this.targetObject = GameObject.FindGameObjectWithTag("Character");
    }

    public void FollowerTarget()
    {
        this.targetObject = GameObject.FindGameObjectWithTag("Character");
        this.GetComponent<Rigidbody2D>().WakeUp();

        if ((this.targetObject == null)||(this.targetObject.activeSelf == false))
        {
            this.GetComponent<Rigidbody2D>().velocity = Vector2.right * 20f;
            return;
        }

        if ((this.targetObject != null) && (this.targetObject.activeSelf == true))
        {
            Vector2 temp = (this.targetObject.transform.position - this.transform.position).normalized;
            this.GetComponent<Rigidbody2D>().velocity = temp * 20f;

        }
        else
        {
            this.GetComponent<Rigidbody2D>().velocity = Vector2.right * 20f;
        }

    }

}
