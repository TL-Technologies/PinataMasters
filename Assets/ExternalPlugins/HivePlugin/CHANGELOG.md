# Hive plugin

## 1.7.26-transfer.4 - 2022-03-24
* Исправлено сохранение пути ключей.

## 1.7.26-transfer.3 - 2022-03-24
* Хранение путей до Keychain теперь происходит в локальном формате.
* Опция для игнорирования выставленных значений в пользу параметров, пришедших с билд машины.в 

## 1.7.26-transfer.2 - 2022-03-23
* Исправлена ошибка которая приводила к падению сборки на Android.

## 1.7.26-transfer.1 - 2022-03-22
* Добавлена возможность локальной установки и хранения настроек подписи приложения на андроид.
* Настройки хранятся в ScriptableObject (Create -> Build Settinga -> AndroidSignatureBuildSettings).

## 1.7.25 - 2022-02-11
* Добавили запись файла, который содержит список используемых плагинов, лежит в директории с билдом. 

## 1.7.24 - 2022-01-31
* Добавлен пермишн GoogleGmsAdId, соответствующий `com.google.android.gms.permission.AD_ID`.

## 1.7.23 - 2022-01-31
* Добавлен Linker Flag `all-load`.

## 1.7.22 - 2022-01-28
* Исправлены ошибки в редакторе при отсутствии 'iOSSupport' модуля.

## 1.7.21 - 2022-01-26
* Убрали неявную зависимость на Sirenix.Utilities. 

## 1.7.20 - 2022-01-21
* Добавлена проверка сабмодулей на наличие нужных зависимостей от других плагинов.

## 1.7.19 - 2021-12-30
* Добавлено окно для обновления директив под определенный AndroidTarget.

## 1.7.18 - 2021-12-30
* Исправлена проблема с 'GradleAaptOption' для Unity 2020.

## 1.7.17 - 2021-12-29
* Исправлена проблема доступа к файлам при билде из windows.
* Убраны лишние зависимости из шаблона baseProjectTemplate.gradle для Unity 2019.

## 1.7.16 - 2021-12-20
* Улучшения удаления плагина во время сборки, для сабмодулей из `PluginDisabler`. 

## 1.7.15 - 2021-12-18
* Исправлена ошибка с распаковкой архива андроид библиотеки при локальном билде.

## 1.7.14 - 2021-12-15
* Исправлена ошибка с распаковкой архива андроид библиотеки при билде на билд машине.
* Убраны невалидные maven зависимости из шаблона gradle для билдов.

## 1.7.13 - 2021-12-08
* Добавлена функциональность удаления плагина из билда во время сборки - `PluginDisabler`. 

## 1.7.12 - 2021-12.09
* androidlib библиотека перемещена в zip архив для избежания ошибок при работе с пакетами.

## 1.7.11 - 2021-12-03
* Добавлена поддержка параметра `enableDeepProfilingSupport` из командной строки.
* Добавлена поддержка параметра `waitForManagedDebugger` из командной строки.

## 1.7.10 - 2021-12-03
* Правка для возможности наследования дефолтного пайплайна.

## 1.7.9 - 2021-11-30
* Добавлена возможность создавать реализации Pipeline отличные от Default Pipeline. 

## 1.7.8 - 2021-10-26
* Исправлен иморт библиотеки кода (jar) из hive-foundation при сборке android билда. 

## 1.7.7 - 2021-10-18
* Добавлена поддержка параметра `enableCrashReportAPI` из командной строки.

## 1.7.6 - 2021-10-25
* Удалены лишние мета-файлы.

## 1.7.5 - 2021-10-18
* Добавлено уведомление при сборке о наличии preview пакетов в проекте.

## 1.7.4 - 2021-10-14
* Исправлен импорт HiveFoundation.androidlib при сборке в Unity 2019.4.X.

## 1.7.3 - 2021-10-01
* Добавлена поддержка андроид платформы `Huawei`.

## 1.7.2 - 2021-09-29
* Версия Gradle в темплейте для Unity 2020.3.X понижена с 4.0.1 до 3.6.0.

## 1.7.1 - 2021-09-16
* Обновлена версия `play-services-ads` для AdMob адаптера.

## 1.7.0 - 2021-09-01
* Добавлена поддержка сборки на Unity 2020 LTS.

## 1.6.18 - 2021-07-22
* Добавлена новая библиотека для `Pangle` адаптера.

## 1.6.17 - 2021-07-22
* Обновлена версия `play-services-ads` в соответствии с рекомендациями для обновления MoPub.

## 1.6.16 - 2021-07-01
* Добавлен `SharpZipLib` для работы с .tgz архивами из windows.

## 1.6.15 - 2021-06-29
* Добавлена возможность записи бандлов запрашиваемых приложений в андроид манифест.

