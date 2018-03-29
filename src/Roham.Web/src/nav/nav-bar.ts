import {autoinject, bindable, bindingMode, InlineViewStrategy} from 'aurelia-framework';
import {Router} from 'aurelia-router'
import {DeviceService, IDeviceService} from './../services/DeviceService'
import {MenuState} from './../App';

@autoinject
export class NavBar {
    @bindable({ defaultBindingMode: bindingMode.twoWay }) sideMenuState: MenuState;    
    @bindable router: Router;

    public triggerClass: string;
    deviceService: DeviceService;

    constructor(private deviceSvc: DeviceService) {
        this.deviceService = deviceSvc;                        
    }

    activate() {
        console.log('navbar activated');    
    }

    public toggleSidebar = () => {
        if (this.deviceService.isLarge()) {
            if (this.sideMenuState == 'closed') {
                this.sideMenuState  = 'partial';
            }
            else {
                this.sideMenuState  = 'closed';
            }
        }
        else {
            if (this.sideMenuState == 'closed') {
                this.sideMenuState = 'full';
            }
            else {
                this.sideMenuState = 'closed';
            }
        }        
    }
}