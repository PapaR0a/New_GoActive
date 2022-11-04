//
//  ScanConnectDevice.h
//  IHealthLabs
//
//  Created by Pham Thanh Dat on 6/5/20.
//  Copyright © 2020 Pham Thanh Dat. All rights reserved.
//

#ifndef ScanConnectDevice_h
#define ScanConnectDevice_h

@interface ScanConnectDevice : NSObject
{

}

+ (void) scan:(NSString *)deviceType;
+ (void) stopscan;
+ (void) connect;
+ (void) disconnect;

@end

#endif /* ScanConnectDevice_h */

