using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPicker : MonoBehaviour {

    [Tooltip("it's important that prefabs are in the same sequence as the display models")]
    public Animator[] _DisplayModels;
    [Tooltip("it's important that prefabs are in the same sequence as the display models")]
    public GameObject[] _PlayerPrefabs;
    public float _ModelRadius;
    public float _Smoothing = .1f;

    private int _CurrentPick = 0;
    private Quaternion _TargetRotation = Quaternion.identity;

    // Update is called once per frame
    void Update () {
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, _TargetRotation, _Smoothing);
    }

    [ContextMenu("LayoutModels")]
    public void LayoutModels()
    {
        for (int i=0; i<_DisplayModels.Length; ++i)
        {
            float degrees = i * 360 / _DisplayModels.Length;
            Quaternion rot = Quaternion.Euler(0, degrees, 0);

            Transform t = _DisplayModels[i].transform;
            t.localPosition = rot * -Vector3.forward * _ModelRadius;
            t.localRotation = rot;
        }
    }

    public void PickNext()
    {
        _DisplayModels[_CurrentPick].gameObject.SetActive(false);
        _CurrentPick = (_CurrentPick + 1) % _DisplayModels.Length;
        _DisplayModels[_CurrentPick].gameObject.SetActive(true);
        _TargetRotation = Quaternion.Euler(0, _CurrentPick * 360 / _DisplayModels.Length, 0);
    }

    public void PickPrevious()
    {
        _DisplayModels[_CurrentPick].gameObject.SetActive(false);
        _CurrentPick = (_CurrentPick - 1) % _DisplayModels.Length;
        _DisplayModels[_CurrentPick].gameObject.SetActive(true);
        _TargetRotation = Quaternion.Euler(0, _CurrentPick * 360 / _DisplayModels.Length, 0);
    }
}
