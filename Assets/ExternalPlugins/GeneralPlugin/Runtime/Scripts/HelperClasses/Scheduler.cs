using Modules.General.Abstraction;
using Modules.General.InitializationQueue;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.General
{
    public class SchedulerTask
    {
        #region Fields

        public object Target;
        public Action Method;

        public float Interval;
        public float Elapsed;
        public bool AutoRemove;
        public bool Paused;
        public bool IsNeedUnscaledDeltaTime;

        #endregion



        #region Methods

        public SchedulerTask(object target, Action name, float interval, bool isNeedUseUnscaledDeltaTime) :
            this(target, name, interval, isNeedUseUnscaledDeltaTime, false)
        { }


        public SchedulerTask(
            object target,
            Action name,
            float interval,
            bool isNeedUseUnscaledDeltaTime,
            bool autoRemove)
        {
            Target = target;
            Method = name;
            Interval = interval;
            AutoRemove = autoRemove;
            Elapsed = 0;
            Paused = false;
            IsNeedUnscaledDeltaTime = isNeedUseUnscaledDeltaTime;
        }


        public void CustomUpdate(float deltaTime)
        {
            if (Target != null && Method != null && !Paused)
            {
                Elapsed += deltaTime;

                if (Elapsed >= Interval)
                {
                    Elapsed -= Interval;
                }
                else
                {
                    return;
                }

                if (AutoRemove)
                {
                    Scheduler.Instance.UnscheduleTask(this);
                }

                Method();
            }
        }


        public float GetRemainingTime()
        {
            return Mathf.Clamp(Interval - Elapsed, 0f, Interval);
        }

        #endregion
    }

    [InitQueueService(-250, bindTo: typeof(IScheduler))]
    public class Scheduler : MonoBehaviour, IScheduler
    {
        #region Fields

        private readonly LinkedList<SchedulerTask> schedulers = new LinkedList<SchedulerTask>();
        private readonly LinkedList<SchedulerTask> addList = new LinkedList<SchedulerTask>();
        private readonly LinkedList<SchedulerTask> removeList = new LinkedList<SchedulerTask>();

        private static IScheduler instance;

        #endregion



        #region Properties

        public static IScheduler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Services.GetService<IScheduler>();

                    if (instance == null)
                    {
                        GameObject gameObject = new GameObject(typeof(Scheduler).Name);
                        DontDestroyOnLoad(gameObject);
                        instance = gameObject.AddComponent<Scheduler>();
                    }
                }

                return instance;
            }
        }


        public static IScheduler InstanceIfExist => instance;

        #endregion



        #region Unity lifecycle

        private void Update()
        {
            if (!addList.IsNullOrEmpty())
            {
                schedulers.AddRangeAsLast(addList);
                addList.Clear();
            }

            if (!removeList.IsNullOrEmpty())
            {
                foreach (SchedulerTask task in removeList)
                {
                    schedulers.Remove(task);
                }

                removeList.Clear();
            }

            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;

            foreach (SchedulerTask task in schedulers)
            {
                float currentDeltaTime = (task.IsNeedUnscaledDeltaTime) ? (unscaledDeltaTime) : (deltaTime);
                task.CustomUpdate(currentDeltaTime);
            }
        }

        #endregion



        #region Methods

        public void ScheduleMethod(
            object target,
            Action selector,
            float pInterval,
            bool isNeedUnscaledDeltaTime = false)
        {
            bool isExist = false;

            foreach (SchedulerTask task in schedulers)
            {
                if ((task.Target == target) && (task.Method == selector))
                {
                    CustomDebug.Log("Scheduler: update interval for method " + selector + " from: " + task.Interval +
                                    " to: " + pInterval);

                    task.Interval = pInterval;
                    task.Elapsed = 0;
                    isExist = true;
                }
            }

            foreach (SchedulerTask task in addList)
            {
                if ((task.Target == target) && (task.Method == selector))
                {
                    CustomDebug.Log("Scheduler: update interval for method " + selector + " from: " + task.Interval +
                                    " to: " + pInterval);

                    task.Interval = pInterval;
                    task.Elapsed = 0;
                    isExist = true;
                }
            }

            if (!isExist)
            {
                SchedulerTask task = new SchedulerTask(target, selector, pInterval, isNeedUnscaledDeltaTime);
                addList.AddLast(task);
            }
        }


        public void ScheduleMethod(object target, Action selector)
        {
            ScheduleMethod(target, selector, 0.0f);
        }


        public SchedulerTask CallMethodWithDelay(
            object target,
            Action selector,
            float delay,
            bool isNeedUnscaledDeltaTime = false)
        {
            SchedulerTask task = new SchedulerTask(target, selector, delay, isNeedUnscaledDeltaTime, true);
            addList.AddLast(task);

            return task;
        }


        public void UnscheduleAllMethodForTarget(object target)
        {
            foreach (SchedulerTask task in schedulers)
            {
                if (task.Target == target)
                {
                    removeList.AddLast(task);
                }
            }

            foreach (SchedulerTask task in addList)
            {
                if (task.Target == target)
                {
                    removeList.AddLast(task);
                }
            }
        }


        public void UnscheduleMethod(object target, Action selector)
        {
            foreach (SchedulerTask task in schedulers)
            {
                if ((task.Target == target) && (task.Method == selector))
                {
                    removeList.AddLast(task);
                }
            }

            foreach (SchedulerTask task in addList)
            {
                if ((task.Target == target) && (task.Method == selector))
                {
                    removeList.AddLast(task);
                }
            }
        }


        public void UnscheduleTask(SchedulerTask unTask)
        {
            removeList.AddLast(unTask);
        }


        public void PauseMethod(object target, Action selector)
        {
            foreach (SchedulerTask task in schedulers)
            {
                if ((task.Target == target) && (task.Method == selector))
                {
                    task.Paused = true;
                }
            }

            foreach (SchedulerTask task in addList)
            {
                if ((task.Target == target) && (task.Method == selector))
                {
                    task.Paused = true;
                }
            }
        }


        public void UnpauseMethod(object target, Action selector)
        {
            foreach (SchedulerTask task in schedulers)
            {
                if ((task.Target == target) && (task.Method == selector))
                {
                    task.Paused = false;
                }
            }

            foreach (SchedulerTask task in addList)
            {
                if ((task.Target == target) && (task.Method == selector))
                {
                    task.Paused = false;
                }
            }
        }


        public void PauseAllMethodForTarget(object target)
        {
            foreach (SchedulerTask task in schedulers)
            {
                if (task.Target == target)
                {
                    task.Paused = true;
                }
            }

            foreach (SchedulerTask task in addList)
            {
                if (task.Target == target)
                {
                    task.Paused = true;
                }
            }
        }


        public void UnpauseAllMethodForTarget(object target)
        {
            foreach (SchedulerTask task in schedulers)
            {
                if (task.Target == target)
                {
                    task.Paused = false;
                }
            }

            foreach (SchedulerTask task in addList)
            {
                if (task.Target == target)
                {
                    task.Paused = false;
                }
            }
        }

        #endregion
    }
}
