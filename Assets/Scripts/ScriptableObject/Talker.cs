/* �ۼ���¥: 2023-11-06
 * ����: 0.0.1ver 
 * ����: ȭ��
 * �ֱ� ���� ��¥: 2023-11-06
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
    //ǥ�� �������� �迭 �־ ���ڷ� �����ص� ��
}

