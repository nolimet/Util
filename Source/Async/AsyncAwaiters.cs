using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Linq;

public static class AsyncAwaiters
{
    public static WaitForUpdate NextFrame { get; } = new WaitForUpdate();

    public static WaitForFixedUpdate FixedUpdate { get; } = new WaitForFixedUpdate();

    public static WaitForEndOfFrame EndOfFrame { get; } = new WaitForEndOfFrame();

    public static WaitForSeconds Seconds(float seconds)
    {
        return new WaitForSeconds(seconds);
    }

    public static WaitForSecondsRealtime SecondsRealtime(float seconds)
    {
        return new WaitForSecondsRealtime(seconds);
    }

    public static WaitUntil Until(Func<bool> predicate)
    {
        return new WaitUntil(predicate);
    }

    public static WaitWhile While(Func<bool> predicate)
    {
        return new WaitWhile(predicate);
    }
}