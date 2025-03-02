//
//  LLGameKitHelper.m
//  Unity-iPhone
//
//  Created by Evgeny Chasovitin on 11/22/17.
//

#import "LLGameKitHelper.h"


static LLGameKitHelper *_instance;
const NSString *LEADERBOARD_NOT_FOUND_ERROR = @"Leaderboard not found.";


@implementation LLGameKitHelper

+ (instancetype)getInstance
{
    if (_instance == nil)
    {
        _instance = [[LLGameKitHelper alloc] init];
    }
    
    return _instance;
}


- (void)loadTopLeaderboardScores:(NSString *)leaderboardId withSize:(NSInteger)rangeLength withCompletion:(LLLibSetCallbackStringString)completion
{
    [GKLeaderboard loadLeaderboardsWithCompletionHandler:^(NSArray<GKLeaderboard *> * _Nullable leaderboards, NSError * _Nullable leaderboardsError) {
        if (leaderboardsError != nil)
        {
            completion(NULL, LLUnityStringFromNSString(leaderboardsError.localizedDescription));
            return;
        }
        
        __weak LLGameKitHelper *weakSelf = self;
        GKLeaderboard *targetLeaderboard = [weakSelf extractLeaderboardWithId:leaderboardId withBoards:leaderboards];
        
        if (targetLeaderboard != nil)
        {
            [targetLeaderboard setPlayerScope:GKLeaderboardPlayerScopeGlobal];
            [targetLeaderboard setRange:NSMakeRange(1, rangeLength)];
            [targetLeaderboard loadScoresWithCompletionHandler:^(NSArray<GKScore *> * _Nullable targetScores, NSError * _Nullable targetScoreError) {
                if (targetScoreError != nil)
                {
                    completion(NULL, LLUnityStringFromNSString(targetScoreError.localizedDescription));
                    return;
                }

                
                __weak GKLeaderboard *weakTargetLeaderboard = targetLeaderboard;
                
                NSMutableArray<GKScore *> *scoresToReturn = [NSMutableArray arrayWithArray:targetScores];
                BOOL hasLocalPlayerScore = NO;
                for (int i = 0; i < scoresToReturn.count && !hasLocalPlayerScore; ++i)
                {
                    if ([scoresToReturn[i].player.playerID isEqualToString:weakTargetLeaderboard.localPlayerScore.player.playerID])
                    {
                        hasLocalPlayerScore = YES;
                    }
                }
                
                if (!hasLocalPlayerScore)
                {
                    [scoresToReturn addObject:weakTargetLeaderboard.localPlayerScore];
                }
                
                LLUnityString resultScoresString = [weakSelf createScoresJSON:scoresToReturn withLocalPlayerId:weakTargetLeaderboard.localPlayerScore.player.playerID];
                completion(resultScoresString, NULL);
            }];
        }
        else
        {
            completion(NULL, LLUnityStringFromNSString((NSString *)LEADERBOARD_NOT_FOUND_ERROR));
        }
    }];
}


- (void)loadCloseLeaderboardScores:(NSString *)leaderboardId rangeHalf:(NSInteger)rangeHalf withCompletion:(LLLibSetCallbackStringString)completion
{
    [GKLeaderboard loadLeaderboardsWithCompletionHandler:^(NSArray<GKLeaderboard *> * _Nullable leaderboards, NSError * _Nullable leaderboardsError) {
        if (leaderboardsError != nil)
        {
            completion(NULL, LLUnityStringFromNSString(leaderboardsError.localizedDescription));
            return;
        }
        
        __weak LLGameKitHelper *weakSelf = self;
        GKLeaderboard *targetLeaderboard = [weakSelf extractLeaderboardWithId:leaderboardId withBoards:leaderboards];
        
        if (targetLeaderboard != nil)
        {
            [targetLeaderboard setPlayerScope:GKLeaderboardPlayerScopeGlobal];
            [targetLeaderboard setRange:NSMakeRange(1, 1)];
            [targetLeaderboard loadScoresWithCompletionHandler:^(NSArray<GKScore *> * _Nullable playerScores, NSError * _Nullable playerScoreError) {
                if (playerScoreError != nil)
                {
                    completion(NULL, LLUnityStringFromNSString(playerScoreError.localizedDescription));
                    return;
                }
                
                __weak GKLeaderboard *weakTargetLeaderboard = targetLeaderboard;
                
                NSInteger clampedTopIdx = weakTargetLeaderboard.localPlayerScore.rank - rangeHalf;
                if (clampedTopIdx < 1)
                {
                    clampedTopIdx = 1;
                }
                
                NSRange fetchRange = NSMakeRange(clampedTopIdx, rangeHalf * 2 + 1);
                [weakTargetLeaderboard setRange:fetchRange];
                [weakTargetLeaderboard loadScoresWithCompletionHandler:^(NSArray<GKScore *> * _Nullable targetScores, NSError * _Nullable targetScoresError) {
                    if (targetScoresError != nil)
                    {
                        completion(NULL, LLUnityStringFromNSString(targetScoresError.localizedDescription));
                        return;
                    }
                    
                    LLUnityString resultScoresString = [weakSelf createScoresJSON:targetScores withLocalPlayerId:weakTargetLeaderboard.localPlayerScore.player.playerID];
                    completion(resultScoresString, NULL);
                }];
            }];
        }
        else
        {
            completion(NULL, LLUnityStringFromNSString((NSString *)LEADERBOARD_NOT_FOUND_ERROR));
        }
    }];
}


