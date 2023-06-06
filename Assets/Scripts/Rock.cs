using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rock : MonoBehaviour
{   
    [SerializeField]
    private int hp; //바위의 체력

    [SerializeField]
    private float destroyTime; //파편 제거 시간

    [SerializeField]
    private SphereCollider col; //구체 컬라이더

    //필요한 게임 오브젝트
    [SerializeField]
    private GameObject go_rock; //일반 바위

    [SerializeField]
    private GameObject go_debris; //깨진 바위
    
    [SerializeField]
    private GameObject go_effect_prefabs;//채굴 이펙트
    [SerializeField]
    private GameObject go_rock_item_prefabs; //돌맹이 아이템

    //돌맹이 아이템 등장 갯수
    [SerializeField]
    private int go_rock_item_count_min;
    [SerializeField]
    private int go_rock_item_count_max;

    //필요한 사운드 이름
    [SerializeField]
    private string snd_strike;
    [SerializeField]
    private string snd_destroy_rock;

    public void Mining() {
        
        SoundManager.instance.PlaySE(snd_strike);
       var clone = Instantiate(go_effect_prefabs,col.bounds.center,Quaternion.identity);
       Destroy(clone, destroyTime);

        hp--;
        if(hp <= 0)
            Destruction();
    }    

    private void Destruction() {

      
        SoundManager.instance.PlaySE(snd_destroy_rock);
        col.enabled = false;

        int count = new System.Random().Next(go_rock_item_count_min, go_rock_item_count_max);
        for (int i = 0; i < count; i++)
        {
        Instantiate(go_rock_item_prefabs, go_rock.transform.position, Quaternion.identity);   
        }

        Destroy(go_rock);

        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);
    }

}
