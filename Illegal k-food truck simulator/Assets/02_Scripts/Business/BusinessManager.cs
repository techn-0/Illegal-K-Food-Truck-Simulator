using UnityEngine;
using System;

public class BusinessManager : MonoBehaviour
{
    public static bool IsBusinessActive { get; private set; } = false;

    public static event Action<bool> OnBusinessStateChanged;

    public static void ToggleBusinessState()
    {
        IsBusinessActive = !IsBusinessActive;
        Debug.Log("Business is now " + (IsBusinessActive ? "Active" : "Inactive"));
        OnBusinessStateChanged?.Invoke(IsBusinessActive);
    }
}
