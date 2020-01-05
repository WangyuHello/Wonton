import React, { Component } from 'react';
import { Button } from 'reactstrap'
import FPGAManager, { manager } from '../../Service/FPGAManager';

export class HButton extends Component {
    
    outputPorts = ['output1']

    state = {
        outputs: [0]
    }

    componentDidMount() {
        manager.Register(this.props.instance, 1);
    }

    ButtonClick = (event) => {
        this.setState((prevState) => {
            let nextOutput = 1 - prevState.outputs[0];
            return {
                outputs : [nextOutput],
            }
        });
        manager.UpdateInput(this.props.instance, [1 - this.state.outputs[0]]);
    }

    render() {
        return (
            <div>
    <Button onClick={this.ButtonClick} active={this.state.outputs[0] === 1}>按钮</Button> {this.state.outputs[0] === 1 ? "按下" : ""} {this.props.instance}
            </div>
        );
    }
}