//
//  Asn1Parser.h
//

#import <Foundation/Foundation.h>

#if __has_feature(objc_arc_weak)

// zeroing weak refs are supported for ivars and properties
#define WEAK_VARIABLE __weak
#define WEAK_PROPERTY weak

#elif __has_feature(objc_arc)

/// zeroing weak refs not supported, fall back to unsafe unretained and assigning
#define WEAK_VARIABLE __unsafe_unretained
#define WEAK_PROPERTY assign

#else

// define something, as this header might be included in a non-ARC project for using compiled code from an ARC static lib
#define WEAK_VARIABLE
#define WEAK_PROPERTY assign

#endif

/**
 Types of ASN1 tags, specifying the type of the following value in TLV notation
 */
typedef NS_ENUM(NSUInteger, LLASN1Type)
{
	//ASN1 type for EOC
	Asn1TypeEoc = 0x00,
  
	//ASN1 type for Boolean
	Asn1TypeBoolean = 0x01,
	
	//ASN1 type for integer numbers
	Asn1TypeInteger = 0x02,
	
	//ASN1 type for Bit Strings
    Asn1TypeBitString = 0x03,
	
    //ASN1 type for Octet Strings
	Asn1TypeOctetString = 0x04,
	
	//ASN1 type for NULL values
	Asn1TypeNull = 0x05,
	
	//ASN1 type for object identifiers
	Asn1TypeObjectIdentifier = 0x06,
	
	//ASN1 type for object descriptors
	Asn1TypeObjectDescriptor = 0x07,
	
	//ASN1 type for external references
	Asn1TypeExternal = 0x08,
	
	//ASN1 type for floating point numbers
	Asn1TypeReal= 0x09,
	
	//ASN1 type for enumerated values
	Asn1TypeEnumerated = 0x0a,
	
	//ASN1 type for embedded PDV values
	Asn1TypeEmbeddedPdv = 0x0b,
	
	//ASN1 type for UTF8 strings
	Asn1TypeUtf8String = 0x0c,
	
	//ASN1 type for sequences
	Asn1TypeSequence = 0x10,
	
	//ASN1 type for sets
	Asn1TypeSet = 0x11,
	
	//ASN1 type for numeric strings
	Asn1TypeNumericString = 0x12,
	
	//ASN1 type for printable strings
	Asn1TypePrintableString = 0x13,
	
	//ASN1 type for teletex strings
	Asn1TypeTeletexString = 0x14,
	
	//ASN1 type for video text strings
	Asn1TypeVideoTextString = 0x15,
	
	//ASN1 type for IA5 strings
	Asn1TypeIA5String = 0x16,
	
	//ASN1 type for UTC times
	Asn1TypeUtcTime = 0x17,
	
	//ASN1 type for generalized times
	Asn1TypeGeneralizedTime = 0x18,
	
	//ASN1 type for generalized time values
	Asn1TypeGraphicString = 0x19,

	//ASN1 type for visible strings
	Asn1TypeVisibleString = 0x1a,
	
	//ASN1 type for general strings
	Asn1TypeGeneralString = 0x1b,
	
    //ASN1 type for universal strings
	Asn1TypeUniversalString = 0x1c,
	
    //ASN1 type for bitmap strings
	Asn1TypeBitmapString = 0x1e,
	
	//ASN1 value to signify that the value uses the long form
	Asn1TypeUsesLongForm = 0x1f
};

@class Asn1Parser, Asn1BitString;

/** The DTASN1ParserDelegate protocol defines the optional methods implemented by delegates of DTASN1Parser objects. 
 */
@protocol Asn1ParserDelegate <NSObject>

@optional

/**
 Sent by the parser object to the delegate when it begins parsing a document.
 
 @param parser A parser object.
 */
- (void)parserDidStartDocument:(Asn1Parser *)parser;

/**
 Sent by the parser object to the delegate when it has successfully completed parsing
 
 @param parser A parser object.
 */
- (void)parserDidEndDocument:(Asn1Parser *)parser;

/**
 Sent by a parser object to its delegate when it encounters the beginning of a constructed element.
 
 @param parser A parser object.
 @param type The tag type that contains the subsequent elements.
 */
- (void)parser:(Asn1Parser *)parser didStartContainerWithType:(LLASN1Type)type;

/**
 Sent by a parser object to its delegate when it encounters the end of a constructed element.
 
 @param parser A parser object.
 @param type A string that is the name of an element (in its end tag).
 */
- (void)parser:(Asn1Parser *)parser didEndContainerWithType:(LLASN1Type)type;

/**
 Sent by a parser object to its delegate when it encounters the beginning of a context-specific tag.
 
 @param parser A parser object.
 @param tag The tag value for the context that contains the subsequent elements.
 */
