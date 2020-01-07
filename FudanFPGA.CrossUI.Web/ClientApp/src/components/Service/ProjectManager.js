import { manager } from './FPGAManager';

export default class ProjectManager {

    projectFile = "E:\\Downloads\\VeriCommSDK-2019-11-22收到\\VeriCommSDK\\Example\\Alarm_Clock\\FDP3P7\\FDE\\src\\AlarClock.hwproj"
    bitfile = "E:\\Documents\\Repo\\ProjectFDB\\FudanFPGAInterface\\FudanFPGA.Test\\AlarmClock_fde_dc.bit"

    projectInfo = {}
    layout = []

    OnRefreshTitle = null
    OnRefreshMainPanel = null

    // Refresh = () => {
    //     this.OnRefreshMainPanel({layout: this.layout});
    //     this.OnRefreshTitle({bitfile: this.bitfile});
    // }

    initialzed = false

    GetPanelData = async () => {
        if (this.initialzed) {
            return {layout: this.layout};
        } else {
            await this.Initialize();
            return {layout: this.layout};
        }
    }

    GetTitleData = async () => {
        if (this.initialzed) {
            return {bitfile: this.bitfile};
        } else {
            await this.Initialize();
            return {bitfile: this.bitfile};
        }
    }

    Initialize = async () => {
        //载入存储在后台进程中的项目文件
        const response = await fetch('/api/fpga/readjson');
        const res = await response.json();

        if (res.message === "") {
            return;
        }
        console.log(res);
        this.projectInfo = JSON.parse(res.message);
        this.projectFile = res.projectPath;
        console.log(`Project Path: ${this.projectFile}`);
        
        this.layout = this.projectInfo['layout'];
        this.bitfile = this.projectInfo['bitfile'];
        manager.subscribedInstances = this.objToMap(this.projectInfo['subscribedInstances']);
        manager.hardwarePortsMap = this.objToMap(this.projectInfo['hardwarePortsMap']);
        manager.inputPortsMap = this.objToMap(this.projectInfo['inputPortsMap']);
        manager.projectInstancePortsMap = this.objToMap(this.projectInfo['projectInstancePortsMap']);
        manager.projectPortsMap = this.objToMap(this.projectInfo['projectPortsMap']);
        //清空
        manager.projectInputPorts.length = 0;
        manager.projectOutputPorts.length = 0;

        this.initialzed = true;
    }

    SetProjectFile = async () => {
        const response = await fetch('/api/fpga/setprojectfile?filename=' + this.projectFile);
        const res = await response.json();
    }

    NewProject = async (projectdir, projectname, projectiofile) => {
        const response = await fetch(`/api/fpga/newproject?projectdir=${projectdir}&projectname=${projectname}&projectiofile=${projectiofile}`);
        const res = await response.json();
    }

    ReadProjectFile = async () => {
        let filename = this.projectFile;

        const response = await fetch('/api/fpga/readjson?filename=' + filename);
        const res = await response.json();
        this.projectInfo = JSON.parse(res.message);
        this.projectFile = res.projectpath;
        this.layout = this.projectInfo['layout'];
        this.bitfile = this.projectInfo['bitfile'];
        manager.subscribedInstances = this.objToMap(this.projectInfo['subscribedInstances']);
        manager.hardwarePortsMap = this.objToMap(this.projectInfo['hardwarePortsMap']);
        manager.inputPortsMap = this.objToMap(this.projectInfo['inputPortsMap']);
        manager.projectInstancePortsMap = this.objToMap(this.projectInfo['projectInstancePortsMap']);
        manager.projectPortsMap = this.objToMap(this.projectInfo['projectPortsMap']);
        //清空
        manager.projectInputPorts.length = 0;
        manager.projectOutputPorts.length = 0;
    }

    ReadProjectIO = async (filename) => {
        const response = await fetch('/api/fpga/readxmltojson?filename=' + filename);
        const res = await response.json();
        let projectIO = JSON.parse(res.message);

        console.log(projectIO);

        let ports = projectIO.design.port;
        manager.projectPortsMap.clear();

        ports.forEach(element => {
            let name = element['@name'];
            let pos = element['@position'];

            manager.projectPortsMap.set(name, pos);
        });

        //清空
        manager.projectInputPorts.length = 0;
        manager.projectOutputPorts.length = 0;
    }

    WriteProjectFile = async () => {
        let filename = this.projectFile;
        console.log(`Project File: ${filename}`);
        this.projectInfo = {
            "subscribedInstances": this.mapToObj(manager.subscribedInstances),
            "hardwarePortsMap": this.mapToObj(manager.hardwarePortsMap),
            "inputPortsMap": this.mapToObj(manager.inputPortsMap),
            "projectInstancePortsMap": this.mapToObj(manager.projectInstancePortsMap),
            "layout": this.layout,
            "projectPortsMap": this.mapToObj(manager.projectPortsMap),
            "bitfile": this.bitfile,
        }

        let transmit = {data: JSON.stringify(this.projectInfo, null, 4)};

        console.log(JSON.stringify(this.projectInfo));

        const response = await fetch('/api/fpga/writejson?filename='+filename,
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(transmit, null, 4)
        });
        const data = await response.json();
    }

    RegisterRefreshTitle = (titlecallback) => {
        this.OnRefreshTitle = titlecallback;
    }

    RegisterRefreshMainPanel = (mainpanelcallback) => {
        this.OnRefreshMainPanel = mainpanelcallback;
    }

    mapToObj = (strmap) => {
        let obj = Object.create(null);
        for (let [k,v] of strmap) {
            obj[k] = v;
        }
        return obj;
    }

    objToMap = (obj) => {
        let map = new Map();
        for(let k of Object.keys(obj)){
            map.set(k, obj[k])
        }
        return map;
    }
}

export const pjManager = new ProjectManager();