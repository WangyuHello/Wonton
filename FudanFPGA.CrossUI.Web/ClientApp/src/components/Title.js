import React, { Component } from 'react';
import { Button, InputGroup, InputGroupAddon, InputGroupText, Input, ButtonGroup, ButtonDropdown, DropdownToggle, DropdownMenu, DropdownItem, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCog, faMicrochip, faPlay, faStop, faServer, faSquareFull, faTimes, faFolderPlus, faSave, faFolderOpen, faPlus } from '@fortawesome/free-solid-svg-icons'
import './Title.css'
import FPGAManager, { manager } from './Service/FPGAManager';
import isElectron from 'is-electron';
import is from 'electron-is';
import os from 'os';

import maximize from './Resource/maximize.svg';
import minimize from './Resource/minimize.svg';
import restore from './Resource/restore.svg';
import close from './Resource/close.svg';


export class Title extends Component {

    state = {
        isRunning: false,
        isProgrammToggle: false,
        runHz: 10,
        bitfile: '',
        isFileModalOpen: false,
        isNewModalOpen: false
    }

    OpenFileModal = (event) => {
        // this.setState((prevState) => {
        //     return {
        //         isFileModalOpen : !prevState.isFileModalOpen,
        //     }
        // });

        var inputObj = document.createElement('input');
        inputObj.setAttribute('id','_ef');
        inputObj.setAttribute('type','file');
        inputObj.setAttribute('accept','.bit');
        inputObj.setAttribute('style','visibility:hidden');
        document.body.appendChild(inputObj);
        inputObj.addEventListener("change",this.OnBitfileChange);
        inputObj.click();
    }

    OnBitfileChange = (event) => {
        // console.log(event.target.files[0]);
        // if (isElectron()) {
        //     this.setState({ bitfile: event.target.files[0].path });
        // } else {
        //     this.setState({ bitfile: event.target.files[0].name });
        // }
        let inputObj = document.getElementById("_ef");

        if (isElectron()) {
            this.setState({ bitfile: inputObj.files[0].path });
        } else {
            this.setState({ bitfile: inputObj.files[0].name });
        }

        console.log(`Open bit file: ${inputObj.files[0].name}`);

        inputObj.removeEventListener("change",function(){});
        document.body.removeChild(inputObj);
    }

    FreqChange = (event) => {
        this.setState({runHz: event.target.value});
    }

    ClickRun = async () => {

        if (this.state.isRunning) {      

            this.setState((prevState) => {
                return {
                    isRunning : !prevState.isRunning,
                }
            }, () => {});
        } else {
            this.setState((prevState) => {
                return {
                    isRunning : !prevState.isRunning,
                }
            }, () => {});

            await this.RunFPGA();
        }
    }

    RunFPGA = async () => {
        // ipcRenderer.send('working-status',true);
        await fetch('/api/window/working-state?state=1');

        await manager.InitIO(4, 4);
        await manager.IoOpen();

        console.log(`Run Frequency: ${this.state.runHz}`);

        while (this.state.isRunning)
        {            
            // let r = await manager.WriteReadData(write);
            let r = await manager.WriteReadData2();

            var hr_out = r[0] & 0x000F;
            var min_out = (r[0] & 0x03F0) >> 4;
            var sec_out = (r[0] & 0xFC00) >> 10;
            var hr_alarm = r[1] & 0x000F;
            var min_alarm = (r[1] & 0x03F0) >> 4;
            var alarm = (r[1] & 0x0400) >> 10;

            console.log(
                `"hr_out[${hr_out}] min_out[${min_out}] sec_out[${sec_out}] hr_alarm[${hr_alarm}] min_alarm[${
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

    systemButtons = <div className="clickTitle">
                              <a className="btn btn-min" href="#" onClick={this.ClickMin}>
                                  <img src={minimize} />
                              </a>
                              <a className="btn btn-max" href="#" onClick={this.ClickMaxRestore}>
                                  <img src={maximize} />
                              </a>
                              <a className="btn btn-close" href="#" onClick={this.ClickClose}>
                                  <img src={close} />
                              </a>
                          </div>;

    New = (event) => {
        this.setState((prevState) => {
            return {
                isNewModalOpen: !prevState.isNewModalOpen
            }
        })
    }

    OnNewfileChange = (event) => {

    }

    Open = (event) => {
        //手动操作DOM的脏方法
        var inputObj = document.createElement('input');
        inputObj.setAttribute('id','_ef');
        inputObj.setAttribute('type','file');
        inputObj.setAttribute('style','visibility:hidden');
        document.body.appendChild(inputObj);
        inputObj.addEventListener("change",this.OnOpenfileChange);
        inputObj.click();
    }

    OnOpenfileChange = (event) => {
        let inputObj = document.getElementById("_ef");
        let file = inputObj.files[0].name;

        console.log(`Open project file: ${file}`);

        inputObj.removeEventListener("change",function(){});
        document.body.removeChild(inputObj);
    }

    Save = (event) => {

    }

    render() {

        const isRunning = this.state.isRunning;
        const runHz = this.state.runHz === 0 ? "" : this.state.runHz;
        const bitf = this.state.bitfile == "" ? "未指定Bit文件" : this.state.bitfile;

        let isMac = is.macOS(); //如果再MacOS上，要添加红绿灯按钮

        let titleLeftMargin = isMac ? "80px" : "20px";

        return (
            <div className="titleBar">
                <div className="myTitle">
                    <div style={{ display: 'flex', alignItems: 'center', marginLeft: titleLeftMargin, marginTop: '10px'}}>
                        <FontAwesomeIcon style={{width:'22px', height:'22px', color: 'white'}} icon={faServer}/>
                        <div className="titleName">复旦FPGA</div>
                    </div>

                    {isMac ? <div /> : this.systemButtons}
                    
                </div>
                <div className="navMenu">
                    <div style={{display: "flex", alignItems: 'center'}}>
                        <div style={{width: "50px"}}/>
                        <div style={{color: 'white', fontSize:'18px', marginBottom:"-2px"}}>器件库</div>
                        <div style={{width: "30px"}}></div>
                        <ButtonGroup size="sm">
                            <Button onClick={this.New}>
                                <FontAwesomeIcon icon={faPlus}></FontAwesomeIcon>
                            </Button>
                            <Button onClick={this.Open}>
                                <FontAwesomeIcon icon={faFolderOpen}></FontAwesomeIcon>
                            </Button>
                            <Button onClick={this.Save}>
                                <FontAwesomeIcon icon={faSave}></FontAwesomeIcon>
                            </Button>
                        </ButtonGroup>

                        <Modal isOpen={this.state.isNewModalOpen} toggle={this.New}>
                                <ModalHeader toggle={this.New} >新建工程</ModalHeader>
                                <ModalBody>
                                    <div>项目地址</div>
                                    <Input type='file' accept='.xml' onChange={this.OnNewfileChange}></Input>
                                    <div>引脚约束文件</div>
                                    <Input type='file' accept='.xml' onChange={this.OnNewfileChange}></Input>
                                </ModalBody>
                                <ModalFooter>
                                    <Button color="primary" onClick={this.New}>确定</Button>
                                    <Button color="secondary" onClick={this.New}>关闭</Button>
                                </ModalFooter>
                        </Modal>
                    </div>

                    <div style={{display: "flex"}} className="no-drag">
                        <div>
                            <InputGroup>
                                <Input style={{width: "100px"}} placeholder="频率" value={runHz} type="number" onChange={this.FreqChange}/>
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
