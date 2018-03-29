import {autoinject} from 'aurelia-framework';
import {Router, RouterConfiguration, RouteConfig} from 'aurelia-router'
import {Busy} from './../busy';
import {NavigationService} from './../services/NavigationService';

@autoinject
export class Roles {
    public viewName: string = "roles";    
    router: Router;

    constructor(private navigationService: NavigationService, private busy: Busy) {
        this.navigationService = navigationService;
    }

    configureRouter(config: RouterConfiguration, router: Router) {
        this.router = router;
        this.router.addRoute({ route: '', name: 'roles-list', moduleId: './roles-list', nav: true, title: 'Roles', settings: { icon: '', level: 0 } });

        this.addRoute({ route: 'new', name: 'create-role', moduleId: './create-role', nav: true, title: 'New Role', settings: { icon: '', level: 0 } });
        this.addRoute({ route: 'edit/:id', name: 'update-role', moduleId: './update-role', nav: false, title: 'Edit Role', settings: { icon: '', level: 0 } });
        //this.addRoute({ route: 'delete', name: 'delete-role', moduleId: './delete-role', nav: false, title: 'Delete Role', settings: { icon: '', level: 0 } });
    }

    private addRoute = (routeConfig: RouteConfig) => {
        if (this.navigationService.isAccessibleRoute(routeConfig.name)) {
            this.router.addRoute(routeConfig);
        }
    }
}