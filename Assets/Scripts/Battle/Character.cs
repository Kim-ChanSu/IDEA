using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;
    private Color charaterOriginalColor = new Color(1, 1, 1);
    private Color charaterTurnEndColor = new Color(100/255f, 100/255f, 100/255f);

    private bool isCantUseSkill;
    private bool isSelect;
    private bool isTurnEnd;
    private bool isSkillEffect = false;
    private const int CHARACTERZPOS = 11;
    private int[] navTile;

    private int defaultStatusEffectTurnCount = 999;

    private Vector3 saveCharacterPosition; 
    //[HideInInspector]
    public SympathyType sympathyType;
    [HideInInspector]
    public bool saveIsHaveBall;
    private bool saveFlipX;
    private int saveSympathy;
    private int saveBattleLogCount;

    public int setXPosition;  
    public int setYPosition;

    public float moveTime = 0.15f;
    [HideInInspector]
    public float effectTime = 0;

    [HideInInspector]
    public int CharacterCurrentXPosition;
    [HideInInspector]
    public int CharacterCurrentYPosition;
    [HideInInspector]
    public int CharacterCurrentZPosition;

    [HideInInspector]
    public int characterDatabaseNum;

    public CharacterStatus status;
    public bool isEnemy;
    [HideInInspector]
    public bool isHaveBall;
    [HideInInspector]
    public GameObject ball;
    //[HideInInspector]  
    public CharacterStatusEffect[] characterStatusEffect;

    public GameObject effectObject;
    public GameObject characterDamageEffectObject;
    [SerializeField]
    private GameObject leaderIcon;

    private CharacterAnimationController characterAnimationController;

    private TileBuff characterTileBuff = new TileBuff();
    private TileBuff saveTileBuff = new TileBuff();

    //[HideInInspector]
    public MapBlock aiTargetMapBlock = null;
    [HideInInspector]
    public bool aiMoveInPlace = false;
    void Awake()
    {
        InitializeCharacter();
    } 

    public void InitializeCharacter()
    {
        #region
        // ��ġ �����Ҷ� �ΰ� �ǵ�����ϴ°� �ƴ϶�� �����ؼ� ������
        if (this.isEnemy == true)
        {
            setXPosition = (int)(this.gameObject.transform.position.x);
            setYPosition = (int)(this.gameObject.transform.position.y);
        }
        //
        Vector3 characterPosition = new Vector3();
        characterPosition.x = setXPosition; 
        characterPosition.y = setYPosition;
        characterPosition.z = CHARACTERZPOS;
        transform.position = characterPosition;
        effectTime = 0;

        CharacterCurrentXPosition = setXPosition;
        CharacterCurrentYPosition = setYPosition;
        CharacterCurrentZPosition = CHARACTERZPOS;

        saveCharacterPosition = this.gameObject.transform.position;

        characterStatusEffect = new CharacterStatusEffect[StatusEffectDatabaseManager.instance.GetStatusEffectLength()];
        for(int i = 0; i < characterStatusEffect.Length; i++)
        { 
            characterStatusEffect[i] = new CharacterStatusEffect();
            characterStatusEffect[i].isOn = false;
            characterStatusEffect[i].statusEffectTurnCount = 0;
        }

        if (isEnemy != true)
        {
            Debug.Log("player " + this.gameObject + "�� ����ȿ���� Ȱ��ȭ");
            SetStatusEffect(StatusEffectDatabaseManager.instance.GetPlayerStrategicPointEffectNum(), true);
        }
        else
        {
            Debug.Log("enemy " + this.gameObject + "�� ����ȿ���� Ȱ��ȭ");
            SetStatusEffect(StatusEffectDatabaseManager.instance.GetEnemyStrategicPointEffectNum(), true);
        }

        ClearCharacterTileBuff();
        #endregion
    }
    
    public void SetCharacterSelectionMode(bool mode) // �������� ��°� �����ϴ°�
    {
        #region
        if(mode == true)
        {          
            isSelect = true;
        }
        else
        {           
            isSelect = false;
        }
        #endregion
    }
    
    public bool GetisSelect()
    { 
        return isSelect;
    }

    public void SetIsTurnEnd(bool mode)
    { 
        #region
        SetCharacterColorTurnEnd(mode);
        if(mode == true)
        {         
            TurnEndCheckStatusEffectCount();
            ChangeCharacterFlipX(isEnemy);
            isTurnEnd = true;
        }
        else
        {           
            isTurnEnd = false;
        }
        #endregion        
    }

    public void SetLeaderIcon(bool mode)
    {
        if (leaderIcon != null)
        {
            leaderIcon.SetActive(mode);
        }
        else
        {
            Debug.LogWarning("���� �������� �����Ǿ� ���� �ʽ��ϴ�!");
        }
    }

    public void SetCharacterColorTurnEnd(bool mode) // ���� �����Ҷ� �� �ٲٵ��� ���� �ִϸ��̼� ������..
    {
        if (mode == true)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = charaterTurnEndColor;
            if (characterAnimationController != null)
            { 
                SetCharacterAnimation(false);                
            }
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = charaterOriginalColor;
            if (characterAnimationController != null)
            { 
                //SetCharacterAnimation(true);
                StartCharacterAnimationByRandomTime();
            }
        }
    }

    public void SetCharacterAnimation(bool mode)
    { 
        if (characterAnimationController == null)
        { 
            return;    
        }

        if(mode == true)
        { 
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            characterAnimationController.SetThisGameObjectActive(true);            
        }
        else
        { 
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            characterAnimationController.SetThisGameObjectActive(false);            
        }
    } 

    public void StartCharacterAnimationByRandomTime(float minTime = 0.1f, float maxTime = 1.0f)
    {
        if(characterAnimationController == null)
        { 
            return;    
        }      

        characterAnimationController.gameObject.GetComponent<Animator>().speed = 0.0f;
        float randTime = Random.Range(minTime, maxTime);
        Invoke("StartCharacterAnimation", randTime);
    }

    private void StartCharacterAnimation()
    { 
        if (GameManager.instance.battleManager == null)
        {
            characterAnimationController.gameObject.GetComponent<Animator>().speed = 1.0f;
            SetCharacterAnimation(true);  
            return;
        }

        if ((this.GetIsTurnEnd() == false) || (this.isEnemy != GameManager.instance.battleManager.battlePhaseManager.GetIsEnemyTurn())) // �����̻����� �� ������� �Ǿ��µ� �������� ���� �ִϸ��̼� �����°� ����
        {
            characterAnimationController.gameObject.GetComponent<Animator>().speed = 1.0f;
            SetCharacterAnimation(true);  
        }
    }

    private void TurnEndCheckStatusEffectCount()
    { 
        for(int i = 0; i < characterStatusEffect.Length; i++)
        { 
            if(characterStatusEffect[i].isOn == true)
            { 
                StatusEffect checkEffect = StatusEffectDatabaseManager.instance.GetStatusEffect(i);
                if(checkEffect.isNotTurnCount == false)
                {
                    ChangeStatusEffectTurnCount(i, -1);
                }    
            }
        }
    }

    public bool GetIsTurnEnd()
    { 
        return isTurnEnd;
    }

    public void SetIsHaveBall(bool mode, GameObject targetBall)
    { 
        if(mode == true)
        { 
            if (this.GetIsHaveBall() == false)
            {
                this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                ball = targetBall;
                isHaveBall = true;
                targetBall.GetComponent<BallController>().SetBallActive(false);
            }
            else
            {
                Debug.LogWarning(this.gameObject + "�� �̹� ���� ��� �ֽ��ϴ�!");
            }
        }
        else
        { 
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            ball = null;
            isHaveBall = false;
        }
    }

    public bool GetIsHaveBall()
    { 
        if((ball != null) && (isHaveBall == true))
        { 
            return true;    
        }
        else
        { 
            return false;    
        }
    }

    public void SetBallActiveByThisPosition()
    {  
        /*
        if(characterAnimationController != null) //����
        { 
            this.ball.GetComponent<BallController>().SetBallPosition(new Vector2 (this.transform.position.x + (characterAnimationController.GetXScale() * GameManager.TILESIZE) ,this.transform.position.y));
        }
        else
        { 
            this.ball.GetComponent<BallController>().SetBallPosition(new Vector2 (this.transform.position.x ,this.transform.position.y));
        }
        */
        this.ball.GetComponent<BallController>().SetBallPosition(new Vector2 (this.transform.position.x ,this.transform.position.y));// ���� �ɲ��� �̰� �ּ��ϰ� ���� �ּ�����

        this.ball.GetComponent<BallController>().SetBallActive(true);        
    }

    public void SetStatusEffect(int statusEffectNum, bool mode)
    { 
        StatusEffect thisEffect = StatusEffectDatabaseManager.instance.GetStatusEffect(statusEffectNum);
        if(statusEffectNum < characterStatusEffect.Length)
        { 
            if(mode == true)
            {                
                characterStatusEffect[statusEffectNum].isOn = true;           
                if(thisEffect.isNotTurnCount == false)
                { 
                    characterStatusEffect[statusEffectNum].statusEffectTurnCount = thisEffect.continueTurn;
                }
                else
                { 
                    characterStatusEffect[statusEffectNum].statusEffectTurnCount =  defaultStatusEffectTurnCount;
                }
            }
            else
            { 
                StatusEffectClear(statusEffectNum);
            }
        }
        else
        { 
            Debug.LogWarning("Character ��ũ��Ʈ�� SetStatusEffect�Լ��� �߸��� ���� ���Խ��ϴ�!");  
        }
    }

    public void ChangeStatusEffectTurnCount(int statusEffectNum,int changeNum)
    { 
        if(statusEffectNum < characterStatusEffect.Length)
        { 
            characterStatusEffect[statusEffectNum].statusEffectTurnCount += changeNum;
            if(characterStatusEffect[statusEffectNum].statusEffectTurnCount <= 0)
            { 
                StatusEffectClear(statusEffectNum);
            }        
        }
        else
        { 
            Debug.LogWarning("Character ��ũ��Ʈ�� ChangeStatusEffectTurnCount�Լ��� �߸��� ���� ���Խ��ϴ�!");   
        }
    }

    public void SetCharacterAnimationController(CharacterAnimationController controller)
    { 
        if(controller != null)
        { 
            characterAnimationController = controller;   
        }
    }

    public bool CheckCharacterAnimationController()
    { 
        if(characterAnimationController != null)
        { 
            return true;   
        }        

        return false;
    }

    public void StatusEffectClear(int statusEffectNum)
    { 
        characterStatusEffect[statusEffectNum].statusEffectTurnCount = 0;
        characterStatusEffect[statusEffectNum].isOn = false;        
    }

    public void SetIsDead(bool mode)
    { 
        if(mode == true)
        { 
            SetStatusEffect(StatusEffectDatabaseManager.instance.GetIsDeadEffectNum(), true);
            this.gameObject.SetActive(false);

            if (GetIsHaveBall() == true)
            {
                SetBallActiveByThisPosition();
                SetIsHaveBall(false, null);
            }

            if(isTurnEnd == false)
            { 
                SetIsTurnEnd(true);
            }
        }
    }

    public bool GetIsDead()
    { 
        if(characterStatusEffect[StatusEffectDatabaseManager.instance.GetIsDeadEffectNum()].isOn == true)
        { 
            return true;    
        }
        else
        { 
            return false;    
        }
    } 

    public void SetIsCantUseSkill(bool mode)
    { 
        isCantUseSkill = mode;    
    }

    public bool GetIsCantUseSkill()
    { 
        return isCantUseSkill;    
    }

    public void MovePosition(int targetXPosition, int targetYPosition)
    {
        transform.position = new Vector3(targetXPosition, targetYPosition, CHARACTERZPOS);

        CharacterCurrentXPosition = targetXPosition;
        CharacterCurrentYPosition = targetYPosition;

        CheckOtherCharacterForZPos();
    }

    public void CheckOtherCharacterForZPos()
    { 
        Vector2 worldPoint = new Vector2 ((int)transform.position.x, (int)transform.position.y + 1);
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
        for(int i = 0; i< hits.Length; i++)
        { 
            if (hits[i].transform.tag == "Character")
            { 
                Debug.Log("���� ĳ���� �߰�!");
                this.transform.GetChild(0).transform.GetComponent<CharcterChecker>().ChangeZpos((int)hits[i].transform.position.z - 1);
                CharacterCurrentZPosition = (int)hits[i].transform.position.z - 1;
            }    
        }        
    }


    public void ChangeZPosition(int zPos)
    { 
        transform.position = new Vector3(transform.position.x, transform.position.y, zPos);
        CharacterCurrentZPosition = zPos;
    }

    public void ResetZPosition()
    { 
        transform.position = new Vector3(transform.position.x, transform.position.y, CHARACTERZPOS);
        CharacterCurrentZPosition = CHARACTERZPOS;

        CheckOtherCharacterForZPos();
    }

    //------����Ʈ��
    #region
    public void StartEffect(Effect effect)
    { 
        Debug.Log("����Ʈ ����");
        isSkillEffect = true;
        effectObject.SetActive(true);
        if(effect.isEffectCharacterUp == true)
        { 
           effectObject.transform.position = new Vector3 (this.transform.position.x , this.transform.position.y + GameManager.TILESIZE, this.transform.position.z);
        }
        else
        { 
            effectObject.transform.position = new Vector3 (this.transform.position.x , this.transform.position.y, this.transform.position.z);
        }

        if(effect.effectSE != null)
        { 
            GameManager.instance.PlaySE(effect.effectSE);
        }

        effectObject.GetComponent<Animator>().runtimeAnimatorController = effect.effect;
        effectObject.GetComponent<Animator>().SetTrigger("effectOn");  

    }

    public void EffectEnd()
    { 
        Debug.Log("����Ʈ ��");
        isSkillEffect = false;
        //this.transform.GetChild(2).gameObject.SetActive(false);
        //this.transform.GetChild(2).gameObject.transform.position = new Vector3 (this.transform.position.x , this.transform.position.y, this.transform.position.z);
        //this.transform.GetChild(2).GetComponent<Animator>().runtimeAnimatorController = null;
        //this.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = null;

        effectObject.SetActive(false);
        effectObject.transform.position = new Vector3 (this.transform.position.x , this.transform.position.y, this.transform.position.z);
        effectObject.GetComponent<Animator>().runtimeAnimatorController = null;
        effectObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    public bool GetIsAnimation()
    { 
        return isSkillEffect;    
    }

    public void DamageEffect(int damage, GameObject target, DamageType damageType = DamageType.HP)
    { 
        if ((characterDamageEffectObject != null) && (characterDamageEffectObject.GetComponent<CharacterDamageEffect>() == true))
        {
            if(damage < 0)
            { 
                damage = damage * -1;
                characterDamageEffectObject.GetComponent<CharacterDamageEffect>().SetDamageEffect(damage, true, damageType);
            }
            else
            { 
                characterDamageEffectObject.GetComponent<CharacterDamageEffect>().SetDamageEffect(damage, false, damageType);
            }
        }
        else
        { 
            Debug.LogWarning("characterDamageEffectObject�� �̻��մϴ�");    
        }

    }

    
    #endregion
    //------

    // characterTileBuff ��
    public void ClearCharacterTileBuff()
    {
        characterTileBuff = new TileBuff();
        characterTileBuff.ATKChange = 0.0f;
        characterTileBuff.MAKChange = 0.0f;
        characterTileBuff.DEFChange = 0.0f;
        characterTileBuff.MDFChange = 0.0f;
        characterTileBuff.moveChange = 0;
        characterTileBuff.rangeChange = 0;
    }

    public void SetCharacterTileBuff(TileBuff tileBuff)
    {
        characterTileBuff.ATKChange = tileBuff.ATKChange;
        characterTileBuff.MAKChange = tileBuff.MAKChange;
        characterTileBuff.DEFChange = tileBuff.DEFChange;
        characterTileBuff.MDFChange = tileBuff.MDFChange;
        characterTileBuff.moveChange = tileBuff.moveChange;
        characterTileBuff.rangeChange = tileBuff.rangeChange;
    }

    public TileBuff GetCharacterTileBuff()
    {
        return characterTileBuff;
    }

    public void CheckTileBuff() // ���� �̵� ���� �Ŀ��� �˻��� ���߿� �а� �̷��� ������ ����������̶�簡 �߰��ؾ���
    {
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y), Vector2.zero);

        for (int i = 0; i < hits.Length; i++)
        {
            if ((hits[i].transform.gameObject.GetComponent<MapBlock>() == true) && (hits[i].transform.gameObject.CompareTag("Tile")))
            {
                SetCharacterTileBuff(hits[i].transform.GetComponent<MapBlock>().tilebuff);
                break;
            }
        }
    }
    //
    public void ReturnForBeforeMove()
    { 
        MovePosition((int)saveCharacterPosition.x, (int)saveCharacterPosition.y);
        ChangeCharacterFlipX(saveFlipX);
        SetIsHaveBall(saveIsHaveBall, ball);
        SetCharacterTileBuff(saveTileBuff);
        GameManager.instance.PlayerCharacter[characterDatabaseNum].Sympathy = saveSympathy;

        if (GameManager.instance.battleManager.battleGetInformationManager.GetBattleLogCount() != saveBattleLogCount)
        {
            int deleteLog = 0;

            if (saveBattleLogCount < GameManager.instance.battleManager.battleGetInformationManager.GetBattleLogCount())
            {
                deleteLog = GameManager.instance.battleManager.battleGetInformationManager.GetBattleLogCount() - saveBattleLogCount;
            }
            else
            {
                deleteLog = GameManager.instance.battleManager.battleGetInformationManager.GetBattleLogCount() - saveBattleLogCount + GameManager.instance.battleManager.battleGetInformationManager.GetBattleLogLength(); 
            }

            GameManager.instance.battleManager.battleGetInformationManager.DeleteBattleLog(deleteLog);
        }
    }

    // �̵������غ�

    public void MovePositionByAnimation(MapBlock mapBlock, ref BattleManager battleManager)
    {
        #region
        if (isEnemy == true)
            return;

        GameManager.instance.SetCameraMoveTarget(this.gameObject); //ī�޶�

        // �� ����
        saveCharacterPosition = this.gameObject.transform.position;
        saveIsHaveBall = isHaveBall;
        saveFlipX = this.gameObject.GetComponent<SpriteRenderer>().flipX;
        saveSympathy = GameManager.instance.PlayerCharacter[characterDatabaseNum].Sympathy;
        saveBattleLogCount = GameManager.instance.battleManager.battleGetInformationManager.GetBattleLogCount();
        saveTileBuff = characterTileBuff;
        //
        Debug.Log("�̵�����");
        battleManager.SetIsAnimation(true);      
        navTile = mapBlock.navTile;
        StartMovePosition(mapBlock, ref battleManager, 0);
        #endregion
    }

    public void AIMovePositionByAnimation(MapBlock mapBlock, ref BattleManager battleManager)
    {
        GameManager.instance.SetCameraMoveTarget(this.gameObject); // ī�޶�
        aiMoveInPlace = false;
        battleManager.SetIsAnimation(true);
        navTile = mapBlock.navTile;
        StartMovePosition(mapBlock, ref battleManager, 0);
    }

    public void StartMovePosition(MapBlock mapBlock, ref BattleManager battleManager, int num)
    { 
        #region
        int direction = 0;
        if(num < navTile.Length)
        { 
           direction = navTile[num];
        }
        else
        { 
            direction = -1;
        }

        switch(direction)
        {
            case UP:
                CharacterCurrentYPosition += GameManager.TILESIZE;
                StartCharacterMove(mapBlock, ref battleManager, UP, num);
                break;
            case DOWN:
                CharacterCurrentYPosition -= GameManager.TILESIZE;
                StartCharacterMove(mapBlock, ref battleManager, DOWN, num);
                break;
            case LEFT:
                CharacterCurrentXPosition -= GameManager.TILESIZE;
                StartCharacterMove(mapBlock, ref battleManager, LEFT, num);
                break;
            case RIGHT:
                CharacterCurrentXPosition += GameManager.TILESIZE;
                StartCharacterMove(mapBlock, ref battleManager, RIGHT, num);
                break;   
            default:       
                AllMoveEnd(new Vector3(mapBlock.mapBlockXpos, mapBlock.mapBlockYpos, this.gameObject.transform.position.z), ref battleManager);
                break;
        }     
        #endregion
    }

    public void StartCharacterMove(MapBlock mapBlock, ref BattleManager battleManager, int direction, int num)
    { 
        // �� ��ġ�� �ִϸ��̼� �߰�(�Լ��� ���λ��� ���Է����� �����ص� ��)
        SetCharacterMoveAnimation(true);
        //--
        CheckDirection(direction);
        StartCoroutine(Move(MoveStartSetting(),mapBlock, battleManager, direction, num));
    }

    public Vector3 MoveStartSetting()   
    { 
        #region
        Vector3 CharacterPos = new Vector3 ((int)this.gameObject.transform.position.x, (int)this.gameObject.transform.position.y, CharacterCurrentZPosition);
        return CharacterPos;
        #endregion
    }        
    
    public void Moving(Vector3 charcherPos ,Vector3 targetPos)   
    { 
        #region
        //Debug.Log("Moving : " + effectTime);
        effectTime += Time.deltaTime/moveTime;
        this.gameObject.transform.position = Vector3.Lerp(charcherPos, targetPos, effectTime);    
        #endregion
    }
    

    public void MoveEnd(Vector3 characterPos, MapBlock mapBlock, BattleManager battleManager, int num)  //��ĭ�̵��Ϸ��� ����
    { 
        #region
        //Debug.Log("�̵�����");
        effectTime = 0;
        this.gameObject.transform.position = characterPos; 
        int nextNum = num + 1;
        StartMovePosition(mapBlock, ref battleManager, nextNum);
        #endregion
    }


    private void AllMoveEnd(Vector3 characterPos, ref BattleManager battleManager)
    { 
        Debug.Log("�̵��� ������ ����");
        
        effectTime = 0;           
        MovePosition((int)characterPos.x, (int)characterPos.y);
        CheckTileBuff();
        battleManager.SetIsAnimation(false);
        battleManager.battleCharacterManager.CharacterMoveEnd();

        SetCharacterMoveAnimation(false);
    }

    private void CheckDirection(int direction)
    { 
        switch(direction)
        { 
            case UP: 
                break;
            case DOWN:  
                break;
            case LEFT:  
                ChangeCharacterFlipX(true); 
                break;
            case RIGHT: 
                ChangeCharacterFlipX(false);
                break;
        }      
    }
    

    public void ChangeCharacterFlipX(bool mode)
    { 
        this.gameObject.GetComponent<SpriteRenderer>().flipX = mode;
        if(characterAnimationController != null)
        { 
            characterAnimationController.ChangeAnimationFlip(mode);
        }
    }

    IEnumerator Move(Vector3 characterPos, MapBlock mapBlock, BattleManager battleManager, int direction, int num)
    { 
        #region
        Vector3 targetPos;
        switch(direction)
        {                 
            case UP:
                targetPos = new Vector3((int)characterPos.x , (int)characterPos.y + GameManager.TILESIZE, CharacterCurrentZPosition);
                if(this.gameObject.transform.position.y < targetPos.y)
                { 
                    Moving(characterPos, targetPos);
                    yield return null;         
                    StartCoroutine(Move(characterPos, mapBlock, battleManager, direction, num)); 
                }
                else
                { 
                    MoveEnd(targetPos, mapBlock, battleManager, num);
                }  
                break;
            case DOWN:
                targetPos = new Vector3((int)characterPos.x , (int)characterPos.y - GameManager.TILESIZE, CharacterCurrentZPosition);
                if(this.gameObject.transform.position.y > targetPos.y)
                { 
                    Moving(characterPos, targetPos);
                    yield return null;         
                    StartCoroutine(Move(characterPos,mapBlock, battleManager, direction, num)); 
                }
                else
                { 
                    MoveEnd(targetPos, mapBlock, battleManager, num);
                }
                break;
            case LEFT:
                targetPos = new Vector3((int)characterPos.x - GameManager.TILESIZE, (int)characterPos.y, CharacterCurrentZPosition);
                if(this.gameObject.transform.position.x > targetPos.x)
                { 
                    Moving(characterPos, targetPos);
                    yield return null;         
                    StartCoroutine(Move(characterPos, mapBlock, battleManager, direction, num)); 
                }
                else
                { 
                    MoveEnd(targetPos, mapBlock, battleManager, num);
                }
                break;
            case RIGHT:
                targetPos = new Vector3((int)characterPos.x + GameManager.TILESIZE, (int)characterPos.y, CharacterCurrentZPosition);
                if(this.gameObject.transform.position.x < targetPos.x)
                { 
                    Moving(characterPos, targetPos);
                    yield return null;         
                    StartCoroutine(Move(characterPos, mapBlock, battleManager, direction, num)); 
                }
                else
                { 
                    MoveEnd(targetPos, mapBlock, battleManager, num);
                }
                break;   
            }
        #endregion
    }   

    public void SetCharacterMoveAnimation(bool mode)
    { 
        if(characterAnimationController != null)
        { 
            characterAnimationController.SetMoveAnimation(mode);
        }        
    }

    public void SetCharacterThrowAnimation(bool mode)
    { 
        if(characterAnimationController != null)
        { 
            characterAnimationController.SetThrowAnimation(mode);
        }        
    }
}
