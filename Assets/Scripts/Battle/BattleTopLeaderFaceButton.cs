using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTopLeaderFaceButton : MonoBehaviour
{
    [SerializeField]
    private bool isEnemy;

    public void SetCameraToCharacter()
    {
        if (GameManager.instance.battleManager.battlePhaseManager.GetIsEnemyTurn() == false)
        {
            GameObject targetObject;

            if (isEnemy == true)
            {
                targetObject = GameManager.instance.battleManager.enemyCharacters[GameManager.instance.enemyLeaderNum];
            }
            else
            { 
                targetObject = GameManager.instance.battleManager.playerCharacters[GameManager.instance.playerLeaderNum];
            }

            if (targetObject != null)
            {
                GameManager.instance.SetCameraPosition(targetObject, true);
                return;
            }
        }
    }
}
