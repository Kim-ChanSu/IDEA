/* �ۼ���¥: 2023-11-3
 * ����: 0.0.1ver 
 * ����: ��Ʋ�� �� ��Ʈ��
 * �ֱ� ���� ��¥: 2023-11-3
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private const int BALLZPOS = 11;
    private Vector3 saveBallPosition;

    private float ballMoveTime = 0.5f;
    private float effectTime = 0;
    private Vector2[] ballTargetPoisitions;

    private float ballSpinSpeed = 5000f;

    void Start()
    {
        BallControllerInit();
    }

    void BallControllerInit()
    {
        saveBallPosition = this.gameObject.transform.position;
    }

    public void SetBallActive(bool mode)
    { 
        this.gameObject.SetActive(mode);
    }

    public void GetForBallBeforeMove()
    { 
        this.gameObject.transform.position = saveBallPosition;
        SetBallActive(true);
    }

    public void SetBallPosition(Vector2 targetPosition)
    { 
        saveBallPosition = this.gameObject.transform.position;
        this.gameObject.transform.position = new Vector3((int)targetPosition.x, (int)targetPosition.y, BALLZPOS);
    }

    public Vector3 GetBallPosition()
    {
        return saveBallPosition;
    }

    public void SetBallMoving(ref BattleManager battleManager, Vector2[] targetPosition)
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        battleManager.SetIsAnimation(true);
        ballTargetPoisitions = new Vector2[targetPosition.Length];
        for(int i = 0; i < targetPosition.Length; i++)
        { 
            ballTargetPoisitions[i] = new Vector2(targetPosition[i].x, targetPosition[i].y);
        } 

        StartBallMovePosition(ref battleManager,0);
    }

    private void StartBallMovePosition(ref BattleManager battleManager, int num)
    { 
        saveBallPosition = this.gameObject.transform.position;
        int ArrayNum = 0;
        if(num < ballTargetPoisitions.Length)
        { 
            ArrayNum = num;
        }
        else
        { 
            ArrayNum = -1;
        }

        if((ArrayNum != -1) && (ArrayNum < ballTargetPoisitions.Length))
        { 
            effectTime = 0;
            StartCoroutine(BallMoving(battleManager, this.gameObject.transform.position, new Vector3((int)ballTargetPoisitions[ArrayNum].x, (int)ballTargetPoisitions[ArrayNum].y, BALLZPOS), num));         
        }
        else
        { 
            MoveEnd(ref battleManager);
        }        
    }

    private void MoveEnd(ref BattleManager battleManager)
    { 
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(new Vector2((int)this.gameObject.transform.position.x, (int)this.gameObject.transform.position.y), Vector2.zero);
        for(int i = 0; i < hits.Length; i++)
        { 
            //�� ����ְų� ���� �ִ� ��� �ѹ� �� Ƣ��
            if ((hits[i].transform.tag == "Character") && (hits[i].transform.gameObject.GetComponent<Character>().GetIsHaveBall() == true) || (hits[i].transform.tag == "Ball") && (hits[i].transform.gameObject.GetComponent<BallController>() == true) && (hits[i].transform.gameObject != this.gameObject))
            {
                battleManager.boundAblePosition.Clear(); 
                while (battleManager.boundAblePosition.Count < 1)
                {
                    int j = 0;
                    battleManager.GetBallBoundPosition(hits[i].transform.gameObject, battleManager.defaultBallBoundRange + j, true);
                    j++;
                }

                Debug.Log("�ٿ�� ������ ��ġ�� " + battleManager.boundAblePosition.Count);
                int boundNum = Random.Range(0, battleManager.boundAblePosition.Count);
                SetBallMoving(ref battleManager, new Vector2[1] {new Vector2((int)battleManager.boundAblePosition[boundNum].transform.position.x, (int)battleManager.boundAblePosition[boundNum].transform.position.y) });
                return;
            }
            else if((hits[i].transform.tag == "Character") && (hits[i].transform.gameObject.GetComponent<Character>().GetIsHaveBall() == false))
            { 
                hits[i].transform.gameObject.GetComponent<Character>().SetIsHaveBall(true, this.gameObject);             
            }
        }

        BallSpin(false);
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);

        battleManager.SetIsAnimation(false);
        battleManager.BallMoveEnd();    
    }

    IEnumerator BallMoving(BattleManager battleManager, Vector3 ballPosition, Vector3 targetPosition, int num)
    { 
        if(effectTime < 1)
        { 
            effectTime += Time.deltaTime/ballMoveTime;
            this.gameObject.transform.position = Vector3.Lerp(ballPosition, targetPosition, effectTime);
            BallSpin(true);
            yield return null;         
            StartCoroutine(BallMoving(battleManager, ballPosition, targetPosition, num));            
        }
        else
        { 
            this.gameObject.transform.position = targetPosition;
            effectTime = 0;
            CheckCharacter(ref battleManager);

            int nextNum = num + 1;
            StartBallMovePosition(ref battleManager, nextNum);
        }      
    }

    private void BallSpin(bool mode)
    { 
        if(mode == true)
        { 
            this.gameObject.transform.Rotate(Vector3.forward * Time.deltaTime * ballSpinSpeed);
        }
        else
        { 
            this.gameObject.transform.rotation = Quaternion.Euler(0,0,0);
        }
    }  

    private void CheckCharacter(ref BattleManager battleManager) //����Ʈ, �� �ڵ��� ���� �����ϱ� ���� �������� ���� ����..
    { 
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(new Vector2 ((int)this.transform.position.x, (int)this.transform.position.y), Vector2.zero);

        for(int i = 0; i < hits.Length; i++)
        { 
            if((hits[i].transform.tag == "Character") && (hits[i].transform.GetComponent<Character>() == true))
            {
                if(hits[i].transform.gameObject == battleManager.skillTarget)
                { 
                    //���� ��ų ��󿡰� ���ư� ���
                    battleManager.skillTarget.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(battleManager.useSkill.skillTargetEffect));

                    //�� �ؿ� isEnemy�� �Ʊ����� ��ȣ�ۿ����� �ƴ��� �������� ���۳����� �������
                }
                else if(hits[i].transform.gameObject == battleManager.interruptObject)
                { 
                    //���ƿ��� ���� ���� þ ���(���� Character���� ������ ���ع��� ���� ����)
                    battleManager.interruptObject.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetShootBallInterruptSuccessEffect());

                    if(hits[i].transform.GetComponent<Character>().isEnemy == battleManager.skillTarget.transform.GetComponent<Character>().isEnemy)
                    { 
                        //���� ���� ���
                        battleManager.skillTarget.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetSafeEffect());
                    }
                    else
                    { 
                        
                    }

                }
                else if(hits[i].transform.GetComponent<Character>().GetIsHaveBall() == false)
                { 
                    //�ٿ�� �� ���� ���� ���
                    hits[i].transform.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetBoundBallSuccessEffectNumEffect());
                    
                    if(hits[i].transform.GetComponent<Character>().isEnemy == battleManager.skillTarget.transform.GetComponent<Character>().isEnemy)
                    { 
                        //���� ���� ���
                        battleManager.skillTarget.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetSafeEffect());
                    }
                    else
                    { 
                        
                    }
                }   
            }
        }
     
    }

}
