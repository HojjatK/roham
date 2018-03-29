import 'jquery';

export interface IDeviceService {
    type(): string;
    isSmall(): boolean;
    isMedium(): boolean;
    isLarge(): boolean;
}

export class DeviceService implements IDeviceService {
    private _callbacks: { (x: IDeviceService): void }[];
    private _deviceType: string;

    constructor() {
        this._deviceType = this.getDeviceType();
        this._callbacks = [];
        
        jQuery(window).resize(() => {
            var currentType = this.getDeviceType();
            if (currentType !== this._deviceType) {
                console.log('Device change from:' + this._deviceType + ' to:' + currentType);
                this._deviceType = currentType;
                this.fireCallbacks();
            }
        });
    }

    public type = () => {
        return this._deviceType;
    }

    public isSmall = () => {
        return this._deviceType == "small";
    }

    public isMedium = () => {
        return this._deviceType == "medium";
    }

    public isLarge = () => {
        return this._deviceType == "large" || this._deviceType == 'xlarge';
    }

    public Register = (callback: { (x: IDeviceService): void }) => {
        this._callbacks.push(callback);
    }

    private getDeviceType = () => {
        return window.getComputedStyle($('body > #device')[0], ':after').content.replace(/[^a-z0-9\s]/gi, '');
    }

    private fireCallbacks = () => {
        var self = this;
        jQuery.each(self._callbacks, function (intex, callback) {
            callback(self);
        });
    }
}