import { manager } from './FPGAManager';
import _ from 'lodash';

export default class ProjectManager {

    // projectFile = "E:\\Downloads\\VeriCommSDK-2019-11-22收到\\VeriCommSDK\\Example\\Alarm_Clock\\FDP3P7\\FDE\\src\\AlarClock.hwproj"
    projectFile = ""
    // bitfile = "E:\\Documents\\Repo\\ProjectFDB\\FudanFPGAInterface\\FudanFPGA.Test\\AlarmClock_fde_dc.bit"
    bitfile = ""
    projectName = "未命名.hwproj"
    projectInitialize = false

    projectInfo = {}
    layout = []
    layoutNoDevice = []

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
            return {
                bitfile: this.bitfile,
                pjName: this.projectName,
                projectInitialize: this.projectInitialize
            };
        } else {
            await this.Initialize();
            return {
                bitfile: this.bitfile,
                pjName: this.projectName,
                projectInitialize: this.projectInitialize
            };
        }
    }

    //初始化程序
    //从Host拿去数据
    Initialize = async () => {
        //载入存储在后台进程中的项目文件
        const response = await fetch('/api/fpga/init');
        const res = await response.json();

        //没有初始化项目文件,则显示开始屏幕
        if (res.status === false) {
            this.projectName = ""
            this.projectInitialize = false;
            return;
        }
        // console.log(res);
        this.projectInfo = JSON.parse(res.message);
        this.projectFile = res.projectPath;
        console.log(`Project Path: ${this.projectFile}`);
        this.projectInitialize = true;
        
        this.layout = this.projectInfo['layout'];
        this.bitfile = this.projectInfo['bitfile'];
        this.projectName = this.projectInfo['projectName'];
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

    // ReadProjectFile = async () => {
    //     let filename = this.projectFile;

    //     const response = await fetch('/api/fpga/readjson?filename=' + filename);
    //     const res = await response.json();
    //     this.projectInfo = JSON.parse(res.message);
    //     this.projectFile = res.projectpath;
    //     this.layout = this.projectInfo['layout'];
    //     this.bitfile = this.projectInfo['bitfile'];
    //     manager.subscribedInstances = this.objToMap(this.projectInfo['subscribedInstances']);
    //     manager.hardwarePortsMap = this.objToMap(this.projectInfo['hardwarePortsMap']);
    //     manager.inputPortsMap = this.objToMap(this.projectInfo['inputPortsMap']);
    //     manager.projectInstancePortsMap = this.objToMap(this.projectInfo['projectInstancePortsMap']);
    //     manager.projectPortsMap = this.objToMap(this.projectInfo['projectPortsMap']);
    //     //清空
    //     manager.projectInputPorts.length = 0;
    //     manager.projectOutputPorts.length = 0;
    // }

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

    MergeLayoutDevice = (mylayout, mylayoutNoDevice) => {
        //mylayout 中包含Device信息,但是长宽不对
        //mylayoutNoDevice 中包含正确长宽信息,但是不含Device
        
        return _.map(mylayout, (ele) => {
            let ind = _.findIndex(mylayoutNoDevice, se => se.i === ele.i);
            let noDevEle = mylayoutNoDevice[ind];

            ele.x = noDevEle.x;
            ele.y = noDevEle.y;
            ele.w = noDevEle.w;
            ele.h = noDevEle.h;

            return ele;
        });
    }

    WriteProjectFile = async () => {
        let filename = this.projectFile;
        console.log(`Project File: ${filename}`);

        let mergedLayout = this.MergeLayoutDevice(this.layout, this.layoutNoDevice);

        this.projectInfo = {
            "subscribedInstances": this.mapToObj(manager.subscribedInstances),
            "hardwarePortsMap": this.mapToObj(manager.hardwarePortsMap),
            "inputPortsMap": this.mapToObj(manager.inputPortsMap),
            "projectInstancePortsMap": this.mapToObj(manager.projectInstancePortsMap),
            "layout": mergedLayout,
            "projectPortsMap": this.mapToObj(manager.projectPortsMap),
            "bitfile": this.bitfile,
            "projectName": this.projectName
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