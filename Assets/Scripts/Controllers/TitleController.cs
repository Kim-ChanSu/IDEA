/* 작성날짜: 2023-09-11
 * 버전: 0.0.2ver 
 * 내용: 타이틀화면 제어
 * 최근 수정 날짜: 2023-09-14
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleController : MonoBehaviour
{
    List<Dictionary<string, object>> masterPieceTalkData; 

    private int titleClickCount = 0;
    private float effectTime = 0;
    private int CSVNum = 0; 
    //private float masterPieceBGMStartTime = 38.5f;
    private float titleTextEffectTime = 0;
    private bool titleTextEffectMode = false;

    public GameObject titleText;
    public GameObject titleImage;
    public GameObject titleButtons;
    [SerializeField]
    private GameObject gameLogo;
    [SerializeField]
    private GameObject teamLogo;

    public GameObject titleBulrEffect;
    public Material M_Bulr;

    [SerializeField]
    private GameObject[] titleParticle;

    public int masterpieceTextPoistion = 400; 
    public float fadeOutTime;
    public float fadeInTime;
    public float fadeDelayTime;
    public float openingColor = 0.4f;
    public float titleTextEffectCycle;
    
    void Start()
    {
        /*
        masterPieceTalkData = CSVReader.Read(GameManager.instance.CSVFolder + "MasterPiece", "");
        for(int i=0;i<titleParticle.Length;i++)
        { 
            titleParticle[i].SetActive(false);
        }
        
        titleBulrEffect.SetActive(false);
        */
        //OpeningEnd();
        SetNewOpeningStart();
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        KeyCheck();
        #region
        if(titleClickCount == 0)
        { 
            TitleTextEffect();
        }
        #endregion
        */
        NewKeyCheck();
    }
    private void SetNewOpeningStart()
    {
        #region
        titleImage.GetComponent<Image>().color = new Color(0, 0, 0);
        GameManager.instance.StopBGM();
        for(int i = 0; i < titleParticle.Length; i++)
        { 
            titleParticle[i].SetActive(false);
        }
        
        for (int i = 0; i < titleButtons.transform.childCount; i++)
        {
            titleButtons.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = false;
            titleButtons.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
        }
        teamLogo.SetActive(true);
        teamLogo.GetComponent<Image>().color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
        gameLogo.GetComponent<Image>().color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
        StartCoroutine(NewOpening());
        #endregion
    }

    IEnumerator NewOpening()
    {
        #region
        effectTime = 0.0f;
        float teamLogoFadeInTime = 1.0f;
        while (effectTime <= 1)
        {
            effectTime = effectTime + Time.deltaTime/teamLogoFadeInTime;
            float alphaColor = Mathf.Lerp(0.0f, 1.0f, effectTime);
            teamLogo.GetComponent<Image>().color = new Color(alphaColor, alphaColor, alphaColor, alphaColor);
            yield return null;
        }

        float teamLogoFadeDelayTime = 0.75f;
        yield return new WaitForSeconds(teamLogoFadeDelayTime);

        effectTime = 0.0f;
        float teamLogoFadeOutTime = 1.0f;
        while (effectTime <= 1)
        {
            effectTime = effectTime + Time.deltaTime/teamLogoFadeOutTime;
            float alphaColor = Mathf.Lerp(1.0f, 0.0f, effectTime);
            teamLogo.GetComponent<Image>().color = new Color(alphaColor, alphaColor, alphaColor, alphaColor);
            yield return null;
        }    
        
        float backgroundFadeDelayTime = 0.5f;
        yield return new WaitForSeconds(backgroundFadeDelayTime);

        effectTime = 0.0f;
        float backgroundFadeInTime = 1.0f;
        while (effectTime <= 1)
        {
            effectTime = effectTime + Time.deltaTime/backgroundFadeInTime;
            float alphaColor = Mathf.Lerp(0.0f, 1.0f, effectTime);
            titleImage.GetComponent<Image>().color = new Color(alphaColor, alphaColor, alphaColor);
            yield return null;
        }    

        yield return new WaitForSeconds(0.3f);

        effectTime = 0.0f;
        float gameLogoFadeInTime = 0.5f;
        while (effectTime <= 1)
        {
            effectTime = effectTime + Time.deltaTime/gameLogoFadeInTime;
            float alphaColor = Mathf.Lerp(0.0f, 1.0f, effectTime);
            gameLogo.GetComponent<Image>().color = new Color (1.0f, 1.0f, 1.0f, alphaColor);
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        effectTime = 0.0f;
        float buttonFadeInTime = 0.4f;
        float delayTime = 0.2f;

        for (int i = 0; i < titleButtons.transform.childCount; i++)
        {
            effectTime = 0.0f;
            while (effectTime <= 1)
            {
                effectTime = effectTime + Time.deltaTime/buttonFadeInTime;
                float alphaColor = Mathf.Lerp(0.0f, 1.0f, effectTime);
                titleButtons.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color (1.0f, 1.0f, 1.0f, alphaColor);
                yield return null;
            }

            yield return new WaitForSeconds(delayTime);
        }

        OpeningEnd();
        #endregion
    }

    private void OpeningEnd()
    {
        #region
        teamLogo.SetActive(false);

        titleImage.GetComponent<Image>().sprite = BackgroundDatabaseManager.instance.GetBackgroundByName("Title");
        titleImage.GetComponent<Image>().color = new Color(1, 1, 1);

        GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("Title"));        
        GameManager.instance.StartBGM();
        GameManager.instance.ChangePlayBGMTime(2.0f);

        for (int i = 0; i < titleParticle.Length; i++)
        { 
            titleParticle[i].SetActive(true);
        }

        for (int i = 0; i < titleButtons.transform.childCount; i++)
        {
            titleButtons.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = true;
            titleButtons.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
        }
        //옵션기능 미구현으로 예외처리
        titleButtons.transform.GetChild(2).gameObject.GetComponent<Button>().interactable = false;
        titleButtons.transform.GetChild(2).gameObject.GetComponent<Image>().color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
        //
        gameLogo.GetComponent<Image>().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

        GameManager.instance.isOpeningEnd = true;
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(titleButtons.transform.GetChild(0).gameObject, new BaseEventData(eventSystem));
        #endregion
    }

    private void NewKeyCheck()
    {
        if((Input.anyKeyDown) && (GameManager.instance != null) && (GameManager.instance.isOpeningEnd == false))
        { 
            StopAllCoroutines();
            OpeningEnd();
        }
    }

    //구 코드
    void StartOpening()
    { 
        #region
        titleClickCount ++;
        /*
        titleText.GetComponent<Text>().text = "Team : Colorful";
        StartCoroutine(SettingOpeningFadeOut(fadeOutTime));
        */
        titleBulrEffect.SetActive(true);
        titleText.SetActive(false);
        StartCoroutine(SettingOpeningFadeIn(fadeInTime));
        #endregion
    }

    IEnumerator SettingOpeningFadeOut(float moveSpeed)
    { 
        #region
        if(titleText.GetComponent<RectTransform>().anchoredPosition.y < masterpieceTextPoistion)
        { 
            effectTime += Time.deltaTime/moveSpeed;
            titleText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(new Vector2(0, 0), new Vector2(0,masterpieceTextPoistion), effectTime);
            float titleImageColor = Mathf.Lerp(1.0f, 0.0f, effectTime);
            titleImage.GetComponent<Image>().color = new Color(titleImageColor,titleImageColor,titleImageColor);

            ChangeTitleTextAlpha(titleImageColor);
                
            yield return null;
            StartCoroutine(SettingOpeningFadeOut(moveSpeed)); 
        }
        else
        { 
            effectTime = 0;          
            /*
            Debug.Log("titleText의 위치 = " + titleText.GetComponent<RectTransform>().anchoredPosition.y);
            titleImage.GetComponent<Image>().sprite = BackgroundDatabaseManager.instance.GetBackgroundByName("Opening"); 
            GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("Opening"));
            GameManager.instance.ChangePlayBGMTime(masterPieceBGMStartTime - fadeInTime - fadeDelayTime); //페이드아웃이 끝난후 노래가 재생되기에 페이드 인에 걸리는 시간 보정
            */
            StartCoroutine(SettingOpeningFadeIn(fadeInTime + fadeDelayTime));

            titleImage.GetComponent<Image>().sprite = BackgroundDatabaseManager.instance.GetBackgroundByName("Title"); //임시로 적어둔거 나중에 지우기
            titleBulrEffect.SetActive(true);
        } 
        #endregion
    }

    IEnumerator SettingOpeningFadeIn(float fadeTime)
    { 
        #region
        if(effectTime < fadeTime)
        {          
            if(effectTime >= 0)//fadeDelayTime)
            { 
                effectTime += Time.deltaTime/fadeInTime;
                //float titleImageColor = Mathf.Lerp(0.0f, openingColor, effectTime - fadeDelayTime);
                //float titleImageColor = Mathf.Lerp(0.0f, 1.0f, effectTime - fadeDelayTime); // 마스터피스 저거 살릴꺼면 이거 지우기
                //titleImage.GetComponent<Image>().color = new Color(titleImageColor,titleImageColor,titleImageColor);      
                
                SetBulrEffectRange(Mathf.Lerp(0.0f, 10.0f, effectTime)); //- fadeDelayTime));
            }
            else
            { 
                effectTime += Time.deltaTime/fadeDelayTime;
            }

            yield return null;
            StartCoroutine(SettingOpeningFadeIn(fadeTime));             
        }    
        else
        { 
            effectTime = 0;
            /*
            titleImage.GetComponent<Image>().color = new Color(openingColor, openingColor,openingColor);           
            StartCoroutine(Opening(float.Parse(masterPieceTalkData[CSVNum]["Branch"].ToString())));
            ChangeTitleTextAlpha(1);
            titleText.GetComponent<Text>().color = new Color(1, 1, 1);
            */           
            EndOpening();
        }
        #endregion
    }

    IEnumerator Opening(float skipTime)
    {  
        #region
        if(CSVNum < masterPieceTalkData.Count)
        { 
            Debug.Log(masterPieceTalkData[CSVNum]["Content"].ToString());
            titleText.GetComponent<Text>().text = masterPieceTalkData[CSVNum]["Content"].ToString();
            float nextTime = float.Parse(masterPieceTalkData[CSVNum]["Branch"].ToString());
            CSVNum++;
            Debug.Log(nextTime);
            yield return new WaitForSeconds(nextTime);
            StartCoroutine(Opening(nextTime));
        }
        else
        { 
            GameManager.instance.StopBGM();
            EndOpening();
        }
        #endregion
    }

    void EndOpening()
    { 
        #region
        StopAllCoroutines();
        
        titleText.SetActive(false);
        titleImage.GetComponent<Image>().color = new Color(1, 1, 1);
        titleImage.GetComponent<Image>().sprite = BackgroundDatabaseManager.instance.GetBackgroundByName("Title");
        GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("Title"));
        GameManager.instance.StartBGM();
        titleClickCount ++;
        titleButtons.SetActive(true);

        for (int i = 0; i < titleParticle.Length; i++)
            titleParticle[i].SetActive(true);

        titleBulrEffect.SetActive(true);
        SetBulrEffectRange(10.0f);

        gameLogo.SetActive(true);

        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(titleButtons.transform.GetChild(0).gameObject, new BaseEventData(eventSystem));
        #endregion
    }

    void SetBulrEffectRange(float value)
    {
        M_Bulr.SetFloat("_Radius", value);
    }

    void KeyCheck()
    { 
        #region
        if(Input.anyKeyDown)
        { 
            //titleClickCount ++; 이거는 StartOpening()과 EndOpening()에 들어있음
            if(titleClickCount == 0)
            {                 
                StartOpening();
            }    
            else if(titleClickCount == 1)
            {                 
                EndOpening();    
            }
        }      
        #endregion
    }

    void TitleTextEffect()
    { 
        #region
        titleTextEffectTime += Time.deltaTime/titleTextEffectCycle;

        if(titleTextEffectMode == false)
        {            
            float titleTextColor = Mathf.Lerp(0.0f, 1.0f, titleTextEffectTime);
            titleText.GetComponent<Text>().color = new Color(titleTextColor,titleTextColor, titleTextColor);
            
            if(titleTextEffectTime >= 1)
            { 
                titleTextEffectTime = 0;
                titleTextEffectMode = true;
            }
        }
        else
        { 
            float titleTextColor = Mathf.Lerp(1.0f, 0.0f, titleTextEffectTime);
            titleText.GetComponent<Text>().color = new Color(titleTextColor,titleTextColor, titleTextColor);     
            
            if(titleTextEffectTime >= 1)
            { 
                titleTextEffectTime = 0;
                titleTextEffectMode = false;
            }
        }  
        #endregion
    }

    void ChangeTitleTextAlpha(float alpha)
    { 
        #region
        Color color = titleText.GetComponent<Text>().color;
        color.a = alpha;
        titleText.GetComponent<Text>().color = color;      
        #endregion
    }
}
