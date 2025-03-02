//
//  LLAppController.m
//  LLLibSet
//
//  Created by anGGod on 5/6/15.
//  Copyright (c) 2015 My Company. All rights reserved.
//

#import "LLAppController.h"

static NSMutableArray *_listeners = nil;

@implementation LLAppController

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object application:application didFinishLaunchingWithOptions:launchOptions];
    }
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}


- (void)applicationDidEnterBackground:(UIApplication*)application
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object applicationDidEnterBackground:application];
    }
    [super applicationDidEnterBackground:application];
}


- (void)applicationWillEnterForeground:(UIApplication*)application
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object applicationWillEnterForeground:application];
    }
    [super applicationWillEnterForeground:application];
}


-(void)applicationDidBecomeActive:(UIApplication *)application
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object applicationDidBecomeActive:application];
    }
    [super applicationDidBecomeActive:application];
}


-(void)applicationWillResignActive:(UIApplication *)application
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object applicationWillResignActive:application];
    }
    [super applicationWillResignActive:application];
}


- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray *_Nullable))restorationHandler
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object application:application continueUserActivity:userActivity restorationHandler:restorationHandler];
    }
    return [super application:application continueUserActivity:userActivity restorationHandler:restorationHandler];
}


- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url options:(NSDictionary *) options
{
    BOOL result = YES;
    for (id<UIApplicationDelegate> object in _listeners)
    {
        result |= [object application:application openURL:url options:options];
    }
    return result;
//    return [super application:application openURL:url options:options];
}


#if !UNITY_TVOS
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
    }
    return [super application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
}
#endif


#if UNITY_USES_REMOTE_NOTIFICATIONS
- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
    }
    
    [super application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
}


-(void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object application:application didReceiveRemoteNotification:userInfo];
    }
    [super application:application didReceiveRemoteNotification:userInfo];
}


-(void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult))completionHandler
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:completionHandler];
    }
    [super application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:completionHandler];
}


-(void)application:(UIApplication *)application didReceiveLocalNotification:(UILocalNotification *)notification
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object application:application didReceiveLocalNotification:notification];
    }
    [super application:application didReceiveLocalNotification:notification];
}


-(void)application:(UIApplication *)application handleActionWithIdentifier:(NSString *)identifier forRemoteNotification:(NSDictionary *)userInfo completionHandler:(void (^)())completionHandler
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object application:application handleActionWithIdentifier:identifier forRemoteNotification:userInfo completionHandler:completionHandler];
    }
    [super application:application handleActionWithIdentifier:identifier forRemoteNotification:userInfo completionHandler:completionHandler];
}


-(void)application:(UIApplication *)application handleActionWithIdentifier:(NSString *)identifier forLocalNotification:(UILocalNotification *)notification completionHandler:(void (^)())completionHandler
{
    for (id<UIApplicationDelegate> object in _listeners)
    {
        [object application:application handleActionWithIdentifier:identifier forLocalNotification:notification completionHandler:completionHandler];
    }
    [super application:application handleActionWithIdentifier:identifier forLocalNotification:notification completionHandler:completionHandler];
}
#endif

@end

IMPL_APP_CONTROLLER_SUBCLASS(LLAppController)



#pragma mark - Proxy

@interface AppDelegateListenerProxy : NSProxy

@property (nonatomic, strong) id object;

+(instancetype) proxyWithObject:(id<UIApplicationDelegate>)object;

@end


@implementation AppDelegateListenerProxy

-(instancetype)initWithObject:(id<UIApplicationDelegate>)object
{
    self.object = object;
    return self;
}


+(instancetype) proxyWithObject:(id<UIApplicationDelegate>)object
{
    return [[self alloc] initWithObject:object];
}


- (NSMethodSignature *)methodSignatureForSelector:(SEL)selector
{
    return [self.object methodSignatureForSelector:selector];
}


- (void)forwardInvocation:(NSInvocation *)invocation
{
    if ([self.object respondsToSelector:invocation.selector])
    {
        [invocation invokeWithTarget:self.object];
    }
}

@end



#pragma mark - Etern methods

void RegisterAppDelegateListener(id<UIApplicationDelegate> obj)
{
    if (_listeners == nil)
    {
        _listeners = [[NSMutableArray alloc] init];
    }
    [_listeners addObject:[AppDelegateListenerProxy proxyWithObject:obj]];
}


void UnregisterAppDelegateListener(id<UIApplicationDelegate> obj)
{
    AppDelegateListenerProxy *objectRemove = nil;
    for (AppDelegateListenerProxy *proxy in _listeners)
    {
        if ([proxy.object isEqual:obj])
        {
            objectRemove = proxy;
            break;
        }
    }
    
    if (objectRemove != nil)
    {
        [_listeners removeObject:objectRemove];
    }
}
