using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

#if UNITY_IOS
	using UnityEngine.iOS;
#endif

namespace MoreMountains.NiceVibrations
{
	/// <summary>
	/// This class will allow you to trigger vibrations and haptic feedbacks on both iOS and Android,
	/// or on each specific platform independently.
	///
	/// For haptics patterns, it takes inspiration from the iOS guidelines :
	/// https://developer.apple.com/ios/human-interface-guidelines/user-interaction/feedback
	/// Of course the iOS haptics are called directly as they are, and they're crudely reproduced on Android.
	/// Feel free to tweak the patterns or create your own.
	///
	/// Here's a brief overview of the patterns :
	///
	/// - selection : light
	/// - success : light / heavy
	/// - warning : heavy / medium
	/// - failure : medium / medium / heavy / light
	/// - light
	/// - medium
	/// - heavy
    /// - soft
    /// - rigid
    ///
    /// In addition, this class will also let you trigger core haptics on supported devices running recent versions of iOS (after iOS 13).
    /// These let you trigger transient or continuous haptics, or play AHAP based JSON strings for even more control.
	/// </summary>
	public static class MMVibrationManager
	{
        /// the duration of the light vibration (in microseconds)
		public static long LightDuration = 35;
        /// the duration of the medium vibration (in microseconds)
		public static long MediumDuration = 40;
        /// the duration of the heavy vibration (in microseconds)
        public static long HeavyDuration = 80;
        /// the duration of the rigid vibration (in microseconds)
        public static long RigidDuration = 35;
        /// the duration of the soft vibration (in microseconds)
        public static long SoftDuration = 80;
        /// the amplitude of the light vibration
        public static int LightAmplitude = 60;
        /// the amplitude of the medium vibration
		public static int MediumAmplitude = 120;
        /// the amplitude of the heavy vibration
		public static int HeavyAmplitude = 255;
        /// the amplitude of the rigid vibration
        public static int RigidAmplitude = 255;
        /// the amplitude of the soft vibration
        public static int SoftAmplitude = 35;

        /// the duration of the light vibration (in seconds)
        public static float LightDurationIOS = 0.2f;
        /// the duration of the medium vibration (in seconds)
        public static float MediumDurationIOS = 0.4f;
        /// the duration of the heavy vibration (in seconds)
        public static float HeavyDurationIOS = 0.8f;
        /// the duration of the rigid vibration (in seconds)
        public static float RigidDurationIOS = 0.2f;
        /// the duration of the soft vibration (in seconds)
        public static float SoftDurationIOS = 0.8f;
        /// the amplitude of the light vibration
        public static float LightAmplitudeIOS = 0.6f;
        /// the amplitude of the medium vibration
        public static float MediumAmplitudeIOS = 1f;
        /// the amplitude of the heavy vibration
        public static float HeavyAmplitudeIOS = 1f;
        /// the amplitude of the rigid vibration
        public static float RigidAmplitudeIOS = 2.55f;
        /// the amplitude of the soft vibration
        public static float SoftAmplitudeIOS = 0.4f;
        
        private static bool _vibrationsActive = true;
        private static bool _debugLogActive = false;
        private static bool _hapticsPlayedOnce = false;

        private static long[] _rigidImpactPattern = { 0, RigidDuration };
        private static int[] _rigidImpactPatternAmplitude = { 0, RigidAmplitude };
        private static long[] _softImpactPattern = { 0, SoftDuration };
        private static int[] _softImpactPatternAmplitude = { 0, SoftAmplitude };
        private static long[] _lightImpactPattern = { 0, LightDuration };
        private static int[] _lightImpactPatternAmplitude = { 0, LightAmplitude };
        private static long[] _mediumImpactPattern = { 0, MediumDuration };
        private static int[] _mediumImpactPatternAmplitude = { 0, MediumAmplitude };
        private static long[] _HeavyImpactPattern = { 0, HeavyDuration };
        private static int[] _HeavyImpactPatternAmplitude = { 0, HeavyAmplitude };
        private static long[] _successPattern = { 0, LightDuration, LightDuration, HeavyDuration};
		private static int[] _successPatternAmplitude = { 0, LightAmplitude, 0, HeavyAmplitude};
		private static long[] _warningPattern = { 0, HeavyDuration, LightDuration, MediumDuration};
		private static int[] _warningPatternAmplitude = { 0, HeavyAmplitude, 0, MediumAmplitude};
		private static long[] _failurePattern = { 0, MediumDuration, LightDuration, MediumDuration, LightDuration, HeavyDuration, LightDuration, LightDuration};
		private static int[] _failurePatternAmplitude = { 0, MediumAmplitude, 0, MediumAmplitude, 0, HeavyAmplitude, 0, LightAmplitude};
        