## 1.6.14 - 2021-06-25
* Исправлена ошибка, при которой сбрасывалась настройка `Create symbols.zip` при локальной сборке.

## 1.6.13 - 2021-06-17
* Добавлена возможность корректного использования embedded packages в `UnityPath`.

## 1.6.12 - 2021-06-15
* Значение `TargetSdkVersion` поднято до 30.
* Запрещен permission QUERY_ALL_PACKAGES.

## 1.6.11 - 2021-05-14
* Добавлен метод `ForceRemovingPermissionElement` в класс `AndroidManifest`.
* Исправлены некорректные логи о добавлении опасных Permissions при Android сборке.

## 1.6.10 - 2021-04-19
* Обновлен файл .gitattributes в соответствии с другими плагинами.
* Исправлен вывод неверной директории при логировании ошибки в методе создания zip архива.

## 1.6.9 - 2021-04-16
* Добавлен alias для Amazon папок в проекте.

## 1.6.8 - 2021-04-13
* Обновлены версии `play-services-ads` и `play-services-base` в соответствии с рекомендациями для обновления MoPub.

## 1.6.7 - 2021-04-13
* Перенесен класс `EventDispatcher` из GeneralPlugin.

## 1.6.6 - 2021-03-31
* Добавлен метод `SetUseInstance` в `IUnityFluentDescriptor`.

## 1.6.5 - 2021-03-30
* Добавлена возможность получения в Runtime значение текущей Android платформы (GooglePlay, Amazon, Samsung или None).

## 1.6.4 - 2021-03-30
* Исправлено добавление `xcassets` в Xcode билд.

## 1.6.3 - 2021-03-25
* Добавлена поддержка сборки Amazon билдов.
* Отрефакторена внутрення логика передачи некоторых параметров при сборке.
* В Changelog добавлено указание дат внесенных изменений.

## 1.6.2 - 2021-03-23
* Добавлено форсирование Modern Build System в Xcode при локальной сборке iOS проекта.

## 1.6.1 - 2021-03-23
* Добавлена возможность выбора типа компрессии билдов при сборке.

## 1.6.0 - 2021-03-11
* Добавлена возможность использования некоторых методов класса ReflectionHelper для нестатических членов класса.
* Исправлена возникновение исключения при использовании метода UnityAssemblyUtilities.IsAssemblyIncludedInBuild.
* Добавлены вспомогательные инструменты для парсинга файлов package.json.

## 1.5.6 - 2021-02-27
* Расширена функциональность класса ReflectionHelper: добавлена возможность создания делегата для нестатического метода, а также задания значения для property.

## 1.5.5 - 2021-02-17
* Исправлена ошибка в ColorConverter с использованием Color32, добавлен анбоксинг перед кастом.

## 1.5.4 - 2021-02-12
* Исправлена потенциальная проблема стриппинга при использовании методов класса DateTime на девайсах с нестандартными языками (арабский и т.д.).
* Добавлена возможность выбора WebGL таргета при билде с помощью командной строки.

## 1.5.3 - 2021-02-10
* В список системных iOS фреймворков добавлен фреймворк `AdServices`.

## 1.5.2 - 2021-02-08
* Добавлена поддержка сборки билдов под платформу WebGL.

## 1.5.1 - 2021-02-05
* Исправлена потенциальная проблема при рассылке ивентов в EventAggregator.

## 1.5.0 - 2021-01-29
* Добавлена поддержка сборки билдов под платформы MacOS и Windows64.

## 1.4.10 - 2021-01-28
* Исправление проблемы с IL2CPP stripping частично расширено на не Playgendary плагины.

