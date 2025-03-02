//
//  StoreUtilities.m
//  Unity-iPhone
//
//  Created by Vadim Marchuk on 26/02/2021.
//

#import <Foundation/Foundation.h>
#import "Receipt.h"


FOUNDATION_EXTERN bool IsPurchasesInSandboxEnvironment()
{
    NSData *receiptData = [NSData dataWithContentsOfURL:[[NSBundle mainBundle] appStoreReceiptURL]];
    Receipt *receipt = [[Receipt alloc] initWithData:receiptData];
    if (receipt)
    {
        return [receipt.receiptType isEqualToString:@"ProductionSandbox"];
    }
    
    return false;
}
