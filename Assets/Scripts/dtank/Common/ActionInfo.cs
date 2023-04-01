using System;
using ActionSequencer;

namespace dtank
{
    [Serializable]
    public class ActionInfo
    {
        public string triggerName;
        public SequenceClip sequenceClip;
    }
}