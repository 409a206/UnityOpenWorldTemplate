using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{

    //활성화 여부
    public static bool isActivate = false;

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
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null; 
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        //base = java의 super
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}
