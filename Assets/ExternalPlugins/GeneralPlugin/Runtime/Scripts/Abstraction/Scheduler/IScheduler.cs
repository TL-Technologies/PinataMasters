using System;


namespace Modules.General.Abstraction
{
    public interface IScheduler
    {
        void ScheduleMethod(object target, Action selector, float pInterval, bool isNeedUnscaledDeltaTime = false);

        void ScheduleMethod(object target, Action selector);
        
        SchedulerTask CallMethodWithDelay(object target, Action selector, float delay, bool isNeedUnscaledDeltaTime = false);

        void UnscheduleAllMethodForTarget(object target);

        void UnscheduleMethod(object target, Action selector);

        void UnscheduleTask(SchedulerTask unTask);

        void PauseMethod(object target, Action selector);

        void UnpauseMethod(object target, Action selector);

        void PauseAllMethodForTarget(object target);

        void UnpauseAllMethodForTarget(object target);
    }
}
