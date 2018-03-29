import {autoinject} from 'aurelia-framework';
import {Router, RouterConfiguration, RouteConfig} from 'aurelia-router'
import {NavigationService} from './../services/NavigationService';

@autoinject
export class Users {
  public viewName: string = "users";
  navigationService: NavigationService;
  router: Router;

  constructor(navigationService: NavigationService) {
      this.navigationService = navigationService;
  }

  configureRouter(config: RouterConfiguration, router: Router) {
      this.router = router;
      this.router.addRoute({ route: '', name: 'users-list', moduleId: './users-list', nav: true, title: 'Users', settings: { icon: '', level: 0 } });

      this.addRoute({ route: 'new', name: 'create-user', moduleId: './create-user', nav: true, title: 'New User', settings: { icon: '', level: 0 } });
      this.addRoute({ route: 'edit/:id', name: 'update-user', moduleId: './update-user', nav: false, title: 'Edit User', settings: { icon: '', level: 0 } });
      //this.addRoute({ route: 'delete', name: 'delete-user', moduleId: './delete-user', nav: false, title: 'Delete User', settings: { icon: '', level: 0 } });      

      this.router.refreshNavigation();
  }

  private addRoute = (routeConfig: RouteConfig) => {
      if (this.navigationService.isAccessibleRoute(routeConfig.name)) {
          this.router.addRoute(routeConfig);
      }
  }
}
