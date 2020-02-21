class FakeElectron {
    on = (str, func) => {

    }

    send = (str, args)=> {
        
    }
}

export const ipcRenderer = new FakeElectron();