#import <Foundation/Foundation.h>
#include <UnityFramework/UnityFramework-Swift.h>
#include <UnityFramework/IHealthLabs-Bridging-Header.h>

#pragma mark - C interface

// Converts C style string to NSString
NSString* CreateNSStringiHealth (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

extern "C" {
    void _authenticationIHealthLabs() {
        [IHealthLabsSwift Authentication];
    }

    void _scanDeviceIHealthLabs(const char* mdeviceType) {
        NSString *deviceType = CreateNSStringiHealth(mdeviceType);
        [IHealthLabsSwift ScanDevice: deviceType];
    }

    void _connectDeviceIHealthLabs() {
        [IHealthLabsSwift ConnectDevice];
    }
    void _disconnectDeviceIHealthLabs() {
        [IHealthLabsSwift DisconnectDevice];
    }
    void _stopScanDeviceIHealthLabs() {
        [IHealthLabsSwift StopScanDevice];
    }
}
