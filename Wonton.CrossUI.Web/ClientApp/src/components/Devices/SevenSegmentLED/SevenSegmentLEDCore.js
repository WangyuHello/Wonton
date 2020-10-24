import React, { Component } from 'react';

import "./SevenSegmentLED.css"

export class SevenSegmentLEDCore extends Component {
    
    static defaultProps = {
        name: "七段数码管",
        ClassName: "SevenSegmentLED",
        onOff: [true, true, true, true, true, true, true, true]
    }

    render() {
        return (      
            <div className="sevenseg">
                <div id="panel"></div>
                <span id="a" className={this.props.onOff[0] ? "on" : ""} />
                <span id="b" className={this.props.onOff[1] ? "on" : ""} />
                <span id="c" className={this.props.onOff[2] ? "on" : ""} />
                <span id="d" className={this.props.onOff[3] ? "on" : ""} />
                <span id="e" className={this.props.onOff[4] ? "on" : ""} />
                <span id="f" className={this.props.onOff[5] ? "on" : ""} />
                <span id="g" className={this.props.onOff[6] ? "on" : ""} />
                <span id="h" className={this.props.onOff[7] ? "on" : ""} />
            </div>
        );
    }
}