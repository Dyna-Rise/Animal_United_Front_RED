using System;
using UnityEngine;

[Serializable]
public class Talk
{
    public string message;
}

[CreateAssetMenu(fileName = "TalkData", menuName = "ScriptableObjects/TalkDatas")]
public class TalkData : ScriptableObject
{
    public Talk[] talks;
}