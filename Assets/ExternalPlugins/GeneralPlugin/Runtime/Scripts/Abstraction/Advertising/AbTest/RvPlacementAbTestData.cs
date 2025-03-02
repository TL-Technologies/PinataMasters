namespace Modules.General.Abstraction
{
    public class RvPlacementAbTestData
    {
        public string PlacementName { get; }
        public AbTestRvSettings RvSettings { get; }

        public RvPlacementAbTestData(string placementName, AbTestRvSettings rvSettings)
        {
            PlacementName = placementName;
            RvSettings = rvSettings;
        }
    }
}
