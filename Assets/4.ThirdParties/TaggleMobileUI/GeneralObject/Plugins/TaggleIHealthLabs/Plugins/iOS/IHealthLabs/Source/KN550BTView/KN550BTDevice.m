//
//  KN550BTDevice.m
//  Unity-iPhone
//
//  Created by Pham Thanh Dat on 7/31/20.
//

#import <Foundation/Foundation.h>
#import "KN550BTDevice.h"
#import <UnityFramework/IHealthLabs-Bridging-Header.h>

@implementation KN550BTDevice
{
    KN550BT *myKN550BT;
    NSString *deviceMac;
}


- (void) initKN550BT{
    NSLog(@"init KN550BT");
    NSUserDefaults *userDefault = [NSUserDefaults standardUserDefaults];
    
    deviceMac = [userDefault valueForKey:@"SelectDeviceMac"];
    
    NSArray*deviceArray=[[KN550BTController shareKN550BTController] getAllCurrentKN550BTInstace];
    
    for(KN550BT *device in deviceArray){
        NSLog(@"KN550BT device mac: %@",device.serialNumber);
        if([deviceMac isEqualToString:device.serialNumber]){
            NSLog(@"finded KN550BT device mac: %@",device.serialNumber);
            myKN550BT = device;
            [self getOfflineData];
        }
    }
}

- (void) disconnect {
    NSLog(@"myKN550BT disconnect ...");
    [myKN550BT commandDisconnectDevice];
}

- (void) getOfflineData {
    NSLog(@"myKN550BT get offline data: ...........");
    
    [myKN550BT commandTransferMemoryDataWithTotalCount:^(NSNumber *count) {
        NSLog(@"%@", [NSString stringWithFormat:@"%@:%@\n",NSLocalizedString(@"myKN550BT MemoryCount", @""),count]);
    } progress:^(NSNumber *progress) {
        NSLog(@"%@", [NSString stringWithFormat:@"%@:%@\n",NSLocalizedString(@"myKN550BT progress", @""),progress]);
    } dataArray:^(NSArray *array) {
        NSLog(@"%@", [NSString stringWithFormat:@"%@:%@\n",NSLocalizedString(@"myKN550BT MemoryData", @""),array]);
        for (NSDictionary *dataDic in array) {
            // android data
            // data: {"date": "2014-02-21 21:15:00","blood_pressure_s": 131,"blood_pressure_d": 78,"pulse_wave": 67,"ahr": false,"udid":"9C1D5817EC99"}
            // ios data
            // data: {chooseHand = 0;dataID = 8224a0cf67aa9936346b8df88e56a374;dia = 96;heartRate = 87;hsdValue = 0;irregular = 0;sys = 161;time = "2014-02-22 16:21:00 +0000"; }
            NSString *json = [NSString stringWithFormat:@"{ \"result_code\": %d, \"callback_name\": \"%@\", \"value\": {\"date\":\"%@\",\"blood_pressure_s\":%@,\"blood_pressure_d\":%@,\"pulse_wave\":%@,\"data_id\":\"%@\",\"udid\":\"%@\" }}", 1, @"SendDataCallback", [dataDic objectForKey:@"time"], [dataDic objectForKey:@"sys"], [dataDic objectForKey:@"dia"], [dataDic objectForKey:@"heartRate"], [dataDic objectForKey:@"dataID"], deviceMac];

                const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
                UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "SendDataCallback", messageStr);
        }
    } errorBlock:^(BPDeviceError error) {
                NSLog(@"KN550BT errorID Data: %lu", (unsigned long)error);
                NSString *json = [NSString stringWithFormat:@"{ \"result_code\": %d, \"callback_name\": \"%@\", \"value\": %@}", 0, @"SendDataCallback",@"ErrorMeasure"];
                const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
                UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "SendDataCallback", messageStr);
                [self kn550btErrorID:error];
    }];
}

-(void)kn550btErrorID:(BPDeviceError)errorID {
    NSString *error = [NSString string];
    switch (errorID) {
        case BPError0:
            error = @"Unable to take measurements due to arm/wrist movements.";
            break;
        case BPError1:
            error = @"Failed to detect systolic pressure";
            break;
        case BPError2:
            error = @"Failed to detect diastolic pressure";
            break;
        case BPError3:
            error = @"Pneumatic system blocked or cuff is too tight during inflation";
            break;
        case BPError4:
            error = @"Pneumatic system leakage or cuff is too loose during inflation";
            break;
        case BPError5:
            error = @"Cuff pressure reached over 300mmHg";
            break;
        case BPError6:
            error = @"Cuff pressure reached over 15 mmHg for more than 160 seconds";
            break;
        case BPError7:
        case BPError8:
        case BPError9:
        case BPError10:
            error = @"Data retrieving error";
            break;
        case BPError11:
        case BPError12:
            error = @"Communication Error";
            break;
        case BPError13:
            error = @"Low battery" ;
            break;
        case BPError14:
            error = @"Device bluetooth set failed" ;
            break;
        case BPError15:
            error = @" Systolic exceeds 260mmHg or diastolic exceeds 199mmHg" ;
            break;
        case BPError16:
            error = @"Systolic below 60mmHg or diastolic below 40mmHg" ;
            break;
        case BPError17:
            error = @"Arm/wrist movement beyond range" ;
            break;
        case BPError18:
        case BPError19:
            error = @"Heart rate in measure result exceeds max limit" ;
            break;
        case BPError20:
            error = @"PP(Average BP) exceeds limit" ;
            break;
        case BPErrorUserStopMeasure:
            error = @"User stop measure(for ABPM history measurement only)" ;
            break;
        case BPNormalError:
            error = @"device error, error message displayed automatically" ;
            break;
        case BPOverTimeError:
        case BPNoRespondError:
            error = @"Abnormal communication" ;
            break;
        case BPBeyondRangeError:
            error = @"device is out of communication range." ;
            break;
        case BPDidDisconnect:
            error = @"Device Disconnect" ;
            break;
        case BPAskToStopMeasure:
            error = @"measurement has been stopped." ;
            break;
        case BPDeviceBusy:
            error = @"device is busy doing other things" ;
            break;
        case BPInputParameterError:
        case BPInvalidOperation:
            error = @"Parameter input error." ;
            break;
        default:
            break;
    
        NSLog(@"KN550BT Error: %@", error );
    }
}
    


@end

