import { inputPortsMapping, outputPortsMapping } from './FPGAPortsMap';

export default class FPGAManager {
    bitfile = ""
    writeCount = 0
    readCount = 0

    subscribedInstances = new Map() //响应输出的Instance i1 => {data: [0], refresh: 回调函数}
    hardwarePortsMap = new Map() //输出端口映射 32 => {instance: "i2", index: 0}
    inputPortsMap = new Map() //输入端口映射 i1 => [31]
    projectInstancePortsMap = new Map() // i1 => ["sec_out[0]"] 包含输入输出

    hardwareValues = [] //输出数据 [0,0,0,0,......0] 64位

    inputValues = [] //输入数据 [0,0,0,0,......0] 64位

    projectPortsMap = new Map([
        ["clk","P77"],
        ["rst_n","P151"],
        ["mode","P148"],
        ["set_hr","P150"],
        ["set_min","P152"],
        ["hr_out[0]","P7"],
        ["hr_out[1]","P6"],
        ["hr_out[2]","P5"],
        ["hr_out[3]","P4"],
        ["min_out[0]","P9"],
        ["min_out[1]","P8"],
        ["min_out[2]","P16"],
        ["min_out[3]","P15"],
        ["min_out[4]","P11"],
        ["min_out[5]","P10"],
        ["sec_out[0]","P20"],
        ["sec_out[1]","P18"],
        ["sec_out[2]","P17"],
        ["sec_out[3]","P22"],
        ["sec_out[4]","P21"],
        ["sec_out[5]","P23"],
        ["hr_alarm[0]","P44"],
        ["hr_alarm[1]","P45"],
        ["hr_alarm[2]","P46"],
        ["hr_alarm[3]","P43"],
        ["min_alarm[0]","P40"],
        ["min_alarm[1]","P41"],
        ["min_alarm[2]","P42"],
        ["min_alarm[3]","P33"],
        ["min_alarm[4]","P34"],
        ["min_alarm[5]","P35"],
        ["alarm","P36"],
    ]
    );

    projectInputPorts = [];
    projectOutputPorts = [];

    constructor() {
        this.subscribedInstances = new Map();
        this.hardwarePortsMap = new Map();
        this.inputPortsMap = new Map();
        this.projectInstancePortsMap = new Map();
    }

    Update = () => {
        this.hardwarePortsMap.forEach((m, key, map) => {
            const value = this.hardwareValues[key];
            const ins = m.instance;
            const insPort = m.index;

            if (this.subscribedInstances.has(ins)) { //可能被删除
                var subIns = this.subscribedInstances.get(ins);
                subIns.data[insPort] = value;
            }
        });
    }

    MapOutputPorts = (projectPortName, instance, instancePortIndex) => {
        let hardwarePortName = this.projectPortsMap.get(projectPortName);
        let hardwarePortIndex = outputPortsMapping.get(hardwarePortName);
        this.hardwarePortsMap.set(hardwarePortIndex, {instance: instance, index: instancePortIndex});
        
        let project_ports = this.projectInstancePortsMap.get(instance);
        project_ports[instancePortIndex] = projectPortName;
        this.projectInstancePortsMap.set(instance, project_ports);

        console.log(`Set Output Mapping ${instance}.${instancePortIndex} <= ${projectPortName}`);
    }

    GetProjectInstanceMapping = (instance, instancePortIndex) => {
        return (this.projectInstancePortsMap.get(instance))[instancePortIndex];
    }

    GetProjectInstanceMappingAll = (instance) => {
        return this.projectInstancePortsMap.get(instance);
    }

    MapInputPorts = (instance, instancePortIndex ,projectPortName, defaultValue) => {
        let hardwarePortName = this.projectPortsMap.get(projectPortName);
        let inputIndex = inputPortsMapping.get(hardwarePortName);
        let map = this.inputPortsMap.get(instance);
        map[instancePortIndex] = inputIndex;
        this.inputPortsMap.set(instance, map);
        this.inputValues[inputIndex] = defaultValue;

        let project_ports = this.projectInstancePortsMap.get(instance);
        project_ports[instancePortIndex] = projectPortName;
        this.projectInstancePortsMap.set(instance, project_ports);

        console.log(`Set Input Mapping ${instance}.${instancePortIndex} => ${projectPortName}`);
    }

    Register = (instance, inputCount) => {
        if (this.inputPortsMap.has(instance)) {
            console.log(`Already Registered Instance: ${instance}, inputCount: ${inputCount}`);
        } else {
            this.inputPortsMap.set(instance, new Array(inputCount).fill(0));
        }
    }

