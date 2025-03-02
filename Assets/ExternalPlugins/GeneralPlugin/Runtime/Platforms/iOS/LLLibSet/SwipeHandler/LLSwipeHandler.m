//
//  LLSwipeHandler.h
//  LLLibSet
//

#import "LLSwipeHandler.h"
#import <UIKit/UIKit.h>
#import <CoreGraphics/CoreGraphics.h>

#if CGFLOAT_IS_DOUBLE

#define CGABS       fabs
#define CGSQRT      sqrt

#else

#define CGABS       fabsf
#define CGSQRT      sqrtf

#endif

char* LLSwipeHandlerStringCopy(NSString *string)
{
    NSUInteger resultLength = ((string != nil) ? strlen(string.UTF8String) : 0) + 1;
    char *result = (char *)calloc(1, resultLength);
    
    if (string != nil)
    {
        strncpy(result, string.UTF8String, resultLength);
    }
    
    return result;
}


@interface LLRunnerGestureRecognizer ()
{
    NSMutableDictionary                     *_tapStartPositions;
}

@property (nonatomic, readonly) NSMutableDictionary *tapStartPositions;
@property (nonatomic, weak) id <LLRunnerGestureRecognizerDelegate> runnerDelegate;

@end


@implementation LLRunnerGestureRecognizer


@synthesize runnerDelegate = _runnerDelegate;


- (NSMutableDictionary *)tapStartPositions
{
    if (_tapStartPositions == nil)
    {
        _tapStartPositions = [[NSMutableDictionary alloc] init];
    }
    
    return _tapStartPositions;
}


- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{
    for (UITouch *curTouch in touches)
    {
        NSValue *curPointer = [NSValue valueWithPointer:((__bridge void *)curTouch)];
        CGPoint curLocation = [curTouch locationInView:self.view];
        
        [self.tapStartPositions setObject:[NSValue valueWithCGPoint:curLocation]
                                   forKey:curPointer];
    }
}


- (CGFloat)minSwipeLength
{
    return (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) ? 15.f : 25.f;
}


- (void)touchesMoved:(NSSet *)touches withEvent:(UIEvent *)event
{
    for (UITouch *curTouch in touches)
    {
        NSValue *curPointer = [NSValue valueWithPointer:((__bridge void *)curTouch)];
        NSValue *curStartPositionValue = [self.tapStartPositions objectForKey:curPointer];
        
        if (curStartPositionValue != nil)
        {
            CGPoint curStartPos = curStartPositionValue.CGPointValue;
            CGPoint curLocation = [curTouch locationInView:self.view];
            
            CGPoint delta = CGPointMake(curLocation.x - curStartPos.x,
                                        curLocation.y - curStartPos.y);
            
            CGPoint absDelta = CGPointMake(CGABS(delta.x), CGABS(delta.y));
            CGFloat deltaLength = CGSQRT(delta.x * delta.x + delta.y * delta.y);
            
            
            if (deltaLength >= self.minSwipeLength)
            {
                if (absDelta.x >= absDelta.y)
                {
                    if (delta.x < 0.0)
                    {
                        [self.runnerDelegate runnerGestureRecognizerDidSwipeLeft:self];
                    }
                    else
                    {
                        [self.runnerDelegate runnerGestureRecognizerDidSwipeRight:self];
                    }
                }
                else
                {
                    if (delta.y > 0.0)
                    {
                        [self.runnerDelegate runnerGestureRecognizerDidSwipeDown:self];
                    }
                    else
                    {
                        [self.runnerDelegate runnerGestureRecognizerDidSwipeUp:self];
                    }
                }
                
                [self.tapStartPositions removeObjectForKey:curPointer];
            }
        }
    }
}


- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event
{
    for (UITouch *curTouch in touches)
    {
        NSValue *curPointer = [NSValue valueWithPointer:((__bridge void *)curTouch)];
        
        if ([self.tapStartPositions objectForKey:curPointer] != nil)
        {
            [self.runnerDelegate runnerGestureRecognizerDidTap:self];
        }
    }
}


- (void)touchesCancelled:(NSSet *)touches withEvent:(UIEvent *)event
{
    [self touchesEnded:touches withEvent:event];
}

@end




@interface LLSwipeHandler () <UIGestureRecognizerDelegate>

@end



@implementation LLSwipeHandler
{
    UIView          *_attachedView;
    NSArray         *_attachedRecognizers;
    
    NSMutableArray  *_swipeHistory;
}


+ (instancetype)sharedInstance
{
    static LLSwipeHandler *__sharedInstance = nil;
    if (__sharedInstance == nil)
    {
        __sharedInstance = [[LLSwipeHandler alloc] init];
    }
    
    [__sharedInstance updateRecognizer];
    
    return __sharedInstance;
}


- (BOOL)isScreenEdgeAvailable
{
    return NSClassFromString(@"UIScreenEdgePanGestureRecognizer") != nil;
}


