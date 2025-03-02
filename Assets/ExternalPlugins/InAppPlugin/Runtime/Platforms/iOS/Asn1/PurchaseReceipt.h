//
//  PurchaseReceipt.h
//


#import <Foundation/Foundation.h>
#import "BaseReceipt.h"

@interface PurchaseReceipt : BaseReceipt

@property (nonatomic, readonly) NSInteger quantity;

@property (nonatomic, readonly) NSString *productIdentifier;
@property (nonatomic, readonly) NSString *transactionIdentifier;
@property (nonatomic, readonly) NSString *originalTransactionIdentifier;

@property (nonatomic, readonly) NSDate *purchaseDate;
@property (nonatomic, readonly) NSDate *originalPurchaseDate;
@property (nonatomic, readonly) NSDate *subscriptionExpirationDate;
@property (nonatomic, readonly) NSDate *cancellationDate;

@property (nonatomic, readonly) NSInteger webOrderLineItemIdentifier;

@end