        /// <summary>
        /// On construction, computes the current iOS version
        /// </summary>
        static MMVibrationManager()
        {
            DebugLog("[MMVibrationManager] Initialize vibration manager");
            iOSInitializeHaptics();
        }
        

        /// <summary>
        /// Enables or disables all haptics called via this class
        /// </summary>
        /// <param name="status"></param>
        public static void SetHapticsActive(bool status)
        {
            DebugLog("[MMVibrationManager] Set haptics active : "+status);
            _vibrationsActive = status;
            if (!status)
            {
                StopAllHaptics();
            }
        }

        /// <summary>
        /// Returns true if haptics are supported on this device
        /// </summary>
        /// <returns></returns>
       

        /// <summary>
        /// Enables or disables console logs (off by default)
        /// </summary>
        /// <param name="log"></param>
        public static void SetDebugMode(bool log)
        {
            _debugLogActive = log;
        }

        /// <summary>
        /// Returns true if the current platform is Android, false otherwise.
        /// </summary>
        public static bool Android()
		{
            return MMNVPlatform.Android();
        }

		/// <summary>
		/// Returns true if the current platform is iOS, false otherwise
		/// </summary>
		/// <returns><c>true</c>, if O was ied, <c>false</c> otherwise.</returns>
		public static bool iOS()
		{
            return MMNVPlatform.iOS();
		}

		/// <summary>
		/// Triggers a simple vibration
		/// </summary>
		public static void Vibrate()
        {
            DebugLog("[MMVibrationManager] Vibrate");
            if (!_vibrationsActive)
            {
                return;
            }
            if (Android())
			{
			    MMNVAndroid.AndroidVibrate (MediumDuration);
			}
			else if (iOS())
			{
#if UNITY_IOS
				Handheld.Vibrate();
#endif
			}
		}

		/// <summary>
		/// Triggers a haptic feedback of the specified type
		/// </summary>
		/// <param name="type">Type.</param>
		public static void Haptic(HapticTypes type, bool defaultToRegularVibrate = false)
		{
            if (!_vibrationsActive)
            {
                return;
            }

            DebugLog("[MMVibrationManager] Regular Haptic");

            if (Android())
			{
				switch (type)
				{
                    case HapticTypes.None:
                        // do nothing
                        break;
					case HapticTypes.Selection:
						MMNVAndroid.AndroidVibrate (LightDuration, LightAmplitude);
						break;

					case HapticTypes.Success:
                        MMNVAndroid.AndroidVibrate(_successPattern, _successPatternAmplitude, -1);
						break;

					case HapticTypes.Warning:
                        MMNVAndroid.AndroidVibrate(_warningPattern, _warningPatternAmplitude, -1);
						break;

					case HapticTypes.Failure:
                        MMNVAndroid.AndroidVibrate(_failurePattern, _failurePatternAmplitude, -1);
						break;

					case HapticTypes.LightImpact:
                        MMNVAndroid.AndroidVibrate (_lightImpactPattern, _lightImpactPatternAmplitude, -1);
						break;

					case HapticTypes.MediumImpact:
                        MMNVAndroid.AndroidVibrate (_mediumImpactPattern, _mediumImpactPatternAmplitude, -1);
						break;

					case HapticTypes.HeavyImpact:
                        MMNVAndroid.AndroidVibrate (_HeavyImpactPattern, _HeavyImpactPatternAmplitude, -1);
						break;
				}
			}
            else if (iOS ())
            {
	            iOSTriggerHaptics (type, defaultToRegularVibrate);
            } 
        }

