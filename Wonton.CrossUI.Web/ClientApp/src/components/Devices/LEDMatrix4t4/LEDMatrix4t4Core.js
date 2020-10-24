import React, { Component } from 'react';

import "./LEDMatrix4t4.css"

export class LEDMatrix4t4Core extends Component {
    
    static defaultProps = {
        name: "LED阵列4x4",
        ClassName: "LEDMatrix4t4",
        onOff: [
            [true, true, true, true],
            [true, true, true, true],
            [true, true, true, true],
            [true, true, true, true]
        ]
    }


    render() {

        return (      
            <div className="LEDMatrix">
                <div id="panel"></div>
                <span id="dot0_0" className={this.props.onOff[0][0] ? "on" : ""} />
                <span id="dot0_1" className={this.props.onOff[0][1] ? "on" : ""} />
                <span id="dot0_2" className={this.props.onOff[0][2] ? "on" : ""} />
                <span id="dot0_3" className={this.props.onOff[0][3] ? "on" : ""} />
                <span id="dot1_0" className={this.props.onOff[1][0] ? "on" : ""} />
                <span id="dot1_1" className={this.props.onOff[1][1] ? "on" : ""} />
                <span id="dot1_2" className={this.props.onOff[1][2] ? "on" : ""} />
                <span id="dot1_3" className={this.props.onOff[1][3] ? "on" : ""} />
                <span id="dot2_0" className={this.props.onOff[2][0] ? "on" : ""} />
                <span id="dot2_1" className={this.props.onOff[2][1] ? "on" : ""} />
                <span id="dot2_2" className={this.props.onOff[2][2] ? "on" : ""} />
                <span id="dot2_3" className={this.props.onOff[2][3] ? "on" : ""} />
                <span id="dot3_0" className={this.props.onOff[3][0] ? "on" : ""} />
                <span id="dot3_1" className={this.props.onOff[3][1] ? "on" : ""} />
                <span id="dot3_2" className={this.props.onOff[3][2] ? "on" : ""} />
                <span id="dot3_3" className={this.props.onOff[3][3] ? "on" : ""} />
            </div>
        );
    }
}