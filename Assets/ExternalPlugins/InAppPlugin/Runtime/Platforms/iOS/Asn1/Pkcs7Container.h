//
//  Pkcs7Container.h
//

#import <Foundation/Foundation.h>

@interface Pkcs7Container : NSObject

@property (nonatomic, readonly) NSData *payloadData;
@property (nonatomic, readwrite) NSString *objectIdentifier;

- (instancetype)initWithData:(NSData *)data;

@end
