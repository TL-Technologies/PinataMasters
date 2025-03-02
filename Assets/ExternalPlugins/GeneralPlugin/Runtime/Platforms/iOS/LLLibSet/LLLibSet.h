//
//  LLLibSet.h
//  LLLibSet
//

#pragma mark - For plugins

typedef const char * LLUnityString;
typedef int32_t LLUnityInt;
typedef const void * LLUnityIntPtr;

#ifdef __cplusplus
extern "C"
{
#endif
    typedef struct
    {
        LLUnityString   productID;
        LLUnityString   subscriptionID;
        LLUnityString   nextSubscriptionID;
        double          purchaseDate;
        double          expirationDate;
    } LLSubscriptionResult;
    
    typedef struct
    {
        LLUnityString   productID;
        LLUnityString   rewardID;
        LLUnityString   transactionID;
        LLUnityInt      transactionState;
        LLUnityInt      validationState;
        
    } LLPurchaseCallbackInfo;
    typedef void (*LLLibsetPurchaseCallbackStruct)(LLPurchaseCallbackInfo);
    
    typedef void (*LLLibSetCallback)();
    typedef void (*LLLibSetCallbackString)(LLUnityString);
    typedef void (*LLLibSetCallbackStringBool)(LLUnityString, bool);
    typedef void (*LLLibSetCallbackStringInt)(LLUnityString, LLUnityInt);
    typedef void (*LLLibSetCallbackStringString)(LLUnityString, LLUnityString);
    typedef void (*LLLibSetCallbackStringStringBoolBool)(LLUnityString, LLUnityString, bool, bool);
    typedef void (*LLLibSetCallbackStringStringStringInt)(LLUnityString, LLUnityString, LLUnityString, LLUnityInt);

    typedef void (*LLLibSetCallbackBool)(bool);
    typedef void (*LLLibSetCallbackInt)(LLUnityInt);
    typedef void (*LLLibSetCallbackIntBool)(LLUnityInt, bool);
    typedef void (*LLLibSetCallbackIntInt)(LLUnityInt, LLUnityInt);
    typedef void (*LLLibSetCallbackIntString)(LLUnityInt, LLUnityString);
    typedef void (*LLLibSetCallbackIntIntString)(LLUnityInt, LLUnityInt, LLUnityString);
    typedef void (*LLLibSetCallbackIntStringString)(LLUnityInt, LLUnityString, LLUnityString);
    typedef void (*LLLibSetCallbackIntIntStringString)(LLUnityInt, LLUnityInt, LLUnityString, LLUnityString);
    typedef void (*LLLibSetCallbackIntIntIntString)(LLUnityInt, LLUnityInt, LLUnityInt, LLUnityString);
    typedef void (*LLLibSetCallbackIntIntIntStringString)(LLUnityInt, LLUnityInt, LLUnityInt, LLUnityString, LLUnityString);
#ifdef __cplusplus
}
#endif

FOUNDATION_EXPORT LLUnityString LLUnityStringFromNSString(NSString *string);
FOUNDATION_EXPORT NSString*     LLNSStringFromUnityString(LLUnityString string);

FOUNDATION_EXPORT NSDictionary* LLNSDictionaryFromUnityStrings(int lengthDictionary, LLUnityString keys[], LLUnityString vals[]);
FOUNDATION_EXPORT NSArray*      LLNSArrayFromUnityStrings(int lengthArray, LLUnityString vals[]);
FOUNDATION_EXPORT NSArray*      LLNSArrayFromFloats(int lengthArray, float vals[]);


#pragma mark - LLExtensions

FOUNDATION_EXPORT LLUnityString LLBundleName();
FOUNDATION_EXPORT LLUnityString LLBundleDisplayName();

FOUNDATION_EXPORT LLUnityString LLBundleVersion();
FOUNDATION_EXPORT LLUnityString LLBundleShortVersionString();


#pragma mark - LLMemoryWarning

FOUNDATION_EXTERN void LLMemoryWarningInit(LLLibSetCallback callback);


#pragma mark - LLSoundsManager

FOUNDATION_EXTERN void LLSoundsManagerInit(int srcAmounts, LLLibSetCallbackString callbackStop, LLLibSetCallbackString callbackError);
FOUNDATION_EXTERN void LLSoundsManagerPlaySound(LLUnityString uid, LLUnityString filePath, float volume, float pitch, bool looped, float x, float y, float z, float minDistance, float maxDistance, float rolloff);
FOUNDATION_EXTERN void LLSoundsManagerUpdateSound(LLUnityString uid, float volume, float pitch, bool looped, float x, float y, float z, float minDistance, float maxDistance, float rolloff);
FOUNDATION_EXTERN void LLSoundsManagerStopSound(LLUnityString uid);
FOUNDATION_EXTERN void LLSoundsManagerPauseSound (LLUnityString uid, bool pause);
FOUNDATION_EXTERN void LLSoundsManagerSetListenerPosition(float x, float y, float z);



#pragma mark - LLRatePopUp

#if !UNITY_TVOS
FOUNDATION_EXTERN bool LLRatePopIsAvalaible();
FOUNDATION_EXTERN void LLRatePopUpShow();
#endif


#pragma mark - LLSystemPopUp

#if !UNITY_TVOS
FOUNDATION_EXTERN void LLSystemPopUpWithoutButtonsShow(LLUnityString title, LLUnityString message);
FOUNDATION_EXTERN void LLSystemPopUpWithoutButtonsHide();
FOUNDATION_EXTERN void LLSystemPopUpShow(LLUnityString title, LLUnityString message, LLUnityString button, LLUnityString callbackName);
FOUNDATION_EXTERN void LLSystemPopUpWithTwoButtonsShow(LLUnityString title, LLUnityString message, LLUnityString firstButtonText, LLUnityString secondButtonText, LLUnityString firstButtonCallbackName, LLUnityString secondButtonCallbackName, bool isVerticalLayout);
FOUNDATION_EXTERN void LLSystemPopUpRegisterCallback(LLLibSetCallbackString unityCallback);
#endif

#pragma mark - LLSystemSharing

#if !UNITY_TVOS
FOUNDATION_EXTERN void LLSystemSharingShareItems(LLUnityString text, LLUnityString imagePath, LLUnityString urlString);
FOUNDATION_EXTERN void LLSystemSharingShareItemsWithData(LLUnityString text, Byte *bytes, LLUnityInt width, LLUnityInt height, LLUnityString urlString);
#endif

#pragma mark - LLSwipeHandler

#if !UNITY_TVOS
FOUNDATION_EXTERN void LLSwipeHandlerUpdate();
FOUNDATION_EXTERN LLUnityString LLSwipeHandlerPopHistory();
#endif

#pragma mark - LLTextureHelper

//FOUNDATION_EXPORT LLTextureHelperLoadResult LLTextureHelperLoadImageAtPath(LLUnityString imagePath, LLUnityInt requestMipMaps, LLUnityInt format);
//FOUNDATION_EXPORT LLUnityString LLTextureHelperLoadImageAtPathAsync(LLUnityString imagePath, LLUnityInt requestMipMaps, LLUnityInt format);
//FOUNDATION_EXPORT void LLTextureHelperReleaseTexture(LLUnityIntPtr textureName);
