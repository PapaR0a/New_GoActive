//
//  PO3Device.m
//

#import <UIKit/UIKit.h>
#import "PO3Device.h"
#import <UnityFramework/IHealthLabs-Bridging-Header.h>

@implementation PO3Device
{
    PO3 *myPO3;
    NSString *deviceMac;
}

- (void) initPO3{
    NSLog(@"init po3");
    NSUserDefaults *userDefault = [NSUserDefaults standardUserDefaults];
    
    deviceMac = [userDefault valueForKey:@"SelectDeviceMac"];
    
    NSArray *deviceArray = [[PO3Controller shareIHPO3Controller] getAllCurrentPO3Instace];
    
    for(PO3 *device in deviceArray){
        NSLog(@"PO3 device mac: %@",device.serialNumber);
        if([deviceMac isEqualToString:device.serialNumber]){
            NSLog(@"finded PO3 device mac: %@",device.serialNumber);
            myPO3 = device;
            
            [self measure];
        }
    }
}

- (void) disconnect {
    
    [myPO3 commandPO3Disconnect:^(BOOL resetSuc) {
        
    } withErrorBlock:^(PO3ErrorID errorID) {
        
        [self po3Error:errorID];
    }];
    
}

- (void) measure {
    NSLog(@"PO3 measure: ...........");
    [myPO3 commandPO3StartMeasure:^(BOOL resetSuc) {
        NSLog(@" StartMeasure...........");

    } withMeasureData:^(NSDictionary *measureDataDic) {
        NSLog(@"PO3 Data: %@", measureDataDic);
        //Android data
        //{"bloodoxygen":99,"heartrate":73,"pulsestrength":0,"pi":4.300000190734863,"pulseWave":[0,0,0],"dataID":"4C2E15BE0378A2048686596914F3708F"}
        //ios data
        //{ PI = "10.06786";  bpm = 66; dataID = 60a03a753dab99a12864022895bb9143;height = 0;spo2 = 98;  wave = (0,0,429); }
        NSString *json = [NSString stringWithFormat:@"{ \"result_code\": %d, \"callback_name\": \"%@\", \"value\": {\"oxygen_saturation\":%@,\"heart_rate\":%@,\"pi\":%@,\"pulse_wave\":\"%@\",\"data_id\":\"%@\",\"udid\":\"%@\" }}", 1, @"SendDataCallback", [measureDataDic objectForKey:@"spo2"], [measureDataDic objectForKey:@"bpm"], [measureDataDic objectForKey:@"PI"], [measureDataDic objectForKey:@"wave"], [measureDataDic objectForKey:@"dataID"], deviceMac];
        const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
        UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "SendDataCallback", messageStr);
        
    } withFinishMeasure:^(BOOL finishData) {
        NSLog(@"PO3 Finish Data: %@", @"FinishMeasure");
        
        NSString *json = [NSString stringWithFormat:@"{ \"result_code\": %d, \"callback_name\": \"%@\", \"value\": %@}", 1, @"SendDataCallback",@"FinishMeasure"];
        const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
        UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "SendDataCallback", messageStr);
        
    } withErrorBlock:^(PO3ErrorID errorID) {
        NSLog(@"PO3 errorID Data: %lu", (unsigned long)errorID);
         NSString *json = [NSString stringWithFormat:@"{ \"result_code\": %d, \"callback_name\": \"%@\", \"value\": %@}", 0, @"SendDataCallback",@"ErrorMeasure"];
                const char *messageStr = [json cStringUsingEncoding:NSUTF8StringEncoding];
                UnitySendMessage("HAGOSmartBluetoothManager/TaggleIHealthLabsManager", "SendDataCallback", messageStr);
        [self po3Error:errorID];
    }];
}

- (void) getOfflineData {
    
    [myPO3 commandPO3OfflineDataCount:^(NSNumber *dataCount) {
        
    } withOfflineData:^(NSDictionary *OfflineData) {
        
    } withOfflineWaveData:^(NSDictionary *offlineWaveDataDic) {
        
    } withFinishMeasure:^(BOOL resetSuc) {
        
    } withErrorBlock:^(PO3ErrorID errorID) {
        [self po3Error:errorID];
    }];
}

-(void) updateDeviceError:(UpdateDeviceError)errorID{
    NSString *error = [NSString string];
    switch (errorID) {
        case UpdateNetworkError:
            error = @"UpdateNetworkError";
            break;
        case UpdateOrderError:
            error = @"UpdateOrderError";
            break;
        case UpdateDeviceDisconnect:
            error = @"UpdateDeviceDisconnect";
            break;
        case UpdateDeviceEnd:
            error = @"UpdateDeviceEnd";
            break;
        case UpdateInputError:
            error = @"UpdateInputError";
            break;
            case NOUpdateUpgrade:
            error = @"NOUpdateUpgrade";
            break;
        default:
            break;
    }
    NSLog(@"PO3 update device Error: %@", error );
    
}


-(void) po3Error:(PO3ErrorID)errorID{
    NSString *error = [NSString string];
    switch (errorID) {
        case PO3Error_OverTime:
             error = @"PO3Error_OverTime";
            break;
        case PO3Error_ResetDeviceFaild:
            error = @"PO3Error_ResetDeviceFaild";
            break;
        case PO3Error_Disconnect:
            error = @"PO3Error_Disconnect";
            break;
        case PO3Error_ParameterError:
            error = @"PO3Error_ParameterError";
            break;
        case PO3Error_FirmwareVersionIsNotSupported:
            error = @"PO3Error_FirmwareVersionIsNotSupported";
            break;
            
        default:
            break;
    }
    NSLog(@"PO3 Error: %@", error );
    
}
@end
