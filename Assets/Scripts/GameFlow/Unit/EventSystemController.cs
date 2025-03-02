using UnityEngine.EventSystems;


public static class EventSystemController
{
    #region Variables

    private static EventSystem eventSystem;

    #endregion



    #region Lifecycle

    static EventSystemController()
    {
        eventSystem = EventSystem.current;
    }

    #endregion



    #region Public methods

    public static void EnableEventSystem()
    {
        eventSystem.enabled = true;
    }


    public static void DisableEventSystem()
    {
        eventSystem.enabled = false;
    }

    #endregion
}
