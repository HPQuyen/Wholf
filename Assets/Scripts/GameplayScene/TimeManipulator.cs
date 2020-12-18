using System;
using System.Collections;
using UnityEngine;

public class TimeManipulator : MonoBehaviour
{
    [SerializeField]
    private static TimeManipulator instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }


    public static TimeManipulator GetInstance()
    {
        return instance;
    }

    public void InvokeRepeatAction(float seconds, Action action, Action onCompleteAction)
    {
        StartCoroutine(Foo(seconds, action, onCompleteAction));
    }
    public void InvokeActionAfterSeconds(float seconds, Action action)
    {
        StartCoroutine(Foo(seconds, action));
    }
    IEnumerator Foo(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }
    IEnumerator Foo(float seconds, Action action, Action onCompleteAction)
    {
        yield return new WaitForSeconds(1f);
        seconds -= 1;
        if(seconds <= 0)
        {
            onCompleteAction.Invoke();
        }
        else
        {
            action.Invoke();
        }
    }
}
