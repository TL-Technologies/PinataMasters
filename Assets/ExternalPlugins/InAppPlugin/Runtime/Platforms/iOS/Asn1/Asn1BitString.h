//
//  Asn1BitString.h
//


#import <Foundation/Foundation.h>
/**
 Represents a string of bits.
 */
@interface Asn1BitString : NSObject

/**
 The designated initializer
 @param data The data of the bit string
 @param unusedBits The number of bits at the end of the data that are not used
 */
- (id)initWithData:(NSData *)data unusedBits:(NSUInteger)unusedBits;

/**
 Returns the bit value of the bit at the given index whereas the index is numbering the individual bits.
 @param index The bit index to query
 @returns `YES` if the bit at this position is 1.
 */
- (BOOL)valueOfBitAtIndex:(NSUInteger)index;

/**
 Creates a string with bits concatenated.
 */
- (NSString *)stringWithBits;

/**
 The number of bits at the end of the data chunk that are not used
 */
@property (nonatomic, assign) NSUInteger unusedBits;

@end
