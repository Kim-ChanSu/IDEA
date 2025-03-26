using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameDatabaseManager : MonoBehaviour
{
    public static NameDatabaseManager instance; //싱글톤

    void Awake() 
    {
        // 싱글톤
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("씬에 NameDatabaseManager가 2개이상 존재합니다.");
            Destroy(gameObject);
        }
        #endregion
    }

    public const string levelName = "LV";
    public const string EXPName = "경험치";
    public const string HPName = "HP";
    public const string MPName = "MP";
    public const string SympathyName = "SP";
    public const string ATKName = "공격력";
    public const string MAKName = "마법력";
    public const string DEFName = "방어력";
    public const string MDFName = "저항력";
    public const string rangeName = "사거리";
    public const string moveName = "이동력";
    public const string goldName = "골드";

    public const string SympathyTypeNoneName = "기본";
    public const string SympathyTypeRationalName = "냉정";
    public const string SympathyTypeAngerName = "분노";
    public const string SympathyTypeEnjoyName = "열정";
}
