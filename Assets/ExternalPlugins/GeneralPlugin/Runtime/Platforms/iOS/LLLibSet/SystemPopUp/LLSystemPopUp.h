//
//  LLSystemPopUp.h
//  LLLibSet
//

#import "LLLibSet.h"
#import "LLLibSetExtentions.h"


static LLLibSetCallbackString _systemPopUpDelegate;

@interface LLSystemPopUp : NSObject

+ (instancetype)sharedInstance;

@end


#if !UNITY_TVOS
FOUNDATION_EXTERN void LLSystemPopUpWithoutButtonsShow(LLUnityString title, LLUnityString message);
FOUNDATION_EXTERN void LLSystemPopUpWithoutButtonsHide();
FOUNDATION_EXTERN void LLSystemPopUpShow(LLUnityString title, LLUnityString message, LLUnityString button, LLUnityString callbackName);
FOUNDATION_EXTERN void LLSystemPopUpRegisterCallback(LLLibSetCallbackString unityCallback);
FOUNDATION_EXTERN void LLSystemPopUpWithTwoButtonsShow(LLUnityString title, LLUnityString message, LLUnityString firstButtonText, LLUnityString secondButtonText, LLUnityString firstButtonCallbackName, LLUnityString secondButtonCallbackName, bool isVerticalLayout);
FOUNDATION_EXTERN void LLSystemPopUpWithTwoButtons(LLUnityString title, LLUnityString message, LLUnityString firstButtonText, LLUnityString secondButtonText, LLUnityString firstButtonCallbackName, LLUnityString secondButtonCallbackName, int isVerticalLayout, int isSecondButtonBold);
#endif
