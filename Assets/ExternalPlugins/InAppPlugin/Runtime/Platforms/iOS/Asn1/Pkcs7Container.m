//
//  Pkcs7Container.m
//

#import "Pkcs7Container.h"
#import "Asn1Serialization.h"


@implementation Pkcs7Container

@synthesize payloadData = _payloadData;
@synthesize objectIdentifier = _objectIdentifier;

- (instancetype)initWithData:(NSData *)data
{
    self = [super init];
    if (self)
    {
        id rootSequence = [Asn1Serialization objectWithData:data];
        if ([rootSequence isKindOfClass:[NSArray class]])
        {
            _payloadData = [self unwrapRootSequence:rootSequence];
        }
    }
  
    return self;
}


- (NSData *)unwrapRootSequence:(NSArray *)sequence
{
    if ([sequence count] == 2)
    {
        id OID = sequence[0];
        if ([OID isKindOfClass:[NSString class]] && [((NSString *)OID) isEqualToString:@"1.2.840.113549.1.7.2"])
        {
            id containedSequence = sequence[1];
            if ([containedSequence isKindOfClass:[NSArray class]] && ([((NSArray *)containedSequence) count] == 1))
            {
                id actualSequence = ((NSArray *)containedSequence)[0];
                if ([actualSequence isKindOfClass:[NSArray class]] && ([((NSArray *)actualSequence) count] == 5))
                {
                    id dataSequence = ((NSArray *)actualSequence)[2];
                    if ([dataSequence isKindOfClass:[NSArray class]])
                    {
                        return [self unwrapSignedDataSequence:(NSArray *)dataSequence];
                    }
                }
            }
        }
    }

    return nil;
}


- (NSData *)unwrapSignedDataSequence:(NSArray *)sequence
{
    if ([sequence count] == 2)
    {
        id OID = sequence[0];
        if ([OID isKindOfClass:[NSString class]] && [((NSString *)OID) isEqualToString:@"1.2.840.113549.1.7.1"])
        {
            id dataSequence = sequence[1];
            if ([dataSequence isKindOfClass:[NSArray class]] && ([((NSArray *)dataSequence) count] == 1))
            {
                id data = ((NSArray *)dataSequence)[0];
                if ([data isKindOfClass:[NSData class]])
                {
                    return (NSData *)data;
                }
            }
        }
    }
  
    return nil;
}


@end