        /// <summary>
        /// Plays a transient haptic, a single, short haptic feedback, of the specified intensity and sharpness
        /// </summary>
        /// <param name="intensity"></param>
        /// <param name="sharpness"></param>
        public static void TransientHaptic(float intensity)
        {
            TransientHaptic(true, intensity, true, intensity, true);
        }

        /// <summary>
        /// Plays a transient haptic, signature with more fine control
        /// </summary>
        /// <param name="vibrateiOS"></param>
        /// <param name="iOSIntensity"></param>
        /// <param name="vibrateAndroid"></param>
        /// <param name="androidIntensity"></param>
        /// <param name="vibrateAndroidIfNoSupport"></param>
        public static void TransientHaptic(bool vibrateiOS, float iOSIntensity,
                                            bool vibrateAndroid, float androidIntensity = 1f,
                                            bool vibrateAndroidIfNoSupport = false)
        {
            if (!_vibrationsActive)
            {
                return;
            }

            DebugLog("[MMVibrationManager] Transient Haptic");

            if (Android() && vibrateAndroid)
            {
                if (!MMNVAndroid.AndroidHasAmplitudeControl() && !vibrateAndroidIfNoSupport)
                {
                    return;
                }
                androidIntensity = Remap(androidIntensity, 0f, 1f, 0, 255);
                MMNVAndroid.AndroidVibrate(100, (int)(androidIntensity));
            }
            else if (iOS() && vibrateiOS)
            {
                if (iOSIntensity < 0.3f)
                {
                    Haptic(HapticTypes.LightImpact);
                }
                else if ((iOSIntensity >= 0.3f) && (iOSIntensity < 0.6f))
                {
                    Haptic(HapticTypes.MediumImpact);
                }
                else
                {
                    Haptic(HapticTypes.HeavyImpact);
                }
            }
        }


        /// <summary>
        /// Stops all currently running haptics
        /// </summary>
        /// <param name="alsoRumble"></param>
		public static void StopAllHaptics(bool alsoRumble = false)
		{
            if (!_hapticsPlayedOnce)
            {
                return;
            }

            DebugLog("[MMVibrationManager] Stop all haptics");
            if (iOS()) ReleaseFeedbackGenerators();
			MMNVAndroid.AndroidCancelVibrations();
		}

        /// <summary>
        /// Stops all running pattern or continuous haptics
        /// </summary>
        public static void StopContinuousHaptic(bool alsoRumble = false)
        {
            DebugLog("[MMVibrationManager] Stop Continuous Haptic");
            if (iOS()) ReleaseFeedbackGenerators();
            MMNVAndroid.AndroidCancelVibrations();
        }

        /// <summary>
        /// Plays a haptic pattern, the most complex type of haptic, defined by a JSON string on iOS, and a pattern on Android
        /// </summary>
        /// <param name="androidPattern"></param>
        /// <param name="androidAmplitudes"></param>
        /// <param name="androidRepeat"></param>
        public static void AdvancedHapticPattern(long[] androidPattern, int[] androidAmplitudes, int androidRepeat)
        {
            AdvancedHapticPattern(androidPattern, androidAmplitudes, androidRepeat, false, true);
        }

        /// <summary>
        /// Plays a advanced haptic pattern, 
        /// </summary>
        /// <param name="androidPattern"></param>
        /// <param name="androidAmplitudes"></param>
        /// <param name="androidRepeat"></param>
        /// <param name="vibrateAndroidIfNoSupport"></param>

