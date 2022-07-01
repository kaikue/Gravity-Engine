using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activatable : MonoBehaviour
{
    public bool startActive;

    protected virtual void Start()
    {
        if (startActive)
        {
            SetActive(true);
        }
    }

    protected abstract void SetActive(bool activated);

    public void Activate()
    {
        SetActive(!startActive);
    }

    public void Deactivate()
    {
        SetActive(startActive);
    }
}
