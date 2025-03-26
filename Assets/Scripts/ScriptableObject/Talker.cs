/* 작성날짜: 2023-11-06
 * 버전: 0.0.1ver 
 * 내용: 화자
 * 최근 수정 날짜: 2023-11-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Talker")]
public class Talker : ScriptableObject
{
    public string callName;
    public string inGameName;
    public Sprite[] characterImage;
    public float talkSpeed;
    //표정 넣을꺼면 배열 넣어서 숫자로 구별해도 됨
}

