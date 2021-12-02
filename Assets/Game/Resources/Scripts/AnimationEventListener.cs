using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Diagnostics;

public class AnimationEventListener : MonoBehaviour
{
    [HideInInspector] public UnityEvent footStepEvent = new UnityEvent();
    [HideInInspector] public UnityEvent attackEvent = new UnityEvent();
    StackTrace stackTrace = new StackTrace(); 

    public void FootstepEvent()
    {
        footStepEvent.Invoke();
    }
    public void AttackEvent()
    {
        attackEvent.Invoke();
        // if (stackTrace != null && stackTrace.GetFrame(1) != null)
        //     UnityEngine.Debug.Log(stackTrace.GetFrame(1).GetMethod().Name);
        // else
        //     UnityEngine.Debug.Log("weirdo");
    }
}
