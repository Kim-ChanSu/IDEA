using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript2 : MonoBehaviour
{

    public GameObject target;
    public RuntimeAnimatorController ani;

    void Start()
    {
        //Invoke("test",1.0f);
        
    }

    void test()
    { 
        Vector3 dir = target.transform.position - this.transform.position;
        
        float RayLength = Mathf.Sqrt(Mathf.Pow(target.transform.position.x - this.transform.position.x, 2) + Mathf.Pow(target.transform.position.y - this.transform.position.y, 2));
        Debug.Log(RayLength);

        RaycastHit2D[] hits; 
        Debug.DrawRay(this.transform.position, dir, Color.blue, 600.0f); //dir * RayLength
        hits = Physics2D.RaycastAll(new Vector2 (this.transform.position.x, this.transform.position.y), new Vector2 (dir.x, dir.y), RayLength);    
        for(int i = 0; i < hits.Length; i++)
        { 
            Debug.Log(hits[i].collider.gameObject);
            if(hits[i].transform.tag == "Tile")
            { 
                hits[i].collider.gameObject.GetComponent<MapBlock>().SetSelectionMode(MapBlock.Highlight.Attackable);
            }
        }    
    }

    public void Testt()
    { 
        Debug.LogWarning("test");    
    }
}
