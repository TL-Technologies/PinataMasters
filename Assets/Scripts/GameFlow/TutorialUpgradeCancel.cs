using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class TutorialUpgradeCancel : MonoBehaviour
    {
        [SerializeField]
        private bool upgradeWeapon;
        [SerializeField]
        private bool upgradeCharacter;


        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();

            if (upgradeWeapon && !TutorialManager.Instance.IsUpgradeWeaponTutorialPassed)
            {
                TutorialManager.OnUpgradeTutorialPassed += AddCancelWeaponUpgrade;
            }

            if (!TutorialManager.Instance.IsUpgradeAbilityTutorialPassed)
            {
                TutorialManager.OnUpgradeCharacterTutorialPassed += AddCancelAbilityUpgrade;
            }
        }


        private void AddCancelWeaponUpgrade()
        {
            button.onClick.AddListener(CancelWeaponUpgrade);
        }

        private void AddCancelAbilityUpgrade()
        {
            button.onClick.AddListener(CancelCharacterUpgrade);
        }


        private void CancelWeaponUpgrade()
        {
            GameAnalytics.ShouldRepeatUpgradeTutorial = false;
            button.onClick.RemoveListener(CancelWeaponUpgrade);
            TutorialManager.OnUpgradeTutorialPassed -= AddCancelWeaponUpgrade;
        }


        private void CancelCharacterUpgrade()
        {
            GameAnalytics.ShouldRepeatUpgradeCharacterTutorial = false;
            button.onClick.RemoveListener(CancelCharacterUpgrade);
            TutorialManager.OnUpgradeCharacterTutorialPassed -= AddCancelAbilityUpgrade;
        }
    }
}