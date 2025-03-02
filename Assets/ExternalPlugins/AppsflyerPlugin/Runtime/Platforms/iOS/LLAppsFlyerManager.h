//
//  LLAppsFlyerManager.h
//  LLLibSet
//
//  Created by Sergey Ageev on 22/01/17.
//  Copyright (c) 2017 My Company. All rights reserved.
//

#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <Foundation/Foundation.h>
#import "LLLibSet.h"
#import "LLAppController.h"
#import <AppsFlyerLib/AppsFlyerLib.h>

@interface LLAppsFlyerManager : NSObject<UIApplicationDelegate, AppsFlyerLibDelegate>

+ (instancetype)sharedInstance;

@end

FOUNDATION_EXTERN void LLAppsFlyerSetUserConsent(bool isConsentAvailable);
FOUNDATION_EXTERN void LLAppsFlyerInit(LLUnityString, LLUnityString, LLUnityString, bool, LLLibSetCallbackIntString, LLLibSetCallbackString);
FOUNDATION_EXTERN void LLAppsFlyerLogEvent(LLUnityString name, int numParams, LLUnityString paramKeys[], LLUnityString paramVals[]);
FOUNDATION_EXTERN void LLAppsFlyerLogPurchase(LLUnityString productName, LLUnityString currencyCode, LLUnityString price, LLUnityString transactionId);
FOUNDATION_EXPORT LLUnityString LLAppsFlyerUID();
FOUNDATION_EXPORT LLUnityString LLAppsFlyerCampaignName();
FOUNDATION_EXPORT void LLAppsFlyerTrackCrossPromoteImpression(LLUnityString appAppleId, LLUnityString campaignName);
FOUNDATION_EXPORT void LLAppsFlyerTrackAndOpenStore(LLUnityString appAppleId, LLUnityString campaignName, int numParams, LLUnityString paramKeys[], LLUnityString paramValues[]);
FOUNDATION_EXPORT void LLAppsFlyerSetCustomerUserId(LLUnityString customerUserId);