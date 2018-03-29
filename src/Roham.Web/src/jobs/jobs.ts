import {autoinject} from 'aurelia-framework';
import {Router, RouterConfiguration, RouteConfig} from 'aurelia-router'
import {NavigationService} from './../services/NavigationService';

@autoinject
export class Jobs {
    public viewName: string = "jobs";
    navigationService: NavigationService;
    router: Router;

    constructor(navigationService: NavigationService) {
        this.navigationService = navigationService;
    }

    configureRouter(config: RouterConfiguration, router: Router) {
        this.router = router;
        this.router.addRoute({ route: '', name: 'jobs-list', moduleId: './jobs-list', nav: true, title: 'Jobs', settings: { icon: '', level: 0 } });

        this.addRoute({ route: 'new', name: 'create-job', moduleId: './create-job', nav: true, title: 'New Job', settings: { icon: '', level: 0 } });
        this.addRoute({ route: 'edit/:id', name: 'update-job', moduleId: './update-job', nav: false, title: 'Edit Job', settings: { icon: '', level: 0 } });
        //this.addRoute({ route: 'delete', name: 'delete-job', moduleId: './delete-job', nav: false, title: 'Delete Job', settings: { icon: '', level: 0 } });
        //this.addRoute({ route: 'execute', name: 'execute-job', moduleId: './execute-job', nav: false, title: 'Execute Job', settings: { icon: '', level: 0 } });
    }

    private addRoute = (routeConfig: RouteConfig) => {
        if (this.navigationService.isAccessibleRoute(routeConfig.name)) {
            this.router.addRoute(routeConfig);
        }
    }
}