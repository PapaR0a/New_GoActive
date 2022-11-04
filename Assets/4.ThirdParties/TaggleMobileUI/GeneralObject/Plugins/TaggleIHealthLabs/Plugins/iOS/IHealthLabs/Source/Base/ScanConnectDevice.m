//
//  ScanConnectDevice.m
//  ihealthlabs
//
//  Created by Pham Thanh Dat on 6/5/20.
//  Copyright © 2020 Pham Thanh Dat. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "ScanConnectDevice.h"
#import "PO3Device.h"
#import "KN550BTDevice.h"
#import <UnityFramework/IHealthLabs-Bridging-Header.h>

@implementation ScanConnectDevice

HealthDeviceType SDKDeviceType;
NSString* deviceSelectName;
NSString* deviceMac;
PO3Device* pop3Device;
KN550BTDevice* kn550btDevice;



+ (void) scan: (NSString *)deviceType{
    NSLog(@"IHealthAuthentication ScanConnectDevice running with device type: %@ ......",deviceType);
    [self initDeviceList: deviceType];
    NSLog(@"IHealthAuthentication ScanConnectDevice scaning ......");
    [[ScanDeviceController commandGetInstance] commandScanDeviceType: SDKDeviceType];
}

+ (void) stopscan{
    NSLog(@"IHealthAuthentication ScanConnectDevice running stop scaning ......");
    //[self initDeviceList];
    NSLog(@"IHealthAuthentication ScanConnectDevice stop scaning ......");
    [[ScanDeviceController commandGetInstance] commandStopScanDeviceType: SDKDeviceType];
    [self deviceStopScan];
}

+ (void) connect{
     NSLog(@"IHealthAuthentication ConnectDevice running ......");
    [[ConnectDeviceController commandGetInstance] commandContectDeviceWithDeviceType: SDKDeviceType andSerialNub: deviceMac];
}

+ (void) disconnect{
     NSLog(@"IHealthAuthentication DisconnectDevice running ......");
	[pop3Device disconnect];
}

+ (void) initDeviceList: (NSString *)deviceType{
    NSLog(@"IHealthAuthentication initDeviceList running with device type: %@ ......",deviceType);
    [ScanDeviceController commandGetInstance];
    
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    
    deviceSelectName = [NSString string];
    deviceMac = [NSString string];
    
    if([deviceType isEqualToString: @"PO3"])
    {
        [self po3Init];
    }
    else if([deviceType isEqualToString: @"KN550BT"] || [deviceType isEqualToString: @"KN-550BT"] || [deviceType isEqualToString: @"BP550BT"])
    {
        [self kn550btInit];
    }

    
    NSLog(@"IHealthAuthentication ScanConnectDevice initDeviceList ......");
        
}

+ (void) po3Init{
    deviceSelectName =@"PO3/PO3M";
    SDKDeviceType = HealthDeviceType_PO3;
            
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(deviceConnect:) name:PO3ConnectNoti object:nil];
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(deviceDisConnect:) name:PO3DisConnectNoti object:nil];
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(deviceDiscover:) name:PO3Discover object:nil];
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(deviceConnectFail:) name:PO3ConnectFailed object:nil];
            
    [PO3Controller shareIHPO3Controller];
}

+ (void) kn550btInit{
    deviceSelectName =@"KN550BT";
    SDKDeviceType = HealthDeviceType_KN550BT;
            
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(deviceConnect:) name:KN550BTConnectNoti object:nil];
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(deviceDisConnect:) name:KN550BTDisConnectNoti object:nil];
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(deviceDiscover:) name:KN550BTDiscover object:nil];
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(deviceConnectFail:) name:KN550BTConnectFailed object:nil];
     
    [KN550BTController shareKN550BTController];
}

+(void) deviceStopScan{ //}:(NSNotification *)tempNoti{
   
    // send message to unity
    NSString *json = [NSString stringWithFormat:@"%d", 1];
    const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
    UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "DeviceStopScanCallback", messageStr);
    
}

