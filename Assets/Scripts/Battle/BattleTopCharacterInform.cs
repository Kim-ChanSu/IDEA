using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTopCharacterInform : MonoBehaviour
{
    [SerializeField]
    private GameObject characterFace;
    [SerializeField]
    private GameObject hpImage;
    [SerializeField]
    private GameObject characterSympathy;
    private bool isDead = false;
    private Color normalColor = new Color(255f/255f, 255f/255f, 255f/255f, 255f/255f);
    private Color isDeadColor = new Color(77f/255f, 77f/255f, 77f/255f, 255f/255f);
    private Character character;

    public void SetBattleTopCharacterInform(Character newCharacter)
    {
        character = newCharacter;
        ChangeFaceNum(character);
        SetHPImage(character);
        this.gameObject.SetActive(true);
        isDead = false;
    }

    public void UpdateCharacterInform()
    {
        ChangeFaceNum(character);
        SetHPImage(character);
    }

    private void ChangeFaceNum(Character character)
    {
        if (isDead == false)
        {
            int faceNum = (int)character.sympathyType;

            if(character.status.face.Length > faceNum) //공감 상태에 따라 얼굴바꾸기
            { 
                characterFace.gameObject.GetComponent<Image>().sprite = character.status.face[faceNum];
            }
            else
            { 
                if(character.status.face.Length > 0)
                { 
                    characterFace.GetComponent<Image>().sprite = character.status.face[0];
                }
                else
                { 
                    characterFace.GetComponent<Image>().sprite = GameManager.instance.GetDefaultFace();
                }
            }

            this.gameObject.GetComponent<Image>().sprite = GameManager.instance.battleManager.battleGetInformationManager.GetCharacterFaceMaskBackground(faceNum);

            if (faceNum < GameManager.instance.battleManager.battleGetInformationManager.GetCharacterFaceMaskBackground().Length)
            {
                characterSympathy.GetComponent<Image>().sprite = GameManager.instance.battleManager.battleGetInformationManager.GetCharacterFaceMaskBackground()[faceNum];
            }
        }
    }

    private void SetHPImage(Character character)
    {
        if ((character.GetIsDead() == false) && (isDead == true))
        {
            characterFace.GetComponent<Image>().GetComponent<Image>().color = normalColor;
            isDead = false;
        }

        if (isDead == false)
        {
            hpImage.gameObject.GetComponent<Image>().fillAmount = (float)character.status.HP / (float)character.status.maxHP;
            if (character.GetIsDead() == true)
            {
                characterFace.GetComponent<Image>().GetComponent<Image>().color = isDeadColor;
                isDead = true;
            }
        }
       
    }

    public void SetCameraToCharacter()
    {
        if ((isDead == false) && (GameManager.instance.battleManager.battlePhaseManager.GetIsEnemyTurn() == false))
        {
            for (int i = 0; i < GameManager.instance.battleManager.playerCharacters.Length; i++)
            {
                if (this.character == GameManager.instance.battleManager.playerCharacters[i].GetComponent<Character>())
                {
                    GameManager.instance.SetCameraPosition(GameManager.instance.battleManager.playerCharacters[i], true);
                    return;
                }
            }

            for (int i = 0; i < GameManager.instance.battleManager.enemyCharacters.Length; i++)
            {
                if (this.character == GameManager.instance.battleManager.enemyCharacters[i].GetComponent<Character>())
                {
                    GameManager.instance.SetCameraPosition(GameManager.instance.battleManager.enemyCharacters[i], true);
                    return;
                }
            }
        }
    }
}
