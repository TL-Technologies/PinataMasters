//
//  LLJSONSerialization.m
//  LLLibSet
//

#import "LLJSONSerialization.h"

@implementation NSJSONSerialization (libset_extentions)


+ (NSString *)JSONStringWithObject:(id)object
{
    NSData *curJSON = [self dataWithJSONObject:object
                                       options:0
                                         error:nil];
    return [[NSString alloc] initWithData:curJSON
                                 encoding:NSUTF8StringEncoding];
}


@end
