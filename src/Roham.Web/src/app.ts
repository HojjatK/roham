import {autoinject} from 'aurelia-framework';
import {Router, RouterConfiguration, RouteConfig} from 'aurelia-router'
import {HttpClient} from 'aurelia-http-client';
import {EventAggregator} from 'aurelia-event-aggregator';
import 'bootstrap';
import 'bootstrap-select';
import 'bootstrap-multiselect';
import 'bootstrap-tagsinput';
import {Busy} from './busy';
import {DeviceService, IDeviceService} from './services/DeviceService'
import {NotificationService} from './services/NotificationService';
import {NavigationService} from './services/NavigationService';

export type MenuState = "full" | "partial" | "closed";

@autoinject
export class App {
    copyright: string;
    router: Router;
    navigationService: NavigationService;    
    sideMenuState: MenuState;
    deviceService: DeviceService;
    httpClient: HttpClient;
    ea: EventAggregator;

    constructor(private deviceSvc: DeviceService, httpClient: HttpClient, ea: EventAggregator, notificationService: NotificationService, navigationService: NavigationService, private busy: Busy) {
        var self = this;

        self.deviceService = deviceSvc;        
        self.httpClient = httpClient;
        self.ea = ea;
        self.navigationService = navigationService;
                
        self.deviceService.Register(x => self.onMenuResponse(x));
        self.configureHttp(httpClient, notificationService);
    }
  
    configureRouter(config: RouterConfiguration, router: Router) {     
        config.map([{ route: ['', 'dashboard'], name: 'dashboard', moduleId: './dashboard/dashboard', nav: true, title: 'Dashboard', settings: { icon: 'fa fa-dashboard', level: 0 } }]);

        var self = this;

        self.router = router;
        self.onMenuResponse(this.deviceService);
        self.navigationService.initNavigation(nav => {
            self.addRoute({ route: 'configs', name: 'configs', moduleId: './configs/configs', nav: true, title: 'Configurations', settings: { icon: 'fa fa-gears', level: 0 } });
            self.addRoute({ route: 'sites', name: 'sites', moduleId: './sites/sites', nav: true, title: 'Sites', settings: { icon: 'fa fa-sitemap', level: 0 } });
            self.addRoute({ route: 'roles', name: 'roles', moduleId: './roles/roles', nav: true, title: 'Roles', settings: { icon: 'fa fa-user-secret', level: 0 } });
            self.addRoute({ route: 'users', name: 'users', moduleId: './users/users', nav: true, title: 'Users', settings: { icon: 'fa fa-users', level: 0 } });
            self.addRoute({ route: 'categories', name: 'categories', moduleId: './categories/categories', nav: true, title: 'Categories', settings: { icon: 'fa fa-tags', level: 0 } });
            self.addRoute({ route: 'posts', name: 'posts', moduleId: './posts/posts', nav: true, title: 'Posts', settings: { icon: 'fa fa-pencil-square', level: 0 } });
            self.addRoute({ route: 'jobs', name: 'jobs', moduleId: './jobs/jobs', nav: true, title: 'Jobs', settings: { icon: 'fa fa-tasks', level: 0 } });

            self.router.refreshNavigation();
            config.title = nav.title;            
        });

        
        self.ea.subscribe('router:navigation:complete', response => {
            if (self.sideMenuState == 'full') {
                self.onMenuResponse(self.deviceService);
            }            
        });

        self.setCopyright();
    }

    private addRoute = (routeConfig: RouteConfig) => {
        if (this.navigationService.isAccessibleRoute(routeConfig.name)) {
            this.router.addRoute(routeConfig);
        }
    }

    private configureHttp(httpClient: HttpClient, notificationService: NotificationService) {
        var self = this;
        httpClient.configure(config => {
            config
                .withInterceptor({
                  request(request) {
                      console.log(`Requesting ${request.method} ${request.url}`);
                      return request;
                  },
                  response(response) {
                      console.log(`Received ${response.statusText} ${response.url}`);
                      return response;
                  },
                  requestError(request) {
                      var errorMsg = `Requesting ${request.method} ${request.url} failed`;
                      notificationService.error(errorMsg);
                      return errorMsg;
                  },
                  responseError(response) {                      
                      var errorMsg = response.statusCode + ' : ' + response.statusText + '<br/>' + response.content.message;
                      notificationService.error(errorMsg);
                      if (self.busy != undefined) {
                          self.busy.off();
                      }
                      return errorMsg;
                  }
              });
      });
  }

    private onMenuResponse = ($device: IDeviceService) => {
      console.log('onMenuResponse is called for device: ' + $device.type());
      if ($device.isLarge()) {
          this.setSideMenuState("partial");
      }
      else {
          this.setSideMenuState("closed");
      }
  }

    private setSideMenuState = (value: MenuState) => {
      var oldValue = this.sideMenuState;
      this.sideMenuState = value;
      console.log('Side menu state changed from ' + oldValue + ' to ' + value);
    }

    private setCopyright = () => {
        var self = this;
        self.httpClient.get('api/portal/copyright')
            .then(response => {
                var data: string = response.content;
                if (data != undefined) {
                    self.copyright = data;
                }
            });
    }
}