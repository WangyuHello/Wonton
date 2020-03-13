
export default class Darkmode {
    ActivateDarkMode = () => {
        console.log("activate dark mode");
        if(!document.body.classList.contains('darkmode'))
        {
            document.body.classList.add('darkmode');
        }
        if(!document.body.classList.contains('darkmode-body'))
        {
            document.body.classList.add('darkmode-body');
        }
    }
    
    DeactivateDarkMode = () => {
        console.log("deactivate dark mode");
        if(document.body.classList.contains('darkmode'))
        {
            document.body.classList.remove('darkmode');
        }
        if(document.body.classList.contains('darkmode-body'))
        {
            document.body.classList.remove('darkmode-body');
        }
    }
}

export const darkmode = new Darkmode();