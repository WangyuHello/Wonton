import React, { Component } from 'react';
import FPGAManager, { manager } from '../../Service/FPGAManager';

export class LED extends Component {
    
    inputPorts = ['input1']

    static defaultProps = {
        input: [0] //LED只有一个输入
    }

    state = {
        inputs: [0]
    }

    componentDidMount() {
        let ins = this.props.instance;

        let that = this;

        manager.Subscribe(ins, this.inputPorts, (inputs) => {
            that.setState({
                inputs: inputs
            });
        });
    }

    render() {

        let on = this.state.inputs[0] == 1 ? true : false;

        return (
            <div>
                LED {on ? "亮" : "灭"} {this.props.instance}
            </div>
        );
    }
}