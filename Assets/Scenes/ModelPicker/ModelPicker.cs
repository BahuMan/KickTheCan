using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPicker : MonoBehaviour {

    public Animator[] _DisplayModels;
    [Tooltip("it's important that prefabs are in the same sequence as the display models")]
    public GameObject[] _PlayerPrefabs;
    public float _ModelRadius;

    private int _CurrentPick = 0;

	// Update is called once per frame
	void Update () {
		
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
        _CurrentPick = (_CurrentPick+1) % _DisplayModels.Length
    }

    public void PickPrevious()
    {
        _CurrentPick = (_CurrentPick - 1) % _DisplayModels.Length
    }
}
