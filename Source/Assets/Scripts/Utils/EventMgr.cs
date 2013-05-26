using System;
using System.Collections.Generic;

// Generic Event System keyable by any type, it will usually be an enum
public class EventMgr<TEventType>
{
    public delegate void EventDelegate();

    public delegate void EventDelegate<T>(T param);

    public delegate void EventDelegate<T1, T2>(T1 p1, T2 p2);

    public delegate void EventDelegate<T1, T2, T3>(T1 p1, T2 p2, T3 p3);

    public delegate void EventDelegate<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4);

    private Dictionary<TEventType, IEventWrapper> _wrappers = new Dictionary<TEventType, IEventWrapper>();

    public void RegisterEvent(TEventType eventType)
    {
        Util.Assert(!_wrappers.ContainsKey(eventType));

        _wrappers.Add(eventType, new EventWrapper0());
    }

    public void RegisterEvent<T>(TEventType eventType)
    {
        Util.Assert(!_wrappers.ContainsKey(eventType));

        _wrappers.Add(eventType, new EventWrapper1<T>());
    }

    public void RegisterEvent<T1, T2>(TEventType eventType)
    {
        Util.Assert(!_wrappers.ContainsKey(eventType));

        _wrappers.Add(eventType, new EventWrapper2<T1, T2>());
    }

    public void RegisterEvent<T1, T2, T3>(TEventType eventType)
    {
        Util.Assert(!_wrappers.ContainsKey(eventType));

        _wrappers.Add(eventType, new EventWrapper3<T1, T2, T3>());
    }

    public void RegisterEvent<T1, T2, T3, T4>(TEventType eventType)
    {
        Util.Assert(!_wrappers.ContainsKey(eventType));

        _wrappers.Add(eventType, new EventWrapper4<T1, T2, T3, T4>());
    }

    public void AddListener(TEventType eventType, EventDelegate eventDelegate)
    {
        Util.Assert(_wrappers.ContainsKey(eventType), "Unregistered event with type '" + eventType + "'");
        _wrappers[eventType].Add(eventDelegate);
    }

    public void AddListener<T>(TEventType eventType, EventDelegate<T> eventDelegate)
    {
        Util.Assert(_wrappers.ContainsKey(eventType), "Unregistered event with type '" + eventType +"'");
        _wrappers[eventType].Add(eventDelegate);
    }

    public void AddListener<T1, T2>(TEventType eventType, EventDelegate<T1, T2> eventDelegate)
    {
        Util.Assert(_wrappers.ContainsKey(eventType), "Unregistered event with type '" + eventType + "'");
        _wrappers[eventType].Add(eventDelegate);
    }

    public void AddListener<T1, T2, T3>(TEventType eventType, EventDelegate<T1, T2, T3> eventDelegate)
    {
        Util.Assert(_wrappers.ContainsKey(eventType), "Unregistered event with type '" + eventType + "'");
        _wrappers[eventType].Add(eventDelegate);
    }

    public void AddListener<T1, T2, T3, T4>(TEventType eventType, EventDelegate<T1, T2, T3, T4> eventDelegate)
    {
        Util.Assert(_wrappers.ContainsKey(eventType), "Unregistered event with type '" + eventType + "'");
        _wrappers[eventType].Add(eventDelegate);
    }

    public void RemoveListener(TEventType eventType, EventDelegate eventDelegate)
    {
        _wrappers[eventType].Remove(eventDelegate);
    }

    public void RemoveListener<T>(TEventType eventType, EventDelegate<T> eventDelegate)
    {
        _wrappers[eventType].Remove(eventDelegate);
    }

    public void RemoveListener<T1, T2>(TEventType eventType, EventDelegate<T1, T2> eventDelegate)
    {
        _wrappers[eventType].Remove(eventDelegate);
    }

    public void RemoveListener<T1, T2, T3>(TEventType eventType, EventDelegate<T1, T2, T3> eventDelegate)
    {
        _wrappers[eventType].Remove(eventDelegate);
    }

    public virtual void Trigger(TEventType eventType)
    {
        Util.Assert(_wrappers.ContainsKey(eventType), "Unregistered event with type '" + eventType + "'");

        _wrappers[eventType].Trigger();
        OnTriggeredEvent(eventType);
    }

    public virtual void Trigger<T>(TEventType eventType, T p1)
    {
        Util.Assert(_wrappers.ContainsKey(eventType), "Unregistered event with type '" + eventType + "'");

        _wrappers[eventType].Trigger(p1);
        OnTriggeredEvent(eventType);
    }

    public virtual void Trigger<T1, T2>(TEventType eventType, T1 p1, T2 p2)
    {
        Util.Assert(_wrappers.ContainsKey(eventType), "Unregistered event with type '" + eventType + "'");

        _wrappers[eventType].Trigger(p1, p2);
        OnTriggeredEvent(eventType);
    }

    public virtual void Trigger<T1, T2, T3>(TEventType eventType, T1 p1, T2 p2, T3 p3)
    {
        Util.Assert(_wrappers.ContainsKey(eventType), "Unregistered event with type '" + eventType + "'");

        _wrappers[eventType].Trigger(p1, p2, p3);
        OnTriggeredEvent(eventType);
    }

    public virtual void Trigger<T1, T2, T3, T4>(TEventType eventType, T1 p1, T2 p2, T3 p3, T4 p4)
    {
        Util.Assert(_wrappers.ContainsKey(eventType), "Unregistered event with type '" + eventType + "'");

        _wrappers[eventType].Trigger(p1, p2, p3, p4);
        OnTriggeredEvent(eventType);
    }
    protected virtual void OnTriggeredEvent(TEventType eventType)
    {
        // Optionally overridden
    }

    /////////  Types

    private interface IEventWrapper
    {
        void Add(object handler);
        void Remove(object handler);
        void Trigger(params object[] args);
    }

    private class EventWrapper0 : IEventWrapper
    {
        private EventDelegate _handlers = delegate { };

        public void Add(object handler)
        {
            _handlers += (EventDelegate)handler;
        }

        public void Remove(object handler)
        {
            _handlers -= (EventDelegate)handler;
        }

        public void Trigger(params object[] args)
        {
            Util.Assert(args.Length == 0);
            _handlers.Invoke();
        }
    }

    private class EventWrapper1<T> : IEventWrapper
    {
        private EventDelegate<T> _handlers = delegate { };

        public void Add(object handler)
        {
            _handlers += (EventDelegate<T>)handler;
        }

        public void Remove(object handler)
        {
            _handlers -= (EventDelegate<T>)handler;
        }

        public void Trigger(params object[] args)
        {
            Util.Assert(args.Length == 1);
            _handlers.Invoke((T)args[0]);
        }
    }

    private class EventWrapper2<T1, T2> : IEventWrapper
    {
        private EventDelegate<T1, T2> _handlers = delegate { };

        public void Add(object handler)
        {
            _handlers += (EventDelegate<T1, T2>) handler;
        }

        public void Remove(object handler)
        {
            _handlers -= (EventDelegate<T1, T2>)handler;
        }

        public void Trigger(params object[] args)
        {
            Util.Assert(args.Length == 2);
            _handlers.Invoke((T1) args[0], (T2) args[1]);
        }
    }

    private class EventWrapper3<T1, T2, T3> : IEventWrapper
    {
        private EventDelegate<T1, T2, T3> _handlers = delegate { };

        public void Add(object handler)
        {
            _handlers += (EventDelegate<T1, T2, T3>)handler;
        }

        public void Remove(object handler)
        {
            _handlers -= (EventDelegate<T1, T2, T3>)handler;
        }

        public void Trigger(params object[] args)
        {
            Util.Assert(args.Length == 3);
            _handlers.Invoke((T1)args[0], (T2)args[1], (T3)args[2]);
        }
    }

    private class EventWrapper4<T1, T2, T3, T4> : IEventWrapper
    {
        private EventDelegate<T1, T2, T3, T4> _handlers = delegate { };

        public void Add(object handler)
        {
            _handlers += (EventDelegate<T1, T2, T3, T4>)handler;
        }

        public void Remove(object handler)
        {
            _handlers -= (EventDelegate<T1, T2, T3, T4>)handler;
        }

        public void Trigger(params object[] args)
        {
            Util.Assert(args.Length == 4);
            _handlers.Invoke((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
        }
    }
}