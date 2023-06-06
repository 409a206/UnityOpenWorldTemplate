using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Water : MonoBehaviour
{   
    [SerializeField] private float waterDrag; //물속 중력
    private float originDrag;

    [SerializeField] private Color waterColor; //물속 색깔
    [SerializeField] private float waterFogDensity; //물 탁함 정도
    [SerializeField] private Color nightWaterColor; //밤 물속 색깔
    [SerializeField] private float nightWaterFogDensity; // 밤 물 탁함 정도

    [SerializeField]
    private Color originNightColor;
    [SerializeField]
    private float originNightForDensity;
    private Color originColor;
    private float originFogDensity;

    [SerializeField] private string snd_water_out;
    [SerializeField] private string snd_water_in;
    [SerializeField] private string snd_water_breath;

    [SerializeField] private float breathTime;
    private float currentBreathTime;

    [SerializeField] private float totalOxygen;
    private float currentOxygen;
    private float temp;
    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private Text text_totalOxygen;
    [SerializeField] private Text text_currentOxygen;
    [SerializeField] private Image image_gauge;

    private StatusController thePlayerStat;

    // Start is called before the first frame update
    void Start()
    {
        originColor = RenderSettings.fogColor;
        originFogDensity = RenderSettings.fogDensity;
        originDrag = 0;
        thePlayerStat = FindObjectOfType<StatusController>();
        currentOxygen = totalOxygen;

        text_totalOxygen.text = totalOxygen.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.isWater) {
           currentBreathTime += Time.deltaTime;
           if(currentBreathTime >= breathTime) {
               SoundManager.instance.PlaySE(snd_water_breath);
               currentBreathTime = 0;
           }
        }

        DecreaseOxygen();
    }

    private void DecreaseOxygen() {
        if(GameManager.isWater) {
            currentOxygen -= Time.deltaTime;
            text_currentOxygen.text = Mathf.RoundToInt(currentOxygen).ToString();
            image_gauge.fillAmount = currentOxygen / totalOxygen;
            if(currentOxygen <= 0) {
                temp += Time.deltaTime;
                if(temp >= 1) {
                    thePlayerStat.DecreaseHp(1);
                    temp = 0;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.tag =="Player") {
            GetInWater(other);
        }
    }
    private void OnTriggerExit(Collider other) {
        GetOutWater(other);
    }

    private void GetInWater(Collider _player) {
        SoundManager.instance.PlaySE(snd_water_in);
        go_BaseUI.SetActive(true);
        _player.transform.GetComponent<Rigidbody>().drag = waterDrag;
        GameManager.isWater = true;

        if(GameManager.isNight) {
             RenderSettings.fogColor = nightWaterColor;
            RenderSettings.fogDensity = nightWaterFogDensity;
        } else {
            RenderSettings.fogColor = waterColor;
            RenderSettings.fogDensity = waterFogDensity;
        }

    }

    private void GetOutWater(Collider _player) {
        if(GameManager.isWater) {
            SoundManager.instance.PlaySE(snd_water_out);
            go_BaseUI.SetActive(false);
            currentOxygen = totalOxygen;
                GameManager.isWater = false;
                _player.transform.GetComponent<Rigidbody>().drag = originDrag;

            if(!GameManager.isNight){
                RenderSettings.fogColor = originColor;
                RenderSettings.fogDensity = originFogDensity;
            } else {
                 RenderSettings.fogColor = originNightColor;
                RenderSettings.fogDensity = originNightForDensity;
            }
        }
    }
}
