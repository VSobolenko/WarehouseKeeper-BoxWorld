using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Pools;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace GameEditor.Pools
{
internal class TypeManagerProfiler : IPoolProfiler
{
    private readonly Dictionary<Type, IObjectPool<IPoolable>> _pool;
    private static int _maxPoolCapacity;
    private static int _maxPoolStackCapacity;

    private Dictionary<Type, PoolableData> _poolData = new();

    public TypeManagerProfiler(object pool)
    {
        _pool = pool as Dictionary<Type, IObjectPool<IPoolable>>;
        if (_pool == null)
        {
            Debug.LogError($"Can't unboxing pool dictionary for {GetType().Name} profiler");

            return;
        }
    }

    public void DrawStatus(VisualElement root)
    {
        if (_pool == null)
            return;

        var infoStyle = new GUIStyle(GUI.skin.box)
        {
        };

        var greenStyle = new GUIStyle(GUI.skin.textArea)
        {
            normal = {textColor = new Color(0.49f, 1f, 0f)},
        };

        var greenStyle2 = new GUIStyle(GUI.skin.textArea)
        {
            normal = {textColor = new Color(0.7f, 1f, 0.75f)},
        };

        OnPoolDataUpdated();

        var stackCapacity = _pool.Values.Sum(stackValue => stackValue.CountInactive);

        if (GUILayout.Button("Clear pool"))
            ClearPool();

        GUILayout.Label($"Profiler type: {GetType().Name}", infoStyle);
        GUILayout.Label($"Pool capacity: {_pool.Keys.Count}\nMax capacity: {_maxPoolCapacity}", greenStyle);
        GUILayout.Label($"Full stack capacity: {stackCapacity}\nMax stack capacity: {_maxPoolStackCapacity}",
                        greenStyle2);
        DrawPoolInfo();
    }

    public void OnPoolDataUpdated()
    {
        if (_pool == null)
            return;

        if (_pool.Keys.Count > _maxPoolCapacity)
            _maxPoolCapacity = _pool.Keys.Count;

        var stackCapacity = _pool.Values.Sum(stackValue => stackValue.CountInactive);
        if (stackCapacity > _maxPoolStackCapacity)
            _maxPoolStackCapacity = stackCapacity;
        RecalculateData();
    }

    private void DrawPoolInfo()
    {
        var redStyle = new GUIStyle(GUI.skin.box)
        {
            normal = {textColor = Color.red},
        };

        var containerStyles = new List<GUIStyle>
        {
            new GUIStyle {normal = {textColor = Color.white}},
            new GUIStyle {normal = {textColor = Color.yellow}},
            new GUIStyle {normal = {textColor = Color.cyan}},
            new GUIStyle {normal = {textColor = Color.magenta}},
        };

        GUILayout.Label("Pool container", redStyle);
        for (var i = 0; i < _pool.Count; i++)
        {
            var (key, value) = _pool.ElementAt(i);

            var maxElements = _poolData.ContainsKey(key) ? _poolData[key].maxElemets.ToString() : "-";

            var data = $"{Verify(key, value)}Count/Max: {value.CountInactive}/{maxElements} \tType: {key.Name}";

            var style = containerStyles[i % containerStyles.Count];
            GUILayout.Label(data, style);
        }
    }

    private string Verify(Type key, IObjectPool<IPoolable> stack)
    {
        return "✓";
    }

    private void ClearPool()
    {
        _pool.Clear();
    }

    private void RecalculateData()
    {
        if (_pool == null)
        {
            Log.Error("Null pool");

            return;
        }

        var newPoolData = new Dictionary<Type, PoolableData>(_pool.Count);

        foreach (var (key, stack) in _pool)
        {
            var poolData = new PoolableData
            {
                maxElemets = stack.CountInactive,
            };

            newPoolData.Add(key, poolData);

            if (_poolData.ContainsKey(key))
            {
                poolData.maxElemets = Mathf.Max(_poolData[key].maxElemets, stack.CountInactive);
            }

            newPoolData[key] = poolData;
        }

        _poolData = newPoolData;
    }

    private struct PoolableData
    {
        public int maxElemets;
    }
}
}