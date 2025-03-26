using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkAutoPlayButtonController : MonoBehaviour
{
    private bool isAutoSkip = false;
    [SerializeField]
    private GameObject spinObject;
    private float spinSpeed = 300.0f;
    private int spinDir = -1;

    private void Update()
    {
        Spin();
    }

    private void Spin()
    {
        if ((spinObject != null) && (isAutoSkip == true))
        {
            spinObject.GetComponent<RectTransform>().Rotate(Vector3.forward * Time.deltaTime * spinSpeed * spinDir);
        }
    }

    public void ChangeAutoSkipMode()
    {
        bool mode = false;
        if (isAutoSkip == false)
        {
            mode = true;
        }

        GameManager.instance.talkManager.SetTalkAutoPlay(mode);
        isAutoSkip = mode;
    }
}
