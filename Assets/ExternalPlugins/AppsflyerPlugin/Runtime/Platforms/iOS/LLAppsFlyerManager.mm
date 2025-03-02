//
//  LLAppsFlyerManager.mm
//  LLLibSet
//
//  Created by Sergey Ageev on 22/01/17.
//  Copyright (c) 2017 My Company. All rights reserved.
//

#import "LLAppsFlyerManager.h"
#import <objc/message.h>

static LLAppsFlyerManager *_sharedInstance = [LLAppsFlyerManager sharedInstance];

typedef void (*bypassDidFinishLaunchingWithOption)(id, SEL, NSInteger);

typedef NS_ENUM(NSInteger, AppsFlyerInitializationStatus)
{
    AppsFlyerInitializationNone = 0,
    AppsFlyerInitializationSuccess = 1,
    AppsFlyerInitializationFailed = 2,
    AppsFlyerInitializationWarning = 3
};


@interface LLAppsFlyerManager ()
{
    BOOL  _isInit;
    BOOL  _didEnteredBackGround;

    NSString *_campaign;

    LLLibSetCallbackString _conversionHandler;
}

@end

@implementation LLAppsFlyerManager


+ (instancetype)sharedInstance
{
    if (_sharedInstance == nil)
    {
        _sharedInstance = [[LLAppsFlyerManager alloc] init];
    }
    return _sharedInstance;
}


- (id)init
{
    self = [super init];
    if (self != nil)
    {
        RegisterAppDelegateListener(self);
    }
    return self;
}


- (void)initializeWithAppId:(NSString *)appId
                     devKey:(NSString *)key
                     userId:(NSString *)playerId
                    isDebug:(BOOL)debug
                    completionHandler:(LLLibSetCallbackIntString)onComplete
                    conversionDataHandler:(LLLibSetCallbackString)conversionDataCallback
{
    if (![appId isEqualToString:@""])
    {
        [AppsFlyerLib shared].appleAppID = appId;
    }
    if (![key isEqualToString:@""])
    {
        [AppsFlyerLib shared].appsFlyerDevKey = key;
    }
    if (![playerId isEqualToString:@""])
    {
        [AppsFlyerLib shared].customerUserID = playerId;
    }

    [AppsFlyerLib shared].isDebug = debug;
    [AppsFlyerLib shared].delegate = self;
    [AppsFlyerLib shared].useReceiptValidationSandbox = debug;
    
    SEL SKSel = NSSelectorFromString(@"__willResolveSKRules:");
    id AppsFlyer = [AppsFlyerLib shared];
    if ([AppsFlyer respondsToSelector:SKSel])
    {
        bypassDidFinishLaunchingWithOption msgSend = (bypassDidFinishLaunchingWithOption)objc_msgSend;
        msgSend(AppsFlyer, SKSel, 2);
    }
    
    _isInit = YES;
    _conversionHandler = conversionDataCallback;
    
    [[AppsFlyerLib shared] startWithCompletionHandler:^(NSDictionary<NSString *,id> * _Nullable dictionary, NSError * _Nullable error)
    {
        if (error != nil)
        {
            // Link for available error codes: https://cutt.ly/JfwJjRr
            NSLog(@"trackAppLaunchWithCompletionHandler.error - %@", error);
        }
    }];
    
    dispatch_async(dispatch_get_main_queue(), ^{
        if (onComplete == nil)
        {
            NSLog(@"initializeWithAppId.error - noCallbackDefined");
            return;
        }
        
        onComplete((int)AppsFlyerInitializationSuccess, "AppsFlyer successfully inited");
    });
}


- (void)setUserConsent:(BOOL)isConsentAvailable
{
    [AppsFlyerLib shared].disableAdvertisingIdentifier = !isConsentAvailable;
    [AppsFlyerLib shared].disableCollectASA = !isConsentAvailable;
    [AppsFlyerLib shared].anonymizeUser = !isConsentAvailable;
    [AppsFlyerLib shared].disableIDFVCollection = !isConsentAvailable;
}


- (NSString *)getCampaignName
{
    return _campaign;
}


#pragma mark - AppDelegate

- (BOOL)application:(UIApplication *)application continueUserActivity:(nonnull NSUserActivity *)userActivity restorationHandler:(nonnull void (^)(NSArray<id<UIUserActivityRestoring>> * _Nullable))restorationHandler
{
    [[AppsFlyerLib shared] continueUserActivity:userActivity restorationHandler:restorationHandler];
    return YES;
}


- (BOOL)application:(UIApplication *)application openUrl:(NSURL *)url options:(NSDictionary *)options
{
    [[AppsFlyerLib shared] handleOpenUrl:url options:options];
    return YES;
}


- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(nullable NSString *)sourceApplication annotation:(nonnull id)annotation
{
    [[AppsFlyerLib shared] handleOpenURL:url sourceApplication:sourceApplication withAnnotation:annotation];
    return YES;
}


- (void)applicationDidBecomeActive:(UIApplication *)application
{
    if ((_didEnteredBackGround == YES) && (_isInit == YES))
    {
        [[AppsFlyerLib shared] start];
        _didEnteredBackGround = NO;
    }
}


- (void)applicationDidEnterBackground:(UIApplication *)application
{
    _didEnteredBackGround = YES;
}


#if UNITY_USES_REMOTE_NOTIFICATIONS
- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    //track uninstalls
    [AppsFlyerLib shared].useUninstallSandbox = NO;
    [[AppsFlyerLib shared] registerUninstall:deviceToken];
    
    [application setMinimumBackgroundFetchInterval:UIApplicationBackgroundFetchIntervalMinimum];
}
#endif



