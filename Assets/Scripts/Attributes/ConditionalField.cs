using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

using CustomExtensions.Strings;
using CustomExtensions.Collections;

/// <summary>
/// Conditionally Show/Hide field in inspector, based on some other field or property value
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ConditionalFieldAttribute : PropertyAttribute {
    public string fieldToCheck;
    public bool inverse = false;
    public string[] compareValues;
    public bool IsSet => fieldToCheck.NotNullOrEmpty() && compareValues.NotNullOrEmpty();

    /// <param name="fieldToCheck">String name of field to check value</param>
    /// <param name="inverse">Inverse check result</param>
    public ConditionalFieldAttribute(string fieldToCheck, params int[] compareValues) : this(fieldToCheck, false, compareValues) { }

    public ConditionalFieldAttribute(string fieldToCheck, bool inverse = false, params int[] compareValues) { 
        this.fieldToCheck = fieldToCheck;
        this.inverse = inverse;
        // Transform to array of strings
        this.compareValues = compareValues.Select(c => c.ToString().ToUpper()).ToArray();
    }
}