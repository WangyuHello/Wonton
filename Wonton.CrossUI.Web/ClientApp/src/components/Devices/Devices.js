import { LED } from "./LED/LED";
import { LEDCore } from "./LED/LEDCore";
import { HButton } from "./HButton/HButton";
import { HButtonCore } from "./HButton/HButtonCore";

export const deviceMap = new Map([
    ['LED', [LED, LEDCore]],
    ['HButton', [HButton, HButtonCore]]
])

export class Devices {
    constructor (className, opts) {
        return new deviceMap[className][0](opts);
    }
}

export class DeviceCore {
    constructor (className, opts) {
        return new deviceMap[className][1](opts);
    }
}