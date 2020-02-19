import React, { Component } from 'react';

import ledOn from './led_on.svg';
import ledOff from './led_off.svg';

export class LEDCore extends Component {
    
    static defaultProps = {
        name: "LED",
        ClassName: "LED",
        onOff: false
    }


    render() {
        return (      
            <div>
                <img src={this.props.onOff ? ledOn : ledOff} alt="led"></img>
            </div>
        );
    }
}