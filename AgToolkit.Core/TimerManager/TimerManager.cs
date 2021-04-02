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

        public UnityEvent Event { get; }

        public bool IsActive = false;

        /// <summary>
        /// Create a Timer with an Id, the limit (ex: 5f -> 5s) and the UnityEvent to invoke
        /// </summary>
        public Timer(string id, float limit, UnityEvent e)
        {
            _Id = id;
            _Limit = limit;
            _CurrentTime = 0f;
            Event = new UnityEvent();
            Event.AddListener(() => TimerManager.Instance.StopTimer(this, false));
            Event.AddListener(() => e.Invoke());

            TimerManager.Instance.Register(this);
        }

        /// <summary>
        /// Add time to the timer and invoke the Event if the Limit is reached
        /// </summary>
        public void AddTime(float t)
        {
            if (!IsActive) return;

            _CurrentTime += t;

            if (_CurrentTime > _Limit)
            {
                Event.Invoke();
            }
        }

        /// <summary>
        /// Reset the timer
        /// </summary>
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
            foreach (Timer t in _Timers.ToArray())
            {
                if (!t.IsActive) continue;

                t.AddTime(Time.deltaTime);
            }
        }

        /// <summary>
        /// Start a timer, register it if his not already registered
        /// </summary>
        public void StartTimer(Timer t, bool resetBefore = true)
        {
            Register(t);
            
            if (resetBefore)
            {
                ResetTimer(t);
            }

            t.IsActive = true;
        }

        /// <summary>
        /// Stop a timer, register it if his not already registered
        /// </summary>
        public void StopTimer(Timer t, bool resetTimer = true)
        {
            Register(t);

            if (resetTimer)
            {
                ResetTimer(t);
            }

            t.IsActive = false;
        }


        /// <summary>
        /// Reset a timer, register it if his not already registered
        /// </summary>
        public void ResetTimer(Timer t)
        {
            Register(t);
            t.ResetTime();
        }


        /// <summary>
        /// Stop & Remove a Timers
        /// </summary>
        public void RemoveTimer(Timer t)
        {
            t.IsActive = false;

            if (_Timers.Contains(t))
            {   
                _Timers.Remove(t);
            }
        }

        /// <summary>
        /// Register a new Timer
        /// </summary>
        public void Register(Timer t)
        {
            if (_Timers.Contains(t)) return;

            _Timers.Add(t);
        }

    }
}