- (void)reportLeaderboardScore:(NSString *)leaderboardId value:(int64_t)scoreValue withCompletion:(LLLibSetCallbackString)completion
{
    GKScore *scoreToReport = [[GKScore alloc] initWithLeaderboardIdentifier:leaderboardId];
    scoreToReport.value = scoreValue;
    
    [GKScore reportScores:@[scoreToReport] withCompletionHandler:^(NSError * _Nullable error) {
        if (completion != NULL)
        {
            completion(error != nil ? LLUnityStringFromNSString(error.localizedDescription) : NULL);
        }
    }];
}


- (LLUnityString)createScoresJSON:(NSArray<GKScore *> *)gkScores withLocalPlayerId:(NSString *)localPlayerId
{
    NSMutableString *resultString = [[NSMutableString alloc] init];
    
    for (int i = 0; i < gkScores.count; ++i)
    {
        GKScore *score = gkScores[i];
        NSString *scoreString = [NSString stringWithFormat:@"\n\t{ \"owner\" : \"%@\", \"value\" : %lld, \"formattedValue\" : \"%@\", \"rank\" : %d, \"me\" : %@ }",
                                 [[score player] alias],
                                 [score value],
                                 [score formattedValue],
                                 [score rank],
                                 [localPlayerId isEqualToString:score.player.playerID] ? @"true" : @"false"
                                ];
        [resultString appendString:scoreString];
        if (i < gkScores.count - 1)
        {
            [resultString appendString:@","];
        }
        else
        {
            [resultString appendString:@"\n"];
        }
    }
    
    return [[NSString stringWithFormat:@"[%@]", resultString] UTF8String];
}


- (GKLeaderboard *)extractLeaderboardWithId:(NSString *)leaderboardId withBoards:(NSArray<GKLeaderboard *> *)boards
{
    GKLeaderboard *targetLeaderboard = nil;
    for (int i = 0; i < boards.count; ++i)
    {
        if ([boards[i].identifier isEqualToString:leaderboardId])
        {
            targetLeaderboard = boards[i];
            break;
        }
    }
    
    return targetLeaderboard;
}

@end


void LLGameCenterHelperLoadTopLeaderboardScores(LLUnityString leaderboardId, int rangeLength, LLLibSetCallbackStringString callback)
{
    [[LLGameKitHelper getInstance] loadTopLeaderboardScores:[NSString stringWithUTF8String:leaderboardId] withSize:rangeLength withCompletion:callback];
}


void LLGameCenterHelperLoadCloseLeaderboardScores(LLUnityString leaderboardId, int rangeHalf, LLLibSetCallbackStringString callback)
{
    [[LLGameKitHelper getInstance] loadCloseLeaderboardScores:[NSString stringWithUTF8String:leaderboardId] rangeHalf:rangeHalf withCompletion:callback];
}


void LLGameCenterHelperReportLeaderboardScore(LLUnityString leaderboardId, int scoreValue, LLLibSetCallbackString callback)
{
    [[LLGameKitHelper getInstance] reportLeaderboardScore:[NSString stringWithUTF8String:leaderboardId] value:(int64_t)scoreValue withCompletion:callback];
}
