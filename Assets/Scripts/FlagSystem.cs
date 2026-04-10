using System.Collections.Generic;
using UnityEngine;

public class FlagSystem : MonoBehaviour
{
    public static FlagSystem Instance { get; private set; }

    private Dictionary<string, bool> _boolFlags = new Dictionary<string, bool>();
    private Dictionary<string, int> _intFlags = new Dictionary<string, int>();

    public void SetFlag(string key, bool value) => _boolFlags[key] = value;
    public bool GetFlag(string key) => _boolFlags.TryGetValue(key, out var v) && v;

    public void SetInt(string key, int value) => _intFlags[key] = value;
    public int GetInt(string key) => _intFlags.TryGetValue(key, out var v) ? v : 0;
}
