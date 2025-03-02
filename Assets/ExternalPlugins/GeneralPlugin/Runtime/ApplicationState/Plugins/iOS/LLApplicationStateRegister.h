//
//  LLApplicationStateRegister.h
//  LLLibSet
//
//  Created by Valera Shostak on 11/29/17.
//  Copyright © 2017 My Company. All rights reserved.
//


#import <Foundation/Foundation.h>
#import "LLLibSet.h"
#import "LLAppController.h"

@interface LLApplicationStateRegister : NSObject<UIApplicationDelegate>

+ (instancetype)sharedInstance;

@end


FOUNDATION_EXTERN void LLApplicationStateRegisterInit(LLLibSetCallbackBool);
