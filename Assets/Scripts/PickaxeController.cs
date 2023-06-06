using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeController : CloseWeaponController
{
    //활성화 여부
    public static bool isActivate = true;

    private void Start() {
           WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
           WeaponManager.currentWeaponAnimator = currentCloseWeapon.anim;
    }

    void Update()
    {   
        if(isActivate)
        TryAttack();
    }

    protected override IEnumerator HitCoroutine()
    {
        while(isSwing) {
            if(CheckObject()) {
                //충돌했음
                if(hitInfo.transform.tag =="Rock"){
                    hitInfo.transform.GetComponent<Rock>().Mining();
                } else if(hitInfo.transform.tag == "WeakAnimal") {
                    hitInfo.transform.GetComponent<WeakAnimal>().Damage(currentCloseWeapon.damage, transform.position);
                    SoundManager.instance.PlaySE("snd_animal_hit");
                } 
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null; 
        }
    }


    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}
