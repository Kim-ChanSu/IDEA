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

    public GameObject trainUICanvas; //esc������ Ʈ���̴� �����°� ��ư���� �����ϱ� ���ؼ� ���⵵ ������;;;;;;;;;;;;;;;

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
            teamHPText.text = "<color=red>" + "�� ü�� : " + "</color>";
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
        if (GameManager.instance.gold < GetInputGold()) // �������� �ּҺ�� ���� ���� �� 
        {
            GameManager.instance.homeManager.SetWarningText("������尡 ������#������� �ؼ� ���� �����", true);
        }
        else if (((GetInputGold() >= minCost) == false) || (GameManager.instance.CheckGold(GetInputGold()) == false)) // ������ �Ǵ� �ּҺ�� ���� ���� ���� �Է��� �� 
        {
            GameManager.instance.homeManager.SetWarningText(minCost.ToString() + " �̻��� ���ڸ� �Է�����", true);
        }
        else if(GameManager.instance.playerTeamHealth - costTeamHP < 0) // Ʈ���̴� ���� �� �� ü���� ������ �� 
        {
            GameManager.instance.homeManager.SetWarningText("������ �޽��� �ʿ���", true);
        }
    }

    public void ShowSpecialTrainError()
    {
        GameManager.instance.homeManager.SetWarningText("�������� ������ ���� ������", true);
    }

    public void ShowToDoMainScenario()
    {
        if (GameManager.instance.isTimeToMainScenario() == true)
        {
            GameManager.instance.homeManager.SetWarningText("������ ��Ⱑ �ִ� ���̴� ����غ� ��ư�� ������ ��⸦ ��������");
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

    public void SetStartTrainButton() // �ƾ�â
    {
        if(CheckGetIsTrainReady() == true)
        {
            SetSpendTrainCost();
            GameManager.instance.ChangeScene(TRAINCUTSCENE1);
        }
    }

    public void SetStartMiniGame() // Ʈ���̴� �̴ϰ��� ó��, ���� ���ó�� ��, �̴ϰ��� �Ϸ�� ���â���� ȹ�����ġ ó��
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



    public void SelectMiniGame() // ����
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
            Debug.Log("�޸��� �̴ϰ��� ����");
        }
        else if ((runningGameCount <= minigameSelector) && (minigameSelector < runningGameCount + throwingGameCount))
        {
            GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("MiniGame_Shoot"));
            GameManager.instance.ChangeScene("ThrowingMinigameScene");
            Debug.Log("�� ������ �̴ϰ��� ����");
        }    
        else if ((throwingGameCount <= minigameSelector) && (minigameSelector  < throwingGameCount + ballCatchGameCount))
        {
            GameManager.instance.ChangeBGM(BGMDatabaseManager.instance.GetBGMByName("MiniGame_Catch"));
            GameManager.instance.ChangeScene("BallCatchMinigameScene");
            Debug.Log("�� ��� �̴ϰ��� ����");
        }
      

    }

    // ���â���� ���� ó��
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