- (NSArray *)requiredRecognizers
{
    NSMutableArray *result = [NSMutableArray array];
    
    
    LLRunnerGestureRecognizer *runRecognizer = [[LLRunnerGestureRecognizer alloc] initWithTarget:self
                                                                                          action:@selector(tapDetected:)];
    runRecognizer.runnerDelegate = self;
    [result addObject:runRecognizer];
    
    
    if (self.isScreenEdgeAvailable)
    {
        UIScreenEdgePanGestureRecognizer *edgeLeftRecognizer = [[UIScreenEdgePanGestureRecognizer alloc] initWithTarget:self
                                                                                                                 action:@selector(tapDetected:)];
        edgeLeftRecognizer.edges = UIRectEdgeLeft;
        edgeLeftRecognizer.delegate = self;
        [result addObject:edgeLeftRecognizer];
    }
    
    
    for (UIGestureRecognizer *curRecognizer in result)
    {
        curRecognizer.cancelsTouchesInView = NO;
        curRecognizer.delaysTouchesBegan = NO;
        curRecognizer.delaysTouchesEnded = NO;
    }
    
    return result;
}


- (instancetype)init
{
    self = [super init];
    if (self)
    {
        _attachedRecognizers = self.requiredRecognizers;
        _attachedView = [UIApplication sharedApplication].keyWindow.rootViewController.view;
        
        _swipeHistory = [[NSMutableArray alloc] init];
        
        [self attachRecognizers];
    }
    
    return self;
}


-(void)dealloc
{
    [self deattachRecognizers];
}


- (void)attachRecognizers
{
    NSMutableArray *newRecognizers = [NSMutableArray arrayWithArray:_attachedView.gestureRecognizers];
    
    for (UIGestureRecognizer *recognizer in _attachedRecognizers)
    {
        if (![newRecognizers containsObject:recognizer])
        {
            [newRecognizers addObject:recognizer];
        }
    }
    
    _attachedView.gestureRecognizers = newRecognizers;
}


- (void)deattachRecognizers
{
    NSMutableArray *newRecognizers = [NSMutableArray arrayWithArray:_attachedView.gestureRecognizers];
    
    for (UIGestureRecognizer *recognizer in _attachedRecognizers)
    {
        if ([newRecognizers containsObject:recognizer])
        {
            [newRecognizers removeObject:recognizer];
        }
    }
    
    _attachedView.gestureRecognizers = newRecognizers;
}


- (void)updateRecognizer
{
    if (_attachedView != [UIApplication sharedApplication].keyWindow.rootViewController.view)
    {
        [self deattachRecognizers];
        
        _attachedView = [UIApplication sharedApplication].keyWindow.rootViewController.view;
        
        [self attachRecognizers];
    }
}


- (NSString *)popHistory
{
    if (_swipeHistory.count > 0)
    {
        NSMutableString *result = [NSMutableString string];
        
        [_swipeHistory enumerateObjectsUsingBlock:^(NSString *curName, NSUInteger idx, BOOL *stop)
         {
             if (idx > 0)
             {
                 [result appendString:@"|"];
             }
             
             [result appendString:curName];
         }];
        
        [_swipeHistory removeAllObjects];
        
        return result;
    }
    else
    {
        return nil;
    }
}



#pragma mark - Feedback UIGestureRecognizer

- (void)tapDetected:(UIGestureRecognizer *)gesture
{
    //    NSLog(@"Recognizer succeed feedback from view");
}



#pragma mark - Protocol LLRunnerGestureRecognizerDelegate

- (void)runnerGestureRecognizerDidSwipeDown:(LLRunnerGestureRecognizer *)recognizer
{
    [_swipeHistory addObject:@"down"];
}


- (void)runnerGestureRecognizerDidSwipeUp:(LLRunnerGestureRecognizer *)recognizer
{
    [_swipeHistory addObject:@"up"];
}


- (void)runnerGestureRecognizerDidSwipeLeft:(LLRunnerGestureRecognizer *)recognizer
{
    [_swipeHistory addObject:@"left"];
}


- (void)runnerGestureRecognizerDidSwipeRight:(LLRunnerGestureRecognizer *)recognizer
{
    [_swipeHistory addObject:@"right"];
}


- (void)runnerGestureRecognizerDidTap:(LLRunnerGestureRecognizer *)recognizer
{
    [_swipeHistory addObject:@"tap"];
}



#pragma mark - Protocol UIGestureRecognizerDelegate

- (BOOL)gestureRecognizerShouldBegin:(UIGestureRecognizer *)gestureRecognizer
{
    if (self.isScreenEdgeAvailable)
    {
        if ([gestureRecognizer isKindOfClass:[UIScreenEdgePanGestureRecognizer class]])
        {
            [_swipeHistory addObject:@"edge_left"];
            return NO;
        }
    }
    
    return YES;
}


@end



#pragma mark - Unity Extern

FOUNDATION_EXTERN void LLSwipeHandlerUpdate()
{
    [[LLSwipeHandler sharedInstance] updateRecognizer];
}



FOUNDATION_EXTERN const char *LLSwipeHandlerPopHistory()
{
    return LLSwipeHandlerStringCopy([[LLSwipeHandler sharedInstance] popHistory]);
}


