using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{   
      [SerializeField]
    protected string animalName; //동물의 이름
    [SerializeField]
    protected int hp; //체력
    [SerializeField]
    protected float walkSpeed;// 걷기속도
    [SerializeField]
    protected float runSpeed;
    [SerializeField]
    //protected float turningSpeed; //회전 스피드
 
    //protected float applySpeed;

    //protected Vector3 direction;

    protected Vector3 destination; //목적지



    //상태 변수
    protected bool isWalking; //걷는지 안걷는지 판별
    protected bool isAction; //행동중인지 아닌지 판별
    protected bool isRunning;//걷는지 아닌지 판별
    protected bool isDead; //죽었는지 판별


    [SerializeField]
    protected float walkTime; //걷기 시간
    [SerializeField]
    protected float waitTime; //대기 시간
    [SerializeField]
    protected float runTime;
    protected float currentTime;

    //필요한 컴포넌트
    [SerializeField]
    protected Animator anim;
    [SerializeField]
    protected Rigidbody rigid;
    [SerializeField]
    protected BoxCollider boxCol;
    protected AudioSource theAudio;
    protected NavMeshAgent nav;
    [SerializeField] protected AudioClip[] snd_normal;
    [SerializeField] protected AudioClip snd_hurt;
    [SerializeField] protected AudioClip snd_dead;

   // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>(); 
        theAudio = GetComponent<AudioSource>();
        currentTime = waitTime;
        isAction = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead){ 
            Move();
            //Rotation();
            ElapseTime();
        }
    }

    protected void Move() {
        if(isWalking || isRunning) {
            //rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime));
              nav.SetDestination(transform.position + destination * 5f);   
            
        }
    }

    // protected void Rotation() {
    //     if(isWalking || isRunning) {
    //         Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y, 0f), turningSpeed);
    //         rigid.MoveRotation(Quaternion.Euler(_rotation)); 
    //     }
    // }
    protected void ElapseTime() {
        if(isAction) {
            currentTime -= Time.deltaTime;
            //Debug.Log(Time.deltaTime);
            if(currentTime <= 0) {
                ResetRandomAction();
            } 
        }
    }

    protected virtual void ResetRandomAction() {
        isWalking = false;
        isRunning = false;
        isAction = true;
        //applySpeed = walkSpeed;
        nav.speed = walkSpeed;
        anim.SetBool("Walking", isWalking); 
        anim.SetBool("Running", isRunning);
        //direction.Set(0f, Random.Range(0f, 360f), 0f);
        nav.ResetPath(); //목적지 초기화(없앰)
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(0.5f, 1f));
       
    }

  

   
     protected void TryWalk() {
         currentTime = walkTime;
         //applySpeed = walkSpeed;
         nav.speed = walkSpeed;
         isWalking = true;
         anim.SetBool("Walking", isWalking);
         Debug.Log("걷기");
    }

    public virtual void Damage(int _dmg, Vector3 _targetPos) {
        if(!isDead) {

        hp -= _dmg;

        if(hp <= 0) {
           Dead();
            return;
        }

        PlaySE(snd_hurt);
        anim.SetTrigger("Hurt");
      
        }
    }

    protected void Dead() {
        PlaySE(snd_dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        anim.SetTrigger("Dead");
    }

    protected void RandomSound() {
        int _random = Random.Range(0, 3); //일상 사운드 3개
        PlaySE(snd_normal[_random]);
    }

    protected void PlaySE(AudioClip _clip) {
        theAudio.clip = _clip;
        theAudio.Play();
    }

}