    UnRegister = (instance) => {
        this.inputPortsMap.delete(instance);
        console.log(`UnRegister instance: ${instance}`);
    }

    RegisterProjectPorts = (instance, count) => {
        if (this.projectInstancePortsMap.has(instance)) {
            //已经从项目文件中回复
            console.log(`Already Registered Instance for Project: ${instance}`);
        } else {
            this.projectInstancePortsMap.set(instance, new Array(count).fill(""));
            console.log(`Registered Instance for Project: ${instance}, inputCount: ${count}`);
        }
    }

    UnRegisterProjectPorts = (instance) => {
        this.projectInstancePortsMap.delete(instance);
        console.log(`UnRegistered Instance for Project: ${instance}`);
    }

    GenInputOutputGroup = () => {
        this.projectPortsMap.forEach((value, key, m) => {
            if (key !== 'clk') {
                let hardwarePorts = this.projectPortsMap.get(key);

                if (inputPortsMapping.has(hardwarePorts)) { //输入
                    this.projectInputPorts.push(key);
                } else {
                    this.projectOutputPorts.push(key);
                }
            }
        });
    }

    //查询所有输出端口
    GetProjectOutputPorts = () => {
        if (this.projectInputPorts.length === 0 || this.projectOutputPorts.length === 0) {
            this.GenInputOutputGroup();
        }

        return this.projectOutputPorts;
    }

    //查询所有输入端口
    GetProjectInputPorts = () => {
        if (this.projectInputPorts.length === 0 || this.projectOutputPorts.length === 0) {
            this.GenInputOutputGroup();
        }

        return this.projectInputPorts;
    }

    Subscribe = (instance, portlist, update) => {
        this.subscribedInstances.set(instance, {data: new Array(portlist.length).fill(0), refresh: update});
    }

    UnSubscribe = (instance) => {
        this.subscribedInstances.delete(instance);
        console.log(`UnSubscribe instance: ${instance}`);
    }

    UpdateInput = (instance, inputs) => {
        let inputMap = this.inputPortsMap.get(instance);
        inputs.forEach((data, index, arr) => {
            let inputDex = inputMap[index];
            this.inputValues[inputDex] = data;
        });
    }

    Cycle = () => {
        this.subscribedInstances.forEach((ins, key, map) => {
            ins.refresh(ins.data);
        });
    }

    Program = async (bitfile) => {
        this.bitfile = bitfile;
        const response = await fetch('/api/fpga/program?bitfile=' + bitfile);
        const data = await response.json();
        console.log(data.message);
    }

    IoOpen = async () => {
        const response = await fetch('/api/fpga/ioopen');
        const data = await response.json();
        console.log(data.message);
    }

    IoClose = async () => {
        const response = await fetch('/api/fpga/ioclose');
        const data = await response.json();
        console.log(data.message);
    }

    InitIO = async (writeCount, readCount) => {
        this.writeCount = writeCount;
        this.readCount = readCount;
        this.hardwareValues = new Array(readCount*16).fill(0);
        this.inputValues = new Array(writeCount*16).fill(0);
        this.tempi16WriteData = new Array(writeCount).fill(0);
        const response = await fetch('/api/fpga/initio?writeCount='+writeCount+'&readCount='+readCount);
        const data = await response.json();
        console.log(data.message);
    }

    Split = (i16values) => {
        //遍历所有i16
        i16values.forEach((i16data, index, arr) => {
            //将i16分为16个数分别存储
            for (let dind = 0; dind < 16; dind++) {
                this.hardwareValues[index*16 + dind] = (i16data >> dind) & 1;
            }
        });
    }

    tempi16WriteData = [];

    GenWriteData = () => {
        //从bit数组生成i16数组
        this.tempi16WriteData.fill(0);
        this.inputValues.forEach((data, index, arr) => {
            let shift = index >> 4; // /16
            let shift2 = index & 15; // 取最后4位
            this.tempi16WriteData[shift] = this.tempi16WriteData[shift] | (data << shift2);
        });
    }

    WriteReadData2 = async () => {
        this.GenWriteData();
        return await this.WriteReadData(this.tempi16WriteData);
    }

    WriteReadData = async (writeData) => {
        const response = await fetch('/api/fpga/writereadfpga',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(writeData)
            });
        const data = await response.json();
        console.log(data.data);

        this.Split(data.data);
        this.Update();

        return data.data;
    }
}

export const manager = new FPGAManager();
