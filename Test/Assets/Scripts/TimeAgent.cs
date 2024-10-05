using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAgent : MonoBehaviour
{
    public Action onTimeTick;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
    GameManager.instance.TimeController.Sub(this);
    }
    public void Invoke()
    {
        onTimeTick?.Invoke();
    } 
    void OnDestroy()
    {
        GameManager.instance.TimeController.UnSub(this);
    }
}
