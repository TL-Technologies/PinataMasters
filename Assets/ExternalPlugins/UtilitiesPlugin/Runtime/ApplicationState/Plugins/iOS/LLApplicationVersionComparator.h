//
//  LLApplicationVersionComparator.h
//  LLLibSet
//
//  Created by Valera Shostak on 08/17/18.
//  Copyright © 2017 My Company. All rights reserved.
//


#import <Foundation/Foundation.h>
#import "LLLibSet.h"
#import "LLAppController.h"

@interface LLApplicationVersionComparator : NSObject

+ (instancetype)sharedInstance;

@end


FOUNDATION_EXTERN bool LLApplicationVersionComparatorIsCurrentGameVersionLatest();