- (void)parser:(Asn1Parser *)parser didStartContextWithTag:(NSUInteger)tag;

/**
 Sent by a parser object to its delegate when it encounters the end of a constructed element.
 
 @param parser A parser object.
 @param tag The tag value for the context that contained the previous elements.
 */
- (void)parser:(Asn1Parser *)parser didEndContextWithTag:(NSUInteger)tag;

/**
 Sent by a parser object to its delegate when it encounters a fatal error.
 
 When this method is invoked, parsing is stopped. For further information about the error, you can query parseError or you can send the parser a parserError message. You can also send the parser lineNumber and columnNumber messages to further isolate where the error occurred. Typically you implement this method to display information about the error to the user.
 
 @param parser A parser object.
 @param parseError An `NSError` object describing the parsing error that occurred.
 */
- (void)parser:(Asn1Parser *)parser parseErrorOccurred:(NSError *)parseError;

/**
 Sent by a parser object when a NULL element is encountered.
 
 @param parser A parser object.
 */
- (void)parserFoundNull:(Asn1Parser *)parser;

/**
 Sent by a parser object to provide its delegate with the date encoded in the current element.
 
 All the ASN1 date types are provided via this method.
 
 @param parser A parser object.
 @param date A date representing the date encoded in the current element.
 */
- (void)parser:(Asn1Parser *)parser foundDate:(NSDate *)date;

/**
 Sent by a parser object to provide its delegate with the object identifier encoded in the current element.
 
 @param parser A parser object.
 @param objIdentifier A string representing the object identifier encoded in the current element.
 */
- (void)parser:(Asn1Parser *)parser foundObjectIdentifier:(NSString *)objIdentifier;

/**
 Sent by a parser object to provide its delegate with the string encoded in the current element.
 
 All the ASN1 string types are provided via this method.
 
 @param parser A parser object.
 @param string A string contained in the current element.
 */
- (void)parser:(Asn1Parser *)parser foundString:(NSString *)string;

/**
 Sent by a parser object to provide its delegate with the octet string encoded in the current element.
 
 Integer data that is longer than 32 bits is also provided this way.
 
 @param parser A parser object.
 @param data A data object representing the contents of the current element.
 */
- (void)parser:(Asn1Parser *)parser foundData:(NSData *)data;

/**
 Sent by a parser object to provide its delegate with the bit string encoded in the current element.
 
 @param parser A parser object.
 @param bitString A bit string object representing the contents of the current element.
 */
- (void)parser:(Asn1Parser *)parser foundBitString:(Asn1BitString *)bitString;

/**
 Sent by a parser object to provide its delegate with number values encoded in the current element.
 
 Note that number values that are longer than supported by the system are provided as Data instead.
 
 @param parser A parser object.
 @param number A number object representing the contents of the current element.
 */
- (void)parser:(Asn1Parser *)parser foundNumber:(NSNumber *)number;
@end

/** Instances of this class parse ASN1 documents in an event-driven manner. A DTASN1Parser notifies its delegate about the items (elements, collections, and so on) that it encounters as it processes an ASN1 document. It does not itself do anything with those parsed items except report them. It also reports parsing errors. For convenience, a DTASN1Parser object in the following descriptions is sometimes referred to as a parser object.
 */
@interface Asn1Parser : NSObject

/**-------------------------------------------------------------------------------------
 @name Initializing a Parser Object
 ---------------------------------------------------------------------------------------
 */

/**
 Initializes the receiver with the ASN1 contents encapsulated in a given data object.
 
 @param data An `NSData` object containing ASN1 encoded data.
 @returns An initialized `DTASN1Parser` object or nil if an error occurs. 
 */
- (id)initWithData:(NSData *)data;

/**-------------------------------------------------------------------------------------
 @name Parsing
 ---------------------------------------------------------------------------------------
 */

/**
 Starts the event-driven parsing operation.
 
 If you invoke this method, the delegate, if it implements parser:parseErrorOccurred:, is informed of the cancelled parsing operation.
 
 @returns `YES` if parsing is successful and `NO` in there is an error or if the parsing operation is aborted. 
 */
- (BOOL)parse;


/**
 Stops the parser object.
 
 @see parse
 @see parserError
 */
- (void)abortParsing;

/**
 An object that is the parsing delegate. It is not retained. The delegate must conform to the DTASN1ParserDelegate Protocol protocol.
 */
@property (nonatomic, WEAK_PROPERTY) id <Asn1ParserDelegate>delegate;


/**
 Returns an `NSError` object from which you can obtain information about a parsing error.
 
 You may invoke this method after a parsing operation abnormally terminates to determine the cause of error.
 */
@property (nonatomic, readonly, strong) NSError *parserError;

@end
