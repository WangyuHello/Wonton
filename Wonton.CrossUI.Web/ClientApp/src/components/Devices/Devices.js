import { LED } from "./LED/LED";
import { LEDCore } from "./LED/LEDCore";
import { HButton } from "./HButton/HButton";
import { HButtonCore } from "./HButton/HButtonCore";
import { SevenSegmentLED } from "./SevenSegmentLED/SevenSegmentLED";
import { SevenSegmentLEDCore } from "./SevenSegmentLED/SevenSegmentLEDCore";
import { LEDMatrix4t4 } from "./LEDMatrix4t4/LEDMatrix4t4";
import { LEDMatrix4t4Core } from "./LEDMatrix4t4/LEDMatrix4t4Core";

export const deviceMap = new Map([
    ['LED', [LED, LEDCore]],
    ['HButton', [HButton, HButtonCore]],
    ['SevenSegmentLED', [SevenSegmentLED, SevenSegmentLEDCore]],
    ['LEDMatrix4t4', [LEDMatrix4t4, LEDMatrix4t4Core]]
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