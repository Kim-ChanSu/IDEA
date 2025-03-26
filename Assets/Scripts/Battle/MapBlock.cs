using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Normal, //�⺻���� Ÿ��
    UnableMoveTile,//�̵��Ұ� Ÿ��
    DamageTile, //������ �ִ� Ÿ��
    HealTile, // �����ִ� Ÿ��
    StrategicPoint //����
}

public enum TileOwner
{
    None,
    Player,
    Enemy
}

[System.Serializable]
public struct TileBuff
{
    public float ATKChange;
    public float MAKChange;
    public float DEFChange;
    public float MDFChange;
    public int moveChange;
    public int rangeChange;
}

public class MapBlock : MonoBehaviour
{
    private GameObject selectionMapBlock;
    private GameObject colorMapBlock;
    private bool isSelect;
    
    //[SerializeField]
    //private Sprite moveableSprite;
    //[SerializeField]
    //private Sprite attackableSprite;
    //[SerializeField]
    //private Sprite interactableSprite;
    //[SerializeField]
    //private Sprite targetSprite;
    [SerializeField]
    private RuntimeAnimatorController targetAnimation;
    [SerializeField]
    private RuntimeAnimatorController moveableAnimation;
    [SerializeField]
    private RuntimeAnimatorController attackableAnimation;
    [SerializeField]
    private RuntimeAnimatorController interactableAnimation;

    public enum Highlight
    {
        Off,
        Select,
        Target,
        Moveable,        
        Attackable,
        Interactable,
        ShowArea
    }

    [HideInInspector]
    public int mapBlockXpos;
    [HideInInspector]
    public int mapBlockYpos;

    [HideInInspector]
    public bool isActionAble;

    [HideInInspector]
    public Highlight highlightMode;

    [HideInInspector]
    public int[] navTile;

    public string mapBlockName;
    [TextArea]
    public string mapBlockEffectText; 
    public TileType tileType;

    public int tileMoveValue = 1;
    public float tileEffectValue = 0;  
    public TileBuff tilebuff;
    public TileOwner tileOwner;
    private string defaultMapBlockEffectText;

    void Awake()
    {
        InitialzeMapBlock();
    }

    private void InitialzeMapBlock()
    {
        InitialzeColliderSize();
        defaultMapBlockEffectText = mapBlockEffectText;
        selectionMapBlock = transform.GetChild(0).gameObject;
        colorMapBlock = transform.GetChild(1).gameObject;
        isActionAble = false;
        ClearNavTileArray();

        if (this.tileType == TileType.StrategicPoint)
        {
            UpdateTileOwnerText();
        }
    }

    private void InitialzeColliderSize() //���ӸŴ����� Ÿ�� ������ ������ collider ũ�� �ʱ�ȭ
    {
        if (this.gameObject.GetComponent<BoxCollider2D>() != true)
        {
            Debug.LogError("Ÿ���� �ݶ��̴��� ������ ���� �ʽ��ϴ�!");
            this.gameObject.AddComponent<BoxCollider2D>();
        }
        this.gameObject.GetComponent<BoxCollider2D>().size = new Vector2 (GameManager.TILESIZE, GameManager.TILESIZE);
    }

    private void SetActiveChild(bool mode)
    { 
        selectionMapBlock.SetActive(mode);
        colorMapBlock.SetActive(mode);
    }

    private void ClearNavTileArray()
    { 
        navTile = new int[1] {-1};
    }

    public void UnSelectMapBlockByHighlightMode(Highlight mode)
    { 
        #region
        if(mode == Highlight.Select)
        { 
            SetselectionMapBlockOff();
        }
        else if((mode == Highlight.Off) || (highlightMode == mode))
        {
            highlightMode = Highlight.Off;
            ClearNavTileArray();
            colorMapBlock.SetActive(false);
            isActionAble = false;                    
        }
        #endregion        
    }

    public void SetselectionMapBlockOff()
    {
        if (selectionMapBlock != null)
        {
            selectionMapBlock.SetActive(false);
            isSelect = false;
        }
    }

