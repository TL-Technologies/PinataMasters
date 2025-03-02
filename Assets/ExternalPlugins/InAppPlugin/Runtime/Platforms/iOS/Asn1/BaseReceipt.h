//
//  BaseReceipt.h
//

#import <Foundation/Foundation.h>

@interface BaseReceipt :NSObject

- (instancetype)initWithData:(NSData *)data;

- (void)parseData:(NSData *)data;
- (NSDate *)dateFromData:(NSData *)data;
- (NSInteger)intFromData:(NSData *)data;
- (NSString *)stringFromData:(NSData *)data;

@end
