
export default class FPGAManager {
    bitfile = ""
    writeCount = 0
    readCount = 0

    subscribedInstances = {}
    hardwarePortsMap = {}
    inputPortsMap = {}

    hardwareValues = []
    hardwarePorts = {}

    inputValues = []

    constructor() {
        this.subscribedInstances = new Map();
        this.hardwarePortsMap = new Map();
        this.inputPortsMap = new Map();
    }

    Update = () => {
        this.hardwarePortsMap.forEach((m, key, map) => {
            const value = this.hardwareValues[key];
            const ins = m.instance;
            const insPort = m.index;

            var subIns = this.subscribedInstances.get(ins);
            subIns.data[insPort] = value;
        });
    }

    MapPorts = (hardwarePortIndex, instance, instancePortIndex) => {
        this.hardwarePortsMap.set(hardwarePortIndex, {instance: instance, index: instancePortIndex});
    }

    MapInputPorts = (instance, instancePortIndex ,inputIndex, defaultValue) => {
        let map = this.inputPortsMap.get(instance);
        map[instancePortIndex] = inputIndex;
        this.inputPortsMap.set(instance, map);
        this.inputValues[inputIndex] = defaultValue;
    }

    Register = (instance, inputCount) => {
        this.inputPortsMap.set(instance, new Array(inputCount).fill(0));
    }

    Subscribe = (instance, portlist, update) => {
        this.subscribedInstances.set(instance, {data: new Array(portlist.length).fill(0), refresh: update});
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
