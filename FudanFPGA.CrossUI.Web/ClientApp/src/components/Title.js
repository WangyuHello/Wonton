import React, { Component } from 'react';
import { Button, InputGroup, InputGroupAddon, InputGroupText, Input, ButtonGroup, ButtonDropdown, DropdownToggle, DropdownMenu, DropdownItem, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCog, faMicrochip, faPlay, faStop, faServer, faSquareFull, faTimes, faFolderPlus, faSave, faFolderOpen, faPlus } from '@fortawesome/free-solid-svg-icons'
import './Title.css'
import FPGAManager, { manager } from './Service/FPGAManager';
import { pjManager } from './Service/ProjectManager';
import { Start } from "./Start";
import isElectron from 'is-electron';
import is from 'electron-is';
import { ipcRenderer } from "electron";


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
        isNewModalOpen: false,
        isOpenModalOpen: false,
        isMaximized: false,
        isSettingDropdownOpen: false,
        pjName: '',
        isStartModalOpen: false
    }

    constructor(props) {
        super(props);

        ipcRenderer.on('window-state-maximize', this.onWindowStateMaximize);
        ipcRenderer.on('window-state-unmaximize', this.onWindowStateUnMaximize);
    }

    onWindowStateMaximize = (event, arg) => {
        console.log("maximized");
        this.setState({
            isMaximized: true
        });
    }

    onWindowStateUnMaximize = (event, arg) => {
        console.log("unmaximized");
        this.setState({
            isMaximized: false
        });
    }

    //从Manager获取初始值
    async componentWillMount() {
        let data = await pjManager.GetTitleData();
        this.setState({
            bitfile: data.bitfile,
            pjName: data.pjName,
            isStartModalOpen: !data.projectInitialize
        });
    }

    // componentDidMount() {
    //     pjManager.RegisterRefreshTitle(this.OnRefresh);
    // }

    // OnRefresh = (data) => {
    //     this.setState({
    //         bitfile: data.bitfile
    //     });
    // }

    OpenFileModal = (event) => {
        this.setState((prevState) => {
            return {
                isFileModalOpen : !prevState.isFileModalOpen,
            }
        });

        // var inputObj = document.createElement('input');
        // inputObj.setAttribute('id','_ef');
        // inputObj.setAttribute('type','file');
        // inputObj.setAttribute('accept','.bit');
        // inputObj.setAttribute('style','visibility:hidden');
        // document.body.appendChild(inputObj);
        // inputObj.addEventListener("change",this.OnBitfileChange);
        // inputObj.click();
    }

    OnBitfileChange = (event) => {
        console.log(event.target.files[0]);
        if (isElectron()) {
            let path = event.target.files[0].path;
            pjManager.bitfile = path
            this.setState({ bitfile: path });
        } else {
            pjManager.bitfile = "E:\\Documents\\Repo\\ProjectFDB\\FudanFPGAInterface\\FudanFPGA.Test\\AlarmClock_fde_dc.bit";
            this.setState({ bitfile: "E:\\Documents\\Repo\\ProjectFDB\\FudanFPGAInterface\\FudanFPGA.Test\\AlarmClock_fde_dc.bit" });
        }
        // let inputObj = document.getElementById("_ef");

        // if (isElectron()) {
        //     this.setState({ bitfile: inputObj.files[0].path });
        // } else {
        //     this.setState({ bitfile: inputObj.files[0].name });
        // }

        // console.log(`Open bit file: ${inputObj.files[0].name}`);

        // inputObj.removeEventListener("change",function(){});
        // document.body.removeChild(inputObj);
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
        ipcRenderer.send('working-status',true);
        // await fetch('/api/window/working-state?state=1');

        await manager.InitIO(4, 4);
        await manager.IoOpen();

        console.log(`Run Frequency: ${this.state.runHz}`);

        while (this.state.isRunning)
        {            
            // let r = await manager.WriteReadData(write);
            let r = await manager.WriteReadData2();

            // For Test
            // var hr_out = r[0] & 0x000F;
            // var min_out = (r[0] & 0x03F0) >> 4;
            // var sec_out = (r[0] & 0xFC00) >> 10;
            // var hr_alarm = r[1] & 0x000F;
            // var min_alarm = (r[1] & 0x03F0) >> 4;
            // var alarm = (r[1] & 0x0400) >> 10;

            // console.log(
            //     `"hr_out[${hr_out}] min_out[${min_out}] sec_out[${sec_out}] hr_alarm[${hr_alarm}] min_alarm[${
            //     min_alarm}] alarm[${alarm}]"`);

            manager.Cycle();

            if (this.state.runHz !== 0) {
                await this.sleep(1000 / this.state.runHz);
            } else {
                await this.sleep(1000);
            }
            
        }

        await manager.IoClose();
        ipcRenderer.send('working-status',false);
        // await fetch('/api/window/working-state?state=0');
    }

    sleep = (time) => {
        return new Promise((resolve) => setTimeout(resolve, time));
    }

    ClickProgram = async () => {
        await manager.Program(this.state.bitfile);
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
        ipcRenderer.send('window-status', 'maximize');
        // const response = await fetch('/api/window/maximize');
    }

    ClickMin = async () => {
        ipcRenderer.send('window-status', 'minimize');
        // const response = await fetch('/api/window/minimize');
    }

    NewPjToggle = (event) => {
        this.setState((prevState) => {
            return {
                isNewModalOpen: !prevState.isNewModalOpen
            }
        })
    }

    NewPj = async (event) => {
        await pjManager.NewProject(this.state.pjdir,this.state.pjName,this.state.iofile);
        window.location.reload(true);
    }

    OnNewPjDirChange = (event) => {
        console.log(`New Project Dir: ${event.target.value}`);
        let path = event.target.value;
        this.setState({ pjdir: path });
    }

    OnNewPjNameChange = (event) => {
        console.log(`New Project Name: ${event.target.value}`);
        this.setState({
            pjName: event.target.value
        })
    }

    OnNewPjIOfileChange = async (event) => {
        if (isElectron()) {
            let path = event.target.files[0].path;
            this.setState({ iofile: path });
        } else {
            let path = "F:\\Repo\\FudanFPGAInterface\\FudanFPGA.Test\\AlarmClock.xml";
            this.setState({ iofile: path });
        }

        // if (isElectron()) {
            
        // } else {
        //     await pjManager.ReadProjectIO("E:\\Downloads\\VeriCommSDK-2019-11-22收到\\VeriCommSDK\\Example\\Alarm_Clock\\FDP3P7\\FDE\\src\\AlarmClock.xml");
        // }

    }

    OpenPjToggle = (event) => {
        this.setState((prevState) => {
            return {
                isOpenModalOpen: !prevState.isOpenModalOpen
            }
        })
    }

    // Open = (event) => {
    //     //手动操作DOM的脏方法
    //     var inputObj = document.createElement('input');
    //     inputObj.setAttribute('id','_ef');
    //     inputObj.setAttribute('type','file');
    //     inputObj.setAttribute('style','visibility:hidden');
    //     document.body.appendChild(inputObj);
    //     inputObj.addEventListener("change",this.OnOpenfileChange);
    //     inputObj.click();
    // }

    OpenPj = async (event) => {
        //将新的项目地址存储在后台进程
        pjManager.projectFile = this.state.openPjName;
        await pjManager.SetProjectFile();

        //刷新页面,重新载入项目
        window.location.reload(true);
    }

    OnOpenfileChange = (event) => {
        // let inputObj = document.getElementById("_ef");

        let file = "F:\\Repo\\FudanFPGAInterface\\FudanFPGA.Test\\haha.hwproj";
        if (isElectron()) {
            file = event.target.files[0].path;
        } else {
            
        }

        console.log(`Open project file: ${file}`);
        this.setState({
            openPjName: file
        })

        // inputObj.removeEventListener("change",function(){});
        // document.body.removeChild(inputObj);


    }

    Save = async (event) => {
        await pjManager.WriteProjectFile();
    }

    SettingToggle = (event) => {
        this.setState((prevState) => {
            return {
                isSettingDropdownOpen: !prevState.isSettingDropdownOpen
            }
        })
    }

    DevToolsToggle = (event) => {
        ipcRenderer.send('dev-tools');
    }

    render() {

        const isRunning = this.state.isRunning;
        const runHz = this.state.runHz === 0 ? "" : this.state.runHz;
        const bitf = this.state.bitfile == "" ? "未指定Bit文件" : this.state.bitfile;

        let isMac = is.macOS(); //如果再MacOS上，要添加红绿灯按钮
        // isMac = true;

        let titleLeftMargin = isMac ? "80px" : "20px";

        return (
            <div className="titleBar">

                <Modal isOpen={this.state.isStartModalOpen} centered>
                    <ModalHeader>开始</ModalHeader>
                    <ModalBody>
                        <Start onOpen={this.OpenPjToggle} onNew={this.NewPjToggle}></Start>
                    </ModalBody>
                </Modal>

                <div className="myTitle">
                    <div style={{ display: 'flex', alignItems: 'top', marginLeft: titleLeftMargin, marginTop: '8px'}}>
                        <FontAwesomeIcon style={{width:'20px', height:'20px', color: 'white', marginTop: "5px"}} icon={faServer}/>
                        <div className="titleName">复旦FPGA</div>
                    </div>

                    <div className="pjTitle">{this.state.pjName}</div>

                    {isMac ? <div /> : 
                    <div className="clickTitle">
                        <a className="btn btn-min" href="#" onClick={this.ClickMin}>
                            {/* <span className="systemIcon">&#xE921;</span> */}
                            <span className="systemIcon">&#xeaba;</span>
                        </a>
                        <a className="btn btn-max" href="#" onClick={this.ClickMaxRestore}>
                            {/* <img src={this.state.isMaximized ? restore : maximize} /> */}
                            {this.state.isMaximized ? <span className="systemIcon">&#xeabb;</span> : <span className="systemIcon">&#xeab9;</span>}
                        </a>
                        <a className="btn btn-close" href="#" onClick={this.ClickClose}>
                            {/* <img src={close} /> */}
                            <span className="systemIcon">&#xeab8;</span>
                        </a>
                    </div>
                    }
                    
                </div>
                <div className="navMenu">
                    <div style={{display: "flex", alignItems: 'center'}}>
                        <div style={{width: "50px"}}/>
                        <div style={{color: 'white', fontSize:'18px', marginBottom:"-2px"}}>器件库</div>
                        <div style={{width: "30px"}}></div>
                        <ButtonGroup size="sm" className="no-drag">
                            <Button onClick={this.NewPjToggle}>
                                <FontAwesomeIcon icon={faPlus}></FontAwesomeIcon>
                            </Button>
                            <Button onClick={this.OpenPjToggle}>
                                <FontAwesomeIcon icon={faFolderOpen}></FontAwesomeIcon>
                            </Button>
                            <Button onClick={this.Save}>
                                <FontAwesomeIcon icon={faSave}></FontAwesomeIcon>
                            </Button>
                        </ButtonGroup>

                        <Modal isOpen={this.state.isOpenModalOpen} toggle={this.OpenPjToggle} >
                            <ModalHeader toggle={this.OpenPjToggle}>打开工程</ModalHeader>
                            <ModalBody>
                                <Input type='file' accept='.hwproj' onChange={this.OnOpenfileChange}></Input>
                            </ModalBody>
                            <ModalFooter>
                                <Button color="primary" onClick={this.OpenPj}>确定</Button>
                                <Button color="secondary" onClick={this.OpenPjToggle}>关闭</Button>
                            </ModalFooter>
                        </Modal>

                        <Modal isOpen={this.state.isNewModalOpen} toggle={this.NewPjToggle}>
                                <ModalHeader toggle={this.NewPjToggle} >新建工程</ModalHeader>
                                <ModalBody>
                                    <div>项目名称</div>
                                    <Input onChange={this.OnNewPjNameChange}></Input>
                                    <div>项目地址</div>
                                    <Input onChange={this.OnNewPjDirChange}></Input>
                                    <div>引脚约束文件</div>
                                    <Input type='file' accept='.xml' onChange={this.OnNewPjIOfileChange}></Input>
                                </ModalBody>
                                <ModalFooter>
                                    <Button color="primary" onClick={this.NewPj}>确定</Button>
                                    <Button color="secondary" onClick={this.NewPjToggle}>关闭</Button>
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
                                <Button onClick={this.ClickProgram}>
                                    <div style={{display:'flex', alignItems: 'center'}} >
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
                        <ButtonGroup className="no-drag">
                            <Button onClick={this.SettingToggle}>
                                <div style={{display:'flex', alignItems: 'center'}}>
                                    <FontAwesomeIcon icon={faCog}/>
                                    <div style={{marginLeft: '4px'}}>设置</div>
                                </div>
                            </Button>
                            <ButtonDropdown  isOpen={this.state.isSettingDropdownOpen} toggle={this.SettingToggle}>
                                <DropdownToggle caret>

                                </DropdownToggle>
                                <DropdownMenu right>
                                    <DropdownItem onClick={this.DevToolsToggle}>
                                        开发者工具
                                    </DropdownItem>
                                </DropdownMenu>
                            </ButtonDropdown>
                        </ButtonGroup>

                        <div style={{width: "20px"}}/>
                    </div>
                </div>
            </div>
        );
    }
}
