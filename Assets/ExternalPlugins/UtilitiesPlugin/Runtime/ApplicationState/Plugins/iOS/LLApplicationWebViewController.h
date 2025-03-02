//
//  LLApplicationWebViewController.h
//  LLLibSet
//
//  Created by Valera Shostak on 12/19/17.
//  Copyright © 2017 My Company. All rights reserved.
//


#import <Foundation/Foundation.h>
#import "LLLibSet.h"

@interface LLApplicationWebViewController : NSObject

+ (instancetype)sharedInstance;

@end


FOUNDATION_EXTERN void LLApplicationWebViewControllerShow(LLUnityString, LLUnityString);