#pragma mark - Appsflyer

- (void) onAppOpenAttribution:(NSDictionary*) attributionData
{
    NSLog(@"onAppOpenAttribution");
    for(id key in attributionData)
    {
        NSLog(@"onAppOpenAttribution: key=%@ value=%@", key, [attributionData objectForKey:key]);
    }
}


- (void) onAppOpenAttributionFailure:(NSError *)error
{
    NSLog(@"onAppOpenAttributionFailure");
    NSLog(@"%@", [error description]);
}


-(void)onConversionDataSuccess:(NSDictionary*) installData
{
    id status = [installData objectForKey:@"af_status"];
    if ([status isEqualToString:@"Non-organic"])
    {
        // Here you can take media source.
        // id sourceID = [installData objectForKey:@"media_source"];
        
        id campaign = [installData objectForKey:@"campaign"];
        if (!(campaign == [NSNull null] || campaign == nil))
        {
            _campaign = campaign;
        }
        else
        {
            _campaign = @"";
        }
    }
    else
    {
        _campaign = @"";
    }
    
    if (_conversionHandler != NULL)
    {
        dispatch_async(dispatch_get_main_queue(), ^{
            _conversionHandler(LLUnityStringFromNSString(_campaign));
        });
    }
}


-(void)onConversionDataFail:(NSError *) error
{
    NSLog(@"onConversionDataFail");
    NSLog(@"%@", [error description]);

    if (_conversionHandler != NULL)
    {
        dispatch_async(dispatch_get_main_queue(), ^{
            _conversionHandler(LLUnityStringFromNSString(_campaign));
        });
    }
}

@end



#pragma mark - Unity Extern

void LLAppsFlyerSetUserConsent(bool isConsentAvailable)
{
    [[LLAppsFlyerManager sharedInstance] setUserConsent:isConsentAvailable];
}


void LLAppsFlyerInit(LLUnityString appId, LLUnityString devKey, LLUnityString userId, bool isDebug, LLLibSetCallbackIntString completionCallback, LLLibSetCallbackString conversionDataCallback)
{
    [[LLAppsFlyerManager sharedInstance] initializeWithAppId:LLNSStringFromUnityString(appId) devKey:LLNSStringFromUnityString(devKey) userId:LLNSStringFromUnityString(userId) isDebug:isDebug completionHandler: completionCallback conversionDataHandler: conversionDataCallback];
}


void LLAppsFlyerLogEvent(LLUnityString name, int numParams, LLUnityString paramKeys[], LLUnityString paramVals[])
{
    NSDictionary *params = LLNSDictionaryFromUnityStrings(numParams, paramKeys, paramVals);
    [[AppsFlyerLib shared] logEvent:LLNSStringFromUnityString(name) withValues:params];
}


void LLAppsFlyerLogPurchase(LLUnityString productName, LLUnityString currencyCode, LLUnityString price, LLUnityString transactionId)
{
    [[AppsFlyerLib shared] validateAndLogInAppPurchase:LLNSStringFromUnityString(productName)
                                                 price:LLNSStringFromUnityString(price)
                                              currency:LLNSStringFromUnityString(currencyCode)
                                         transactionId:LLNSStringFromUnityString(transactionId)
                                  additionalParameters:@{}
                                               success:^(NSDictionary *result)
    {
        NSLog(@"AppsFlyer: purchase succeeded and verified");
    }
    failure:^(NSError *error, id response)
    {
        NSLog(@"AppsFlyer: purchase verify failed, response = %@", response);
    }];
}


LLUnityString LLAppsFlyerUID()
{
    NSString *appsflyerUID = [[AppsFlyerLib shared] getAppsFlyerUID];
    return LLUnityStringFromNSString(appsflyerUID);
}


void LLAppsFlyerTrackCrossPromoteImpression(LLUnityString appAppleId, LLUnityString campaignName)
{
    [AppsFlyerCrossPromotionHelper logCrossPromoteImpression:LLNSStringFromUnityString(appAppleId)
                                                    campaign:LLNSStringFromUnityString(campaignName)
                                                  parameters:NULL];
}


LLUnityString LLAppsFlyerCampaignName()
{
    NSString *campaignName = [[LLAppsFlyerManager sharedInstance] getCampaignName];
    NSLog(@"AppsFlyer Campaign: %@", campaignName);
    return LLUnityStringFromNSString(campaignName);
}


void LLAppsFlyerTrackAndOpenStore(LLUnityString appAppleId, LLUnityString campaignName, int numParams, LLUnityString paramKeys[], LLUnityString paramValues[])
{
    NSString* appId = LLNSStringFromUnityString(appAppleId);
    NSDictionary *params = LLNSDictionaryFromUnityStrings(numParams, paramKeys, paramValues);

    [AppsFlyerCrossPromotionHelper logAndOpenStore:appId
                                          campaign:LLNSStringFromUnityString(campaignName)
                                        parameters:params
                                         openStore:^(NSURLSession * _Nonnull urlSession, NSURL * _Nonnull clickURL)
    {
        // create request
        NSURLSessionDataTask* dataTask = [urlSession dataTaskWithURL:clickURL
                                           completionHandler:^(NSData * _Nullable data, NSURLResponse * _Nullable response, NSError * _Nullable error)
        {
            if (error)
            {
                NSLog(@"AppsFlyer crossPromotionViewed Connection failed! Error - %@", [error localizedDescription]);
            }
        }];
        
        // send request
        [dataTask resume];
    }];
}


void LLAppsFlyerSetCustomerUserId(LLUnityString customerUserId)
{
    [AppsFlyerLib shared].customerUserID = LLNSStringFromUnityString(customerUserId);
}
