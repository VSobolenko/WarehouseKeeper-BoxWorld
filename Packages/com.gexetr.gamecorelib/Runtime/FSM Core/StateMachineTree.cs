using System.Collections.Generic;
using System.Linq;
using Game.FSMCore.States;
using Game.FSMCore.Transitions;
using UnityEngine;

namespace Game.FSMCore
{
public class StateMachineTree
{
    private List<TransitionData> _transitions = new(5);
    private readonly List<IState> _states = new();

    internal void UpdateTree()
    {
        foreach (var transitionData in _transitions)
        {
            if (transitionData.transition.Decide() == false)
                continue;

            transitionData.transition.Transit();

            return;
        }
    }

    internal void DisposeMachine()
    {
        foreach (var transitionData in _transitions)
        {
            transitionData.transition.Dispose();
        }
    }

    
    #region States

    public void AddState(IState state) => _states.Add(state);

    public void AddState(params IState[] states)
    {
        foreach (var state in states)
        {
            AddState(state);
        }
    }

    public void RemoveState(IState state)
    {
        if (_states.Remove(state) == false)
        {
            Log.Warning($"Can't remove state from tree: StateType={state.GetType()}");
        }
    }

    public List<IState> GetStates() => _states;

    #endregion
    
    #region Transitions

    public void AddTransition(BaseTransition baseTransition, int priority = 0)
    {
        _transitions.Add(new TransitionData
        {
            transition = baseTransition,
            priority = priority,
        });

        _transitions = _transitions.OrderByDescending(x => x.priority).ToList();
    }

    public void AddTransition(params BaseTransition[] baseTransition)
    {
        foreach (var transition in baseTransition)
        {
            _transitions.Add(new TransitionData
            {
                transition = transition,
                priority = 0,
            });
        }

        _transitions = _transitions.OrderByDescending(x => x.priority).ToList();
    }

    public void RemoveTransition(BaseTransition baseTransition)
    {
        var transitionData = _transitions.FirstOrDefault(x => x.transition == baseTransition);
        if (transitionData.Equals(default) || transitionData.transition == null)
        {
            Log.Warning($"Can't remove transition from tree: Transition={baseTransition}");

            return;
        }

        _transitions.Remove(transitionData);
    }

    public TTransition GetTransition<TTransition>() where TTransition : BaseTransition
    {
        foreach (var transitionData in _transitions)
        {
            if (transitionData.transition.GetType() == typeof(TTransition))
            {
                return transitionData.transition as TTransition;
            }
        }

        return default;
    }

    #endregion
}

internal struct TransitionData
{
    public BaseTransition transition;
    public int priority;
}
}