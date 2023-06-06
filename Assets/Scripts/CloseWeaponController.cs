using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//미완성 클래스 = 자식클래스
public abstract class CloseWeaponController : MonoBehaviour
{
    
    //현재 장착된 Hand형 타입 무기
    [SerializeField]
    protected CloseWeapon currentCloseWeapon;

    //공격중? //protected 접근제한자는 상속받는 클래스에서 접근 가능
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitInfo;
    [SerializeField]
    protected LayerMask layerMask;

    // Update is called once per frame
    
    protected void TryAttack(){

        if(Input.GetButton("Fire1") && !Inventory.inventoryActivated) {
                if(!isAttack) {
                    //딜레이를 위해 코루틴 실행
                    StartCoroutine(AttackCoroutine());
                }
        }
    }

    protected IEnumerator AttackCoroutine() {
        
        isAttack = true;
        currentCloseWeapon.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayA);
        isSwing = true;

        // 공격 활성화 시점.
        StartCoroutine(HitCoroutine());




        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentCloseWeapon.attackDelay - currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB);
        isAttack = false;

    }

    //미완성 = 추상 코루틴. 자식클래스가 완성시켜야함
    protected abstract IEnumerator HitCoroutine(); 

    protected bool CheckObject() {

        if(Physics.Raycast(transform.position
                            //, transform.TransformDirection(Vector3.forward) 라고 써도됨
                            , transform.forward
                            , out hitInfo
                            , currentCloseWeapon.range, layerMask)) {
            return true; //충돌한게 있을경우

        }

        return false; //없을경우
    }

    //가상 함수 = 완성함수이지만 추가 편집이 가능한 함수
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon) {
        if(WeaponManager.currentWeapon != null) {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }
        currentCloseWeapon = _closeWeapon;
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnimator = currentCloseWeapon.anim;

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);

       
    }

}
