import React, { Component } from 'react';
import { Button, InputGroup, InputGroupAddon, InputGroupText, Input, ButtonGroup, ButtonDropdown, DropdownToggle, DropdownMenu, DropdownItem, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCog, faMicrochip, faPlay, faStop, faServer, faSquareFull, faTimes } from '@fortawesome/free-solid-svg-icons'
import './Title.css'
import FPGAManager, { manager } from './Service/FPGAManager';
import isElectron from 'is-electron';


export class Title extends Component {

    state = {
        isRunning: false,
        isProgrammToggle: false,
        runHz: 10,
        bitfile: '',
        isFileModalOpen: false
    }

    OpenFileModal = (event) => {
        this.setState((prevState) => {
            return {
                isFileModalOpen : !prevState.isFileModalOpen,
            }
        });
    }

    OnBitfileChange = (event) => {
        console.log(event.target.files[0]);
        if (isElectron()) {
            this.setState({ bitfile: event.target.files[0].path });
        } else {
            this.setState({ bitfile: event.target.files[0].name });
        }
    }

    FreqChange = (event) => {
        this.setState({runHz: event.target.value});
    }

    ClickRun = async () => {
        this.setState((prevState) => {
            return {
                isRunning : !prevState.isRunning,
            }
        }, () => {});

        // ipcRenderer.send('working-status',true);
        await fetch('/api/window/working-state?state=1');

        await manager.InitIO(4, 4);
        await manager.IoOpen();

        console.log(this.state.runHz);

        manager.MapPorts(15, 'i1', 0);
        manager.MapPorts(14, 'i2', 0);
        manager.MapPorts(13, 'i3', 0);
        manager.MapPorts(12, 'i4', 0);
        manager.MapPorts(11, 'i5', 0);
        manager.MapPorts(10, 'i6', 0);

        let write = [0, 0, 0, 0];

        for (var i = 0; i < 80; i++) {
            if (i < 2) {
                write[0] = 0;
            } else {
                write[0] = 1;
            }

            let r = await manager.WriteReadData(write);

            var hr_out = r[0] & 0x000F;
            var min_out = (r[0] & 0x03F0) >> 4;
            var sec_out = (r[0] & 0xFC00) >> 10;
            var hr_alarm = r[1] & 0x000F;
            var min_alarm = (r[1] & 0x03F0) >> 4;
            var alarm = (r[1] & 0x0400) >> 10;

            console.log(
                `"[${i}] hr_out[${hr_out}] min_out[${min_out}] sec_out[${sec_out}] hr_alarm[${hr_alarm}] min_alarm[${
                min_alarm}] alarm[${alarm}]"`);

            manager.Cycle();

            if (this.state.runHz !== 0) {
                await this.sleep(1000 / this.state.runHz);
            } else {
                await this.sleep(1000);
            }
            
        }

        await manager.IoClose();
        // ipcRenderer.send('working-status',false);
        await fetch('/api/window/working-state?state=0');
    }

    sleep = (time) => {
        return new Promise((resolve) => setTimeout(resolve, time));
    }

    ClickProgram = async () => {
        if (isElectron()) {
            await manager.Program(this.state.bitfile);
        } else {
            await manager.Program("E:\\Documents\\Repo\\ProjectFDB\\FudanFPGAInterface\\FudanFPGA.Test\\AlarmClock_fde_dc.bit");
        }
        
        
    }

    ClickProgrammToggle = () => {

        this.setState((prevState) => {
            return {
                isProgrammToggle : !prevState.isProgrammToggle,
            }
        }, () => {});
    }

    ClickClose = () => {
        window.close();
    }

    ClickMaxRestore = async () => {
        // const browserWindow = remote.getCurrentWindow();
        // const isMax = browserWindow.isMaximized();
        // if (isMax) {
        //     browserWindow.restore();
        // } else {
        //     browserWindow.maximize();
        // }
        const response = await fetch('/api/window/maximize');
    }

    ClickMin = async () => {
        // const browserWindow = remote.getCurrentWindow();
        // browserWindow.minimize();
        const response = await fetch('/api/window/minimize');
    }


