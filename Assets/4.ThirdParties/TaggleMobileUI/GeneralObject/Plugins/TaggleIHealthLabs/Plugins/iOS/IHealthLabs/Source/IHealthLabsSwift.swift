//
//  IHealthLabs.swift
//  Unity-iPhone
//
//  Created by Pham Thanh Dat on 6/9/20.
//

import Foundation
import UIKit

@objc public class IHealthLabsSwift : NSObject
{
    @objc public static func Authentication() -> Void {
        IHealthAuthentication.authentication()
    }
    
    @objc public static func ScanDevice(_ deviceType : String) -> Void {
        ScanConnectDevice.scan(deviceType)
    }
    
    @objc public static func StopScanDevice() -> Void {
        ScanConnectDevice.stopscan()
    }
    
    @objc public static func ConnectDevice() -> Void{
        ScanConnectDevice.connect()
    }
	
    @objc public static func DisconnectDevice() -> Void{
        ScanConnectDevice.disconnect()
    }
	
}
