//
//  LLApplicationVersionComparator.h
//  LLLibSet
//
//  Created by Valera Shostak on 08/17/18.
//  Copyright © 2017 My Company. All rights reserved.
//

#import "LLApplicationVersionComparator.h"

@interface LLApplicationVersionComparator()
{
}

@property (nonatomic) BOOL isNeedToUpdate;

@end

@implementation LLApplicationVersionComparator


+ (instancetype)sharedInstance
{
    static LLApplicationVersionComparator *sharedInstance = nil;
    
    if (sharedInstance == nil)
    {
        sharedInstance = [[LLApplicationVersionComparator alloc] init];
    }
    
    return sharedInstance;
}


- (BOOL)isNeedToUpdate
{
    NSDictionary *infoDictionary = [[NSBundle mainBundle] infoDictionary];
    NSString *appID = infoDictionary[@"CFBundleIdentifier"];
    NSURL *url = [NSURL URLWithString:[NSString stringWithFormat:@"http://itunes.apple.com/lookup?bundleId=%@", appID]];
    NSData *data = [NSData dataWithContentsOfURL:url];
    
    if (data != nil)
    {
        NSDictionary *lookup = [NSJSONSerialization JSONObjectWithData:data
                                                               options:0
                                                                 error:nil];
        
        if ([lookup[@"resultCount"] integerValue] == 1)
        {
            NSString *appStoreVersion = lookup[@"results"][0][@"version"];
            NSString *currentVersion = infoDictionary[@"CFBundleShortVersionString"];
            
            if ([currentVersion compare:appStoreVersion
                                options:NSNumericSearch] == NSOrderedAscending)
            {
                NSLog(@"Need to update");
                return YES;
            }
        }
    }
    
    return NO;
}



#pragma mark - Unity Extern

bool LLApplicationVersionComparatorIsCurrentGameVersionLatest()
{
    return ![LLApplicationVersionComparator sharedInstance].isNeedToUpdate;
}

@end
