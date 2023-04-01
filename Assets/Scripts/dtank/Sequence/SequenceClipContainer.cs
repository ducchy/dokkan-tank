using System.Collections.Generic;
using ActionSequencer;

namespace dtank
{
    public class SequenceClipContainer
    {
        public static SequenceClipContainer Create(ActionInfo[] infos)
        {
            if (infos == null)
                return null;

            var dictionary = new Dictionary<string, SequenceClip>();
            foreach (var info in infos)
            {
                if (info == null || dictionary.ContainsKey(info.triggerName))
                    continue;

                dictionary.Add(info.triggerName, info.sequenceClip);
            }

            return new SequenceClipContainer(dictionary);
        }

        private readonly Dictionary<string, SequenceClip> _dictionary;

        public SequenceClip this[string key]
        {
            get
            {
                if (_dictionary == null)
                    return null;

                return !_dictionary.TryGetValue(key, out var item) ? null : item;
            }
        }

        private SequenceClipContainer(Dictionary<string, SequenceClip> dictionary)
        {
            _dictionary = dictionary;
        }
    }
}