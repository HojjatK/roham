import {autoinject} from 'aurelia-framework';
import {Router, RouterConfiguration, RouteConfig} from 'aurelia-router'
import {NavigationService} from './../services/NavigationService';

@autoinject
export class Categories {
    public viewName: string = "categories";
    navigationService: NavigationService;
    router: Router;

    constructor(navigationService: NavigationService) {
        this.navigationService = navigationService;
    }

    configureRouter(config: RouterConfiguration, router: Router) {
        this.router = router;
        this.router.addRoute({ route: '', name: 'categories-list', moduleId: './categories-list', nav: true, title: 'Categories', settings: { icon: '', level: 0 } });

        this.addRoute({ route: 'new', name: 'create-category', moduleId: './create-category', nav: true, title: 'New Category', settings: { icon: '', level: 0 } });
        this.addRoute({ route: 'edit/:id', name: 'update-category', moduleId: './update-category', nav: false, title: 'Edit Category', settings: { icon: '', level: 0 } });        
    }

    private addRoute = (routeConfig: RouteConfig) => {
        if (this.navigationService.isAccessibleRoute(routeConfig.name)) {
            this.router.addRoute(routeConfig);
        }
    }
}