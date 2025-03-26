/* 작성날짜: 2023-11-3
 * 버전: 0.0.1ver 
 * 내용: 배틀시 공 컨트롤
 * 최근 수정 날짜: 2023-11-3
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
            //공 들고있거나 공이 있는 경우 한번 더 튀김
            if ((hits[i].transform.tag == "Character") && (hits[i].transform.gameObject.GetComponent<Character>().GetIsHaveBall() == true) || (hits[i].transform.tag == "Ball") && (hits[i].transform.gameObject.GetComponent<BallController>() == true) && (hits[i].transform.gameObject != this.gameObject))
            {
                battleManager.boundAblePosition.Clear(); 
                while (battleManager.boundAblePosition.Count < 1)
                {
                    int j = 0;
                    battleManager.GetBallBoundPosition(hits[i].transform.gameObject, battleManager.defaultBallBoundRange + j, true);
                    j++;
                }

                Debug.Log("바운드 가능한 위치는 " + battleManager.boundAblePosition.Count);
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

    private void CheckCharacter(ref BattleManager battleManager) //이펙트, 이 코드의 단점 방해하기 인지 공격인지 구별 못함..
    { 
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(new Vector2 ((int)this.transform.position.x, (int)this.transform.position.y), Vector2.zero);

        for(int i = 0; i < hits.Length; i++)
        { 
            if((hits[i].transform.tag == "Character") && (hits[i].transform.GetComponent<Character>() == true))
            {
                if(hits[i].transform.gameObject == battleManager.skillTarget)
                { 
                    //공이 스킬 대상에게 날아간 경우
                    battleManager.skillTarget.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetEffect(battleManager.useSkill.skillTargetEffect));

                    //이 밑에 isEnemy로 아군끼리 상호작용인지 아닌지 구별가능 동작넣을때 사용하자
                }
                else if(hits[i].transform.gameObject == battleManager.interruptObject)
                { 
                    //날아오는 공을 가로 첸 경우(위에 Character조건 때문에 방해물은 여기 못옴)
                    battleManager.interruptObject.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetShootBallInterruptSuccessEffect());

                    if(hits[i].transform.GetComponent<Character>().isEnemy == battleManager.skillTarget.transform.GetComponent<Character>().isEnemy)
                    { 
                        //같은 편인 경우
                        battleManager.skillTarget.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetSafeEffect());
                    }
                    else
                    { 
                        
                    }

                }
                else if(hits[i].transform.GetComponent<Character>().GetIsHaveBall() == false)
                { 
                    //바운드 한 공을 잡은 경우
                    hits[i].transform.GetComponent<Character>().StartEffect(EffectDatabaseManager.instance.GetBoundBallSuccessEffectNumEffect());
                    
                    if(hits[i].transform.GetComponent<Character>().isEnemy == battleManager.skillTarget.transform.GetComponent<Character>().isEnemy)
                    { 
                        //같은 편인 경우
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