        public static void AdvancedHapticPattern(long[] androidPattern, int[] androidAmplitudes, int androidRepeat,
                                                bool vibrateAndroidIfNoSupport, bool threaded = false)
        {
            if (!_vibrationsActive)
            {
                return;
            }

            DebugLog("[MMVibrationManager] Advanced Haptic Pattern");

            if (Android())
            {
                if (!MMNVAndroid.AndroidHasAmplitudeControl() && !vibrateAndroidIfNoSupport)
                {
                    return;
                }
                MMNVAndroid.AndroidVibrate(androidPattern, androidAmplitudes, androidRepeat, threaded);
            }

        }

        // DEBUG -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Outputs the specified string to the console if in debug mode
        /// </summary>
        /// <param name="log"></param>
        private static void DebugLog(string log)
        {
            if (_debugLogActive)
            {
                Debug.Log(log);
            }
        }

        /// <summary>
        /// Remaps value x between AB to CD
        /// </summary>
        /// <param name="x"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static float Remap(float x, float A, float B, float C, float D)
        {
            float remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }
        
        
        // iOS ----------------------------------------------------------------------------------------------------------------

		// The following will only work if the iOSHapticInterface.m file is in a Plugins folder in your project.
		// It's a pretty straightforward implementation of iOS's UIFeedbackGenerator's methods.
		// You can learn more about them there : https://developer.apple.com/documentation/uikit/uifeedbackgenerator

		#if UNITY_IOS && !UNITY_EDITOR
			[DllImport ("__Internal")]
			private static extern void InstantiateFeedbackGenerators();
			[DllImport ("__Internal")]
			private static extern void ReleaseFeedbackGenerators();
			[DllImport ("__Internal")]
			private static extern void SelectionHaptic();
			[DllImport ("__Internal")]
			private static extern void SuccessHaptic();
			[DllImport ("__Internal")]
			private static extern void WarningHaptic();
			[DllImport ("__Internal")]
			private static extern void FailureHaptic();
			[DllImport ("__Internal")]
			private static extern void LightImpactHaptic();
			[DllImport ("__Internal")]
			private static extern void MediumImpactHaptic();
			[DllImport ("__Internal")]
			private static extern void HeavyImpactHaptic();
		#else
			private static void InstantiateFeedbackGenerators() {}
			private static void ReleaseFeedbackGenerators() {}
			private static void SelectionHaptic() {}
			private static void SuccessHaptic() {}
			private static void WarningHaptic() {}
			private static void FailureHaptic() {}
			private static void LightImpactHaptic() {}
			private static void MediumImpactHaptic() {}
			private static void HeavyImpactHaptic() {}
		#endif
		private static bool iOSHapticsInitialized = false;

		/// <summary>
		/// Call this method to initialize the haptics. If you forget to do it, Nice Vibrations will do it for you the first time you
		/// call iOSTriggerHaptics. It's better if you do it though.
		/// </summary>
		public static void iOSInitializeHaptics()
		{
			if (!iOS ()) { return; }
			InstantiateFeedbackGenerators ();
			iOSHapticsInitialized = true;
		}

		/// <summary>
		/// Releases the feedback generators, usually you'll want to call this at OnDisable(); or anytime you know you won't need 
		/// vibrations anymore.
		/// </summary>
		public static void iOSReleaseHaptics ()
		{
			if (!iOS ()) { return; }
			ReleaseFeedbackGenerators ();
		}

