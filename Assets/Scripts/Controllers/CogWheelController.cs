using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogWheelController : MonoBehaviour
{
    private enum SpinMode
    { 
        Original,
        Creak
    }

    [SerializeField]
    private SpinMode spinMode;
    [SerializeField]
    private float spinSpeed = 180.0f;
    [SerializeField]
    private float creakTime = 0.5f;
    [SerializeField]
    private int spinDir = 1;

    private float effectTime;

    private void Start()
    {
        effectTime = creakTime;
    }

    private void Update()
    {
        Spin();
    }


    private void Spin()
    {
        if (spinMode == SpinMode.Original)
        {
            this.gameObject.transform.Rotate(Vector3.forward * Time.deltaTime * spinSpeed * spinDir);
        }
        else
        {
            if (effectTime >= creakTime)
            {
                effectTime = 0.0f;
                this.gameObject.transform.Rotate(Vector3.forward * spinSpeed * spinDir);
            }
            effectTime = effectTime + Time.deltaTime;
        }
    }
}
