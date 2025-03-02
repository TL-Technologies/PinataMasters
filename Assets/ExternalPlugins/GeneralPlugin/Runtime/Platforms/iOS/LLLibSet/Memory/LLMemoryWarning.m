//
//  LLMemoryWarning.m
//  LLLibSet
//

#import "LLMemoryWarning.h"
#import <UIKit/UIKit.h>

@interface LLMemoryWarning()
{
    LLLibSetCallback _memoryCallback;
}

@end


static LLMemoryWarning *_sharedInstance = nil;


@implementation LLMemoryWarning

+ (instancetype)sharedInstance
{
    if (_sharedInstance == nil)
    {
        _sharedInstance = [[LLMemoryWarning alloc] init];
    }
    
    return _sharedInstance;
}


- (instancetype)init
{
    self = [super init];

    if (self != nil)
    {
        [[NSNotificationCenter defaultCenter] addObserver:self
                                         selector:@selector(didRecieveMemoryWarning)
                                             name:UIApplicationDidReceiveMemoryWarningNotification
                                           object:[UIApplication sharedApplication]];
    }
    
    return self;
}


- (void)didRecieveMemoryWarning
{
    if (_memoryCallback != nil)
    {
        _memoryCallback();
    }
}


- (void)setMemoryCallback:(LLLibSetCallback)callback
{
    _memoryCallback = callback;
}


#pragma mark - Extern

void LLMemoryWarningInit(LLLibSetCallback callback)
{
    [[LLMemoryWarning sharedInstance] setMemoryCallback:callback];
}

@end
