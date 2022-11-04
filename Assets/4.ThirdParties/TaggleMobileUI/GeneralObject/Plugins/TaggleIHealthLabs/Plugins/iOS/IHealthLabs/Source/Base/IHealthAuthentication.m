//
//  IHealthAuthentication.m
//  IHealthLabs
//
//  Created by Pham Thanh Dat on 6/3/20.
//  Copyright © 2020 Pham Thanh Dat. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "IHealthAuthentication.h"
#import <UnityFramework/IHealthLabs-Bridging-Header.h>

#pragma mark - C interface

// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

@implementation IHealthAuthentication

+ (void) authentication
{
        NSLog(@"IHealthAuthentication authentication running ......");
        NSString *filePath = [[NSBundle mainBundle] pathForResource:@"com_taggle_healthapp_ios" ofType:@"pem"];
        NSLog(@"IHealthAuthentication authentication file path: %@", filePath);
        NSData*data = [NSData dataWithContentsOfFile:filePath];

        [[IHSDKCloudUser commandGetSDKUserInstance] commandSDKUserValidationWithLicense:data UserDeviceAccess:^(NSArray *DeviceAccessArray)
        {

            NSLog(@"DeviceAccessArray :%@",DeviceAccessArray);
           
        } UserValidationSuccess:^(UserAuthenResult result) {

            //[IHealthLabsUtilities SendMessage( 1, IHealthLabsConstant.MAP_CALLBACK objectForKey:IHealthLabsConstant.TYPE_AUTHENTICATION)];
            NSString*failMessage=[NSString string];
            switch (result) {
                case UserAuthen_AppSecretVerifyFailed:
                    failMessage = @"UserAuthen_AppSecretVerifyFailed";
                    UnitySendMessage("GeneralObject/TaggleIHealthLabsManager", "AuthenticationCallback", "{result_code:0}");
                    break;
                case UserAuthen_AppSecretVerifySuccess:
                     failMessage = @"UserAuthen_AppSecretVerifySuccess";
                    break;
                case UserAuthen_CertificateExpired:
                     failMessage = @"UserAuthen_CertificateExpired";
                    UnitySendMessage("GeneralObject/TaggleIHealthLabsManager", "AuthenticationCallback", "{result_code:0}");
                    break;
                case UserAuthen_CombinedSuccess:
                     failMessage = @"UserAuthen_CombinedSuccess";
                    break;
                case UserAuthen_InputError:
                     failMessage = @"UserAuthen_InputError";
                    UnitySendMessage("GeneralObject/TaggleIHealthLabsManager", "AuthenticationCallback", "{result_code:0}");
                    break;
                case UserAuthen_InternetError:
                     failMessage = @"UserAuthen_InternetError";
                    UnitySendMessage("GeneralObject/TaggleIHealthLabsManager", "AuthenticationCallback", "{result_code:0}");
                    break;
                case UserAuthen_InvalidateUserInfo:
                     failMessage = @"UserAuthen_InvalidateUserInfo";
                    UnitySendMessage("GeneralObject/TaggleIHealthLabsManager", "AuthenticationCallback", "{result_code:0}");
                    break;
                case UserAuthen_InvalidCertificate:
                     failMessage = @"UserAuthen_InvalidCertificate";
                    UnitySendMessage("GeneralObject/TaggleIHealthLabsManager", "AuthenticationCallback", "{result_code:0}");
                    break;
                case UserAuthen_LoginSuccess:
                     failMessage = @"UserAuthen_LoginSuccess";
                     UnitySendMessage("GeneralObject/TaggleIHealthLabsManager", "AuthenticationCallback", "{result_code:1}");
                    break;
                case UserAuthen_RegisterSuccess:
                     failMessage = @"UserAuthen_RegisterSuccess";
                    break;
                case UserAuthen_SDKInvalidateRight:
                     failMessage = @"UserAuthen_SDKInvalidateRight";
                    break;
                case UserAuthen_TrySuccess:
                     failMessage = @"UserAuthen_TrySuccess";
                    break;
                case UserAuthen_UserInvalidateRight:
                     failMessage = @"UserAuthen_UserInvalidateRight";
                    UnitySendMessage("GeneralObject/TaggleIHealthLabsManager", "AuthenticationCallback", "{result_code:0}");
                    break;
                default:
                    failMessage = @"UserAuthenResult UserValidationSuccess Error";
                     UnitySendMessage("GeneralObject/TaggleIHealthLabsManager", "AuthenticationCallback", "{result_code:0}");
                    break;
            }
            NSLog(@"UserValidationSuccess :%@",failMessage);
//            UnitySendMessage("GeneralObject/TaggleIHealthLabsManager", "AuthenticationCallback", "{result_code:1}");
            
        } DisposeErrorBlock:^(UserAuthenResult errorID) {

            // Send error message to Unity
            NSString*failMessage=[NSString string];

            switch (errorID) {
                case UserAuthen_InputError:
                    failMessage=NSLocalizedString(@"Input error", @"Input error");
                    break;
                case UserAuthen_CertificateExpired:
                    failMessage=NSLocalizedString(@"Certificate expired", @"Certificate expired");
                    break;
                case UserAuthen_InvalidCertificate:
                    failMessage=NSLocalizedString(@"Invalid certificate", @"Invalid certificate");
                    break;
                default:
                    failMessage = @"UserAuthenResult DisposeErrorBlock Error";
                    break;
            }

            NSLog(@"Authentication result: %@ ",  failMessage);
        }];
}
        
              
    

@end
