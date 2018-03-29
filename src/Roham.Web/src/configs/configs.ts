import {autoinject} from 'aurelia-framework';
import {Router, RouterConfiguration, RouteConfig} from 'aurelia-router'
import {Busy} from './../busy';
import {NavigationService} from './../services/NavigationService';

@autoinject
export class Configs {
    public viewName: string = "configs";    
    router: Router;

    constructor(private navigationService: NavigationService, private busy: Busy) {
    }

    configureRouter(config: RouterConfiguration, router: Router) {
        this.router = router;
        this.router.addRoute({ route: '', name: 'portal', moduleId: './portal', nav: true, title: 'Portal', settings: { icon: '', level: 0 } });
        
        this.addRoute({ route: 'portal-settings-smtp', name: 'portal-settings', moduleId: './portal-smtp', nav: true, title: 'Smtp', settings: { icon: '', level: 0 } });
        this.addRoute({ route: 'portal-settings-cache', name: 'portal-settings', moduleId: './portal-cache', nav: true, title: 'Cache', settings: { icon: '', level: 0 } });
    }

    private addRoute = (routeConfig: RouteConfig) => {
        if (this.navigationService.isAccessibleRoute(routeConfig.name)) {
            this.router.addRoute(routeConfig);
        }
    }
}