//
//  LLGameKitHelper.h
//  Unity-iPhone
//
//  Created by Evgeny Chasovitin on 11/22/17.
//

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "LLLibSet.h"

@interface LLGameKitHelper : NSObject

+ (instancetype)getInstance;

- (void)loadTopLeaderboardScores:(NSString *)leaderboardId withSize:(NSInteger)rangeLength withCompletion:(LLLibSetCallbackStringString)completion;
- (void)loadCloseLeaderboardScores:(NSString *)leaderboardId rangeHalf:(NSInteger)rangeHalf withCompletion:(LLLibSetCallbackStringString)completion;

- (void)reportLeaderboardScore:(NSString *)leaderboardId value:(int64_t)scoreValue withCompletion:(LLLibSetCallbackString)completion;

@end


FOUNDATION_EXTERN void LLGameCenterHelperLoadTopLeaderboardScores(LLUnityString leaderboardId, int rangeLength, LLLibSetCallbackStringString callback);
FOUNDATION_EXTERN void LLGameCenterHelperLoadCloseLeaderboardScores(LLUnityString leaderboardId, int rangeHalf, LLLibSetCallbackStringString callback);

FOUNDATION_EXTERN void LLGameCenterHelperReportLeaderboardScore(LLUnityString leaderboardId, int scoreValue, LLLibSetCallbackString callback);
