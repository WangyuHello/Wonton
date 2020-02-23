class FakeipcRenderer {
    on = (str, func) => {

    }

    send = (str, args)=> {
        
    }
}

class Fakeshell {

}

export const ipcRenderer = new FakeipcRenderer();
export const shell = new Fakeshell();