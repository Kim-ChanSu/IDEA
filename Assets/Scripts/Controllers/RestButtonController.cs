using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestButtonController : MonoBehaviour
{
    [SerializeField]
    private int restCost = 0;
    [SerializeField]
    private int restFill = 25;

    public void Rest()
    {
        if (GameManager.instance.homeManager != null)
        {
            GameManager.instance.homeManager.Rest(restFill, restCost);
        }
    }

    public int GetCost()
    {
        return restCost;
    }
}
