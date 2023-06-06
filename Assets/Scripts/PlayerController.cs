using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //스피드 조정 변수
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float swimSpeed;
    [SerializeField]
    private float swimFastSpeed;
    [SerializeField]
    private float upSwimSpeed;
    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float crouchSpeed;

    //상태 변수
    private bool isWalk = false;
    
 
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;
    
    //움직임 체크 변수
    private Vector3 lastPos;

    //앉았을 때 얼마나 앉을 지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY; 

    //땅 착지 여부
    private CapsuleCollider capsuleCollider;

    [SerializeField] //보호수준을 유지하면서 inspector 창에서 볼수 있게 해줌
                     //하지만 유니티에선 권장하지 않음
    private float walkSpeed;
    
    //민감도
    [SerializeField]
    private float lookSensitivity;

    //카메라 한계
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    //필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    private Crosshair theCrosshair;
    private StatusController theStatusController;


    [SerializeField]
    private GunController theGunController;



    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        //theCamera = FindObjectOfType<Camera>();
        myRigid = GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();
        theStatusController = FindObjectOfType<StatusController>();


        //초기화
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y; //카메라는 플레이어 내부에 있는 객체이기 때문에 localPosition을 씀
        applyCrouchPosY = originPosY;

    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.canPlayerMove){
            WaterCheck();
            IsGround();
            TryJump();

            if(!GameManager.isWater){
                TryRun();
            }

            TryCrouch();
            float checkMoveXZ = Move();
            MoveCheck(checkMoveXZ);
            // if(!Inventory.inventoryActivated){
                cameraRotation();
                CharacterRotation();
            // }
        }
    }
    private void WaterCheck(){
        if(GameManager.isWater) {
            if(Input.GetKeyDown(KeyCode.LeftShift)) {
                applySpeed = swimFastSpeed;
            } else {
                applySpeed = swimSpeed;
            }
        }
    }

    //앉기 시도
    private void TryCrouch() {
        if(Input.GetKeyDown(KeyCode.LeftControl)) {
            Crouch();
        }
    }


    //앉기 동작
    private void Crouch() {
        
        isCrouch = !isCrouch;
        // if(isCrouch) {
        //     isCrouch = false;
        // } else {
        //     isCrouch = true;
        // }  //이거와 같은뜻임

        theCrosshair.CrouchingAnimation(isCrouch);

        if(isCrouch) {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        } else {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        // theCamera.transform.localPosition = new Vector3(
        //     theCamera.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z);

        StartCoroutine(CrouchCoroutine());
    }

    //부드러운 동작 실행
    //병렬처리 가능. 즉, 메소드가 실행될 때 코루틴 함수 밑의 실행 항목들과 코루틴함수가 함깨 처리됨
    IEnumerator CrouchCoroutine() {
        
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(_posY != applyCrouchPosY) {
            count++;

            //_posY = Mathf.Lerp(1, 2, 1); //1에서 2까지 1의 비율로 증가 //보간이동 
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if(count > 15)
                break; //보간이 무한반복되지 않게 처리한것
            

            yield return null; //null은 한 프레임 대기
        }

        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);

            //yield return new WaitForSeconds(1f);

    }

    //지면 체크
    private void IsGround() {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f); //대각선 땅이나 오차때문에 0.1f만큼의 여유를 준 것
        theCrosshair.JumpingAnimation(!isGround);
    }

    //점프 시도
    private void TryJump() {
        if(Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetCurrentSp() > 0 && !GameManager.isWater) {
            Jump();
        } else if(Input.GetKeyDown(KeyCode.Space) && GameManager.isWater) {
            SwimUp();
        }
    }

    private void SwimUp() {
        myRigid.velocity = transform.up * upSwimSpeed;
    }

    private void Jump() {
        
        //앉은 상태에서 점프 시 앉은 상태 해제
        if(isCrouch)
            Crouch();
        theStatusController.DecreaseStamina(100);
        myRigid.velocity = transform.up * jumpForce;
    }

    //달리기 시도
    private void TryRun(){
        
        if(Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSp() > 0) {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSp() <=  0) {
            RunningCancel();
        }
    }

    //달리기 실행
    private void Running() {

        if(isCrouch)
            Crouch();
            
        theGunController.CancelFineSight();

        isRun = true;
        theCrosshair.RunningAnimation(isRun);
         theStatusController.DecreaseStamina(10);
        applySpeed = runSpeed;
    }

    //달리기 취소
    private void RunningCancel() {
        isRun = false;
        applySpeed = walkSpeed;
        theCrosshair.RunningAnimation(isRun);
    }

    //움직임 실행
    private float Move(){
        
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // 1 -1 0
        float _moveDirZ = Input.GetAxisRaw("Vertical");
        float moveXZAbsSum = Mathf.Abs(_moveDirX) + Mathf.Abs(_moveDirZ);
        
        Vector3 _moveHorizontal = transform.right * _moveDirX; //transform.right는 (1,0,0)을 의미
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        //(1,0,0) (0,0,1)
        //(1,0,1)--> 합이 2가됨
        //.normalize하면 (0.5, 0, 0.5) 합이 1이됨. 결국 방향은 같음-- > 유니티에서 권장
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        //Time.deltaTime 하는이유는 순간이동 방지
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

        return moveXZAbsSum;
    }

    private void MoveCheck(float moveXZ) {
        
        if(!isRun && !isCrouch && isGround){

         //if(Vector3.Distance(lastPos, transform.position)>= 0.01f) //약간의 경사면에서 미끄러지는걸 걷는거로 착각하지 않게 하기 위해 이정도의 오차를 둠
         if(moveXZ != 0)
            isWalk = true;
         else
            isWalk = false;
        
        theCrosshair.WalkingAnimation(isWalk);
         lastPos = transform.position;    
        }     
    }
    //좌우 캐릭터 회전
    private void CharacterRotation() {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0, _yRotation, 0) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        
        //Debug.Log(myRigid.rotation);
       //Debug.Log(myRigid.rotation.eulerAngles);


    }

    //상하 카메라 회전
    private void cameraRotation(){
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        
        //설정값 안에 가두기
        currentCameraRotationX 
        = Mathf.Clamp(
            currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0, 0);
    }


}
