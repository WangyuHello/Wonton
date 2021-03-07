import { inputPortsMapping, outputPortsMapping } from './FPGAPortsMap';


export default class FPGAManager {
    bitfile = ""
    writeCount = 0
    readCount = 0

    subscribedInstances = new Map() //响应输出的Instance i1 => {data: [0], refresh: 回调函数}
    hardwarePortsMap = new Map() //从FPGA输入端口映射（aka输入端口） 32 => [{instance: "i2", index: 0}, ...] 支持多订阅 结合每个元件引脚、xml和PortsMap多次映射
    inputPortsMap = new Map() //输出到FPGA端口映射（aka输出端口） i1 => [31]
    projectInstancePortsMap = new Map() // i1 => ["sec_out[0]"] 包含输入输出

    hardwareValues = [] //从FPGA输入数据 [0,0,0,0,......0] 64位

    inputValues = [] //输出到FPGA数据 [0,0,0,0,......0] 64位

    projectPortsMap = new Map(); //xml

    projectInputPorts = [];
    projectOutputPorts = [];

    constructor() {
        this.subscribedInstances = new Map();
        this.hardwarePortsMap = new Map();
        this.inputPortsMap = new Map();
        this.projectInstancePortsMap = new Map();
    }

    /* example alias for Test.hwproj (sec_out[0])
     * hardwarePortName: "P20"
     * projectPortName: "sec_out[0]"
     * hardwarePortIndex: 10
     * inputIndex: 0 (for rst_n)
     */

    // 根据端口映射表将从FPGA输入的64bit数据分发到各个组件上
    Update = () => {
        this.hardwarePortsMap.forEach((ms, key) => { //key: 第32个数 ms: [{instance: "i2", index: 0}, ...]
            const value = this.hardwareValues[key]; //第32个数的值(0 or 1)

            // console.log(ms)

            ms.forEach(m => { //遍历所有订阅
                const ins = m.instance;
                const insPortIndex = m.index;
    
                if (this.subscribedInstances.has(ins)) { //可能被删除
                    var subIns = this.subscribedInstances.get(ins);
                    subIns.data[insPortIndex] = value;
                }
            });
        });
    }

    //更新输入端口映射
    MapOutputPorts = (projectPortName, instance, instancePortIndex) => { 
        let hardwarePortName = this.projectPortsMap.get(projectPortName); 
        let hardwarePortIndex = outputPortsMapping.get(hardwarePortName); //outputPortsMapping引自PortsMap

        if (this.hardwarePortsMap.has(hardwarePortIndex)) {
            let ms = this.hardwarePortsMap.get(hardwarePortIndex)
            
            let found = false;

            for (var i = 0; i < ms.length; i++) {  
                if (ms[i].instance === instance) {
                    ms[i].index = instancePortIndex
                    found = true
                }
            }  

            if (!found) {
                ms.push({instance: instance, index: instancePortIndex})
            }
            // this.hardwarePortsMap.set(hardwarePortIndex, ms);
        } else {
            this.hardwarePortsMap.set(hardwarePortIndex, [{instance: instance, index: instancePortIndex}]);
        }
        
        let project_ports = this.projectInstancePortsMap.get(instance);
        project_ports[instancePortIndex] = projectPortName;
        this.projectInstancePortsMap.set(instance, project_ports);

        console.log(`Set Output Mapping ${instance}.${instancePortIndex} <= ${projectPortName}`);
    }

    //返回projectPortName
    GetProjectInstanceMapping = (instance, instancePortIndex) => { 
        return (this.projectInstancePortsMap.get(instance))[instancePortIndex];
    }

    //返回projectPortName的集合
    GetProjectInstanceMappingAll = (instance) => { 
        return this.projectInstancePortsMap.get(instance);
    }

    //更新输出端口映射
    MapInputPorts = (instance, instancePortIndex ,projectPortName, defaultValue) => { 
        let hardwarePortName = this.projectPortsMap.get(projectPortName);
        let inputIndex = inputPortsMapping.get(hardwarePortName); //inputPortsMapping引自PortsMap
        let map = this.inputPortsMap.get(instance);
        map[instancePortIndex] = inputIndex;
        this.inputPortsMap.set(instance, map);
        this.inputValues[inputIndex] = defaultValue;

        let project_ports = this.projectInstancePortsMap.get(instance);
        project_ports[instancePortIndex] = projectPortName;
        this.projectInstancePortsMap.set(instance, project_ports);

        console.log(`Set Input Mapping ${instance}.${instancePortIndex} => ${projectPortName}`);
    }

