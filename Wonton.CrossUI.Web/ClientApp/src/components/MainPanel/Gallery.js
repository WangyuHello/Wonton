import React, { Component } from 'react';
import { push as Menu } from 'react-burger-menu';
import { DeviceItem } from '../DeviceItem/DeviceItem';

import './Gallery.css';
import { LEDCore } from '../Devices/LED/LEDCore';
import { HButtonCore } from '../Devices/HButton/HButtonCore';

export class Gallery extends Component {
    
    OnAdd = (event, name) => {
        this.props.OnAdd(event, name);
    }

    render() {
        return (      
            <Menu pageWrapId={"panel-content"} outerContainerId={"outer-container"}  noOverlay isOpen={this.props.isOpen} onStateChange={(s) => this.props.onGalleryStateChange(s)}>
                <DeviceItem OnAdd={this.OnAdd}>
                    <LEDCore />
                </DeviceItem>
                <DeviceItem OnAdd={this.OnAdd}>
                    <HButtonCore />
                </DeviceItem>
            </Menu>
        );
    }
}