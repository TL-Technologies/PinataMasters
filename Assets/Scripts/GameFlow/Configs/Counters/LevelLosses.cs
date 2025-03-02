using Modules.General.HelperClasses;


public static class LevelLosses
{
    #region Variables

    private const string LEVEL_LOSSES = "level_losses";

    #endregion



    #region Properties

    public static int Count
    {
        get
        {
            return CustomPlayerPrefs.GetInt(LEVEL_LOSSES, 0);
        }
        private set
        {
            CustomPlayerPrefs.SetInt(LEVEL_LOSSES, value);
        }
    }

    #endregion



    #region Public methods

    public static void Increase()
    {
        Count++;
    }


    public static void Reset()
    {
        Count = 0;
    }

    #endregion
}
