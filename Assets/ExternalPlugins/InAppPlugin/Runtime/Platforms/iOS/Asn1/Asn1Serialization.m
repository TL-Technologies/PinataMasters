//
//  Asn1Serialization.m
//

#import "Asn1Serialization.h"
#import "Asn1Parser.h"

@interface Asn1Serialization() <Asn1ParserDelegate>

@property (nonatomic, readonly) id rootObject;

- (id)initWithData:(NSData *)data;

@end

@implementation Asn1Serialization
{
    id _rootObject;
	id _currentContainer;
	NSMutableArray *_stack;
}

@synthesize rootObject = _rootObject;


+ (nullable id)objectWithData:(nonnull NSData *)data
{
	Asn1Serialization *decoder = [[Asn1Serialization alloc] initWithData:data];
	
	return decoder.rootObject;
}


// private initializer
- (nullable id)initWithData:(nonnull NSData *)data
{
	self = [super init];
	if (self)
	{
        Asn1Parser *parser = [[Asn1Parser alloc] initWithData:data];
		parser.delegate = self;
		
		if (![parser parse])
		{
			return nil;
		}
	}
    
	return self;
}

- (void)_pushContainer:(id)container
{
    if (!_stack)
	{
		_stack = [NSMutableArray array];
		_rootObject = container;
	}
	
	[_currentContainer addObject:container];
	
	[_stack addObject:container];
	_currentContainer = container;
}

- (void)_addObjectToCurrentContainer:(id)object
{
	if (!_stack)
	{
		_stack = [NSMutableArray array];
		_rootObject = object;
	}
	
	[_currentContainer addObject:object];
}

- (void)_popContainer
{
	[_stack removeLastObject];
	_currentContainer = [_stack lastObject];
}

#pragma mark - DTASN1 Parser Delegate

- (void)parser:(Asn1Parser *)parser didStartContainerWithType:(LLASN1Type)type
{
	NSMutableArray *newContainer = [NSMutableArray array];
    [self _pushContainer:newContainer];
}

- (void)parser:(Asn1Parser *)parser didEndContainerWithType:(LLASN1Type)type
{
	[self _popContainer];
}

- (void)parser:(Asn1Parser *)parser didStartContextWithTag:(NSUInteger)tag constructed:(BOOL)constructed
{
	NSNumber *tagNumber = [NSNumber numberWithUnsignedInteger:tag];
	
	NSMutableArray *newContainer = [NSMutableArray array];
	NSDictionary *dictionary = [NSDictionary dictionaryWithObject:newContainer forKey:tagNumber];
	
	[self _pushContainer:dictionary];
	_currentContainer = newContainer;
}

- (void)parser:(Asn1Parser *)parser didEndContextWithTag:(NSUInteger)tag constructed:(BOOL)constructed
{
	[self _popContainer];
}

- (void)parserFoundNull:(Asn1Parser *)parser
{
	[self _addObjectToCurrentContainer:[NSNull null]];
}

- (void)parser:(Asn1Parser *)parser foundDate:(NSDate *)date
{
	[self _addObjectToCurrentContainer:date];
}

- (void)parser:(Asn1Parser *)parser foundObjectIdentifier:(NSString *)objIdentifier
{
	[self _addObjectToCurrentContainer:objIdentifier];
}

- (void)parser:(Asn1Parser *)parser foundString:(NSString *)string
{
	[self _addObjectToCurrentContainer:string];
}

- (void)parser:(Asn1Parser *)parser foundData:(NSData *)data
{
	[self _addObjectToCurrentContainer:data];
}

- (void)parser:(Asn1Parser *)parser foundBitString:(Asn1BitString *)bitString
{
	[self _addObjectToCurrentContainer:bitString];
}

- (void)parser:(Asn1Parser *)parser foundNumber:(NSNumber *)number
{
	[self _addObjectToCurrentContainer:number];
}

@end


