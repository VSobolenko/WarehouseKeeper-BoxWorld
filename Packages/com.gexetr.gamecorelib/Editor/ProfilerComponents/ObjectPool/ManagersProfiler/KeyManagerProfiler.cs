using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Pools;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEditor.Pools
{
internal class KeyManagerProfiler : IPoolProfiler
{
    private static int _maxPoolCapacity;
    private static int _maxPoolStackCapacity;
    private readonly Dictionary<string, Stack<IPoolable>> _pool;
    private Dictionary<string, PoolableData> _poolData = new();

    public KeyManagerProfiler(object pool)
    {
        _pool = pool as Dictionary<string, Stack<IPoolable>>;
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

        var stackCapacity = _pool.Values.Sum(stackValue => stackValue.Count);

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

        var stackCapacity = _pool.Values.Sum(stackValue => stackValue.Count);
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

            var data = $"{Verify(key, value)}Count/Max: {value.Count}/{maxElements} \tKey: {key}";

            var style = containerStyles[i % containerStyles.Count];
            GUILayout.Label(data, style);
        }
    }

    private string Verify(string key, Stack<IPoolable> stack)
    {
        var existsCopy = stack.Count == stack.Distinct().Count();
        var hasNoNull = stack.Count(x => x == null) == 0;

        return existsCopy || hasNoNull ? "✓" : "x";
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

        var newPoolData = new Dictionary<string, PoolableData>(_pool.Count);

        foreach (var (key, stack) in _pool)
        {
            var poolData = new PoolableData
            {
                maxElemets = stack.Count,
            };

            newPoolData.Add(key, poolData);

            if (_poolData.ContainsKey(key))
            {
                poolData.maxElemets = Mathf.Max(_poolData[key].maxElemets, stack.Count);
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