## 1.4.9 - 2021-01-25
* Поднята версия gradle до 3.4.3 для исправления бага при сборке MoPub Android (подробно о баге https://stackoverflow.com/questions/62969917/how-to-fix-unexpected-element-queries-found-in-manifest-error).

## 1.4.8 - 2021-01-25
* Исправлена проблема с IL2CPP stripping, применение которого к пакетам Playgendary периодически приводило к крашам приложения.
* Исправлена логика метода ProjectSnapshot.MarkAssetToDelete: теперь при удалении файла также рекурсивно удаляются все родительские пустые папки.

## 1.4.7 - 2021-01-19
* Добавлены утилиты для парсинга и изменения файла manifest.json, которые позволяют делать это изменение синхронно (в отличие от методов встроенного класса UnityEditor.PackageManager.Client).
* В классе FileSystemUtilities добавлен вспомогательный метод DeleteEntryAndEmptyParentsDirectories.

## 1.4.6 - 2021-01-05
* В классе FileSystemUtilities добавлены вспомогательные методы Delete (для удаления директорий и файлов вместе с соответствующими метафайлами) и IsDirectoryEmpty.
* Переменная UnityPath.StreamingAssetPath переименована в UnityPath.StreamingAssetsAssetPath.

## 1.4.5 - 2020-12-09
* Добавлена константа ExternalPluginsSettingsAssetPath, содержащая путь для расположения проектно зависимых настроек плагинов.

## 1.4.4 - 2020-12-09
* Добавлен класс CustomAssert, методы которого позволяют обходить некоторые баги и странное поведение UnityTestFramework и Jenkins NUnit plugin.

## 1.4.3 - 2020-12-07
* Сборки Newtonsoft.Json и Ionic.Zip.Reduced добавлены в исключения IL2CPP stripping.

## 1.4.2 - 2020-12-07
* Добавлена утилита для получения текущего BuildTargetGroup из аргументов командной строки.

## 1.4.1 - 2020-12-02
* Плагин NewtonsoftJson обратно добавлен внутрь Hive, из-за проблем с использованием Git зависимостей в UnityPackageManager на Windows.

## 1.4.0 - 2020-12-01
* Плагин NewtonsoftJson вынесен в отдельный пакет, указан в качестве зависимости Hive и обновлён до версии 12.0.3.
* Добавлены вспомогательные классы ColorConverter и VectorConverter для сериализации.
* Добавлен класс UnityPackageInfo.json для удобства десериализации файлов package.json внутри плагинов.
* Изменено форматирование поля internal в package.json.

## 1.3.7 - 2020-11-13
* Исправлена проблема сборки на Jenkins, из-за которой в Android билдах в качестве артефактов могли сохраняться symbols файлы при выключенной опции SaveSymbolsFile.

## 1.3.6 - 2020-11-11
* Добавлено логирование ошибок внутренних ошибок сборки Unity (например, ошибок в сериализации сцены).
* Доработан класс по работе с настройками Unity Console.
* В интерфейс IPipelineTask добавлен метод Prepare, который вызывается в начале выполнения Pipeline, содержащего эту Task.
* Расширены возможности класса ReflectionHelper: теперь он умеет создавать делегаты к Get и Set методам свойств.
* Удалён класс UnityLogMessagesReceiver, поскольку используемый внутри него ивент Application.logMessageReceived не позволяет получать все виды сообщений об ошибке.

## 1.3.5 - 2020-11-04
* В отчёт об ошибках Hive сборки добавлено запоминание Asserts.
* Добавлено логирование количества ошибок, произошедших при выполнении стандартной сборки Unity.
* Удалены неиспользуемые классы для последующего подключения PlayServicesResolver (aka EDM4U).

## 1.3.4 - 2020-10-19
* Обновлен google-play-services до 19.4.0 версии (для работы рекламного SDK).

## 1.3.3 - 2020-10-13
* Реализована возможность корректного использования новой системы iOS лодеров на Unity 2019.3.

## 1.3.2 - 2020-10-08
* В список стандартных iOS фреймворков добавлены Metal и MetalKit.

## 1.3.1 - 2020-09-28
* Реализована возможность добавления файлов xcframework в Xcode билд.

## 1.3.0 - 2020-09-25
* Добавлена поддержка сборки под Unity 2019.4.
* Класс ReflectionHelper разделён на Editor и Runtime части.

## 1.2.19 - 2020-09-16
* Исправлена конфигурация iOS entitlements для сборки.

## 1.2.18 - 2020-09-14
* Форсирование некоторых настроек сборки перенесено на более ранний этап для возможности их детекта и изменения.

## 1.2.17 - 2020-09-14
* Добавлено форсирование 29 версии AndroidTargetSdk при сборке.

## 1.2.16 - 2020-09-13
* Добавлена возможность определения попадания Assemblies в билд.

## 1.2.15 - 2020-09-12
* Добавлена константа названия папки с ресурсами в класс UnityPath.

## 1.2.14 - 2020-09-10
* Исправлен NullReferenceException в случае использования класса UnityAssebmlyDefinition для отсутствующих в проекте assembly.

## 1.2.13 - 2020-09-10
* В класс UnityAssemblyDefinition добавлена поддержка зависимостей, указанных с использованием GUID.
* Добавлен класс PreprocessorDirectivesUtilities, упрощающий модификацию директив препроцессора в проекте.

## 1.2.12 - 2020-09-10
* Добавлена поддержка Unity 2020 в контексте отключения Android минификации.

## 1.2.11 - 2020-09-09
* Доработано поведение функции Unity.Combine в случае передачи некорректных аргументов.
* Исправлено некорректное поведение функций UnityPath.GetAssetPathFromFullPath и UnityPath.GetFullPathFromAssetPath в случае вырожденных путей.

## 1.2.10 - 2020-09-09
* Реализована корректная работа функций UnityPath.GetAssetPathFromFullPath и UnityPath.GetFullPathFromAssetPath в случае использования путей внутри пакетов.

## 1.2.9 - 2020-09-08
* Исправлена логика функции UnityPath.FindPathByPattern: теперь поиск корректен и в том случае, если искомый паттерн находится внутри Unity пакета.

## 1.2.8 - 2020-09-04
* Добавлена возможность управления появлением SplashScreen в кастомных билд скриптах внутри проекта.

## 1.2.7 - 2020-09-03
* Добавлена новая динамическая библиотека "libbz2" в список доступных.

## 1.2.6 - 2020-08-26
* Добавлена поддержка фреймворка и пользовательского разрешения для iOS 14.

## 1.2.5 - 2020-08-25
* Добавлена возможность экспорта Android проекта и информации о нём в артефакты сборки на Jenkins.

## 1.2.4 - 2020-08-19
* MavenCentral добавлена в список константных репозиторий Gradle.

## 1.2.3 - 2020-08-18
* Исправлена проблема с абсолютным путём к корню плагина на Windows.

## 1.2.2 - 2020-08-18
* Android библиотека `play-services-ads` обновлена до версии `19.3.0`.

## 1.2.1 - 2020-08-13
* iOS фреймворк Hive пересобран в соответствии с актуальной нативной частью.

## 1.2.0 - 2020-08-11
* Добавлена возможность выполнения кастомного кода после сборки независимо от успешности билда.
* На Андроид добавлена возможность указания домена в `network_security_config.xml`.
* Добавлена возможность кастомизации запуска Hive сборки в Unity Editor.

## 1.1.9 - 2020-08-03
* Добавлена возможность использования библиотеки Ionic.Zip на платформе Standalone.

## 1.1.8 - 2020-07-31
* Исправлена проблема распаковки zip архивов по пути, содержащему специальные символы, например, символы скобок '(', ')'.

## 1.1.7 - 2020-07-28
* Добавлена возможность редактирования контекста пайплайна сборки с помощью реализации интерфейса `IBuildPipelineOptionsModifier`.

## 1.1.6 - 2020-07-27
* Исправлен абсолютный путь для пакетов в классе PluginHierarchy.
* Добавлено свойство PackageInfo в PluginHierarchy.

## 1.1.5 - 2020-07-24
* Для Android приложений по умолчанию отключена backup функциональность.

## 1.1.4 - 2020-07-22
* Исправлена проблема, из-за которой отключенные сцены попадали в билд.

## 1.1.3 - 2020-07-21
* Улучшено логгирование ошибок сборки Unity проекта.
* Добавлено логгирование этапов сборки Hive.

## 1.1.2 - 2020-07-17
* В методе разархивирования добавлен флаг, позволяющий заменить файлы, если они существуют.
* Метод разархивирования теперь создает все папки в outputPath.

## 1.1.1 - 2020-07-16
* Ресериализация и актуализация метафайлов под Unity 2019.

## 1.1.0 - 2020-07-15
* Конструктор класса PluginHierarchy теперь принимает имя Assembly в качестве параметра.

## 1.0.8 - 2020-07-14
* Исправлена проблема с остававшимися после сборки Android билда изменениями в Git в случае, если в проекте отсутствовала папка Assets/Plugins/Android.

## 1.0.7 - 2020-07-10
* Добавлена поддержка генерации symbols файлов под iOS и Android.

## 1.0.6 - 2020-07-09
* Добавлена возможность использования библиотеки Ionic.Zip на платформах iOS и Android.

## 1.0.5 - 2020-07-08
* Обновлена Android библиотека `play-services-ads` до версии `19.2.0`.

## 1.0.4 - 2020-07-07
* Во внутренней логике использование класса System.Version заменено использованием кастомного класса ExtendedVersion, который:
	* поддерживает версию с более чем 4 подверсиями
	* не игнорирует нули в начале подверсии.

## 1.0.3 - 2020-07-02
* Директива препроцессора SPINE_TK2D добавлена в список допустимых заранее заданных директив.

## 1.0.2 - 2020-06-30
* Следующие запрашиваемые разрешения исключены из Unity AndroidManifest.xlm файла:
	* READ_PHONE_STATE
	* READ_EXTERNAL_STORAGE
	* WRITE_EXTERNAL_STORAGE
	* ACCESS_FINE_LOCATION
	* ACCESS_COARSE_LOCATION

## 1.0.1 - 2020-06-29
* Исправлена проблема сборки Android билда при отсутствующей папке Assets/Plugins/Android.

## 1.0.0 - 2020-05-20
* Добавлен основной контент плагина
* Добавлена поддержка Unity 2019.3.
