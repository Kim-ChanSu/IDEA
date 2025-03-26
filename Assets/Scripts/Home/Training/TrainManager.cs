using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainManager : MonoBehaviour
{
    private const string TRAINCUTSCENE1 = "TrainCutScene_1";

    [SerializeField]
    private int costTeamHP;

    [SerializeField]
    private GameObject characterPrefab;
    [SerializeField]
    private RectTransform createPrefebPosition;
    [SerializeField]
    private GameObject parentObject;

    [SerializeField]
    private Text trainTeamHPText;
    [SerializeField]
    private Image trainTeamHPImage;
    [SerializeField]
    private Text teamHPText;
    [SerializeField]
    private Text goldText;
    [SerializeField]
    private InputField costInput;
    [SerializeField]
    private int minCost = 1000;

    public int playerSelectCharacterNum;

    [SerializeField]
    private string InputGold;

    [SerializeField]
    private Image selectBasicTrainButton;
    [SerializeField]
    private Image selectSpecialTrainButton;
    [SerializeField]
    private Button specialTrainButton;

    private Color selectcolor = new Color(241f / 255f, 241f / 255f, 241f / 255f, 150f / 255f);
    private Color resetcolor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 0f / 255f);

    [SerializeField]
    private Toggle checkMinigameSkip;

    public GameObject trainUICanvas; //esc누르면 트레이닝 꺼지는거 버튼에서 연동하기 뭐해서 여기도 만들어둠;;;;;;;;;;;;;;;

    private void Awake()
    {
        costInput.onEndEdit.AddListener(SetInputGold);
    }

    private void Start()
    {
        costTeamHP = 20;
        playerSelectCharacterNum = 0;
        SettingCostInfomation();
        SelectBasicTrainButton();
    }

    
    public void ResetSelectTrainButton()
    {
        selectBasicTrainButton.color = resetcolor;
        selectSpecialTrainButton.color = resetcolor;
    }

    public void SelectBasicTrainButton()
    {
        ResetSelectTrainButton();
        selectBasicTrainButton.color = selectcolor;
        SetSkipMiniGameButton();
    }

    public void SelectSpecialTrainButton()
    {
        ResetSelectTrainButton();
        selectSpecialTrainButton.color = selectcolor;
        specialTrainButton.interactable = false;
        Invoke("SetInteractableSpecialTrainButton", 0.5f);
    }
    
    public void SetInteractableSpecialTrainButton()
    {
        specialTrainButton.interactable = true;
    }

    public void SettingTrainUI()
    {
        for (int i = 0; i < GameManager.instance.useableCharacter.Length; i++)
        {
            if (GameManager.instance.useableCharacter[i] == true)
            {
                var newUi = Instantiate(characterPrefab, createPrefebPosition).GetComponent<RectTransform>();
                newUi.GetComponent<TrainCharacterInformation>().SetTrainCharacter(i);
                newUi.GetComponent<TrainCharacterInformation>().LoadDropDownValue(i);
            }
        }
    }

    public void EndTrainUI()
    {   if (GameManager.instance.homeManager.GetIsWarningText() == false)
        {
            for (int i = 0; i < parentObject.transform.childCount; i++)
            {
                parentObject.transform.GetChild(i).gameObject.GetComponent<TrainCharacterInformation>().EndListen();
                Destroy(parentObject.transform.GetChild(i).gameObject);
            }

            trainUICanvas.SetActive(false);

            if (GameManager.instance.homeManager != null)
            {
                GameManager.instance.homeManager.SetAllHomeButtons(true);
                GameManager.instance.homeManager.isTrainingUIOn = false;
            }
        }
    }

    public void SettingCostInfomation()
    {
        goldText.text = GameManager.instance.gold.ToString();
        costInput.text = "1000";
        SettingCostTextColor();
        trainTeamHPText.GetComponent<Text>().text = GameManager.instance.playerTeamHealth + " / " + GameManager.instance.playerTeamMaxHealth;
        trainTeamHPImage.GetComponent<Image>().fillAmount = (float)GameManager.instance.playerTeamHealth / (float)GameManager.instance.playerTeamMaxHealth;
        SelectBasicTrainButton();
    }

    private void SettingCostTextColor()
    {
        if(GameManager.instance.playerTeamHealth < costTeamHP)
        {
            teamHPText.text = "<color=red>" + "팀 체력 : " + "</color>";
        }
        

        int temp = int.Parse(costInput.text);

        if(GameManager.instance.gold < temp)
        {
            goldText.text = "<color=yellow>" + GameManager.instance.gold.ToString() + "</color>";
        }
    }

    public void SetInputGold(string value)
    {
        InputGold = value;
    }

    public void ShowInputError()
    {
        if (GameManager.instance.gold < GetInputGold()) // 소지금이 최소비용 보다 작을 때 
        {
            GameManager.instance.homeManager.SetWarningText("소유골드가 부족해#경기출전 해서 돈을 벌어보자", true);
        }
        else if (((GetInputGold() >= minCost) == false) || (GameManager.instance.CheckGold(GetInputGold()) == false)) // 음수값 또는 최소비용 보다 작은 값을 입력할 때 
        {
            GameManager.instance.homeManager.SetWarningText(minCost.ToString() + " 이상의 숫자를 입력해줘", true);
        }
        else if(GameManager.instance.playerTeamHealth - costTeamHP < 0) // 트레이닝 진행 전 팀 체력이 부족할 때 
        {
            GameManager.instance.homeManager.SetWarningText("가끔은 휴식이 필요해", true);
        }
    }

    public void ShowSpecialTrainError()
    {
        GameManager.instance.homeManager.SetWarningText("선수들의 레벨이 아직 부족해", true);
    }

    public void ShowToDoMainScenario()
    {
        if (GameManager.instance.isTimeToMainScenario() == true)
        {
            GameManager.instance.homeManager.SetWarningText("오늘은 경기가 있는 날이니 경기준비 버튼을 눌러서 경기를 진행하자");
        }
    }

    public int GetInputGold()
    {
        return int.Parse(InputGold);
    }

    public void ResetInputGold()
    {
        costInput.text = "1000";
        InputGold = "1000";
    }

    public bool CheckGetIsTrainReady()
    {
        if((GameManager.instance.CheckGold(GetInputGold()) == true) && (GameManager.instance.CheckPlayerTeamHP(costTeamHP) == true) && ((GetInputGold() >= minCost) == true))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetStartTrainButton() // 컷씬창
    {
        if(CheckGetIsTrainReady() == true)
        {
            SetSpendTrainCost();
            GameManager.instance.ChangeScene(TRAINCUTSCENE1);
        }
    }

    public void SetStartMiniGame() // 트레이닝 미니게임 처리, 먼저 비용처리 후, 미니게임 완료시 결과창에서 획득경험치 처리
    {
        if (CheckGetIsTrainReady() == true)
        {
            SetSpendTrainCost(); 
            SelectMiniGame();
        }
    }


    public void StartTraining()
    {
        if(this.checkMinigameSkip.isOn == true)
        {
            GameManager.instance.checkTrainingSkipBox = true;
            SetStartTrainButton();
        }
        else
        {
            GameManager.instance.checkTrainingSkipBox = false;
            SetStartMiniGame();
        }
    }



    public void SelectMiniGame() // 공식
    {
        int minigameSelector = 0;
        int runningGameCount = 0;
        int throwingGameCount = 0;
        int ballCatchGameCount = 0;
        int characterNumber = 0;

        for(int i=0;i<GameManager.instance.useableCharacter.Length;i++)
        {
            if (GameManager.instance.useableCharacter[i] == true)
            {
                characterNumber++;

                switch (GameManager.instance.PlayerCharacter[i].trainStatus)
                {
                    case IncreasableStatus.maxHP:
                    case IncreasableStatus.maxMP:
                        runningGameCount++;
                        break;
                    case IncreasableStatus.ATK:
                    case IncreasableStatus.MAK:
                        throwingGameCount++;
                        break;
                    case IncreasableStatus.DEF:
                    case IncreasableStatus.MDF:
                        ballCatchGameCount++;
                        break;
                    default:
                        break;
                }
            }
        }

        minigameSelector = Random.Range(0, characterNumber);

        if ((0 <= minigameSelector)  && (minigameSelector < runningGameCount)) 
        {
            GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("MiniGame_Run"));
            GameManager.instance.ChangeScene("RunningMinigameScene"); 
            Debug.Log("달리기 미니게임 실행");
        }
        else if ((runningGameCount <= minigameSelector) && (minigameSelector < runningGameCount + throwingGameCount))
        {
            GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("MiniGame_Shoot"));
            GameManager.instance.ChangeScene("ThrowingMinigameScene");
            Debug.Log("공 던지기 미니게임 실행");
        }    
        else if ((throwingGameCount <= minigameSelector) && (minigameSelector  < throwingGameCount + ballCatchGameCount))
        {
            GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("MiniGame_Catch"));
            GameManager.instance.ChangeScene("BallCatchMinigameScene");
            Debug.Log("공 잡기 미니게임 실행");
        }
      

    }

    // 결과창에서 따로 처리
    /*
    public void SetEXPUpgrade() 
    {
        for (int i = 0; i < GameManager.instance.useableCharacter.Length; i++)
        {
            if (GameManager.instance.useableCharacter[i] == true)
            {                
                GameManager.instance.IncreaseStatusEXP(i, GameManager.instance.PlayerCharacter[i].trainStatus, GetInputGold());
            }
        }
    }
    */

    public void SetSpendTrainCost()
    {
        GameManager.instance.playerTeamHealth = GameManager.instance.playerTeamHealth - costTeamHP;
        GameManager.instance.trainSpendGold = GetInputGold();
        GameManager.instance.gold = GameManager.instance.gold - GetInputGold();
    }

    public void SaveSkipData()
    {
        GameManager.instance.checkTrainingSkipBox = this.checkMinigameSkip.isOn;
    }

    public void SetSkipMiniGameButton()
    {
        this.checkMinigameSkip.isOn = GameManager.instance.checkTrainingSkipBox;
    }

}
