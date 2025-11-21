using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dictionnary", menuName = "Scriptable Objects/Dictionnary")]
public class Dictionnary<T> : ScriptableObject
{
    public List<T> listtest;
}
