import { LED } from "./LED/LED";
import { LEDCore } from "./LED/LEDCore";
import { HButton } from "./HButton/HButton";
import { HButtonCore } from "./HButton/HButtonCore";
import { SevenSegmentLED } from "./SevenSegmentLED/SevenSegmentLED";
import { SevenSegmentLEDCore } from "./SevenSegmentLED/SevenSegmentLEDCore";

export const deviceMap = new Map([
    ['LED', [LED, LEDCore]],
    ['HButton', [HButton, HButtonCore]],
    ['SevenSegmentLED', [SevenSegmentLED, SevenSegmentLEDCore]]
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