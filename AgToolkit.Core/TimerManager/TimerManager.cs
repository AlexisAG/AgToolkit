using System;
using AgToolkit.AgToolkit.Core.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace AgToolkit.AgToolkit.Core.Timer
{
    public class Timer
    {
        private string _Id;
        private float _Limit;
        private float _CurrentTime;
        private UnityEvent _Event;

        public UnityEvent Event => _Event;
        public bool IsActive = false;

        public Timer(string id, float limit, UnityEvent e)
        {
            _Id = id;
            _Limit = limit;
            _CurrentTime = 0f;
            _Event = e;

            if (TimerManager.IsInstanced)
            {
                TimerManager.Instance.Register(this);
            }
        }

        public void AddTime(float t)
        {
            if (!IsActive) return;

            _CurrentTime += t;

            if (_CurrentTime > _Limit)
            {
                Event.Invoke();
            }
        }

        public void ResetTime()
        {
            _CurrentTime = 0f;
        }
    }

    public class TimerManager : Singleton<TimerManager>
    {
        private readonly List<Timer> _Timers = new List<Timer>();

        private void Update()
        {
            foreach (Timer t in _Timers)
            {
                if (!t.IsActive) continue;

                t.AddTime(Time.deltaTime);
            }
        }

        private void IsComplete(Timer t)
        {
            Debug.LogWarning($"Timer finished");
            StopTimer(t, false);
        }

        public void StartTimer(Timer t, bool resetBefore = true)
        {
            Register(t);
            if (resetBefore)
            {
                ResetTimer(t);
            }

            t.IsActive = true;
        }

        public void StopTimer(Timer t, bool resetTimer = true)
        {
            Register(t);

            if (resetTimer)
            {
                ResetTimer(t);
            }

            t.IsActive = false;
        }

        public void ResetTimer(Timer t)
        {
            Register(t);
            t.ResetTime();
        }

        public void RemoveTimer(Timer t)
        {
            if (_Timers.Contains(t))
            {
                _Timers.Remove(t);
            }
        }

        public void Register(Timer t)
        {
            if (_Timers.Contains(t)) return;

            t.Event.AddListener((() => {IsComplete(t);}));
            _Timers.Add(t);
        }

    }
}
