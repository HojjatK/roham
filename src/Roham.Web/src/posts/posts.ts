import {autoinject} from 'aurelia-framework';
import {Router, RouterConfiguration, RouteConfig} from 'aurelia-router'
import {NavigationService} from './../services/NavigationService';

@autoinject
export class Posts {
    public viewName: string = "posts";
    navigationService: NavigationService;
    router: Router;

    constructor(navigationService: NavigationService) {
        this.navigationService = navigationService;
    }

    configureRouter(config: RouterConfiguration, router: Router) {
        this.router = router;
        this.router.addRoute({ route: '', name: 'posts-list', moduleId: './posts-list', nav: true, title: 'Posts', settings: { icon: '', level: 0 } });

        this.addRoute({ route: 'new', name: 'create-post', moduleId: './create-post', nav: true, title: 'New Post', settings: { icon: '', level: 0 } });
        this.addRoute({ route: 'edit/:id', name: 'save-post', moduleId: './update-post', nav: false, title: 'Update Post', settings: { icon: '', level: 0 } });
        this.addRoute({ route: 'comments', name: 'comments', moduleId: './post-comments', nav: true, title: 'Comments', settings: { icon: '', level: 0 } });        
        this.addRoute({ route: 'comments/edit/:id', name: 'update-comment', moduleId: './update-comment', nav: false, title: 'Update Comment', settings: { icon: '', level: 0 } });        
        this.addRoute({ route: 'series', name: 'post-series', moduleId: './post-series', nav: true, title: 'Series', settings: { icon: '', level: 0 } });
        this.addRoute({ route: 'series/new', name: 'create-post-serie', moduleId: './create-serie', nav: false, title: 'New Serie', settings: { icon: '', level: 0 } });
    }

    private addRoute = (routeConfig: RouteConfig) => {
        if (this.navigationService.isAccessibleRoute(routeConfig.name)) {
            this.router.addRoute(routeConfig);
        }
    }
}