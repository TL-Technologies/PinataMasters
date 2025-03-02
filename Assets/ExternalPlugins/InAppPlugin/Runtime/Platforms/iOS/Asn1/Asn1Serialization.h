//
//  ASN1Serialization.h
//


#import <Foundation/Foundation.h>

/**
 Convenience factory for deserializing ASN.1 data
 */
@interface Asn1Serialization : NSObject

/**
 Creates an object from ASN.1 data
 @param data The ASN.1 data
 @returns The deserialized object or `nil` if an error occured
 */
+ (nullable id)objectWithData:(nonnull NSData *)data;

@end
