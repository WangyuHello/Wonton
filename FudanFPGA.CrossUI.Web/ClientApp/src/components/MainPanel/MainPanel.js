import React, { PureComponent } from 'react';
import RGL, { WidthProvider } from "react-grid-layout";
import { LED } from '../Devices/LED/LED';
import { HButton } from '../Devices/HButton/HButton';
import { DeviceLayout } from '../Devices/DeviceLayout';
import { Modal, ModalHeader, ModalBody, Table, Input } from 'reactstrap';
import _ from 'lodash';

import './MainPanel.css';
import { manager } from '../Service/FPGAManager';
import { pjManager } from "../Service/ProjectManager";
import { Gallery } from './Gallery';

const ReactGridLayout = WidthProvider(RGL);

export class MainPanel extends PureComponent {

    //从Manager获取数据
    async componentWillMount() {
        let data = await pjManager.GetPanelData();
        this.setState({
            layout: data.layout,
            instanceCounter: data.layout.length
        });
    }

    componentDidMount() {
        //for test
        // manager.MapOutputPorts("sec_out[5]", 'i1', 0);
        // manager.MapOutputPorts("sec_out[4]", 'i2', 0);
        // manager.MapOutputPorts("sec_out[3]", 'i3', 0);
        // manager.MapOutputPorts("sec_out[2]", 'i4', 0);
        // manager.MapOutputPorts("sec_out[1]", 'i5', 0);
        // manager.MapOutputPorts("sec_out[0]", 'i6', 0);
        // manager.MapInputPorts('i7', 0, "rst_n", 0);

        // pjManager.RegisterRefreshMainPanel(this.OnRefresh);

        //for test
        // pjManager.layout = this.state.layout;
    }

    // OnRefresh = (data) => {
    //     this.setState({
    //         layout: data.layout,
    //         instanceCounter: data.layout.length
    //     });
    // }

    OnSettingClick = (event) => {
        let connMap = this.GenerateSettingTable(event);
        this.setState((prevState) => {
            return {
                isSettingModalOpen : !prevState.isSettingModalOpen,
                projectConnections : connMap
            }
        });
    }

    OnSettingModalToggle = (event) => {
        this.setState((prevState) => {
            return {
                isSettingModalOpen : !prevState.isSettingModalOpen,
            }
        });
    }

    OnRemoveClick = (event, instance) => {
        console.log(`Remove device: ${instance}`);
        this.setState({
            layout: _.reject(this.state.layout, {i: instance})
        });
    }

    OnPortChange = (event, index, direction, instance) =>　{
        let prevConnectionMap = this.state.projectConnections;
        let prevConnection = prevConnectionMap.get(instance);
        prevConnection[index] = event.target.value;
        prevConnectionMap.set(instance, prevConnection);

        if (direction === "输出") { //组件的输入接硬件的输出
            manager.MapInputPorts(instance, index, event.target.value, 0);
        } else {
            manager.MapOutputPorts(event.target.value, instance, index);
        }

        this.setState({
            projectConnections: prevConnectionMap
        });
    }

    GenPortsSelects = (instance, index, direction) => { 
        let projectPorts = direction === "输出" ? manager.GetProjectInputPorts() : manager.GetProjectOutputPorts(); //组件的输入接硬件的输出

        let selected = "";

        if (this.state.projectConnections.has(instance)) {
            selected = (this.state.projectConnections.get(instance))[index];
        } else {
            selected = manager.GetProjectInstanceMapping(instance, index);
        }

        return <Input type="select" bsSize="sm" key={index} value={selected} onChange={(e) => this.OnPortChange(e, index, direction, instance)}>
            <option>请选择</option>
            {
                projectPorts.map((value, index) => {
                    return <option key={index} >
                        {value}
                    </option>
                    
                })
            }
        </Input>
    }

    SettingTable = <Table hover>
            <thead>
                <tr>
                    <th>#</th>
                    <th>组件端口</th>
                    <th>I/O</th>
                    <th>硬件端口</th>
                </tr>
            </thead>
        </Table>

