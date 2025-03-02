using UnityEngine;

namespace PinataMasters
{
    [CreateAssetMenu]
    public class SelectorLevels : ScriptableObject
    {
        private static readonly ResourceAsset<SelectorLevels> asset = new ResourceAsset<SelectorLevels>("Game/SelectorLevels");

        [SerializeField]
        private Levels easy = null;
        [SerializeField]
        private Levels normal = null;

        #region Properties

        public static Levels GetLevels
        {
            get { return ABTest.DifficultyEasy ? asset.Value.easy : asset.Value.normal; }
        }

        #endregion
    }
}