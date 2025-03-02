//
//  Receipt.m
//

#import "Receipt.h"
#import "PurchaseReceipt.h"
#import "Pkcs7Container.h"


@implementation Receipt
{
    NSMutableArray *_sortedSubscriptions;
}

@synthesize bundleIdentifier = _bundleIdentifier;
@synthesize appVersion = _appVersion;
@synthesize ageRating = _ageRating;
@synthesize receiptType= _receiptType;
@synthesize originalAppVersion = _originalAppVersion;
@synthesize bundleIdentifierData = _bundleIdentifierData;
@synthesize opaqueValue = _opaqueValue;
@synthesize SHA1Hash = _SHA1Hash;
@synthesize receiptCreationDate = _receiptCreationDate;
@synthesize receiptExpirationDate = _receiptExpirationDate;
@synthesize unknownPurposeDate = _unknownPurposeDate;
@synthesize inAppPurchaseReceipts = _inAppPurchaseReceipts;
@synthesize inAppSubscriptionReceipts = _inAppSubscriptionReceipts;


- (instancetype)initWithData:(NSData *)data
{
    self = [super init];
    if (self)
    {
        _inAppPurchaseReceipts = [[NSMutableArray alloc] init];
        _sortedSubscriptions = [[NSMutableArray alloc] init];
        Pkcs7Container *container = [[Pkcs7Container alloc] initWithData:data];
        
        [self parseData:container.payloadData];

        _inAppSubscriptionReceipts = [_sortedSubscriptions sortedArrayUsingComparator:^NSComparisonResult(id a, id b) {
            NSDate *first = [(PurchaseReceipt *)a subscriptionExpirationDate];
            NSDate *second = [(PurchaseReceipt *)b subscriptionExpirationDate];
            return [first compare:second];
        }];
    }
  
    return self;
}


- (void)processItem:(NSInteger)type withData:(NSData *)data
{
    switch(type)
    {
        case 0:
            _receiptType = [self stringFromData:data];
          break;
        case 2:
            _bundleIdentifier = [self stringFromData:data];
            _bundleIdentifierData = data;
            break;
        case 3:
            _appVersion = [self stringFromData:data];
            break;
        case 4:
            _opaqueValue = data;
            break;
        case 5:
            _SHA1Hash = data;
            break;
        case 10:
            _ageRating = [self stringFromData:data];
            break;
        case 12:
            _receiptCreationDate = [self dateFromData:data];
            break;
        case 17:
        {
            PurchaseReceipt *purchaseReceipt = [[PurchaseReceipt alloc] initWithData:data];
            if (purchaseReceipt.subscriptionExpirationDate != nil)
            {
                [_sortedSubscriptions addObject:purchaseReceipt];
            }
            else
            {
                [_inAppPurchaseReceipts addObject:purchaseReceipt];
            }
        }
            break;
        case 18:
            _unknownPurposeDate = [self dateFromData:data];
            break;
        case 19:
            _originalAppVersion = [self stringFromData:data];
            break;
        case 21:
            _receiptExpirationDate = [self dateFromData:data];
            break;
        default:
            break;
    }
}


@end
