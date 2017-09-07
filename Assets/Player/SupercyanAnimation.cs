using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupercyanAnimation : MonoBehaviour {

    [SerializeField]
    private Camera _cam;

    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Debug.DrawLine(_cam.transform.position, _cam.transform.position - _cam.transform.forward);
        _anim.SetLookAtPosition(_cam.transform.position - _cam.transform.forward);
        _anim.SetLookAtWeight(1f);
    }
}
