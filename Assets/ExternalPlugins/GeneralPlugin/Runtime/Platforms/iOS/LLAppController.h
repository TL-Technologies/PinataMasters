//
//  LLAppController.h
//  LLLibSet
//
//  Created by anGGod on 5/6/15.
//  Copyright (c) 2015 My Company. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "UnityAppController.h"

@interface LLAppController : UnityAppController

FOUNDATION_EXTERN void RegisterAppDelegateListener(id<UIApplicationDelegate> obj);
FOUNDATION_EXTERN void UnregisterAppDelegateListener(id<UIApplicationDelegate> obj);

@end