    public void ClearNavTileByLength(int length)
    { 
        navTile = new int[length];
        for(int i = 0; i < navTile.Length ;i++)
        { 
            navTile[i] = -1;
        }

    }
    
    public bool GetisSelect()
    { 
         return isSelect;
    }

    public void SetSelectionMode(Highlight mode) //isSelect�϶��� ��尡 �ȹٲ�°� ������ �� ���͸� ������ ��������
    {
        switch(mode)
        {
            case Highlight.Off:
                SetActiveChild(false);
                break;
            case Highlight.Select:                
                isSelect = true;
                if(selectionMapBlock != null)
                    selectionMapBlock.SetActive(true);
                break;
            case Highlight.Target:
                //SetColorMapBlockSprite(targetSprite);
                colorMapBlock.GetComponent<Animator>().runtimeAnimatorController = targetAnimation;
                colorMapBlock.SetActive(true);
                highlightMode = mode;
                break;
            case Highlight.Moveable:
                //SetColorMapBlockSprite(moveableSprite);
                colorMapBlock.GetComponent<Animator>().runtimeAnimatorController = moveableAnimation;
                colorMapBlock.SetActive(true);
                highlightMode = mode;
                break;
            case Highlight.Attackable:
                //SetColorMapBlockSprite(attackableSprite);
                colorMapBlock.GetComponent<Animator>().runtimeAnimatorController = attackableAnimation;
                colorMapBlock.SetActive(true);
                highlightMode = mode;
                break;
            case Highlight.Interactable:
                //SetColorMapBlockSprite(interactableSprite);
                colorMapBlock.GetComponent<Animator>().runtimeAnimatorController = interactableAnimation;
                colorMapBlock.SetActive(true);
                highlightMode = mode;
                break;
            case Highlight.ShowArea:
                //SetColorMapBlockSprite(moveableSprite);
                colorMapBlock.GetComponent<Animator>().runtimeAnimatorController = moveableAnimation;
                colorMapBlock.SetActive(true);
                highlightMode = mode;
                break;
        }              
    }

    public void SetColorMapBlockSprite(Sprite image)
    { 
        colorMapBlock.GetComponent<Animator>().runtimeAnimatorController = null;
        colorMapBlock.GetComponent<SpriteRenderer>().sprite = image;
    }

    public void TileTurnEffect(GameObject target)
    {
        switch (this.tileType)
        {
            case TileType.DamageTile:
                GameManager.instance.battleManager.TurnHPDamage(target, tileEffectValue); 
                break;
            case TileType.HealTile:
                GameManager.instance.battleManager.TurnHPDamage(target, tileEffectValue * -1);
                break;
            case TileType.StrategicPoint:
                ChangeTileOwner(CheckTileOwner(target));
                break;
            default:
                break;
        }
    }

    public void ChangeTileOwner(TileOwner newTileOwner)
    {
        tileOwner = newTileOwner;
        Debug.Log(this.gameObject + "�� ������ �ٲ�����ϴ�. �� ������ " + tileOwner);
        UpdateTileOwnerText();
        GameManager.instance.battleManager.UpdateStrategicPointCount();
    }

    public void UpdateTileOwnerText()
    {
        string lineChangeText = "\n";
        if (defaultMapBlockEffectText == "")
        {
            lineChangeText = "";
        }

        switch (tileOwner)
        {
            case TileOwner.None:
                mapBlockEffectText = "������ : " + "����" + lineChangeText + defaultMapBlockEffectText;
                break;
            case TileOwner.Player:
                mapBlockEffectText = "������ : " + GameManager.instance.playerTeamName + lineChangeText + defaultMapBlockEffectText;
                break;
            case TileOwner.Enemy:
                mapBlockEffectText = "������ : " + GameManager.instance.enemyTeamName + lineChangeText + defaultMapBlockEffectText;
                break;
            default:
                mapBlockEffectText = defaultMapBlockEffectText;
                break;
        }
    }

    private TileOwner CheckTileOwner(GameObject target)
    {
        switch (target.GetComponent<Character>().isEnemy)
        {
            case true:
                return TileOwner.Enemy;
            case false:
                return TileOwner.Player;
        }
    }
}
