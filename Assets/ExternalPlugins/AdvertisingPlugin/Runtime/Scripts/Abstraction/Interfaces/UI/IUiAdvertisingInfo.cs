namespace Modules.Advertising
{
	public interface IUiAdvertisingInfo
	{
		bool IsGalleryScreenShowInterstitialAvailable { get; }

		bool IsGalleryScreenCloseInterstitialAvailable { get; }

		bool IsSettingsScreenOpenInterstitialAvailable { get; }

		bool IsSettingsScreenCloseInterstitialAvailable { get; }

		bool IsApplicationBackgroundInterstitialAvailable { get; }

		bool IsApplicationInactivityInterstitialAvailable { get; }

		bool IsInterstitialAfterResultAvailable { get; }
		
		bool IsInterstitialBeforeResultAvailable { get; }

		bool IsInGameRestartInterstitialAvailable { get; }

		float InactivityGameTimeInterstitial { get; }
	}
}
