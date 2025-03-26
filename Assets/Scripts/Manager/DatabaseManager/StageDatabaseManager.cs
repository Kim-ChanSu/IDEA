using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDatabaseManager : MonoBehaviour
{
    public static StageDatabaseManager instance; 

    void Awake() 
    {
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("���� StageDatabaseManager 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
    }

    [SerializeField]
    private Stage[] stage;

    [SerializeField]
    private Stage[] subStage;

    public Stage GetStage(int i)
    {
        #region
        if(0 <= i && i < stage.Length)
        {
            return stage[i];
        }
        else
        {
            Debug.LogWarning("StageDatabaseManager�� GetStage�Լ��� �߸��� ���� ���Խ��ϴ�.");
            return stage[0];
        }
        #endregion
    }

    public int GetStageLength()
    { 
        return stage.Length;
    }

    public Stage GetSubStage(int i)
    {
        #region
        if(0 <= i && i < subStage.Length)
        {
            return subStage[i];
        }
        else
        {
            Debug.LogWarning("StageDatabaseManager�� GetSubStage�Լ��� �߸��� ���� ���Խ��ϴ�.");
            return subStage[0];
        }
        #endregion
    }

    public int GetSubStageLength()
    {
        return subStage.Length;
    }
}
