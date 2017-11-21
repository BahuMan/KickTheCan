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

    void Update () {
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, _TargetRotation, _Smoothing);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.PickPrevious();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.PickNext();
        }
    }

    //this method is not called at runtime, but from the editor to put the models in a nice circle
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

    //public method is called by buttonpress
    public void PickNext()
    {
        SetPick((_CurrentPick + 1) % _DisplayModels.Length);
    }

    //public method is called by buttonpress
    public void PickPrevious()
    {
        SetPick((_CurrentPick - 1) % _DisplayModels.Length);
    }

    private void SetPick(int p)
    {
        _CurrentPick = p;
        //rotate the modelpicker in order to make current model center of view:
        _TargetRotation = Quaternion.Euler(0, -_CurrentPick * 360 / _DisplayModels.Length, 0);
        Debug.Log("current choice: " + _DisplayModels[_CurrentPick].name);
    }

    //public method is called by buttonpress
    public void ChooseCurrent()
    {
        Debug.Log("Model chosen: " + _DisplayModels[_CurrentPick].name);

    }
}
