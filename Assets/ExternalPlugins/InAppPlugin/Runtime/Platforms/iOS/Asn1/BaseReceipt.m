//
//  BaseReceipt.m
//

#import "BaseReceipt.h"
#import "Asn1Serialization.h"

@implementation BaseReceipt

- (instancetype)initWithData:(NSData *)data
{
    self = [super init];
    if (self)
    {
        [self parseData: data];
    }
  
    return self;
}


- (void)parseData:(NSData *)data
{
    id rootArray = [Asn1Serialization objectWithData:data];
    if ([rootArray isKindOfClass:[NSArray class]])
    {
        for (NSArray *item in (NSArray *)rootArray)
        {
            if ([item count] == 3)
            {
                NSInteger type = ((NSNumber *)item[0]).integerValue;
                NSInteger version = ((NSNumber *)item[1]).integerValue;
                NSData *itemData = item[2];
                if (version > 0)
                {
                    [self processItem:type withData:itemData];
                }
            }
        }
    }
}


- (void)processItem:(NSInteger)type withData:(NSData *)data
{
    //OVERRIDE
}


- (NSInteger)intFromData:(NSData *)data
{
    id obj = [Asn1Serialization objectWithData:data];
    if ([obj isKindOfClass:[NSNumber class]])
    {
        return ((NSNumber *)obj).integerValue;
    }
    else
    {
        return 0;
    }
}


- (NSString *)stringFromData:(NSData *)data
{
    id obj = [Asn1Serialization objectWithData:data];
    if ([obj isKindOfClass:[NSString class]])
    {
        return ((NSString *)obj);
    }
    else
    {
        return @"";
    }
}


- (NSDate *)dateFromData:(NSData *)data
{
    NSString *dateString = [self stringFromData:data];
    if ([dateString length] == 0)
    {
        return nil;
    }
    else
    {
        return [self dateFromRFC3339String:dateString];
    }
}


- (NSDate *)dateFromRFC3339String:(NSString *)string
{
    NSDateFormatter *formatter = [[NSDateFormatter alloc] init];
    formatter.locale = [NSLocale localeWithLocaleIdentifier:@"en_US_POSIX"];
    formatter.dateFormat = @"yyyy-MM-dd'T'HH:mm:ss'Z'";
    formatter.timeZone = [NSTimeZone timeZoneForSecondsFromGMT:0];
  
    return [formatter dateFromString:string];
}

@end
