//
//  KeychainUtils.h
//  LLLibSet
//
//  Created by valery.p on 02.09.2020.
//

#import <Foundation/Foundation.h>
#import "LLLibSet.h"

@interface KeychainUtils : NSObject

+ (instancetype)sharedInstance;

@end

FOUNDATION_EXTERN void KeychainUtilsInitialize(LLUnityString);
FOUNDATION_EXTERN LLUnityString KeychainUtilsLoadByKey(LLUnityString);
FOUNDATION_EXTERN void KeychainUtilsRemoveForKey(LLUnityString);
FOUNDATION_EXTERN void KeychainUtilsSaveForKey(LLUnityString, LLUnityString);
