//
//  LLMemoryWarning.h
//  LLLibSet
//

#import "LLLibSet.h"


@interface LLMemoryWarning : NSObject

+ (instancetype)sharedInstance;

@end


FOUNDATION_EXTERN void LLMemoryWarningInit(LLLibSetCallback callback);
