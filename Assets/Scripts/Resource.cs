using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public abstract class Resource : MonoBehaviour {
    public enum @ResourceType : uint {
        /// <summary>
        /// <see cref="Amount">Amount</see> will start off at it's <see cref="Max">Max</see> value.
        /// </summary>
        Clasic = 0,
        /// <summary>
        /// <see cref="Amount">Amount</see> will start off at it's <see cref="Min">Min</see> value.
        /// </summary>
        Charge = 1,
    }

    // ==================== Configuration ====================
    [field: SerializeField, Min(0)] public int Min { get; private set; }
    [field: SerializeField, Min(1)] public int Max { get; private set; }

    /// <value>
    /// Determines the behaviour of the <see cref="Reset">Reset()</see> method.
    /// </value>
    public abstract ResourceType ResType { get; }

    // ====================== Variables ======================    
    [field: SerializeField, ShowOnly] private int _amount;
    public int Amount {
        get => _amount;
        protected set {
            _amount = Mathf.Clamp(value, Min, Max);
            onChange?.Invoke(_amount);
        }
    }

    // ====================== Unity Code ======================
    protected void Reset() {
        switch (ResType) {
            case ResourceType.Charge:
                Amount = Min; break;
            case ResourceType.Clasic:
                Amount = Max; break;
            default:
                throw new NotImplementedException($"'Resource.Reset()': Missing implementation for enum variant: '{ResType}'");
        }
    }

    // ================== Outside Facing API ==================
    private event Action<int> onChange;
    public event Action<int> OnChange {
        add    { lock(this) { onChange += value; } }
        remove { lock(this) { onChange -= value; } }
    }
}