+(void) deviceDiscover:(NSNotification *)tempNoti{
    NSLog(@"IHealthAuthentication ScanConnectDevice deviceDiscover ......");
    
    NSDictionary *dic = [tempNoti userInfo];
    
//    if ([[tempNoti name] isEqualToString:PO3Discover])
//    {
//        [[ConnectDeviceController commandGetInstance] commandContectDeviceWithDeviceType: SDKDeviceType andSerialNub:dic[@"ID"]];
//        deviceMac = [NSString stringWithFormat:@"%@",[dic objectForKey:@"SerialNumber"]];
//        NSLog(@"IHealthAuthentication ScanConnectDevice deviceMac  isEqualToString...... : %@", deviceMac);
//        return;
//    }
    
    if (dic[@"ID"]!=nil) {
        [[ConnectDeviceController commandGetInstance] commandContectDeviceWithDeviceType: SDKDeviceType andSerialNub:dic[@"ID"]];
        return;
    }

    deviceMac = [NSString stringWithFormat:@"%@",[dic objectForKey:@"SerialNumber"]];
    NSLog(@"IHealthAuthentication ScanConnectDevice deviceMac ...... : %@", deviceMac);
    
    // send message to unity
    NSString *json = [NSString stringWithFormat:@"{ \"result_code\": %d, \"callback_name\": \"%@\", \"value\": { \"deviceMac\": \"%@\", \"deviceName\": \"%@\" }}", 1, @"DeviceScanCallback", deviceMac, deviceSelectName];
    const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
    UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "DeviceScanCallback", messageStr);
    
    
}
+(void) deviceConnectFail:(NSNotification *)tempNoti{
    
    [[ScanDeviceController commandGetInstance] commandScanDeviceType: SDKDeviceType];
    
    NSDictionary *dic = [tempNoti userInfo];
    NSString *deviceMacStr = [dic objectForKey:@"SerialNumber"];
    NSLog(@" deviceMacStr ......%@", deviceMacStr);
    
    // send message to unity
    NSString *json = [NSString stringWithFormat:@"{ \"result_code\": %d, \"callback_name\": \"%@\", \"value\": { \"deviceMac\": \"%@\", \"status\": \"%@\", \"deviceName\": \"%@\" }}", 0, @"ConnectCallback", deviceMacStr, @"DEVICE_STATE_CONNECTION_FAIL", deviceSelectName];
    const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
    UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "ConnectCallback", messageStr);
}
+(void) deviceConnect:(NSNotification *)tempNoti{
    NSLog(@"IHealthAuthentication ScanConnectDevice deviceConnect ......");
    NSDictionary *dic = [tempNoti userInfo];
    
    NSString *deviceMacStr = [dic objectForKey:@"SerialNumber"];
    NSLog(@" deviceMacStr ......%@", deviceMacStr);
    NSUserDefaults *userDefault = [NSUserDefaults standardUserDefaults];
    
    [userDefault setObject: deviceMac forKey:@"SelectDeviceMac"];
   
    [userDefault synchronize];
    
    switch (SDKDeviceType) {
        case HealthDeviceType_PO3:
        {
            NSLog(@"IHealthAuthentication PO3Device measure ......");
            pop3Device = [[PO3Device alloc]init] ;
            [pop3Device initPO3];
            [pop3Device measure];
            //[[[PO3Device alloc]init] initPO3 measure];
            
            // send message to unity
            NSString *json = [NSString stringWithFormat:@"{ \"result_code\": %d, \"callback_name\": \"%@\", \"value\": { \"deviceMac\": \"%@\", \"status\": \"%@\", \"deviceName\": \"%@\" }}", 1, @"ConnectCallback", deviceMacStr, @"DEVICE_STATE_CONNECTED", deviceSelectName];
            const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
            UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "ConnectCallback", messageStr);
            break;
        }
        case HealthDeviceType_KN550BT:
        {
                 NSLog(@"IHealthAuthentication KN550BT3Device get offline data ......");
                kn550btDevice = [[KN550BTDevice alloc]init] ;
                [kn550btDevice initKN550BT];
                [kn550btDevice getOfflineData];

                // send message to unity
                NSString *json = [NSString stringWithFormat:@"{ \"result_code\": %d, \"callback_name\": \"%@\", \"value\": { \"deviceMac\": \"%@\", \"status\": \"%@\", \"deviceName\": \"%@\" }}", 1, @"ConnectCallback", deviceMacStr, @"DEVICE_STATE_CONNECTED", deviceSelectName];
                const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
                UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "ConnectCallback", messageStr);
            break;
        }
        default:
            break;
    }

}
+(void) deviceDisConnect:(NSNotification *)tempNoti{
     
    NSDictionary *dic = [tempNoti userInfo];
    
    deviceMac = [NSString stringWithFormat:@"%@",[dic objectForKey:@"SerialNumber"]];
    
//    deviceMac = @"";
    
    [[ScanDeviceController commandGetInstance] commandScanDeviceType: SDKDeviceType];
    if([deviceSelectName isEqualToString: @"PO3"])
    {
//        [[[PO3Device alloc]init] disconnect];
        [pop3Device disconnect];
    }
    else if([deviceSelectName isEqualToString: @"KN550BT"] || [deviceSelectName isEqualToString: @"KN_550BT"] || [deviceSelectName isEqualToString: @"BP550BT"])
    {
//        [[[KN550BTDevice alloc]init] disconnect];
        [kn550btDevice disconnect];
    }
    
    // send message to unity
    NSString *json = [NSString stringWithFormat:@"{ \"result_code\": %d, \"callback_name\": \"%@\", \"value\": { \"deviceMac\": \"%@\", \"status\": \"%@\", \"deviceName\": \"%@\" }}", 1, @"DisconnectCallback", deviceMac, @"DEVICE_STATE_DISCONNECTED", deviceSelectName];
    const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
    UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "DisconnectCallback", messageStr);
}
@end

