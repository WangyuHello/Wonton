import { BrowserWindow, app, ipcMain, dialog } from 'electron';
import * as path from 'path'
import { spawn, ChildProcessWithoutNullStreams } from 'child_process';
import * as portscanner from 'portscanner';

let aspcoreProcess: ChildProcessWithoutNullStreams;
let mainWin: Electron.BrowserWindow;

let aspcoreName = 'Wonton.CrossUI.Web';

const currentBinPath = path.join(__dirname.replace('app.asar', ''), 'bin');

//单例
const mainInstance = app.requestSingleInstanceLock();
app.on('second-instance', () => {
    const windows = BrowserWindow.getAllWindows();
    if (windows.length) {
        if (windows[0].isMinimized()) {
            windows[0].restore();
        }
        windows[0].focus();
    }
});

if (!mainInstance) {
    app.quit();
}

app.allowRendererProcessReuse = true;

startBackend();

app.on('ready', () => {
    createWindow();
});

function createWindow() {
    // 创建浏览器窗口
    mainWin = new BrowserWindow({
        width: 1000,
        height: 650,
        frame: false,
        titleBarStyle: "hiddenInset",
        backgroundColor: "#FFF",
        show: true,
        webPreferences: {
            nodeIntegration: true
        }
    });

    // 加载URL
    // let loadURL = `http://localhost:${webPort}`;
    // mainWin.loadURL(loadURL);

    // 加载Skeleton
    const loadSkeletonUrl = path.join(__dirname, 'skeleton.html');
    mainWin.loadURL('file://' + loadSkeletonUrl);

    // mainWin.once('ready-to-show', () => {
    //     mainWin.show()
    // })

    // 打开开发者工具
    //mainWin.webContents.openDevTools();

    mainWin.on("maximize", () => {
        mainWin.webContents.send("window-state-maximize", 1);
    });

    mainWin.on("unmaximize", () => {
        mainWin.webContents.send("window-state-unmaximize", 0);
    });

    mainWin.on("blur", () => {
        mainWin.webContents.send("window-state-blur", 0);
    });

    mainWin.on("focus", () => {
        mainWin.webContents.send("window-state-focus", 0);
    });

    mainWin.on("close", () => {
        // 关闭ASPNET
        aspcoreProcess.kill("SIGKILL");
    });
}

// Quit when all windows are closed.
app.on('window-all-closed', () => {
    // 在 macOS 上，除非用户用 Cmd + Q 确定地退出，
    // 否则绝大部分应用及其菜单栏会保持激活。
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

app.on('activate', () => {
    // 在macOS上，当单击dock图标并且没有其他窗口打开时，
    // 通常在应用程序中重新创建一个窗口。
    if (BrowserWindow.getAllWindows().length === 0) {
        //createWindow();
        startBackend();
    }
});

function startBackend() {
    // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
    portscanner.findAPortNotInUse(8000, 65535, 'localhost', function (error, port) {
        console.log('Electron Socket IO Port: ' + port);
        startSocketApiBridge(port);
    });
}

function startSocketApiBridge(port: number) {
    // 启动gRPC port

    // 然后启动ASP.NET Core
    startAspCoreBackend(port);
}

function startAspCoreBackend(electronPort: number) {
    // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
    portscanner.findAPortNotInUse(electronPort + 1, 65535, 'localhost', function (error, electronWebPort) {
        startBackend(electronWebPort);
    });

    function startBackend(aspCoreBackendPort: number) {
        console.log('ASP.NET Core Port: ' + aspCoreBackendPort);

        const parameters = [`/electronPort=${electronPort}`, `/electronWebPort=${aspCoreBackendPort}`].concat(process.argv);
        let binaryFile = aspcoreName;

        const os = require('os');
        if (os.platform() === 'win32') {
            binaryFile = binaryFile + '.exe';
        }

        let binFilePath = path.join(currentBinPath, binaryFile);
        var options = { cwd: currentBinPath };
        aspcoreProcess = spawn(binFilePath, parameters, options);

        const decoder = new TextDecoder('utf-8');

        aspcoreProcess.stdout.on('data', (data) => {
            let line = decoder.decode(data);
            console.log(line);
            if (line.trim().includes("ELECTRONASPNETCORESTAERTED")) {
                console.log("Captured start signal");
                // 加载URL
                let loadURL = `http://localhost:${aspCoreBackendPort}`;
                mainWin.loadURL(loadURL);
            }
        });
    }
}




ipcMain.on('working-status', (e, arg: boolean) => {
    if (arg) {
        mainWin.setProgressBar(2);
    } else {
        mainWin.setProgressBar(-1);
    }
});

ipcMain.on('window-status', (e, args: string) => {
    switch (args) {
        case "minimize":
            mainWin.minimize();
            break;
        case "maximize":
            if (mainWin.isMaximized()) {
                mainWin.restore();
            } else {
                mainWin.maximize();
            }
            break;
        case "restore":
            mainWin.restore();
            break;
        default:
            break;
    }
});

ipcMain.on('dev-tools', (e) => {
    mainWin.webContents.openDevTools({ mode: "detach" }); 
});

ipcMain.on('open-project-save-folder', async (e) => {
    let dirs = await dialog.showOpenDialog(mainWin,
        {
            title: "选择项目目录",
            properties: ["openDirectory"]
        });
    if (!dirs.canceled && dirs.filePaths.length != 0) {
        mainWin.webContents.send("open-project-save-folder-callback", dirs.filePaths[0]);
    }
});

ipcMain.on('open-bitfile', async (e) => {
    let files = await dialog.showOpenDialog(mainWin,
        {
            title: '选择Bitfile',
            properties: ["openFile"],
            filters: [{ extensions: ['bit'], name: 'Bitfile'}]
        });

    if (files.filePaths.length != 0) {
        mainWin.webContents.send("open-bitfile-callback", files.filePaths[0]);
    }
});

ipcMain.on('open-project-io-file', async (e) => {
    let files = await dialog.showOpenDialog(mainWin,
        {
            title: '选择引脚约束文件',
            properties: ["openFile"],
            filters: [{ extensions: ['xml'], name: '引脚约束文件' }]
        });

    if (files.filePaths.length != 0) {
        mainWin.webContents.send("open-project-io-file-callback", files.filePaths[0]);
    }
});

ipcMain.on('open-project-file', async (e) => {
    let files = await dialog.showOpenDialog(mainWin,
        {
            title: '选择项目文件',
            properties: ["openFile"],
            filters: [{ extensions: ['hwproj'], name: '项目文件' }]
        });

    if (files.filePaths.length != 0) {
        mainWin.webContents.send("open-project-callback", files.filePaths[0]);
    }
});

ipcMain.on("show-unsave-prompt", async (e) => {
    let result = await dialog.showMessageBox(mainWin,
        {
            message: "是否关闭未保存的项目",
            title: "关闭",
            type: "question",
            buttons: ["ok", "cancel"],
            cancelId: 1,
            defaultId: 2
        });

    switch (result.response) {
        case 0:
            mainWin.webContents.send("exit-callback", 1);
            break;
        default:
            break;
    }
});
