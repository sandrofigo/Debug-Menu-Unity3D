using System;

namespace DebugMenu
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DebugMethodPriority : Attribute
    {
        public DebugMethodPriority(int priority)
        {
            this.priority = priority;
        }

        public readonly int priority;
    }
}