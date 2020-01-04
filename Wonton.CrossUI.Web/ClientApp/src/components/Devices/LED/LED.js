import React, { Component } from 'react';
import FPGAManager, { manager } from '../../Service/FPGAManager';
import { LEDCore } from './LEDCore';

import ledOn from './led_on.svg';
import ledOff from './led_off.svg';

export class LED extends Component {
    
    static defaultProps = {
        name: 'LED',
        input: [0], //LED只有一个输入
        ports: ['输入1'],
        portsDirs: ['输入']
    }

    state = {
        inputs: [0]
    }

    componentDidMount() {
        let ins = this.props.instance;

        let that = this;

        manager.Subscribe(ins, this.props.ports, (inputs) => {
            that.setState({
                inputs: inputs
            });
        });

        manager.RegisterProjectPorts(this.props.instance, this.state.inputs.length);
    }

    componentWillUnmount() {
        manager.UnSubscribe(this.props.instance);
        manager.UnRegisterProjectPorts(this.props.instance);
    }

    render() {

        let on = this.state.inputs[0] == 1 ? true : false;

        return (      
            <LEDCore onOff={on}/>
        );
    }
}