import React, { Component } from 'react';
import { manager } from '../../Service/FPGAManager';
import { SevenSegmentLEDCore } from './SevenSegmentLEDCore';


export class SevenSegmentLED extends Component {
    
    static defaultProps = {
        name: '七段数码管',
        input: [0, 0, 0, 0, 0, 0, 0, 0], //
        ports: ['输入a', '输入b', '输入c', '输入d', '输入e', '输入f', '输入g', '输入h'],
        portsDirs: ['输入', '输入', '输入', '输入', '输入', '输入', '输入', '输入']
    }

    state = {
        inputs: [0, 0, 0, 0, 0, 0, 0, 0]
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

        let on = this.state.inputs.map((val) => val === 1 ? true : false)

        return (      
            <SevenSegmentLEDCore onOff={on}/>
        );
    }
}