    render() {

        const isRunning = this.state.isRunning;
        const runHz = this.state.runHz === 0 ? "" : this.state.runHz;
        const bitf = this.state.bitfile == "" ? "未指定Bit文件" : this.state.bitfile;
        

        return (
            <div className="titleBar">
                <div className="myTitle">
                    <div style={{display: 'flex', alignItems:'center' ,marginLeft: '30px', marginTop: '10px'}}>
                        <FontAwesomeIcon style={{width:'28px', height:'28px', color: 'white'}} icon={faServer}/>
                        <div className="titleName">复旦FPGA</div>
                    </div>
                    <div className="clickTitle">
                        <a className="btn btn-min" href="#" onClick={this.ClickMin}>-</a>
                        <a className="btn btn-max" href="#" onClick={this.ClickMaxRestore}>
                            <FontAwesomeIcon icon={faSquareFull}/>
                        </a>
                        <a className="btn btn-close" href="#" onClick={this.ClickClose}>
                            <FontAwesomeIcon icon={faTimes}/>
                        </a>
                    </div>
                </div>
                <div className="navMenu">
                    <div style={{display: "flex", alignItems: 'stretch'}}>
                        <div style={{width: "50px"}}/>
                        <div style={{color: 'white', display: 'inline-block', verticalAlign: 'bottom', fontSize:'18px'}}>器件库</div>
                    </div>

                    <div style={{display: "flex"}} className="no-drag">
                        <div>
                            <InputGroup>
                                <Input style={{width: "100px"}} placeholder="频率" value={runHz} onChange={this.FreqChange}/>
                                <InputGroupAddon addonType="append">
                                    <InputGroupText>Hz</InputGroupText>
                                </InputGroupAddon>
                            </InputGroup>
                        </div>
                        <div style={{width: "10px"}}/>
                        <Button className="no-drag" color={isRunning ? "success" : "info"} onClick={this.ClickRun}>
                            <FontAwesomeIcon icon={isRunning ? faStop : faPlay}/>
                        </Button>
                    </div>

                    <div style={{display: "flex"}} className="no-drag">
                        <div>
                            <ButtonGroup >
                                <Button >
                                    <div style={{display:'flex', alignItems: 'center'}} onClick={this.ClickProgram}>
                                        <FontAwesomeIcon icon={faMicrochip}/>
                                        <div style={{marginLeft: '8px'}}>Program</div>
                                    </div>
                                </Button>
                                 <ButtonDropdown isOpen={this.state.isProgrammToggle} toggle={this.ClickProgrammToggle} >
                                    <DropdownToggle caret>

                                    </DropdownToggle> 
                                    <DropdownMenu right>
                                        <DropdownItem disabled>
                                            <div>{bitf}</div>
                                        </DropdownItem>
                                        <DropdownItem divider ></DropdownItem>
                                        <DropdownItem onClick={this.OpenFileModal}> 
                                            选择文件
                                         </DropdownItem> 
                                    </DropdownMenu> 
                                </ButtonDropdown> 
                            </ButtonGroup>

                            <Modal isOpen={this.state.isFileModalOpen} toggle={this.OpenFileModal}>
                                <ModalHeader toggle={this.OpenFileModal} >选择Bit文件</ModalHeader>
                                <ModalBody>
                                    <Input type='file' accept='.bit' onChange={this.OnBitfileChange}></Input>
                                </ModalBody>
                                <ModalFooter>
                                    <Button color="primary" onClick={this.OpenFileModal}>确定</Button>
                                    <Button color="secondary" onClick={this.OpenFileModal}>关闭</Button>
                                </ModalFooter>
                            </Modal>
                            
                        </div>
                        
                        <div style={{width: "20px"}}/>
                        <Button className="no-drag">
                            <div style={{display:'flex', alignItems: 'center'}}>
                                <FontAwesomeIcon icon={faCog}/>
                                <div style={{marginLeft: '4px'}}>设置</div>
                            </div>
                        </Button>
                        <div style={{width: "20px"}}/>
                    </div>
                </div>
            </div>
        );
    }
}
