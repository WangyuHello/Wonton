import React, { Component } from 'react';
import './Title.css'

export class Title extends Component {

    render() {
        return (
            <div className="myTitle">
                <div className="titleName">复旦FPGA</div>
                <div className="clickTitle">
                    <a className="btn btn-primary" href="javascript:window.close()">关闭</a>
                </div>
            </div>
        );
    }
}
