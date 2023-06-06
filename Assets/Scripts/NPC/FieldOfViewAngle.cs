using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle; //시야각 (120도);
    [SerializeField] private float viewDistance; //시야거리(10미터);
    [SerializeField] private LayerMask targetMask; //타겟 마스크 (플레이어)

    private Pig thePig;
    // Update is called once per frame

    private void Start() {
        thePig = GetComponent<Pig>();
    }
    void Update()
    {
        View();
    }

    private Vector3 BoundaryAngle(float _angle) {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad)); //공식처럼 외우자
    }

    private void View() {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        //transform.up을 더해주는 이유는 너무 아래로 쏘지 않게 하기 위함
        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red); 
        
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask); //특정 거리에 있는 모든 콜라이더 정보를 저장
            for (int i = 0; i < _target.Length; i++)
            {
                Transform _targetTf = _target[i].transform;
                if(_targetTf.name == "Player") {
                    Vector3 _direction = (_targetTf.position - transform.position).normalized; //normalized는 거리말고 방향만 필요하기 때문에 쓰는것
                    float _angle = Vector3.Angle(_direction, transform.forward);

                    if(_angle < viewAngle * 0.5f) {
                        //시야 내에 있음
                        RaycastHit _hit;
                        if(Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance)) {
                            if(_hit.transform.name == "Player"){
                                Debug.Log("플레이어가 돼지 시야 내에 있습니다.");
                                Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                                thePig.Run(_hit.transform.position);
                            }
                        }
                    }
                }
            }


    }
}
