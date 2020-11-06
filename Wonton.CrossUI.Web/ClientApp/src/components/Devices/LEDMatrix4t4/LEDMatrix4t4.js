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
        onOff: [
            [false, false, false, false],
            [false, false, false, false],
            [false, false, false, false],
            [false, false, false, false]
        ],
        onffDeltaTime: [
            [0,0,0,0],
            [0,0,0,0],
            [0,0,0,0],
            [0,0,0,0]
        ]
    } 
    // 视觉暂留现象约0.4s

    componentDidMount() {
        let ins = this.props.instance;

        let that = this;

        manager.Subscribe(ins, this.props.ports, (inputs, deltaTime) => {
            let l_onOff = [
                [false, false, false, false],
                [false, false, false, false],
                [false, false, false, false],
                [false, false, false, false]
            ]
    
            let row_sel = inputs.slice(0, 4)
            let col_sel = inputs.slice(4, 8)
    
            for (let r = 0; r < row_sel.length; r++) {
                const ele = row_sel[r];
                if(ele ===  1) {
                    let new_row = col_sel.map(val => val === 1 ? true : false)
                    let old_row = that.state.onOff[r]
                    l_onOff[r] = new_row
                }
                else {
                    let new_row = [false,false,false,false]
                    l_onOff[r] = new_row
                }
            }

            that.setState({
                onOff: l_onOff
            });
        });

        let length = this.state.onOff.map(v => v.length).reduce((prev, curr) => prev + curr);
        manager.RegisterProjectPorts(this.props.instance, length);
    }

    componentWillUnmount() {
        manager.UnSubscribe(this.props.instance);
        manager.UnRegisterProjectPorts(this.props.instance);
    }

    render() {
        return (      
            <LEDMatrix4t4Core onOff={this.state.onOff}/>
        );
    }
}