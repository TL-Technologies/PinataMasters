//
//  LLSwipeHandler.h
//  LLLibSet
//

#import "LLLibSet.h"
#import <UIKit/UIGestureRecognizerSubclass.h>

@interface LLRunnerGestureRecognizer : UIGestureRecognizer
{
}

@end


@protocol LLRunnerGestureRecognizerDelegate <NSObject>

- (void)runnerGestureRecognizerDidSwipeLeft:(LLRunnerGestureRecognizer *)recognizer;
- (void)runnerGestureRecognizerDidSwipeRight:(LLRunnerGestureRecognizer *)recognizer;
- (void)runnerGestureRecognizerDidSwipeUp:(LLRunnerGestureRecognizer *)recognizer;
- (void)runnerGestureRecognizerDidSwipeDown:(LLRunnerGestureRecognizer *)recognizer;
- (void)runnerGestureRecognizerDidTap:(LLRunnerGestureRecognizer *)recognizer;

@end


@interface LLSwipeHandler : NSObject<LLRunnerGestureRecognizerDelegate>
{
}

@end

#if !UNITY_TVOS
FOUNDATION_EXTERN void LLSwipeHandlerUpdate();
FOUNDATION_EXTERN LLUnityString LLSwipeHandlerPopHistory();
#endif
