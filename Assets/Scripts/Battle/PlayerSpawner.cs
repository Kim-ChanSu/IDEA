using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour
{
    private bool isPlayerGetReadySetting = false;
    private int setPlayerNum = 0;
    private BattleManager battleManager;
    [SerializeField]
    private GameObject CharacterFace;
    [SerializeField]
    private GameObject SettingText;


    void Start()
    {
        InitialzePlayerSpawner();
    }

    void Update()
    { 
        
    }

    void OnMouseOver()
    { 
        if(isPlayerGetReadySetting == true)
        {
            battleManager.CallGetMapBlockByMousePosition();
            PlayerSetting();
        }
    }

    private bool CheckNumber(int n)
    {
        #region
        if ((n % 2 == 0) && (n != 0))
        {
            return true;
        }
        else if (n % 2 == 1)
        {
            return false;
        }
        else
        {
            return false;
        }
        #endregion
    }

    private void InitialzePlayerSpawner()
    { 
        #region
        int Wid = (int)this.gameObject.GetComponent<BoxCollider2D>().size.x;
        int Hei = (int)this.gameObject.GetComponent<BoxCollider2D>().size.y;

        /*
        if(CheckNumber((int)this.gameObject.GetComponent<BoxCollider2D>().size.x) == true)
        { 
            this.gameObject.GetComponent<Transform>().position = new Vector2(-0.5f, this.gameObject.GetComponent<Transform>().position.y);
        }

        if(CheckNumber((int)this.gameObject.GetComponent<BoxCollider2D>().size.y) == true)
        { 
            this.gameObject.GetComponent<Transform>().position = new Vector2(this.gameObject.GetComponent<Transform>().position.x, -0.5f);
        }         
        */

        this.gameObject.GetComponent<Transform>().localScale = new Vector2(Wid,Hei);
        this.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1,1);
        #endregion
    }


    private void PlayerSetting()
    { 
        #region
        if((GameManager.instance.KeyCheckAccept() == true) && (setPlayerNum < battleManager.playerCharacters.Length))
        { 
            bool checker = false;

            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(new Vector2 (battleManager.selectTileXpos, battleManager.selectTileYpos), Vector2.zero);            
            for(int i = 0; i< hits.Length; i++)
            { 
                if(hits[i].transform.tag == "Tile")
                { 
                    if (hits[i].transform.GetComponent<MapBlock>().tileType == TileType.UnableMoveTile) 
                    {
                        checker = true;
                        break;
                    }
                }
                else if (GameManager.instance.CantMoveTagCheck(hits[i].transform.tag) == true)
                {
                    checker = true;
                    break;
                }
            }

            if(checker == false)
            {
                battleManager.playerCharacters[setPlayerNum].GetComponent<Character>().setXPosition = battleManager.selectTileXpos;
                battleManager.playerCharacters[setPlayerNum].GetComponent<Character>().setYPosition = battleManager.selectTileYpos;
                battleManager.playerCharacters[setPlayerNum].GetComponent<Character>().InitializeCharacter();
                battleManager.playerCharacters[setPlayerNum].SetActive(true);
                setPlayerNum++;
            }
        }
        else if((GameManager.instance.KeyCheckEscape() == true) && (setPlayerNum > 0))
        { 
            setPlayerNum--;
            battleManager.playerCharacters[setPlayerNum].SetActive(false);
        }        

        if(setPlayerNum >= battleManager.playerCharacters.Length)
        { 
            EndPlayerSetting();
        }
        else
        { 
            SettingUI();
        }
        #endregion
    }

    private void SettingUI()
    { 
        CharacterFace.GetComponent<Image>().sprite = battleManager.playerCharacters[setPlayerNum].GetComponent<Character>().status.face[0];
        SettingText.GetComponent<Text>().text = battleManager.playerCharacters[setPlayerNum].GetComponent<Character>().status.inGameName + "의 배치 위치를 정해주세요";
    }

    private void EndPlayerSetting()
    {
        isPlayerGetReadySetting = false;
        this.gameObject.SetActive(false);
        battleManager.SetCharacterSettingComplete(true);
    }


    public void StartPlayerSetting(BattleManager Manager)
    {
        isPlayerGetReadySetting = true;
        battleManager = Manager;
        SettingUI();
    }

    
}
