import {autoinject} from 'aurelia-framework';
import {Router, RouterConfiguration, RouteConfig} from 'aurelia-router'
import {Busy} from './../busy';
import {NavigationService} from './../services/NavigationService';

@autoinject
export class Sites {    
    public viewName: string = "sites";    
    router: Router;

    constructor(private navigationService: NavigationService, private busy: Busy) {        
    }

    configureRouter(config: RouterConfiguration, router: Router) {        
        this.router = router;        
        this.router.addRoute({ route: '', name: 'sites-list', moduleId: './sites-list', nav: true, title: 'Sites', settings: { icon: '', level: 0 } });

        this.addRoute({ route: 'new', name: 'create-site', moduleId: './create-site', nav: true, title: 'New Site', settings: { icon: '', level: 0 } });
        this.addRoute({ route: 'edit/:id', name: 'update-site', moduleId: './update-site', nav: false,  title: 'Edit Site', settings: { icon: '', level: 0 } });        
        this.addRoute({ route: ':siteId/zones/new', name: 'create-zone', moduleId: './create-zone', nav: false, title: 'New Zone', settings: { icon: '', level: 0 } });
        this.addRoute({ route: ':siteId/zones/edit/:id', name: 'update-zone', moduleId: './update-zone', nav: false, title: 'Edit Zone', settings: { icon: '', level: 0 } });
        this.addRoute({ route: 'settings/:id', name: 'site-settings', moduleId: './site-settings', nav: false, title: 'Site Settings', settings: { icon: '', level: 0 } });        
    }

    private addRoute = (routeConfig: RouteConfig) => {
        if (this.navigationService.isAccessibleRoute(routeConfig.name)) {
            this.router.addRoute(routeConfig);
        }
    }
}