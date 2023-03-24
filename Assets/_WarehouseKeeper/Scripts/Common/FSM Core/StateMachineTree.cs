using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.FSMCore
{
internal class StateMachineTree
{
    protected List<TransitionData> transitions = new List<TransitionData>();

    public void UpdateTree(IStateMachine stateMachine)
    {
        foreach (var transitionData in transitions)
        {
            try
            {
                if (transitionData.transition.Decide() == false) 
                    continue;
                
                transitionData.transition.Transit();
                return;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogException(e);
#elif DEVELOPMENT_BUILD
                Extension.Log.WriteError($"[OnStateFinished] {e.Message}");
#endif
            }
        }
    }
    
    public void AddTransition(BaseTransition baseTransition, int priority = 0)
    {
        try
        {
            transitions.Add(new TransitionData
            {
                transition = baseTransition,
                priority = priority,
            });

            transitions = transitions.OrderByDescending(x => x.priority).ToList();
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(e);
#elif DEVELOPMENT_BUILD
            Extension.Log.WriteError($"[OnStateFinished] {e.Message}");
#endif
        }
    }

    public void AddTransition(params BaseTransition[] baseTransition)
    {
        try
        {
            foreach (var transition in baseTransition)
            {
                transitions.Add(new TransitionData
                {
                    transition = transition,
                    priority = 0,
                });
            }
        
            transitions = transitions.OrderByDescending(x => x.priority).ToList();
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(e);
#elif DEVELOPMENT_BUILD
            Extension.Log.WriteError($"[OnStateFinished] {e.Message}");
#endif
        }
    }
    
    public void RemoveTransition(BaseTransition baseTransition)
    {
        try
        {
            var transitionData = transitions.FirstOrDefault(x => x.transition == baseTransition);
            if (transitionData.Equals(default) || transitionData.transition == null)
            {
                Log.WriteWarning($"Can't remove transition from tree: Transition={baseTransition}");
                return;
            }

            transitions.Remove(transitionData);
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(e);
#elif DEVELOPMENT_BUILD
            Extension.Log.WriteError($"[OnStateFinished] {e.Message}");
#endif
        }
    }

    public BaseTransition this[int index] => transitions[index].transition;

    public TTransition GetTransition<TTransition>() where TTransition : BaseTransition
    {
        foreach (var transitionData in transitions)
        {
            if (transitionData.transition.GetType() == typeof(TTransition))
            {
                return transitionData.transition as TTransition;
            }
        }

        return default;
    }

    public void DisposeMachine()
    {
        foreach (var transitionData in transitions)
        {
            try
            {
                transitionData.transition.Dispose();
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogException(e);
#elif DEVELOPMENT_BUILD
            Extension.Log.WriteError($"[OnStateFinished] {e.Message}");
#endif
            }
        }
    }
}

internal struct TransitionData
{
    public BaseTransition transition;
    public int priority;
}
}