//
//  IHealthLabsConstant.swift
//  IHealthLabs
//
//  Created by Pham Thanh Dat on 6/3/20.
//  Copyright © 2020 Pham Thanh Dat. All rights reserved.
//

import Foundation

public class IHealthConstant
{
    static let IHEALTH_LABS_MANAGER_OBJECT = "HAGOSmartBluetoothManager/TaggleIHealthLabsManager"
    
    static let TYPE_AUTHENTICATION = "AUTHENTICATION"

    
    static let AUTHENTICATION_CALLBACK = "AuthenticationCallback"

    
    static let MAP_CALLBACK =
    [
        TYPE_AUTHENTICATION: AUTHENTICATION_CALLBACK,
//        TYPE_JAILBROKEN: JAILBROKEN_CALLBACK,
//        TYPE_SIMULATOR: SIMULATOR_CALLBACK,
//        TYPE_REVERSE_ENGINEERING: REVERSE_ENGINEERING_CALLBACK
    ]
}
