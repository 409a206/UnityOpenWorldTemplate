using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true; //플레이어의 움직임 제어

    public static bool isOpenInventory = false; //인벤토리 활성화

    public static bool isOpenCraftManual = false; //건축 메뉴창 활성화

    public static bool isNight = false;
    public static bool isWater = false;
    public static bool isPause = false; //메뉴가 호출되면 true

    private bool flag = false;

    private WeaponManager theWM;


    // Start is called before the first frame update
    void Start()
    {
        theWM = FindObjectOfType<WeaponManager>();
       Cursor.lockState = CursorLockMode.Locked;
       Cursor.visible = false; // Cursor.lockState = CursorLockMode.Locked; <-- 이걸 실행하면 Cursor.visible = false;도 자동적으로 실행됨
    }

    // Update is called once per frame
    void Update()
    {
         if(isOpenInventory || isOpenCraftManual || isPause){
              Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canPlayerMove = false;
         }
        else{
            canPlayerMove = true;    
        }

        if(isWater) {
            if(!flag){
                StopAllCoroutines();
                StartCoroutine(theWM.WeaponInCoroutine());
                flag = true;
            }
        } else {
            if(flag) {
                flag = false;
                theWM.WeaponOut();
            }
        }
    }
}
