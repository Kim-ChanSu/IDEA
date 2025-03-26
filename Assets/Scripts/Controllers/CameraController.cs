/* 작성날짜: 2023-09-25
 * 버전: 0.0.1ver 
 * 내용: 카메라의 이동 제어
 * 최근 수정 날짜: 2023-09-26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float CAMERAMAXSIZE = 9f;
    private const float CAMERAMINSIZE = 5f;

    private Camera theCamera;

    private float halfWidth; 
    private float halfHeight; 
    private Vector3 minInvisibleWall;
    private Vector3 maxInvisibleWall;

    private Vector3 CameraTargetPosition;
    private Vector3 defaultCameraPosition;

    private float temp_value;
    private float defaultCameraSize;
    private float effectTime;

    public float cameraMoveSpeed;
    public float cameraZoomSpeed;
    public float cameraWarpTime;
    [SerializeField]
    private BoxCollider2D invisibleWall;

    private bool isCameraMoveByTarget = false; //카메라가 대상을 따라이동
    private GameObject cameraMoveTargetObject; //카메라가 대상을 따라이동
    
    private bool isCameraMoveByPosition = false;

    void Start() 
    {
        InitialzeCamera();
    }

    void Update()
    {
        CameraMoveCheck();
    }

    private void CameraMoveCheck()
    { 
        if(GameManager.instance.battleManager.IsCanCameraMoveCheck() == true)
        { 
            if(isCameraMoveByTarget == true)
            { 
                CameraMoveByTarget();
            }
            else if(isCameraMoveByPosition == true)
            { 
                
            }
            else
            {
                KeyCheck();
                CameraZoom();
            }
        }        
    }

    private void InitialzeCamera()
    { 
        theCamera = GetComponent<Camera>();
        halfHeight = theCamera.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;
        CameraTargetPosition = this.transform.position;

        defaultCameraSize = this.gameObject.GetComponent<Camera>().orthographicSize;
        defaultCameraPosition = this.gameObject.transform.position;       
        GameManager.instance.cameraController = this.gameObject.GetComponent<CameraController>();
        effectTime = 0;

        isCameraMoveByTarget = false;
        SetCameraClamp();
    }

    private void SetCameraClamp()
    { 
        if(invisibleWall != null)
        { 
            minInvisibleWall= invisibleWall.bounds.min;
            maxInvisibleWall = invisibleWall.bounds.max;                    
            halfHeight = theCamera.orthographicSize;
            halfWidth = halfHeight * Screen.width / Screen.height;
            CameraClamp();
        }
    }

    private void CameraClamp()
    { 
        if(invisibleWall != null)
        { 
            float clampedX = Mathf.Clamp(this.transform.position.x, minInvisibleWall.x + halfWidth, maxInvisibleWall.x - halfWidth);
            float clampedY = Mathf.Clamp(this.transform.position.y, minInvisibleWall.y + halfHeight, maxInvisibleWall.y - halfHeight);

            this.transform.position = new Vector3(clampedX, clampedY, this.transform.position.z); 
        }
    }

    private void KeyCheck()
    { 
        if(GameManager.instance.KeyCheckUp() == true)
        { 
            CameraTargetPosition = new Vector3 (this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
            CameraMove();
        }    

        if(GameManager.instance.KeyCheckDown() == true)
        { 
            CameraTargetPosition = new Vector3 (this.transform.position.x, this.transform.position.y - 1, this.transform.position.z);
            CameraMove();
        }    

        if(GameManager.instance.KeyCheckLeft() == true)
        { 
            CameraTargetPosition = new Vector3 (this.transform.position.x - 1, this.transform.position.y, this.transform.position.z);
            CameraMove();
        }

        if(GameManager.instance.KeyCheckRight() == true)
        { 
            CameraTargetPosition = new Vector3 (this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
            CameraMove();
        }    
        
    }

    private void CameraMove()
    { 
        this.transform.position = Vector3.Lerp(this.transform.position, CameraTargetPosition, cameraMoveSpeed * Time.deltaTime);   
        CameraClamp();
    }

    private void CameraZoom()
    { 
        if (GameManager.instance.CantCameraZoomCheck() == false)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel") * cameraZoomSpeed;

            // scroll < 0 : scroll down하면 줌인
            if ((theCamera.orthographicSize <= CAMERAMINSIZE) && (scroll > 0))
            {
                temp_value = theCamera.orthographicSize;
                theCamera.orthographicSize = temp_value; // maximize zoom in
                SetCameraClamp();
               // 최대로 Zoom in 했을 때 특정 값을 지정했을 때

               // 최대 줌 인 범위를 벗어날 때 값에 맞추려고 한번 줌 아웃 되는 현상을 방지
            }

            // scroll > 0 : scroll up하면 줌아웃
            else if ((theCamera.orthographicSize >= CAMERAMAXSIZE )&& (scroll < 0))
            {
                temp_value = theCamera.orthographicSize;
                theCamera.orthographicSize = temp_value; // maximize zoom out
                SetCameraClamp();
            }
            else
            { 
                theCamera.orthographicSize -= scroll * 0.5f;
                SetCameraClamp();
            } 
        }       
    }

    private void CameraMoveByTarget()
    {
        if (cameraMoveTargetObject != null)
        {
            CameraTargetPosition.Set(cameraMoveTargetObject.transform.position.x, cameraMoveTargetObject.transform.position.y, this.transform.position.z);
            this.transform.position = Vector3.Lerp(this.transform.position, CameraTargetPosition, cameraMoveSpeed * Time.deltaTime);
            CameraClamp();
        }
        else
        {
            isCameraMoveByTarget = false;
        }
    }

    public void SetCameraPosition(GameObject target, bool ignoreMovingIfAlreadyMoving = false)
    {                 
        if (isCameraMoveByPosition == true && ignoreMovingIfAlreadyMoving == false)
        {
            StopCoroutine(CameraMoving(this.gameObject.transform.position, this.gameObject.transform.position));
        }
        else if(isCameraMoveByPosition == true && ignoreMovingIfAlreadyMoving == true)
        {
            return;
        }
        effectTime = 0;
        isCameraMoveByPosition = true;
        StartCoroutine(CameraMoving(this.gameObject.transform.position, new Vector3(target.gameObject.transform.position.x, target.gameObject.transform.position.y, this.gameObject.transform.position.z)));  
    }

    public void resetCamera()
    { 
        this.gameObject.GetComponent<Camera>().orthographicSize = defaultCameraSize;
        this.gameObject.transform.position = defaultCameraPosition;   
    }

    public void SetCameraMoveTarget(GameObject target)
    {
        isCameraMoveByTarget = true;
        cameraMoveTargetObject = target;
    }

    public void ResetCameraMoveTarget()
    {
        isCameraMoveByTarget = false;
        cameraMoveTargetObject = null;
    }

    IEnumerator CameraMoving(Vector3 cameraPosition,Vector3 targetPosition)
    { 
        if (isCameraMoveByTarget == false)
        {
            if(effectTime < 1)
            { 
                effectTime += Time.deltaTime/cameraWarpTime;
                this.gameObject.transform.position = Vector3.Lerp(cameraPosition, targetPosition, effectTime);
                CameraClamp();
                yield return null;         
                StartCoroutine(CameraMoving(cameraPosition, targetPosition));            
            }
            else
            {                 
                this.gameObject.transform.position = targetPosition;
                CameraClamp();
                effectTime = 0;
                isCameraMoveByPosition = false;
            }
        }  
        else
        {
            effectTime = 0;
            isCameraMoveByPosition = false;
        }
    }
}
