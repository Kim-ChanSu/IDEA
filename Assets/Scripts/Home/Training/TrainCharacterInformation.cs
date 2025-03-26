using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrainCharacterInformation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private int characterNum;
    private Sprite[] characterFace;

    private bool isSelectOrDeselect = false;
    private int originalCharacterFaceNum = 0;
    private int mouseOnCharacterFaceNum = 1;
    private int selectCharacterFaceNum = 3;

    [SerializeField]
    private Text nameText;

    [SerializeField]
    private Text HPText;
    [SerializeField]
    private Text MPText;
    [SerializeField]
    private Text ATKText;   
    [SerializeField]
    private Text MAKText;
    [SerializeField]
    private Text DEFText;
    [SerializeField]
    private Text MDFText;

    [SerializeField]
    private Dropdown selectOption;

    [SerializeField]
    private Image EXPFilled;
    [SerializeField]
    private Text EXPText;

    private void Awake()
    {
        selectOption.onValueChanged.AddListener(SettingEXPUI);
        selectOption.onValueChanged.AddListener(SaveDropDownValue);
    }

    private void Start()
    {
        SettingEXPUI(selectOption.value);
    }

    public void SetTrainCharacter(int num)
    {
        if ((num < GameManager.instance.PlayerCharacter.Length) && (num >= 0))
        {           
            SettingCharacterInfo(num);
         
            characterNum = num;
            characterFace = new Sprite[GameManager.instance.PlayerCharacter[num].face.Length];
            for (int i = 0; i < GameManager.instance.PlayerCharacter[num].face.Length; i++)
            {
                characterFace[i] = GameManager.instance.PlayerCharacter[num].face[i];
            }
            ChangeFace(0);
        }
        else
        {
            Debug.LogWarning("잘못 된 값을 가진 CharacterStatusButton이 생성되었습니다!");
            Destroy(this.gameObject);
        }
    }

    private void SettingCharacterInfo(int index)
    {
        nameText.text = GameManager.instance.PlayerCharacter[index].inGameName;
        HPText.text = GameManager.instance.PlayerCharacter[index].maxHP.ToString();
        MPText.text = GameManager.instance.PlayerCharacter[index].maxMP.ToString();
        ATKText.text = GameManager.instance.PlayerCharacter[index].ATK.ToString();
        MAKText.text = GameManager.instance.PlayerCharacter[index].MAK.ToString();
        DEFText.text = GameManager.instance.PlayerCharacter[index].DEF.ToString();
        MDFText.text = GameManager.instance.PlayerCharacter[index].MDF.ToString();
    }

    public void SettingEXPUI(int index)
    {
        SelectTrainCharacter();

        switch (index)
        {
            case (int)IncreasableStatus.maxHP:
                EXPText.text = GameManager.instance.PlayerCharacter[characterNum].maxHPEXP.ToString() + " / " + GameManager.instance.maxStatusEXP.ToString();
                EXPFilled.fillAmount = (float)GameManager.instance.PlayerCharacter[characterNum].maxHPEXP / (float)GameManager.instance.maxStatusEXP;
                break;
            case (int)IncreasableStatus.maxMP:
                EXPText.text = GameManager.instance.PlayerCharacter[characterNum].maxMPEXP.ToString() + " / " + GameManager.instance.maxStatusEXP.ToString();
                EXPFilled.fillAmount = (float)GameManager.instance.PlayerCharacter[characterNum].maxMPEXP / (float)GameManager.instance.maxStatusEXP;
                break;
            case (int)IncreasableStatus.ATK:
                EXPText.text = GameManager.instance.PlayerCharacter[characterNum].ATKEXP.ToString() + " / " + GameManager.instance.maxStatusEXP.ToString();
                EXPFilled.fillAmount = (float)GameManager.instance.PlayerCharacter[characterNum].ATKEXP / (float)GameManager.instance.maxStatusEXP;
                break;
            case (int)IncreasableStatus.MAK:
                EXPText.text = GameManager.instance.PlayerCharacter[characterNum].MAKEXP.ToString() + " / " + GameManager.instance.maxStatusEXP.ToString();
                EXPFilled.fillAmount = (float)GameManager.instance.PlayerCharacter[characterNum].MAKEXP / (float)GameManager.instance.maxStatusEXP;
                break;
            case (int)IncreasableStatus.DEF:
                EXPText.text = GameManager.instance.PlayerCharacter[characterNum].DEFEXP.ToString() + " / " + GameManager.instance.maxStatusEXP.ToString();
                EXPFilled.fillAmount = (float)GameManager.instance.PlayerCharacter[characterNum].DEFEXP / (float)GameManager.instance.maxStatusEXP;
                break;
            case (int)IncreasableStatus.MDF:
                EXPText.text = GameManager.instance.PlayerCharacter[characterNum].MDFEXP.ToString() + " / " + GameManager.instance.maxStatusEXP.ToString();
                EXPFilled.fillAmount = (float)GameManager.instance.PlayerCharacter[characterNum].MDFEXP / (float)GameManager.instance.maxStatusEXP;
                break;
            default:
                break;
        }

    }

    public void SetFace(int mode)
    {
        #region
        //0 select, 1 deselect, 2 pointerenter, 3 pointerexit
        //이중 bool로 구별하려다 bool두개는 지원안해서 int로 구현함..

        if (mode == 0)
        {
            //선택한 경우
            isSelectOrDeselect = true;
            ChangeFace(selectCharacterFaceNum);
        }

        if (mode == 1)
        {
            //선택해제 한 경우
            isSelectOrDeselect = false;
            ChangeFace(originalCharacterFaceNum);
        }

        if (mode == 2)
        {
            //마우스만 올린 경우
            if (isSelectOrDeselect == false)
            {
                ChangeFace(mouseOnCharacterFaceNum);
            }
        }

        if (mode == 3)
        {
            //마우스를 밖으로 보낸 경우
            if (isSelectOrDeselect == false)
            {
                ChangeFace(originalCharacterFaceNum);
            }
        }
        #endregion
    }

    private void ChangeFace(int num)
    {
        if (num < characterFace.Length && (num >= 0))
        {
            this.gameObject.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<Image>().sprite = characterFace[num];
        }
        else
        {
            this.gameObject.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<Image>().sprite = characterFace[0];
        }
    }

    public void SelectTrainCharacter()
    {
        // 선택한 캐릭터 num값 넘겨주기
        GameManager.instance.homeManager.trainManager.playerSelectCharacterNum = characterNum;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeFace(3);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeFace(1);
    }

    public void OnSelect(BaseEventData eventData)
    {
        ChangeFace(0);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ChangeFace(2);
    }

    public void SaveDropDownValue(int value)
    {
        GameManager.instance.PlayerCharacter[characterNum].trainStatus = (IncreasableStatus)value;
    }

    public void LoadDropDownValue(int num)
    {
        selectOption.value = (int)GameManager.instance.PlayerCharacter[num].trainStatus;
    }

    public void EndListen()
    {
        selectOption.onValueChanged.RemoveAllListeners();
    }

}
