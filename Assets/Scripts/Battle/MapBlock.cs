using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Normal, //기본적인 타일
    UnableMoveTile,//이동불가 타일
    DamageTile, //데미지 주는 타일
    HealTile, // 힐해주는 타일
    StrategicPoint //거점
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

    private void InitialzeColliderSize() //게임매니저의 타일 사이즈 값으로 collider 크기 초기화
    {
        if (this.gameObject.GetComponent<BoxCollider2D>() != true)
        {
            Debug.LogError("타일이 콜라이더를 가지고 있지 않습니다!");
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

    public void SetSelectionMode(Highlight mode) //isSelect일때는 모드가 안바뀌는게 좋을듯 걍 저것만 별도로 생각하자
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
        Debug.Log(this.gameObject + "의 주인이 바뀌었습니다. 현 주인은 " + tileOwner);
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
                mapBlockEffectText = "소유주 : " + "없음" + lineChangeText + defaultMapBlockEffectText;
                break;
            case TileOwner.Player:
                mapBlockEffectText = "소유주 : " + GameManager.instance.playerTeamName + lineChangeText + defaultMapBlockEffectText;
                break;
            case TileOwner.Enemy:
                mapBlockEffectText = "소유주 : " + GameManager.instance.enemyTeamName + lineChangeText + defaultMapBlockEffectText;
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
