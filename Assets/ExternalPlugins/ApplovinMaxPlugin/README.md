##  Изменения в MaxSdk ##

### ApplovinMaxPlugin v1.2.0 - MaxSdk v5.1.1 ###
`ExternalDependencies/applovin-max-unity-plugin/MaxSdk/Scripts/Editor/MaxPostProcessBuildiOS.cs`
- Получение версии фейсбук адаптера в методе ShouldAddSwiftSupportForFacebook заменен на получение из FacebookAdapter.

### ApplovinMaxPlugin v1.0.8 - MaxSdk v4.3.12 ###
`ExternalDependencies/applovin-max-unity-plugin/MaxSdk/Scripts/Editor/MaxPostProcessBuildiOS.cs`
- Билд пост процесс заменен на построцесс хайва. Процесс юнити иногда отрабатывает некорректно.

### ApplovinMaxPlugin v1.0.3 - MaxSdk v4.3.12 ###
`ExternalDependencies/applovin-max-unity-plugin/MaxSdk/Scripts/IntegrationManager/Editor/AppLovinSettings.cs` :
- Скрыты неиспользуемые поля, связанные с Consent Flow

### ApplovinMaxPlugin v1.0.0 - MaxSdk v4.3.12 ###

`ExternalDependencies/applovin-max-unity-plugin/MaxSdk/Editor/Mediation/Google/PreProcessor.cs` :
- Закомментирован.  Процесс перенесен в Editor/MaxBuildProcess.cs

`ExternalDependencies/applovin-max-unity-plugin/MaxSdk/Mediation/Google/Editor/PostProcessor.cs` :
- Закомментирован.  Процесс перенесен в Editor/MaxBuildProcess.cs

`ExternalDependencies/applovin-max-unity-plugin/MaxSdk/Scripts/Editor/MaxPostProcessBuildiOS.cs` :
- В полях SwiftLanguageNetworks и EmbedSwiftStandardLibrariesNetworks - добавился FBAudienceNetwork, удалился MoPub (в текущей версии возвращен Мопаб)
- PluginMediationDirectory - получение пути к медиациям через хайв.

`ExternalDependencies/applovin-max-unity-plugin/MaxSdk/Mediation` :
- Возвращена структура папки /Editor/Mediation/[mediation]/[files] => /Mediation/[mediation]/Editor/[files]

`ExternalDependencies/applovin-max-unity-plugin/MaxSdk/Scripts/IntegrationManager/Editor/AppLovinSettings.cs` :
- Добавлен атрибут CreateAssetMenu
- Добавлен класс AdMobAndroidPlatformInfo
- Добавлено поле adMobAndroidPlatformIds
- Получение инстанса заменено на Resources.Load<AppLovinSettings>("AppLovinSettings");
- Получение AdMobAndroidAppId изменено на поиск в массиве adMobAndroidPlatformIds

`ExternalDependencies/applovin-max-unity-plugin/MaxSdk/Scripts/IntegrationManager/Editor/AppLovinMenuItems.cs`
- Добавлено отключение Integration Manager по флагу isEnabled.
  