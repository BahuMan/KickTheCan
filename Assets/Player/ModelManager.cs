using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour {

    public Animator[] ModelPrefabs;

    public Animator CreateAnimatedModel(int index)
    {
        Animator result = Instantiate<Animator>(ModelPrefabs[index]);
        return result;
    }

    /**
     * by routing the discard through this code, I can make use of object pools if I want to
     */
    public void discardAnimatedModel(Animator model)
    {
        Destroy(model);
    }
}
