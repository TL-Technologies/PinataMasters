# README #

## Изменения, внесенные в Unity IAP:

### 1.4.7 
* Добавлен `HuaweiAppGallery` в перечисление `UnityEngine.Purchasing.AppStore`

### 1.4.3
* Абсолютные пути в папке `ExternalDependencies/com.unity.purchasing/Editor` заменены на относительные, добавлен `UnityIapPluginHierarchy`

### 1.4.1:
* Добавлена повторная попытка подключения при получении продуктов в `GooglePlayStoreService` - для обновления статуса `GoogleBillingConnectionState`

### 1.4.0:
* Изменен метод AppleStoreImpl:OnPurchaseSucceeded : удалена проверка времени при успешной покупке подписки