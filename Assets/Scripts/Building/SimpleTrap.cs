using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTrap : MonoBehaviour
{
    private Rigidbody[] rigid;
    [SerializeField]
    private GameObject go_Meat;

    [SerializeField] private int damage;

    private bool isActivated = false;

    private AudioSource theAudio;
    [SerializeField] private AudioClip snd_activate_trap;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponentsInChildren<Rigidbody>();
        theAudio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        if(!isActivated) {
            if(other.transform.tag != "Untagged") {
                isActivated = true;
                theAudio.clip = snd_activate_trap;
                theAudio.Play();
                Destroy(go_Meat); //고기 제거

                for (int i = 0; i < rigid.Length; i++)
                {
                    rigid[i].useGravity = true;
                    rigid[i].isKinematic = false;
                }

                if(other.transform.tag == "Player") {
                    other.transform.GetComponent<StatusController>().DecreaseHp(damage);
                }
            }
        }
    }


}
