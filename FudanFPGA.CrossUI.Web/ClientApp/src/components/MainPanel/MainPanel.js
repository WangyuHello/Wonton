import React, { PureComponent } from 'react';
import { push as Menu } from 'react-burger-menu';
import RGL, { WidthProvider } from "react-grid-layout";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Device } from '../Device/Device';
import { DeviceItem } from '../DeviceItem/DeviceItem';
import { faStore, faStoreAlt } from '@fortawesome/free-solid-svg-icons';
import { LED } from '../Devices/LED/LED';

import './MainPanel.css';

const ReactGridLayout = WidthProvider(RGL);

export class MainPanel extends PureComponent {
    
    render() {

        const layout = [
            {i: 'i1', x:0,y:0,w:3,h:3},
            {i: 'i2', x:3,y:0,w:3,h:3},
            {i: 'i3', x:6,y:0,w:3,h:3},
            { i: 'i4', x: 9, y: 0,w:3,h:3},
            { i: 'i5', x: 12, y: 0,w:3,h:3},
            { i: 'i6', x: 15, y: 0,w:3,h:3}
        ];


        return (
            <div id="outer-container">
                <Menu pageWrapId={"panel-content"} outerContainerId={"outer-container"}  noOverlay>
                    <DeviceItem></DeviceItem>
                    <DeviceItem></DeviceItem>
                    <DeviceItem></DeviceItem>
                    <DeviceItem></DeviceItem>
                    <DeviceItem></DeviceItem>
                    <DeviceItem></DeviceItem>
                </Menu>
                <main id='panel-content' >
                    <ReactGridLayout layout={layout} cols={24} rowHeight={24}>
                        <div key="i1" className='grid-base'>
                            <LED instance={"i1"} input={[1]}/>
                        </div>
                        <div key="i2" className='grid-base'>
                            <LED instance={"i2"} input={[1]}/>
                        </div>
                        <div key="i3" className='grid-base'>
                            <LED instance={"i3"} input={[1]}/>
                        </div>
                        <div key="i4" className='grid-base'>
                            <LED instance={"i4"} input={[1]} />
                        </div>
                        <div key="i5" className='grid-base'>
                            <LED instance={"i5"} input={[1]} />
                        </div>
                        <div key="i6" className='grid-base'>
                            <LED instance={"i6"} input={[1]} />
                        </div>
                    </ReactGridLayout>
                </main>
            </div>
        );
    }
}