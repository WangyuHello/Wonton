import React, { Component } from 'react';
import { manager } from '../../Service/FPGAManager';
import { LEDMatrix4t4Core } from './LEDMatrix4t4Core';


export class LEDMatrix4t4 extends Component {
    
    static defaultProps = {
        name: 'LED阵列4x4',
        input: [0, 0, 0, 0, 0, 0, 0, 0], 
        ports: ['行[0]', '行[1]', '行[2]', '行[3]', '列[0]', '列[1]', '列[2]', '列[3]'],
        portsDirs: ['输入', '输入', '输入', '输入', '输入', '输入', '输入', '输入']
    }

    state = {
        inputs: [0, 0, 0, 0, 0, 0, 0, 0], 
    }

    componentDidMount() {
        let ins = this.props.instance;

        let that = this;

        manager.Subscribe(ins, this.props.ports, (inputs, deltaTime) => {
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
        
        let onOff = [
            [false, false, false, false],
            [false, false, false, false],
            [false, false, false, false],
            [false, false, false, false]
        ]

        let row_sel = this.state.inputs.slice(0, 4)
        let col_sel = this.state.inputs.slice(4, 8)

        let selcted_row = row_sel.indexOf(1)
        onOff[selcted_row] = col_sel

        return (      
            <LEDMatrix4t4Core onOff={onOff}/>
        );
    }
}