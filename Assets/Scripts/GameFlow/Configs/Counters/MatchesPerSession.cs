public static class MatchesPerSession
{
    #region Properties

    public static int Count { get; private set; }

    #endregion



    #region Public methods

    public static void Increment()
    {
        Count++;
    }

    #endregion
}
