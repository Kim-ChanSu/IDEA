using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{

    public GameObject TitleText;
    public Sprite MasterPieceImage;
    public GameObject TitleImage;
    private int TitleNum = 0;
    private float EffectTime = 0;
    private int MasterpieceTextPoistion = 400; 
    List<Dictionary<string, object>> TalkData; 
    private int CSVNum = 0; 
    
    void Start()
    {
        TalkData = CSVReader.Read(GameManager.instance.CSVFolder + "MasterPiece", "");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        { 
            if(TitleNum <= 0)
            { 
                StartMasterPiece();
            }    
        }
    }

    void StartMasterPiece()
    { 
        TitleNum ++;
        
        StartCoroutine(SettingMasterPiece(1.5f));
    }

    IEnumerator MasterPiece(float SkipTime)
    {   
        if(CSVNum < TalkData.Count)
        { 
            Debug.Log(TalkData[CSVNum]["Content"].ToString());
            TitleText.GetComponent<Text>().text = TalkData[CSVNum]["Content"].ToString();
            float NextTime = float.Parse(TalkData[CSVNum]["Branch"].ToString());
            CSVNum++;
            Debug.Log(NextTime);
            yield return new WaitForSeconds(NextTime);
            StartCoroutine(MasterPiece(NextTime));
        }
        else
        { 
            TitleText.GetComponent<Text>().text = "End";
        }
    }

    IEnumerator SettingMasterPiece(float MoveSpeed)
    { 
        if(TitleText.GetComponent<RectTransform>().anchoredPosition.y < MasterpieceTextPoistion)
        { 
            EffectTime += Time.deltaTime/MoveSpeed;
            TitleText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(new Vector2(0, 0), new Vector2(0,MasterpieceTextPoistion), EffectTime);
            yield return null;
            StartCoroutine(SettingMasterPiece(MoveSpeed)); 
        }
        else
        { 
            EffectTime = 0;            
            Debug.Log(TitleText.GetComponent<RectTransform>().anchoredPosition.y);
            TitleText.GetComponent<Text>().color = new Color(1, 1, 1);
            TitleImage.GetComponent<Image>().sprite = MasterPieceImage; 
            TitleImage.GetComponent<Image>().color = new Color(0.4f,0.4f,0.4f);
            StartCoroutine(MasterPiece(float.Parse(TalkData[CSVNum]["Branch"].ToString())));
        }         
    }
}
