//
//  LLSystemPopUp.h
//  LLLibSet
//

@interface LLRatePopUp : NSObject

+ (instancetype)sharedInstance;

@end


#if !UNITY_TVOS
FOUNDATION_EXTERN bool LLRatePopIsAvalaible();
FOUNDATION_EXTERN void LLRatePopUpShow();
#endif
