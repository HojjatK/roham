import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {Router} from 'aurelia-router';
import {INavigation, INavItem} from './../models/Navigation';

@autoinject
export class NavigationService {
    private httpClient: HttpClient;
    private router: Router;
    public navigation: INavigation;    

    constructor(httpClient: HttpClient, router: Router) {
        this.httpClient = httpClient;
        this.router = router;
    }

    public initNavigation = (callback: (nav: INavigation) => void) => {
        this.httpClient.get("/api/nav/nav-items")
            .then(response => {
                this.navigation = response.content;

                if (callback != undefined) {
                    callback(this.navigation);
                }
            });
    }    

    public getNavItems = () : INavigation => {
        return this.navigation;
    }

    public isAccessibleRoute = (routeName: string): boolean => {
        for (var navItem of this.navigation.navItems) {
            if (navItem.name == routeName) {
                return true;
            }
            if (this.isAccessible(routeName, navItem)) {
                return true;
            }
        }
        return false;
    }

    public goBack = () => {
        if (this.canGoBack()) {
            this.router.navigateBack();
        }
    }

    private isAccessible = (routeName: string, navItem: INavItem) => {
        for (var item of navItem.subItems) {
            if (item.name == routeName) {
                return true;
            }
            if (this.isAccessible(routeName, item)) {
                return true;
            }
        }
        return false;
    }

    private canGoBack = (): boolean => {
        if (this.router.isNavigating) {
            return false;
        }

        var history: any = this.router.history;
        var previousLocation = history.previousLocation;
        if (previousLocation == undefined || previousLocation == '/') {
            return false;
        }

        return true;
    }
}