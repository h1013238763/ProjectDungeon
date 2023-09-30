using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

/// <summary>
/// Memory management and garbage recycling module
/// </summary>
public class MemoryController : BaseController<MemoryController>
{
    /// <summary>
    /// Force collect garbage
    /// </summary>
    public void ForceCollectGarbage(){
        System.GC.Collect();
    }

    /// <summary>
    /// Force collect garbage async
    /// </summary>
    /// <param name="t">time for garbage collecting</param>
    public void ForceCollectGarbageAsync(ulong t){
        GarbageCollector.CollectIncremental(t);
    }
}