		/// <summary>
		/// This methods tests the current device generation against a list of devices that don't support haptics, and returns true if haptics are supported, false otherwise.
		/// </summary>
		/// <returns><c>true</c>, if supported was hapticsed, <c>false</c> otherwise.</returns>
		public static bool HapticsSupported()
		{
			bool hapticsSupported = false;
#if UNITY_IOS
			DeviceGeneration generation = Device.generation;
			if ((generation == DeviceGeneration.iPhone3G)
			|| (generation == DeviceGeneration.iPhone3GS)
			|| (generation == DeviceGeneration.iPodTouch1Gen)
			|| (generation == DeviceGeneration.iPodTouch2Gen)
			|| (generation == DeviceGeneration.iPodTouch3Gen)
			|| (generation == DeviceGeneration.iPodTouch4Gen)
			|| (generation == DeviceGeneration.iPhone4)
			|| (generation == DeviceGeneration.iPhone4S)
			|| (generation == DeviceGeneration.iPhone5)
			|| (generation == DeviceGeneration.iPhone5C)
			|| (generation == DeviceGeneration.iPhone5S)
			|| (generation == DeviceGeneration.iPhone6)
			|| (generation == DeviceGeneration.iPhone6Plus)
			|| (generation == DeviceGeneration.iPhone6S)
			|| (generation == DeviceGeneration.iPhone6SPlus)
            || (generation == DeviceGeneration.iPhoneSE1Gen)
            || (generation == DeviceGeneration.iPad1Gen)
            || (generation == DeviceGeneration.iPad2Gen)
            || (generation == DeviceGeneration.iPad3Gen)
            || (generation == DeviceGeneration.iPad4Gen)
            || (generation == DeviceGeneration.iPad5Gen)
            || (generation == DeviceGeneration.iPadAir1)
            || (generation == DeviceGeneration.iPadAir2)
            || (generation == DeviceGeneration.iPadMini1Gen)
            || (generation == DeviceGeneration.iPadMini2Gen)
            || (generation == DeviceGeneration.iPadMini3Gen)
            || (generation == DeviceGeneration.iPadMini4Gen)
            || (generation == DeviceGeneration.iPadPro10Inch1Gen)
            || (generation == DeviceGeneration.iPadPro10Inch2Gen)
            || (generation == DeviceGeneration.iPadPro11Inch)
            || (generation == DeviceGeneration.iPadPro1Gen)
            || (generation == DeviceGeneration.iPadPro2Gen)
            || (generation == DeviceGeneration.iPadPro3Gen)
            || (generation == DeviceGeneration.iPadUnknown)
            || (generation == DeviceGeneration.iPodTouch1Gen)
            || (generation == DeviceGeneration.iPodTouch2Gen)
            || (generation == DeviceGeneration.iPodTouch3Gen)
            || (generation == DeviceGeneration.iPodTouch4Gen)
            || (generation == DeviceGeneration.iPodTouch5Gen)
            || (generation == DeviceGeneration.iPodTouch6Gen)
			|| (generation == DeviceGeneration.iPhone6SPlus))
			{
			    hapticsSupported = false;
			}
			else
			{
			    hapticsSupported = true;
			}
#endif

                return hapticsSupported;


        }
	
		/// <summary>
		/// iOS only : triggers a haptic feedback of the specified type
		/// </summary>
		/// <param name="type">Type.</param>
		public static void iOSTriggerHaptics(HapticTypes type, bool defaultToRegularVibrate = false)
		{
			if (!iOS ()) { return; }

			if (!iOSHapticsInitialized)
			{
				iOSInitializeHaptics ();
			}

			// this will trigger a standard vibration on all the iOS devices that don't support haptic feedback

			if (HapticsSupported())
			{
				switch (type)
				{
					case HapticTypes.Selection:
						SelectionHaptic ();
						break;

					case HapticTypes.Success:
						SuccessHaptic ();
						break;

					case HapticTypes.Warning:
						WarningHaptic ();
						break;

					case HapticTypes.Failure:
						FailureHaptic ();
						break;

					case HapticTypes.LightImpact:
						LightImpactHaptic ();
						break;

					case HapticTypes.MediumImpact:
						MediumImpactHaptic ();
						break;

					case HapticTypes.HeavyImpact:
						HeavyImpactHaptic ();
						break;
				}
			}
			else if (defaultToRegularVibrate)
			{
				#if UNITY_IOS 
					Handheld.Vibrate();
				#endif
			}
		}

		/// <summary>
		/// Returns a string containing iOS SDK informations
		/// </summary>
		/// <returns>The OSSDK version.</returns>
		public static string iOSSDKVersion() 
		{
			#if UNITY_IOS && !UNITY_EDITOR
				return Device.systemVersion;
			#else
				return null;
			#endif
		}

    }
}
