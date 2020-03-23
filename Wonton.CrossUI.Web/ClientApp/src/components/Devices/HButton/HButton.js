import React, { Component } from 'react';
import { manager } from '../../Service/FPGAManager';
import { HButtonCore } from './HButtonCore'

import './HButton.css'

export class HButton extends Component {
    
    outputPorts = ['输出1']

    static defaultProps = {
        name: '按钮',
        ports: ['输出1'],
        portsDirs: ['输出']
    }

    state = {
        outputs: [0]
    }

    componentDidMount() {
        manager.Register(this.props.instance, this.state.outputs.length);
        manager.RegisterProjectPorts(this.props.instance, this.state.outputs.length);
    }

    componentWillUnmount() {
        manager.UnRegister(this.props.instance);
        manager.UnRegisterProjectPorts(this.props.instance);
    }

    ButtonClick = (event) => {
        this.setState((prevState) => {
            let nextOutput = 1 - prevState.outputs[0];
            console.log("Button click "+this.props.instance + ": "+nextOutput)
            return {
                outputs : [nextOutput],
            }
        });
        
        manager.UpdateInput(this.props.instance, [1 - this.state.outputs[0]]);
    }

    render() {
        return (
            <HButtonCore onClick={this.ButtonClick} active={this.state.outputs[0] === 1} />
        );
    }
}