    //加载输出组件到输出端口映射上
    Register = (instance, inputCount) => { 
        if (this.inputPortsMap.has(instance)) {
            console.log(`Already Registered Instance: ${instance}, inputCount: ${inputCount}`);
        } else {
            this.inputPortsMap.set(instance, new Array(inputCount).fill(0));
        }
    }

    //卸载输出组件
    UnRegister = (instance) => { 
        this.inputPortsMap.delete(instance);
        console.log(`UnRegister instance: ${instance}`);
    }

    //加载任意组件
    RegisterProjectPorts = (instance, count) => { 
        if (this.projectInstancePortsMap.has(instance)) {
            //已经从项目文件中回复
            console.log(`Already Registered Instance for Project: ${instance}`);
        } else {
            this.projectInstancePortsMap.set(instance, new Array(count).fill(""));
            console.log(`Registered Instance for Project: ${instance}, inputCount: ${count}`);
        }
    }

    //卸载任意组件
    UnRegisterProjectPorts = (instance) => { 
        this.projectInstancePortsMap.delete(instance);
        console.log(`UnRegistered Instance for Project: ${instance}`);
    }

    //产生输入输出端口映射
    GenInputOutputGroup = () => { 
        this.projectPortsMap.forEach((value, key, m) => {
            if (key !== 'clk') {
                let hardwarePortsName = this.projectPortsMap.get(key);

                if (inputPortsMapping.has(hardwarePortsName)) { //输入
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

    //加载输入组件
    Subscribe = (instance, portlist, update) => { 
        this.subscribedInstances.set(instance, {data: new Array(portlist.length).fill(0), refresh: update});
    }

    //卸载输入组件
    UnSubscribe = (instance) => { 
        this.subscribedInstances.delete(instance);
        console.log(`UnSubscribe instance: ${instance}`);
    }

    //将各个输出组件的端口数据加载到FPGA(64bits)
    UpdateInput = (instance, inputs) => { 
        let inputMap = this.inputPortsMap.get(instance); //inputMap是instance的port的index（0 for i7HButton）
        inputs.forEach((data, index, arr) => {
            let inputDex = inputMap[index]; //遍历inputMap
            this.inputValues[inputDex] = data;
        });
    }

    prevTime = 0
    d = new Date()

    // 调用回调函数更新每个组件的界面值
    Cycle = () => {

        const currentTime = this.d.getTime();
        let deltaTime = currentTime - this.prevTime;
        if (this.prevTime === 0) {
            deltaTime = 0
        }
        this.prevTime = currentTime; //单位毫秒

        //console.log(this.subscribedInstances.get("i1").data)

        this.subscribedInstances.forEach((ins) => {
            ins.refresh(ins.data, deltaTime);
        });
    }

    Program = async (bitfile) => {
        this.bitfile = bitfile;
        const response = await fetch('/api/fpga/program?bitfile=' + bitfile);
        const data = await response.json();
        if (data.status === false) {
            new Notification('馄饨FPGA', {
                body: 'Program失败'
            });
        }
        else {
            new Notification('馄饨FPGA', {
                body: 'Program成功'
            });
        }
        return data.status;
    }

    IoOpen = async () => {
        const response = await fetch('/api/fpga/ioopen');
        const data = await response.json();
        if (data.status === false) {
            new Notification('馄饨FPGA', {
                body: 'FPGA连接失败'
            });
        }
        return data.status;
    }

    IoClose = async () => {
        const response = await fetch('/api/fpga/ioclose');
        const data = await response.json();
        return data.status;
    }

    InitIO = async (writeCount, readCount) => {
        this.writeCount = writeCount;
        this.readCount = readCount;
        this.hardwareValues = new Array(readCount*16).fill(0);
        this.inputValues = new Array(writeCount*16).fill(0);
        this.tempi16WriteData = new Array(writeCount).fill(0);
        const response = await fetch('/api/fpga/initio?writeCount='+writeCount+'&readCount='+readCount);
        const data = await response.json();
        return data.status;
    }

    // 从服务器端获取4个int16数, 含64个1bit数
    // 需要将其拆分到一个64位数组里(hardwareValues)
    Split = (i16values) => {
        //遍历所有i16
        console.log("Read: " + i16values);
        i16values.forEach((i16data, index, arr) => {
            //将i16分为16个数分别存储
            for (let dind = 0; dind < 16; dind++) {
                this.hardwareValues[index * 16 + dind] = (i16data >> dind) & 1;
                //console.log("hardware " + parseInt(index * 16 + dind) + ": " + (this.hardwareValues[index * 16 + dind]));
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
        this.WriteLog();
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

        if (!data.status) { //失败
            return false;
        } 

        this.Split(data.data);
        this.Update();

        return true;
    }
}

export const manager = new FPGAManager();
