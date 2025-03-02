//
//  LLApplicationStateRegister.m
//  LLLibSet
//
//  Created by Valera Shostak on 11/29/17.
//  Copyright © 2017 My Company. All rights reserved.
//

#import "LLApplicationStateRegister.h"

@interface LLApplicationStateRegister()
{
    BOOL  _didEnteredBackGround;
    LLLibSetCallbackBool    _backgroundCallback;
}

- (void)initialize:(LLLibSetCallbackBool) callback;

@end

@implementation LLApplicationStateRegister


+ (instancetype)sharedInstance
{
    static LLApplicationStateRegister *sharedInstance = nil;
    
    if (sharedInstance == nil)
    {
        sharedInstance = [[LLApplicationStateRegister alloc] init];
    }
    
    return sharedInstance;
}


- (void)initialize:(LLLibSetCallbackBool) callback
{
    _backgroundCallback = callback;
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


#pragma mark - AppDelegate

-(void)applicationDidBecomeActive:(UIApplication *)application
{
    if (_didEnteredBackGround == YES)
    {
        _didEnteredBackGround = NO;
        
        if (_backgroundCallback)
        {
            _backgroundCallback(_didEnteredBackGround);
        }
    }
}


-(void)applicationDidEnterBackground:(UIApplication *)application
{
    _didEnteredBackGround = YES;
    
    if (_backgroundCallback)
    {
        _backgroundCallback(_didEnteredBackGround);
    }
}


#pragma mark - Unity Extern

void LLApplicationStateRegisterInit(LLLibSetCallbackBool callback)
{
    [[LLApplicationStateRegister sharedInstance] initialize:(LLLibSetCallbackBool) callback];
}

@end
