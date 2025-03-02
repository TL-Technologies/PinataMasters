//
//  PurchaseReceipt.m
//  LLLibSet
//

#import "PurchaseReceipt.h"


@implementation PurchaseReceipt

@synthesize quantity = _quantity;
@synthesize productIdentifier = _productIdentifier;
@synthesize transactionIdentifier = _transactionIdentifier;
@synthesize originalTransactionIdentifier = _originalTransactionIdentifier;
@synthesize purchaseDate = _purchaseDate;
@synthesize originalPurchaseDate = _originalPurchaseDate;
@synthesize subscriptionExpirationDate = _subscriptionExpirationDate;
@synthesize cancellationDate = _cancellationDate;
@synthesize webOrderLineItemIdentifier = _webOrderLineItemIdentifier;


- (void)processItem:(NSInteger)type withData:(NSData *)data
{
  switch(type)
  {
    case 1701:
      _quantity = [self intFromData:data];
      break;
    case 1702:
      _productIdentifier = [self stringFromData:data];
      break;
    case 1703:
      _transactionIdentifier = [self stringFromData:data];
      break;
    case 1704:
      _purchaseDate = [self dateFromData:data];
      break;
    case 1705:
      _originalTransactionIdentifier = [self stringFromData:data];
      break;
    case 1706:
      _originalPurchaseDate = [self dateFromData:data];
      break;
    case 1708:
      _subscriptionExpirationDate = [self dateFromData:data];
      break;
    case 1711:
      _webOrderLineItemIdentifier = [self intFromData:data];
      break;
    case 1712:
      _cancellationDate = [self dateFromData:data];
      break;
    default:
      break;
  }
}


@end




