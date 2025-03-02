//
//  LLCountryCodeRegister.m
//  Unity-iPhone
//
//  Created by Valera Shostak on 03/03/2018.
//
//

#import "LLCountryCodeRegister.h"


#pragma mark - Unity -

const char *LLCountryCodeRegisterLocalPlayerCountryCode()
{
    return [[[NSLocale currentLocale] objectForKey:NSLocaleCountryCode] UTF8String];
}
