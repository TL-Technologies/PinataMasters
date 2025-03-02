//
//  Receipt.h
//

#import <Foundation/Foundation.h>
#import "BaseReceipt.h"


@interface Receipt : BaseReceipt

@property (nonatomic, readonly) NSString *bundleIdentifier;
@property (nonatomic, readonly) NSString *appVersion;
@property (nonatomic, readonly) NSString *ageRating;
@property (nonatomic, readonly) NSString *receiptType;
@property (nonatomic, readonly) NSString *originalAppVersion;

@property (nonatomic, readonly) NSData *bundleIdentifierData;
@property (nonatomic, readonly) NSData *opaqueValue;
@property (nonatomic, readonly) NSData *SHA1Hash;

@property (nonatomic, readonly) NSDate *receiptCreationDate;
@property (nonatomic, readonly) NSDate *receiptExpirationDate;
@property (nonatomic, readonly) NSDate *unknownPurposeDate;

@property (nonatomic, readonly) NSMutableArray *inAppPurchaseReceipts;
@property (nonatomic, readonly) NSArray *inAppSubscriptionReceipts;

@end
