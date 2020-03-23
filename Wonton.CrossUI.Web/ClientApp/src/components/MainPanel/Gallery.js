import React, { Component } from 'react';
import { push as Menu } from 'react-burger-menu';
import { DeviceItem } from '../DeviceItem/DeviceItem';

import './Gallery.css';
import { deviceMap } from "../Devices/Devices";

export class Gallery extends Component {
    
    OnAdd = (event, name) => {
        this.props.OnAdd(event, name);
    }

    GenDevices = (deviceMap) => {
        let devs = []
        for (let [k,v] of deviceMap) {
            // let core  = getDeviceCore(k)
            let core = React.createElement(v[1]);
            devs.push(
                <DeviceItem OnAdd={this.OnAdd} key={k}>
                    {core}
                </DeviceItem>
            )
        }
        return devs;
    }

    render() {
        return (      
            <Menu pageWrapId={"panel-content"} outerContainerId={"outer-container"}  noOverlay isOpen={this.props.isOpen} onStateChange={(s) => this.props.onGalleryStateChange(s)}>
                {this.GenDevices(deviceMap)}
                {/* <DeviceItem OnAdd={this.OnAdd}>
                    <HButtonCore />
                </DeviceItem> */}
            </Menu>
        );
    }
}