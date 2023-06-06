using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakAnimal : Animal
{
     public void Run(Vector3 _targetPos) {
        //direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles;

        destination = new Vector3(transform.position.x - _targetPos.x, 0f, transform.position.z - _targetPos.z).normalized;

        currentTime = runTime;
        //applySpeed = runSpeed;
        nav.speed = runSpeed;
        isWalking = false;
        isRunning = true;
        anim.SetBool("Running", isRunning);
        Debug.Log("뛰기");
    }
    
    public override void Damage(int _dmg, Vector3 _targetPos) {
        base.Damage(_dmg, _targetPos);
        if(!isDead)
              Run(_targetPos);
    }
}
