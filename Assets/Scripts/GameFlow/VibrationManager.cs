using Modules.General.HelperClasses;
using MoreMountains.NiceVibrations;


public class VibrationManager : SingletonMonoBehaviour<VibrationManager>
{

	public const string VIBRATION_KEY = "Vibration";

	#region Properties

	public bool IsVibrationEnabled
	{
		get
		{
			return CustomPlayerPrefs.GetBool(VIBRATION_KEY, true);
		}
		set
		{
			CustomPlayerPrefs.SetBool(VIBRATION_KEY, value);
		}
	}

	#endregion



	#region Public Methods

	public void PlayDefaultVibration()
	{
		if (IsVibrationEnabled)
		{
			MMVibrationManager.Vibrate();
		}
	}


	public void PlayVibration(HapticTypes hapticType)
	{
		if (IsVibrationEnabled)
		{
			MMVibrationManager.Haptic(hapticType);
		}
	}

	#endregion
}
