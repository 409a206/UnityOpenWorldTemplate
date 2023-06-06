using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Sound{
    public string name; //곡의 이름
    public AudioClip clip; //곡
}

//싱글턴. singleton. 1개
public class SoundManager : MonoBehaviour
{
    static public SoundManager instance;

    #region singleton
    //n개 유지시킬수도 있지만, 보통 1개만 살려둠
    private void Awake() { //객체 생성시 최초 실행
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            //(Scene 이동등으로 인하여) 새로 객체가 만들어졌을 경우 기존의것은 살려두고 새로 만들어진것은 파괴
            Destroy(this.gameObject);  //this. 생략 가능
    }
    #endregion


    // private void OnEnable() { //매번 활성화시 실행 //코루틴 실행 x
        
    // }

    
    // void Start() //매번 활성화된 실행. //코루틴 실행 o
    // {
        
    // }

    // Update is called once per frame


    public AudioSource[] audioSourceEffects;
    public AudioSource[] audioSourceBGM;

    public string[] playSoundName;

    [SerializeField]
    public Sound[] effectSounds;
    [SerializeField]
    public Sound[] BGMSounds;

    private void Start() {
        playSoundName = new string[audioSourceEffects.Length + audioSourceBGM.Length];
        
        //임시코드
        Scene m_Scene = SceneManager.GetActiveScene();
        if(m_Scene.name == "SampleScene"){
        PlaySE("snd_main_scene"); 
        }
        
    }
    public void PlaySE(string _name) {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if(_name == effectSounds[i].name) {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if(!audioSourceEffects[j].isPlaying) {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return; 
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
    }

    public void StopAllSE() {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name) {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if(playSoundName[i] == _name) {
                audioSourceEffects[i].Stop();
                break;
            }  
        }
        Debug.Log("재생중인 " + _name + " 사운드가 없습니다.");
    }

    public void PlayBGM(string _name) {
        for (int i = 0; i < BGMSounds.Length; i++)
        {
            if(_name == BGMSounds[i].name) {
                for (int j = 0; j < audioSourceBGM.Length; j++)
                {
                    if(!audioSourceBGM[j].isPlaying) {
                        playSoundName[j] = BGMSounds[i].name;
                        audioSourceBGM[j].clip = BGMSounds[i].clip;
                        audioSourceBGM[j].Play();
                        return; 
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
    }

    public void StopAllBGM() {
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            audioSourceBGM[i].Stop();
        }
    }

    public void StopBGM(string _name) {
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            if(playSoundName[i] == _name) {
                audioSourceBGM[i].Stop();
                break;
            }  
        }
        Debug.Log("재생중인 " + _name + " 사운드가 없습니다.");
    }
}
