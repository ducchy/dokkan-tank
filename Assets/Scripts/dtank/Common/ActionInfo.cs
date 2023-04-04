using System;
using ActionSequencer;
using UnityEngine;

namespace dtank
{
    [Serializable]
    public class ActionInfo
    {
        [SerializeField] private string _triggerName;
        [SerializeField] private SequenceClip _sequenceClip;

        public string TriggerName => _triggerName;
        public SequenceClip SequenceClip => _sequenceClip;
    }
}