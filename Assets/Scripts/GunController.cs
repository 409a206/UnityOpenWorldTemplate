using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{       
    //활성화 여부
    public static bool isActivate = false;

    //현재 장착된 총
    [SerializeField]
    private Gun currentGun; 

    //연사 속도 계산    
    private float currentFireRate;

    //상태 변수
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false;

    //본래 포지션 값
    private Vector3 originPos;

    //레이저 충돌 정보 받아옴
    private RaycastHit hitInfo;
    [SerializeField]
    protected LayerMask layerMask;

    //필요한 컴포넌트
    [SerializeField]
    private Camera theCam;
    private Crosshair theCrosshair;
  
    //피격 이펙트
    [SerializeField]
    private GameObject hit_effect_prefab;

     //필요한 사운드 이름
    [SerializeField]
    private string snd_shoot;
    [SerializeField]
    private string snd_reload;

    void Start() {

        originPos = Vector3.zero;
        
        // originPos = transform.localPosition; 
        theCrosshair = FindObjectOfType<Crosshair>();

     
    }

    // Update is called once per frame
    void Update()
    {   
        if(isActivate){
        GunFireRateCalc();
        TryFire();
        TryReload();
        TryFineSight();
        }
    }

    //연사 속도 재계산
    private void GunFireRateCalc() {
        if(currentFireRate > 0) {
            currentFireRate -= Time.deltaTime; //Time.deltaTime은 1초의 역수. 대략60분의 1 // 즉 1초에 1감소
        }
    }

    //발사 시도    
    private void TryFire() {
        if(Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload && !Inventory.inventoryActivated) {
            Fire();
        }
    }

    //발사 전 계산
    private void Fire() {
        //currentFireRate = currentGun.fireRate;
        if(!isReload) {

            if(currentGun.currentBulletCount > 0)
                Shoot();
            else{
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }

    }

    //발사 후 계산
    private void Shoot() {
        theCrosshair.FireAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; //연사 속도 재계산
        currentGun.muzzleFlash.Play();
        Hit();
       SoundManager.instance.PlaySE(snd_shoot);

        //총기 반동 코루틴 실행
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());

        // Debug.Log("총알 발사함");
        
    }

    private void Hit() {
        if(Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
        new Vector3(Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy)
                   ,Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy)
                   ,0)
        , out hitInfo, currentGun.range, layerMask)) {
            //Debug.Log(hitInfo.transform.name); 

            var clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
           Destroy(clone, 2f);

        //    if(hitInfo.transform.name == "Terrain") {
        //        Debug.Log("땅을 맞춤");
        //    }

        if(hitInfo.transform.tag == "WeakAnimal") {
                    hitInfo.transform.GetComponent<WeakAnimal>().Damage(currentGun.damage, transform.position);
                    
        }
           
            
        }
    }

    //재장전 시도
    private void TryReload() {
        if(Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount) {
            CancelFineSight();
             StartCoroutine(ReloadCoroutine());
        }
    }
    
    //재장전 취소
    public void CancelReload() {
        if(isReload) {
            StopAllCoroutines();
            isReload = false;
        }
    }
    //재장전
    IEnumerator ReloadCoroutine() {
        if(currentGun.carryBulletCount > 0) {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");
           SoundManager.instance.PlaySE(snd_reload);

            //현재 탄알집에 탄알이 남아있을때 장전 시 그것도 더해주기 위함
            currentGun.carryBulletCount +=  currentGun.currentBulletCount; 
            currentGun.currentBulletCount = 0; 


            yield return new WaitForSeconds(currentGun.reloadTime);

            if(currentGun.carryBulletCount >= currentGun.reloadBulletCount) {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            } else {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
        } 
        else {
            Debug.Log("소유한 총알이 없습니다");
        }
    }

    //정조준 시도
    private void TryFineSight() {
        if(Input.GetButtonDown("Fire2") && !isReload) {
            FineSight();
        }
    }

    //정조준 취소 
    public void CancelFineSight() {
        if(isFineSightMode) {
            FineSight();
        }
    }

    //정조준 로직 가동
    private void FineSight() {
        isFineSightMode = !isFineSightMode;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrosshair.FineSightAnimation(isFineSightMode);

        if(isFineSightMode) {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        } else {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }

    }

    //정조준 활성화
    IEnumerator FineSightActivateCoroutine() {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos) {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f); //lerp는 값이 딱 떨어지지 않아서 while문을 못빠져나감
            yield return null; //1프레임 대기
        }
        
    }

    //정조준 비활성화
    IEnumerator FineSightDeactivateCoroutine() {
         while(currentGun.transform.localPosition != originPos) {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null; //1프레임 대기
        }
    }

    //반동 코루틴
    IEnumerator RetroActionCoroutine() {
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z); //정조준 안했을때의 최대 반동
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z); //정조준 했을때의 최대 반동

        //정조준 상태가 아닐경우의 반동
        if(!isFineSightMode) {
            currentGun.transform.localPosition = originPos; //다시 원래상태로 돌림

            //반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f) { //0.02f뺀것은 lerp가 완벽하게 떨어지지 않기 때문에 여유를 준 것임
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            //원위치
            while(currentGun.transform.localPosition != originPos) {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        } else { //정조준 상태일때의 반동
             currentGun.transform.localPosition = currentGun.fineSightOriginPos; //다시 원래상태로 돌림
                //반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f) { //0.02f뺀것은 lerp가 완벽하게 떨어지지 않기 때문에 여유를 준 것임
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            //원위치
            while(currentGun.transform.localPosition != currentGun.fineSightOriginPos) {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }

    }


    public Gun GetGun() {
        return currentGun;
    }

    public bool GetFineSightMode() {
        return isFineSightMode;
    }

    public void GunChange(Gun _gun) {
        if(WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false);

        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnimator = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);

        isActivate = true;
    }

}
