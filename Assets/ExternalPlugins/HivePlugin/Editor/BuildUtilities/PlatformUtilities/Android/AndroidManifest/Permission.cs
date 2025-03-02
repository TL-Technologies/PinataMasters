using System;

// Source: https://developer.android.com/reference/android/Manifest.permission.html

namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public struct Permission
    {
        /// <summary>
        /// Allows query of any normal app on the device, regardless of manifest declarations.
        /// Added in API level 30.
        /// <para>Corresponds to 'android.permission.QUERY_ALL_PACKAGES'.</para>
        /// </summary>
        public static Permission QueryAllPackages => new Permission("android.permission.QUERY_ALL_PACKAGES", PermissionProtectionLevel.Normal);
        
        /// <summary>
        /// Allows a calling app to continue a call which was started in another app.
        /// <para>Corresponds to 'android.permission.ACCEPT_HANDOVER'.</para>
        /// </summary>
        public static Permission AcceptHandover => new Permission("android.permission.ACCEPT_HANDOVER", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an app to access location in the background.
        /// <para>Corresponds to 'android.permission.ACCESS_BACKGROUND_LOCATION'.</para>
        /// </summary>
        public static Permission AccessBackgroundLocation => new Permission("android.permission.ACCESS_BACKGROUND_LOCATION", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an app to access approximate location.
        /// <para>Corresponds to 'android.permission.ACCESS_COARSE_LOCATION'.</para>
        /// </summary>
        public static Permission AccessCoarseLocation => new Permission("android.permission.ACCESS_COARSE_LOCATION", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an app to access precise location.
        /// <para>Corresponds to 'android.permission.ACCESS_FINE_LOCATION'.</para>
        /// </summary>
        public static Permission AccessFineLocation => new Permission("android.permission.ACCESS_FINE_LOCATION", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to access extra location provider commands.
        /// <para>Corresponds to 'android.permission.ACCESS_LOCATION_EXTRA_COMMANDS'.</para>
        /// </summary>
        public static Permission AccessLocationExtraCommands => new Permission("android.permission.ACCESS_LOCATION_EXTRA_COMMANDS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to access any geographic locations persisted in the user's shared collection.
        /// <para>Corresponds to 'android.permission.ACCESS_MEDIA_LOCATION'.</para>
        /// </summary>
        public static Permission AccessMediaLocation => new Permission("android.permission.ACCESS_MEDIA_LOCATION", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows applications to access information about networks.
        /// <para>Corresponds to 'android.permission.ACCESS_NETWORK_STATE'.</para>
        /// </summary>
        public static Permission AccessNetworkState => new Permission("android.permission.ACCESS_NETWORK_STATE", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Marker permission for applications that wish to access notification policy.
        /// <para>Corresponds to 'android.permission.ACCESS_NOTIFICATION_POLICY'.</para>
        /// </summary>
        public static Permission AccessNotificationPolicy => new Permission("android.permission.ACCESS_NOTIFICATION_POLICY", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows applications to access information about Wi-Fi networks.
        /// <para>Corresponds to 'android.permission.ACCESS_WIFI_STATE'.</para>
        /// </summary>
        public static Permission AccessWifiState => new Permission("android.permission.ACCESS_WIFI_STATE", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to recognize physical activity.
        /// <para>Corresponds to 'android.permission.ACTIVITY_RECOGNITION'.</para>
        /// </summary>
        public static Permission ActivityRecognition => new Permission("android.permission.ACTIVITY_RECOGNITION", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to add voicemails into the system.
        /// <para>Corresponds to 'android.permission.ADD_VOICEMAIL'.</para>
        /// </summary>
        public static Permission AddVoicemail => new Permission("android.permission.ADD_VOICEMAIL", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows the app to answer an incoming phone call.
        /// <para>Corresponds to 'android.permission.ANSWER_PHONE_CALLS'.</para>
        /// </summary>
        public static Permission AnswerPhoneCalls => new Permission("android.permission.ANSWER_PHONE_CALLS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to collect battery statistics.
        /// <para>Corresponds to 'android.permission.BATTERY_STATS'.</para>
        /// </summary>
        public static Permission BatteryStats => new Permission(
            "android.permission.BATTERY_STATS",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged | PermissionProtectionLevel.Development);

        /// <summary>
        /// Must be required by an AccessibilityService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_ACCESSIBILITY_SERVICE'.</para>
        /// </summary>
        public static Permission BindAccessibilityService => new Permission("android.permission.BIND_ACCESSIBILITY_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by a AutofillService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_AUTOFILL_SERVICE'.</para>
        /// </summary>
        public static Permission BindAutofillService => new Permission("android.permission.BIND_AUTOFILL_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by a CallRedirectionService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_CALL_REDIRECTION_SERVICE'.</para>
        /// </summary>
        public static Permission BindCallRedirectionService => new Permission(
            "android.permission.BIND_CALL_REDIRECTION_SERVICE",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// A subclass of CarrierMessagingClientService must be protected with this permission.
        /// <para>Corresponds to 'android.permission.BIND_CARRIER_MESSAGING_CLIENT_SERVICE'.</para>
        /// </summary>
        public static Permission BindCarrierMessagingClientService => new Permission("android.permission.BIND_CARRIER_MESSAGING_CLIENT_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// This constant was deprecated in API level 23. Use BIND_CARRIER_SERVICES instead
        /// <para>Corresponds to 'android.permission.BIND_CARRIER_MESSAGING_SERVICE'.</para>
        /// </summary>
        [Obsolete("This constant was deprecated in API level 23. Use BIND_CARRIER_SERVICES instead")]
        public static Permission BindCarrierMessagingService => new Permission(
            "android.permission.BIND_CARRIER_MESSAGING_SERVICE",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// The system process that is allowed to bind to services in carrier apps will have this permission.
        /// <para>Corresponds to 'android.permission.BIND_CARRIER_SERVICES'.</para>
        /// </summary>
        public static Permission BindCarrierServices => new Permission(
            "android.permission.BIND_CARRIER_SERVICES",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Must be required by a ChooserTargetService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_CHOOSER_TARGET_SERVICE'.</para>
        /// </summary>
        public static Permission BindChooserTargetService => new Permission("android.permission.BIND_CHOOSER_TARGET_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by a ConditionProviderService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_CONDITION_PROVIDER_SERVICE'.</para>
        /// </summary>
        public static Permission BindConditionProviderService => new Permission("android.permission.BIND_CONDITION_PROVIDER_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by device administration receiver, to ensure that only the system can interact with it.
        /// <para>Corresponds to 'android.permission.BIND_DEVICE_ADMIN'.</para>
        /// </summary>
        public static Permission BindDeviceAdmin => new Permission("android.permission.BIND_DEVICE_ADMIN", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by an DreamService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_DREAM_SERVICE'.</para>
        /// </summary>
        public static Permission BindDreamService => new Permission("android.permission.BIND_DREAM_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by a InCallService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_INCALL_SERVICE'.</para>
        /// </summary>
        public static Permission BindIncallService => new Permission(
            "android.permission.BIND_INCALL_SERVICE",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Must be required by an InputMethodService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_INPUT_METHOD'.</para>
        /// </summary>
        public static Permission BindInputMethod => new Permission("android.permission.BIND_INPUT_METHOD", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by an MidiDeviceService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_MIDI_DEVICE_SERVICE'.</para>
        /// </summary>
        public static Permission BindMidiDeviceService => new Permission("android.permission.BIND_MIDI_DEVICE_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by a HostApduService or OffHostApduService to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_NFC_SERVICE'.</para>
        /// </summary>
        public static Permission BindNfcService => new Permission("android.permission.BIND_NFC_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by an NotificationListenerService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_NOTIFICATION_LISTENER_SERVICE'.</para>
        /// </summary>
        public static Permission BindNotificationListenerService => new Permission("android.permission.BIND_NOTIFICATION_LISTENER_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by a PrintService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_PRINT_SERVICE'.</para>
        /// </summary>
        public static Permission BindPrintService => new Permission("android.permission.BIND_PRINT_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by a RemoteViewsService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_REMOTEVIEWS'.</para>
        /// </summary>
        public static Permission BindRemoteViews => new Permission(
            "android.permission.BIND_REMOTEVIEWS",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Must be required by a CallScreeningService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_SCREENING_SERVICE'.</para>
        /// </summary>
        public static Permission BindScreeningService => new Permission(
            "android.permission.BIND_SCREENING_SERVICE",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Must be required by a ConnectionService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_TELECOM_CONNECTION_SERVICE'.</para>
        /// </summary>
        public static Permission BindTelecomConnectionService => new Permission(
            "android.permission.BIND_TELECOM_CONNECTION_SERVICE",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Must be required by a TextService (e.g.
        /// <para>Corresponds to 'android.permission.BIND_TEXT_SERVICE'.</para>
        /// </summary>
        public static Permission BindTextService => new Permission("android.permission.BIND_TEXT_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by a TvInputService to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_TV_INPUT'.</para>
        /// </summary>
        public static Permission BindTvInput => new Permission(
            "android.permission.BIND_TV_INPUT",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Must be required by a link VisualVoicemailService to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_VISUAL_VOICEMAIL_SERVICE'.</para>
        /// </summary>
        public static Permission BindVisualVoicemailService => new Permission(
            "android.permission.BIND_VISUAL_VOICEMAIL_SERVICE",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Must be required by a VoiceInteractionService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_VOICE_INTERACTION'.</para>
        /// </summary>
        public static Permission BindVoiceInteraction => new Permission("android.permission.BIND_VOICE_INTERACTION", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by a VpnService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_VPN_SERVICE'.</para>
        /// </summary>
        public static Permission BindVpnService => new Permission("android.permission.BIND_VPN_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by an VrListenerService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_VR_LISTENER_SERVICE'.</para>
        /// </summary>
        public static Permission BindVrListenerService => new Permission("android.permission.BIND_VR_LISTENER_SERVICE", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Must be required by a WallpaperService, to ensure that only the system can bind to it.
        /// <para>Corresponds to 'android.permission.BIND_WALLPAPER'.</para>
        /// </summary>
        public static Permission BindWallpaper => new Permission(
            "android.permission.BIND_WALLPAPER",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Allows applications to connect to paired bluetooth devices.
        /// <para>Corresponds to 'android.permission.BLUETOOTH'.</para>
        /// </summary>
        public static Permission Bluetooth => new Permission("android.permission.BLUETOOTH", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows applications to discover and pair bluetooth devices.
        /// <para>Corresponds to 'android.permission.BLUETOOTH_ADMIN'.</para>
        /// </summary>
        public static Permission BluetoothAdmin => new Permission("android.permission.BLUETOOTH_ADMIN", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to access data from sensors that the user uses to measure what is happening inside his/her body, such as heart rate.
        /// <para>Corresponds to 'android.permission.BODY_SENSORS'.</para>
        /// </summary>
        public static Permission BodySensors => new Permission("android.permission.BODY_SENSORS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to broadcast sticky intents.
        /// <para>Corresponds to 'android.permission.BROADCAST_STICKY'.</para>
        /// </summary>
        public static Permission BroadcastSticky => new Permission("android.permission.BROADCAST_STICKY", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an app which implements the InCallService API to be eligible to be enabled as a calling companion app.
        /// <para>Corresponds to 'android.permission.CALL_COMPANION_APP'.</para>
        /// </summary>
        public static Permission CallCompanionApp => new Permission("android.permission.CALL_COMPANION_APP", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to initiate a phone call without going through the Dialer user interface for the user to confirm the call.
        /// <para>Corresponds to 'android.permission.CALL_PHONE'.</para>
        /// </summary>
        public static Permission CallPhone => new Permission("android.permission.CALL_PHONE", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Required to be able to access the camera device.
        /// <para>Corresponds to 'android.permission.CAMERA'.</para>
        /// </summary>
        public static Permission Camera => new Permission("android.permission.CAMERA", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to modify the current configuration, such as locale.
        /// <para>Corresponds to 'android.permission.CHANGE_CONFIGURATION'.</para>
        /// </summary>
        public static Permission ChangeConfiguration => new Permission(
            "android.permission.CHANGE_CONFIGURATION",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged | PermissionProtectionLevel.Development);

        /// <summary>
        /// Allows applications to change network connectivity state.
        /// <para>Corresponds to 'android.permission.CHANGE_NETWORK_STATE'.</para>
        /// </summary>
        public static Permission ChangeNetworkState => new Permission("android.permission.CHANGE_NETWORK_STATE", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows applications to enter Wi-Fi Multicast mode.
        /// <para>Corresponds to 'android.permission.CHANGE_WIFI_MULTICAST_STATE'.</para>
        /// </summary>
        public static Permission ChangeWifiMulticastState => new Permission("android.permission.CHANGE_WIFI_MULTICAST_STATE", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows applications to change Wi-Fi connectivity state.
        /// <para>Corresponds to 'android.permission.CHANGE_WIFI_STATE'.</para>
        /// </summary>
        public static Permission ChangeWifiState => new Permission("android.permission.CHANGE_WIFI_STATE", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to clear the caches of all installed applications on the device.
        /// <para>Corresponds to 'android.permission.CLEAR_APP_CACHE'.</para>
        /// </summary>
        public static Permission ClearAppCache => new Permission(
            "android.permission.CLEAR_APP_CACHE",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Old permission for deleting an app's cache files, no longer used, but signals for us to quietly ignore calls instead of throwing an exception.
        /// <para>Corresponds to 'android.permission.DELETE_CACHE_FILES'.</para>
        /// </summary>
        public static Permission DeleteCacheFiles => new Permission(
            "android.permission.DELETE_CACHE_FILES",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Allows applications to disable the keyguard if it is not secure.
        /// <para>Corresponds to 'android.permission.DISABLE_KEYGUARD'.</para>
        /// </summary>
        public static Permission DisableKeyguard => new Permission("android.permission.DISABLE_KEYGUARD", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to expand or collapse the status bar.
        /// <para>Corresponds to 'android.permission.EXPAND_STATUS_BAR'.</para>
        /// </summary>
        public static Permission ExpandStatusBar => new Permission("android.permission.EXPAND_STATUS_BAR", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows a regular application to use Service.startForeground.
        /// <para>Corresponds to 'android.permission.FOREGROUND_SERVICE'.</para>
        /// </summary>
        public static Permission ForegroundService => new Permission("android.permission.FOREGROUND_SERVICE", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows access to the list of accounts in the Accounts Service.
        /// <para>Note: Beginning with Android 6.0 (API level 23), if an app shares the signature of the authenticator that manages an account,
        /// it does not need "GET_ACCOUNTS" permission to read information about that account. On Android 5.1 and lower,
        /// all apps need "GET_ACCOUNTS" permission to read information about any account.</para>
        /// <para>Corresponds to 'android.permission.GET_ACCOUNTS'.</para>
        /// </summary>
        public static Permission GetAccounts => new Permission("android.permission.GET_ACCOUNTS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows access to the list of accounts in the Accounts Service.
        /// <para>Corresponds to 'android.permission.GET_ACCOUNTS_PRIVILEGED'.</para>
        /// </summary>
        public static Permission GetAccountsPrivileged => new Permission(
            "android.permission.GET_ACCOUNTS_PRIVILEGED",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Allows an application to find out the space used by any package.
        /// <para>Corresponds to 'android.permission.GET_PACKAGE_SIZE'.</para>
        /// </summary>
        public static Permission GetPackageSize => new Permission("android.permission.GET_PACKAGE_SIZE", PermissionProtectionLevel.Normal);

        /// <summary>
        /// This constant was deprecated in API level 21. No longer enforced.
        /// <para>Corresponds to 'android.permission.GET_TASKS'.</para>
        /// </summary>
        [Obsolete("Deprecated in API level 21. No longer enforced.")]
        public static Permission GetTasks => new Permission("android.permission.GET_TASKS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// This permission can be used on content providers to allow the global search system to access their data.
        /// <para>Corresponds to 'android.permission.GLOBAL_SEARCH'.</para>
        /// </summary>
        public static Permission GlobalSearch => new Permission(
            "android.permission.GLOBAL_SEARCH",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Allows an application to install a shortcut in Launcher.
        /// <para>Corresponds to 'android.permission.INSTALL_SHORTCUT'.</para>
        /// </summary>
        public static Permission InstallShortcut => new Permission("android.permission.INSTALL_SHORTCUT", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an instant app to create foreground services.
        /// <para>Corresponds to 'android.permission.INSTANT_APP_FOREGROUND_SERVICE'.</para>
        /// </summary>
        public static Permission InstantAppForegroundService => new Permission(
            "android.permission.INSTANT_APP_FOREGROUND_SERVICE",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Development | PermissionProtectionLevel.Instant | PermissionProtectionLevel.AppOp);

        /// <summary>
        /// Allows applications to open network sockets.
        /// <para>Corresponds to 'android.permission.INTERNET'.</para>
        /// </summary>
        public static Permission Internet => new Permission("android.permission.INTERNET", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to call ActivityManager.killBackgroundProcesses(String).
        /// <para>Corresponds to 'android.permission.KILL_BACKGROUND_PROCESSES'.</para>
        /// </summary>
        public static Permission KillBackgroundProcesses => new Permission("android.permission.KILL_BACKGROUND_PROCESSES", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to manage access to documents, usually as part of a document picker.
        /// <para>This permission should only be requested by the platform document management app.
        /// This permission cannot be granted to third-party apps.</para>
        /// <para>Corresponds to 'android.permission.MANAGE_DOCUMENTS'.</para>
        /// </summary>
        public static Permission ManageDocuments => new Permission("android.permission.MANAGE_DOCUMENTS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows a calling application which manages it own calls through the self-managed ConnectionService APIs.
        /// <para>Corresponds to 'android.permission.MANAGE_OWN_CALLS'.</para>
        /// </summary>
        public static Permission ManageOwnCalls => new Permission("android.permission.MANAGE_OWN_CALLS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to modify global audio settings.
        /// <para>Corresponds to 'android.permission.MODIFY_AUDIO_SETTINGS'.</para>
        /// </summary>
        public static Permission ModifyAudioSettings => new Permission("android.permission.MODIFY_AUDIO_SETTINGS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows applications to perform I/O operations over NFC.
        /// <para>Corresponds to 'android.permission.NFC'.</para>
        /// </summary>
        public static Permission Nfc => new Permission("android.permission.NFC", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows applications to receive NFC transaction events.
        /// <para>Corresponds to 'android.permission.NFC_TRANSACTION_EVENT'.</para>
        /// </summary>
        public static Permission NfcTransactionEvent => new Permission("android.permission.NFC_TRANSACTION_EVENT", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to collect component usage statistics
        /// Declaring the permission implies intention to use the API and the user of the device can grant permission through the Settings application.
        /// <para>Corresponds to 'android.permission.PACKAGE_USAGE_STATS'.</para>
        /// </summary>
        public static Permission PackageUsageStats => new Permission(
            "android.permission.PACKAGE_USAGE_STATS",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged | PermissionProtectionLevel.Development | PermissionProtectionLevel.AppOp);

        /// <summary>
        /// This constant was deprecated in API level 15. This functionality will be removed in the future; please do not use. Allow an application to make its activities persistent.
        /// <para>Corresponds to 'android.permission.PERSISTENT_ACTIVITY'.</para>
        /// </summary>
        [Obsolete("Deprecated in API level 15.")]
        public static Permission PersistentActivity => new Permission("android.permission.PERSISTENT_ACTIVITY", PermissionProtectionLevel.Normal);

        /// <summary>
        /// This constant was deprecated in API level 29. Applications should use CallRedirectionService instead of the Intent.ACTION_NEW_OUTGOING_CALL broadcast.
        /// <para>Corresponds to 'android.permission.PROCESS_OUTGOING_CALLS'.</para>
        /// </summary>
        [Obsolete("Deprecated in API level 29.")]
        public static Permission ProcessOutgoingCalls => new Permission("android.permission.PROCESS_OUTGOING_CALLS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to read the user's calendar data.
        /// <para>Corresponds to 'android.permission.READ_CALENDAR'.</para>
        /// </summary>
        public static Permission ReadCalendar => new Permission("android.permission.READ_CALENDAR", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to read the user's call log.
        /// <para>Corresponds to 'android.permission.READ_CALL_LOG'.</para>
        /// </summary>
        public static Permission ReadCallLog => new Permission("android.permission.READ_CALL_LOG", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to read the user's contacts data.
        /// <para>Corresponds to 'android.permission.READ_CONTACTS'.</para>
        /// </summary>
        public static Permission ReadContacts => new Permission("android.permission.READ_CONTACTS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to read from external storage.
        /// <para>Corresponds to 'android.permission.READ_EXTERNAL_STORAGE'.</para>
        /// </summary>
        public static Permission ReadExternalStorage => new Permission("android.permission.READ_EXTERNAL_STORAGE", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows read access to the device's phone number(s).
        /// <para>Corresponds to 'android.permission.READ_PHONE_NUMBERS'.</para>
        /// </summary>
        public static Permission ReadPhoneNumbers => new Permission("android.permission.READ_PHONE_NUMBERS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows read only access to phone state, including the phone number of the device, current cellular network information, the status of any ongoing calls, and a list of any PhoneAccounts registered on the device.
        /// <para>Corresponds to 'android.permission.READ_PHONE_STATE'.</para>
        /// </summary>
        public static Permission ReadPhoneState => new Permission("android.permission.READ_PHONE_STATE", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to read SMS messages.
        /// <para>Corresponds to 'android.permission.READ_SMS'.</para>
        /// </summary>
        public static Permission ReadSms => new Permission("android.permission.READ_SMS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows applications to read the sync settings.
        /// <para>Corresponds to 'android.permission.READ_SYNC_SETTINGS'.</para>
        /// </summary>
        public static Permission ReadSyncSettings => new Permission("android.permission.READ_SYNC_SETTINGS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows applications to read the sync stats.
        /// <para>Corresponds to 'android.permission.READ_SYNC_STATS'.</para>
        /// </summary>
        public static Permission ReadSyncStats => new Permission("android.permission.READ_SYNC_STATS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to read voicemails in the system.
        /// <para>Corresponds to 'android.permission.READ_VOICEMAIL'.</para>
        /// </summary>
        public static Permission ReadVoicemail => new Permission(
            "android.permission.READ_VOICEMAIL",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Allows an application to receive the Intent.ACTION_BOOT_COMPLETED that is broadcast after the system finishes booting.
        /// <para>Corresponds to 'android.permission.RECEIVE_BOOT_COMPLETED'.</para>
        /// </summary>
        public static Permission ReceiveBootCompleted => new Permission("android.permission.RECEIVE_BOOT_COMPLETED", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to monitor incoming MMS messages.
        /// <para>Corresponds to 'android.permission.RECEIVE_MMS'.</para>
        /// </summary>
        public static Permission ReceiveMms => new Permission("android.permission.RECEIVE_MMS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to receive SMS messages.
        /// <para>Corresponds to 'android.permission.RECEIVE_SMS'.</para>
        /// </summary>
        public static Permission ReceiveSms => new Permission("android.permission.RECEIVE_SMS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to receive WAP push messages.
        /// <para>Corresponds to 'android.permission.RECEIVE_WAP_PUSH'.</para>
        /// </summary>
        public static Permission ReceiveWapPush => new Permission("android.permission.RECEIVE_WAP_PUSH", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to record audio.
        /// <para>Corresponds to 'android.permission.RECORD_AUDIO'.</para>
        /// </summary>
        public static Permission RecordAudio => new Permission("android.permission.RECORD_AUDIO", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to change the Z-order of tasks.
        /// <para>Corresponds to 'android.permission.REORDER_TASKS'.</para>
        /// </summary>
        public static Permission ReorderTasks => new Permission("android.permission.REORDER_TASKS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows a companion app to run in the background.
        /// <para>Corresponds to 'android.permission.REQUEST_COMPANION_RUN_IN_BACKGROUND'.</para>
        /// </summary>
        public static Permission RequestCompanionRunInBackground => new Permission("android.permission.REQUEST_COMPANION_RUN_IN_BACKGROUND", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows a companion app to use data in the background.
        /// <para>Corresponds to 'android.permission.REQUEST_COMPANION_USE_DATA_IN_BACKGROUND'.</para>
        /// </summary>
        public static Permission RequestCompanionUseDataInBackground => new Permission("android.permission.REQUEST_COMPANION_USE_DATA_IN_BACKGROUND", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to request deleting packages.
        /// <para>Corresponds to 'android.permission.REQUEST_DELETE_PACKAGES'.</para>
        /// </summary>
        public static Permission RequestDeletePackages => new Permission("android.permission.REQUEST_DELETE_PACKAGES", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Permission an application must hold in order to use Settings.ACTION_REQUEST_IGNORE_BATTERY_OPTIMIZATIONS.
        /// <para>Corresponds to 'android.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS'.</para>
        /// </summary>
        public static Permission RequestIgnoreBatteryOptimizations => new Permission("android.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to request installing packages.
        /// <para>Corresponds to 'android.permission.REQUEST_INSTALL_PACKAGES'.</para>
        /// </summary>
        public static Permission RequestInstallPackages => new Permission("android.permission.REQUEST_INSTALL_PACKAGES", PermissionProtectionLevel.Signature);

        /// <summary>
        /// Allows an application to request the screen lock complexity and prompt users to update the screen lock to a certain complexity level.
        /// <para>Corresponds to 'android.permission.REQUEST_PASSWORD_COMPLEXITY'.</para>
        /// </summary>
        public static Permission RequestPasswordComplexity => new Permission("android.permission.REQUEST_PASSWORD_COMPLEXITY", PermissionProtectionLevel.Normal);

        /// <summary>
        /// This constant was deprecated in API level 15. The ActivityManager.restartPackage(String) API is no longer supported.
        /// <para>Corresponds to 'android.permission.RESTART_PACKAGES'.</para>
        /// </summary>
        [Obsolete("Deprecated in API level 15.")]
        public static Permission RestartPackages => new Permission("android.permission.RESTART_PACKAGES", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to send SMS messages.
        /// <para>Corresponds to 'android.permission.SEND_SMS'.</para>
        /// </summary>
        public static Permission SendSms => new Permission("android.permission.SEND_SMS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to broadcast an Intent to set an alarm for the user.
        /// <para>Corresponds to 'android.permission.SET_ALARM'.</para>
        /// </summary>
        public static Permission SetAlarm => new Permission("android.permission.SET_ALARM", PermissionProtectionLevel.Normal);

        /// <summary>
        /// This constant was deprecated in API level 15. No longer useful, see PackageManager.addPackageToPreferred(String) for details.
        /// <para>Corresponds to 'android.permission.SET_PREFERRED_APPLICATIONS'.</para>
        /// </summary>
        [Obsolete("Deprecated in API level 15.")]
        public static Permission SetPreferredApplications => new Permission("android.permission.SET_PREFERRED_APPLICATIONS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows applications to set the wallpaper.
        /// <para>Corresponds to 'android.permission.SET_WALLPAPER'.</para>
        /// </summary>
        public static Permission SetWallpaper => new Permission("android.permission.SET_WALLPAPER", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows applications to set the wallpaper hints.
        /// <para>Corresponds to 'android.permission.SET_WALLPAPER_HINTS'.</para>
        /// </summary>
        public static Permission SetWallpaperHints => new Permission("android.permission.SET_WALLPAPER_HINTS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows financial apps to read filtered sms messages.
        /// <para>Corresponds to 'android.permission.SMS_FINANCIAL_TRANSACTIONS'.</para>
        /// </summary>
        public static Permission SmsFinancialTransactions => new Permission(
            "android.permission.SMS_FINANCIAL_TRANSACTIONS",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);

        /// <summary>
        /// Allows the holder to start the permission usage screen for an app.
        /// <para>Corresponds to 'android.permission.START_VIEW_PERMISSION_USAGE'.</para>
        /// </summary>
        public static Permission StartViewPermissionUsage => new Permission(
            "android.permission.START_VIEW_PERMISSION_USAGE",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Installer);

        /// <summary>
        /// Allows an app to create windows using the type WindowManager.LayoutParams.TYPE_APPLICATION_OVERLAY, shown on top of all other apps.
        /// <para>Corresponds to 'android.permission.SYSTEM_ALERT_WINDOW'.</para>
        /// </summary>
        public static Permission SystemAlertWindow => new Permission(
            "android.permission.SYSTEM_ALERT_WINDOW",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Preinstalled | PermissionProtectionLevel.AppOp | PermissionProtectionLevel.Pre23 | PermissionProtectionLevel.Development);

        /// <summary>
        /// Allows using the device's IR transmitter, if available.
        /// <para>Corresponds to 'android.permission.TRANSMIT_IR'.</para>
        /// </summary>
        public static Permission TransmitIr => new Permission("android.permission.TRANSMIT_IR", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an app to use device supported biometric modalities.
        /// <para>Corresponds to 'android.permission.USE_BIOMETRIC'.</para>
        /// </summary>
        public static Permission UseBiometric => new Permission("android.permission.USE_BIOMETRIC", PermissionProtectionLevel.Normal);

        /// <summary>
        /// This constant was deprecated in API level 28. Applications should request USE_BIOMETRIC instead
        /// <para>Corresponds to 'android.permission.USE_FINGERPRINT'.</para>
        /// </summary>
        [Obsolete("Deprecated in API level 28. Use USE_BIOMETRIC instead.")]
        public static Permission UseFingerprint => new Permission("android.permission.USE_FINGERPRINT", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Required for apps targeting Build.VERSION_CODES.Q that want to use notification full screen intents.
        /// <para>Corresponds to 'android.permission.USE_FULL_SCREEN_INTENT'.</para>
        /// </summary>
        public static Permission UseFullScreenIntent => new Permission("android.permission.USE_FULL_SCREEN_INTENT", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to use SIP service.
        /// <para>Corresponds to 'android.permission.USE_SIP'.</para>
        /// </summary>
        public static Permission UseSip => new Permission("android.permission.USE_SIP", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows access to the vibrator.
        /// <para>Corresponds to 'android.permission.VIBRATE'.</para>
        /// </summary>
        public static Permission Vibrate => new Permission("android.permission.VIBRATE", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows using PowerManager WakeLocks to keep processor from sleeping or screen from dimming.
        /// <para>Corresponds to 'android.permission.WAKE_LOCK'.</para>
        /// </summary>
        public static Permission WakeLock => new Permission("android.permission.WAKE_LOCK", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to write the user's calendar data.
        /// <para>Corresponds to 'android.permission.WRITE_CALENDAR'.</para>
        /// </summary>
        public static Permission WriteCalendar => new Permission("android.permission.WRITE_CALENDAR", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to write (but not read) the user's call log data.
        /// <para>Corresponds to 'android.permission.WRITE_CALL_LOG'.</para>
        /// </summary>
        public static Permission WriteCallLog => new Permission("android.permission.WRITE_CALL_LOG", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to write the user's contacts data.
        /// <para>Corresponds to 'android.permission.WRITE_CONTACTS'.</para>
        /// </summary>
        public static Permission WriteContacts => new Permission("android.permission.WRITE_CONTACTS", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to write to external storage.
        /// <para>Corresponds to 'android.permission.WRITE_EXTERNAL_STORAGE'.</para>
        /// </summary>
        public static Permission WriteExternalStorage => new Permission("android.permission.WRITE_EXTERNAL_STORAGE", PermissionProtectionLevel.Dangerous);

        /// <summary>
        /// Allows an application to read or write the system settings.
        /// <para>Corresponds to 'android.permission.WRITE_SETTINGS'.</para>
        /// </summary>
        public static Permission WriteSettings => new Permission(
            "android.permission.WRITE_SETTINGS",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Preinstalled | PermissionProtectionLevel.AppOp | PermissionProtectionLevel.Pre23);

        /// <summary>
        /// Allows applications to write the sync settings.
        /// <para>Corresponds to 'android.permission.WRITE_SYNC_SETTINGS'.</para>
        /// </summary>
        public static Permission WriteSyncSettings => new Permission("android.permission.WRITE_SYNC_SETTINGS", PermissionProtectionLevel.Normal);

        /// <summary>
        /// Allows an application to modify and remove existing voicemails in the system.
        /// <para>Corresponds to 'android.permission.WRITE_VOICEMAIL'.</para>
        /// </summary>
        public static Permission WriteVoicemail => new Permission(
            "android.permission.WRITE_VOICEMAIL",
            PermissionProtectionLevel.Signature | PermissionProtectionLevel.Privileged);
        
        /// <summary>
        /// Allows using the advertising ID through Google Play services.
        /// Required in API level 31.
        /// <para>Corresponds to 'com.google.android.gms.permission.AD_ID'.</para>
        /// </summary>
        public static Permission GoogleGmsAdId => new Permission("com.google.android.gms.permission.AD_ID", PermissionProtectionLevel.Normal);


        /// <summary>
        /// Gets the key that describes the android permission.
        /// </summary>
        public string Key { get; }


        /// <summary>
        /// Gets a protection level flags of the permission.
        /// </summary>
        public PermissionProtectionLevel ProtectionLevel { get; }


        /// <summary>
        /// Gets whether the permission is flagged as dangerous.
        /// </summary>
        public bool IsDangerous => ProtectionLevel.HasFlag(PermissionProtectionLevel.Dangerous);


        private Permission(string key, PermissionProtectionLevel level)
        {
            Key = key;
            ProtectionLevel = level;
        }
    }
}