    GenerateSettingTable = (instance) => {
        let ports = instance.props.ports;
        let portsDirs = instance.props.portsDirs;

        this.SettingTable = <Table hover>
            <thead>
                <tr>
                    <th>#</th>
                    <th>组件端口</th>
                    <th>I/O</th>
                    <th>硬件端口</th>
                </tr>
            </thead>
            <tbody>
                {
                    ports.map((item, index) => {
                        return <tr key={index}>
                            <th scope="row">{index + 1}</th>
                            <td>{item}</td>
                            <td>{portsDirs[index]}</td>
                            <td>{this.GenPortsSelects(instance.props.instance, index, portsDirs[index])}</td>
                        </tr>
                    })
                }

            </tbody>
        </Table>

        let insProjMap = new Map();
        insProjMap.set(instance.props.instance, manager.GetProjectInstanceMappingAll(instance.props.instance));

        return insProjMap;
    }

    OnAdd = (event, name) => {
        let insCounter = this.state.instanceCounter;
        insCounter = insCounter + 1;

        let nextX = (this.state.layout.length * 3) % 24;
        let nextY = 3 * Math.floor(this.state.layout.length / 8);

        console.log(`Add instance: ${name} at (${nextX}, ${nextY})`);

        let newLayout = this.state.layout.concat({
            i: 'i' + insCounter, device: name, x: nextX, 
             y: nextY,w:3,h:3, minW:3, minH:3
        });

        //更新当前状态和PJManager中的值
        pjManager.layout = newLayout;

        this.setState({
            layout: newLayout,
            instanceCounter: insCounter
        })
    }

    GenLayoutDevices = (layout) => {
        console.log(layout);
        return _.map(layout, el => {
        
            let dev = <div></div>

            switch (el.device) {
                case "LED":
                    dev = <LED instance={el.i}/>
                    break;
                case "HButton":
                    dev = <HButton instance={el.i}/>
                    break;
                default:
                    break;
            }

            return <div key={el.i} data-grid={el} className='grid-base'>
                <DeviceLayout instance={el.i} OnSettingClick={this.OnSettingClick} OnRemoveClick={(e) => this.OnRemoveClick(e, el.i)}>
                    {dev}
                </DeviceLayout>
            </div>
        });
    }

    state = {
        isSettingModalOpen: false,
        instanceCounter: 7,
        projectConnections: new Map(),
        layout: [
            // { i: 'i1', device:'LED', x: 0,  y: 0,w:3,h:3, minW:3, minH:3},
            // { i: 'i2', device:'LED', x: 3,  y: 0,w:3,h:3, minW:3, minH:3},
            // { i: 'i3', device:'LED', x: 6,  y: 0,w:3,h:3, minW:3, minH:3},
            // { i: 'i4', device:'LED', x: 9,  y: 0,w:3,h:3, minW:3, minH:3},
            // { i: 'i5', device:'LED', x: 12, y: 0,w:3,h:3, minW:3, minH:3},
            // { i: 'i6', device:'LED', x: 15, y: 0,w:3,h:3, minW:3, minH:3},
            // { i: 'i7', device:'HButton', x: 18, y: 0,w:3,h:3, minW:3, minH:3}
        ]
    }
    
    render() {

        return (
            <div id="outer-container">
                <Gallery OnAdd={this.OnAdd}></Gallery>
                <main id='panel-content' >
                    <ReactGridLayout cols={24} rowHeight={24} compactType={null}>
                        {this.GenLayoutDevices(this.state.layout)}
                    </ReactGridLayout>
                </main>
                <Modal isOpen={this.state.isSettingModalOpen} toggle={this.OnSettingModalToggle}>
                    <ModalHeader toggle={this.OnSettingModalToggle} >设置</ModalHeader>
                    <ModalBody>
                        {this.SettingTable}
                    </ModalBody>
                </Modal>
            </div>
        );
    }
}