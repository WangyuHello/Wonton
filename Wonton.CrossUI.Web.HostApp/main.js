"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
var electron_1 = require("electron");
var path = require("path");
var child_process_1 = require("child_process");
var portscanner = require("portscanner");
var aspcoreProcess;
var mainWin;
var aspcoreName = 'Wonton.CrossUI.Web';
var currentBinPath = path.join(__dirname.replace('app.asar', ''), 'bin');
var mainInstance = electron_1.app.requestSingleInstanceLock();
electron_1.app.on('second-instance', function () {
    var windows = electron_1.BrowserWindow.getAllWindows();
    if (windows.length) {
        if (windows[0].isMinimized()) {
            windows[0].restore();
        }
        windows[0].focus();
    }
});
if (!mainInstance) {
    electron_1.app.quit();
}
startBackend();
function createWindow(webPort) {
    mainWin = new electron_1.BrowserWindow({
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
    var loadURL = "http://localhost:" + webPort;
    mainWin.loadURL(loadURL);
    mainWin.on("maximize", function () {
        mainWin.webContents.send("window-state-maximize", 1);
    });
    mainWin.on("unmaximize", function () {
        mainWin.webContents.send("window-state-unmaximize", 0);
    });
    mainWin.on("blur", function () {
        mainWin.webContents.send("window-state-blur", 0);
    });
    mainWin.on("focus", function () {
        mainWin.webContents.send("window-state-focus", 0);
    });
    mainWin.on("close", function () {
        aspcoreProcess.kill("SIGKILL");
    });
}
electron_1.app.on('window-all-closed', function () {
    if (process.platform !== 'darwin') {
        electron_1.app.quit();
    }
});
electron_1.app.on('activate', function () {
    if (electron_1.BrowserWindow.getAllWindows().length === 0) {
        startBackend();
    }
});
function startBackend() {
    portscanner.findAPortNotInUse(8000, 65535, 'localhost', function (error, port) {
        console.log('Electron Socket IO Port: ' + port);
        startSocketApiBridge(port);
    });
}
function startSocketApiBridge(port) {
    startAspCoreBackend(port);
}
function startAspCoreBackend(electronPort) {
    portscanner.findAPortNotInUse(electronPort + 1, 65535, 'localhost', function (error, electronWebPort) {
        startBackend(electronWebPort);
    });
    function startBackend(aspCoreBackendPort) {
        console.log('ASP.NET Core Port: ' + aspCoreBackendPort);
        var parameters = ["/electronPort=" + electronPort, "/electronWebPort=" + aspCoreBackendPort].concat(process.argv);
        var binaryFile = aspcoreName;
        var os = require('os');
        if (os.platform() === 'win32') {
            binaryFile = binaryFile + '.exe';
        }
        var binFilePath = path.join(currentBinPath, binaryFile);
        var options = { cwd: currentBinPath };
        aspcoreProcess = child_process_1.spawn(binFilePath, parameters, options);
        var decoder = new TextDecoder('utf-8');
        aspcoreProcess.stdout.on('data', function (data) {
            console.log("stdout: " + decoder.decode(data));
        });
        createWindow(aspCoreBackendPort);
    }
}
electron_1.ipcMain.on('working-status', function (e, arg) {
    if (arg) {
        mainWin.setProgressBar(2);
    }
    else {
        mainWin.setProgressBar(-1);
    }
});
electron_1.ipcMain.on('window-status', function (e, args) {
    switch (args) {
        case "minimize":
            mainWin.minimize();
            break;
        case "maximize":
            if (mainWin.isMaximized()) {
                mainWin.restore();
            }
            else {
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
electron_1.ipcMain.on('dev-tools', function (e) {
    mainWin.webContents.openDevTools({ mode: "detach" });
});
electron_1.ipcMain.on('open-project-save-folder', function (e) { return __awaiter(void 0, void 0, void 0, function () {
    var dirs;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4, electron_1.dialog.showOpenDialog(mainWin, {
                    title: "选择项目目录",
                    properties: ["openDirectory"]
                })];
            case 1:
                dirs = _a.sent();
                if (!dirs.canceled && dirs.filePaths.length != 0) {
                    mainWin.webContents.send("open-project-save-folder-callback", dirs.filePaths[0]);
                }
                return [2];
        }
    });
}); });
electron_1.ipcMain.on('open-bitfile', function (e) { return __awaiter(void 0, void 0, void 0, function () {
    var files;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4, electron_1.dialog.showOpenDialog(mainWin, {
                    title: '选择Bitfile',
                    properties: ["openFile"],
                    filters: [{ extensions: ['bit'], name: 'Bitfile' }]
                })];
            case 1:
                files = _a.sent();
                if (files.filePaths.length != 0) {
                    mainWin.webContents.send("open-bitfile-callback", files.filePaths[0]);
                }
                return [2];
        }
    });
}); });
electron_1.ipcMain.on('open-project-io-file', function (e) { return __awaiter(void 0, void 0, void 0, function () {
    var files;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4, electron_1.dialog.showOpenDialog(mainWin, {
                    title: '选择引脚约束文件',
                    properties: ["openFile"],
                    filters: [{ extensions: ['xml'], name: '引脚约束文件' }]
                })];
            case 1:
                files = _a.sent();
                if (files.filePaths.length != 0) {
                    mainWin.webContents.send("open-project-io-file-callback", files.filePaths[0]);
                }
                return [2];
        }
    });
}); });
electron_1.ipcMain.on('open-project-file', function (e) { return __awaiter(void 0, void 0, void 0, function () {
    var files;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4, electron_1.dialog.showOpenDialog(mainWin, {
                    title: '选择项目文件',
                    properties: ["openFile"],
                    filters: [{ extensions: ['hwproj'], name: '项目文件' }]
                })];
            case 1:
                files = _a.sent();
                if (files.filePaths.length != 0) {
                    mainWin.webContents.send("open-project-callback", files.filePaths[0]);
                }
                return [2];
        }
    });
}); });
electron_1.ipcMain.on("show-unsave-prompt", function (e) { return __awaiter(void 0, void 0, void 0, function () {
    var result;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4, electron_1.dialog.showMessageBox(mainWin, {
                    message: "是否关闭未保存的项目",
                    title: "关闭",
                    type: "question",
                    buttons: ["ok", "cancel"],
                    cancelId: 1,
                    defaultId: 2
                })];
            case 1:
                result = _a.sent();
                switch (result.response) {
                    case 0:
                        mainWin.webContents.send("exit-callback", 1);
                        break;
                    default:
                        break;
                }
                return [2];
        }
    });
}); });
