//
//  LLSystemPopUp.m
//  LLLibSet
//

#import "LLRatePopUp.h"
#import <StoreKit/StoreKit.h>

static LLRatePopUp *_sharedInstance = nil;

static NSOperatingSystemVersion minVersion = (NSOperatingSystemVersion){.majorVersion = 10, .minorVersion = 3, .patchVersion = 0};

@implementation LLRatePopUp

+ (instancetype)sharedInstance
{
    if (_sharedInstance == nil)
    {
        _sharedInstance = [[LLRatePopUp alloc] init];
    }
    
    return _sharedInstance;
}


- (bool)isAvalaiblePopup
{
    bool result = false;
    
    if ([[NSProcessInfo processInfo] isOperatingSystemAtLeastVersion:minVersion])
    {
        if ([SKStoreReviewController class])
        {
            result = true;
        }
    }
    
    return result;
}

- (void)showPopup
{
    if ([self isAvalaiblePopup])
    {
        [SKStoreReviewController requestReview];
    }
}

@end


#pragma mark - C interface

bool LLRatePopIsAvalaible()
{
    return [[LLRatePopUp sharedInstance] isAvalaiblePopup];
}


void LLRatePopUpShow()
{
    [[LLRatePopUp sharedInstance] showPopup];
}
