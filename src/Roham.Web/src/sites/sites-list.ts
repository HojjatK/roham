import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {Router} from 'aurelia-router'
import {NotificationService} from './../services/NotificationService';
import {ISite, SiteModel} from './../models/SiteModel';
import {IZone, ZoneModel} from './../models/ZoneModel';

@autoinject
export default class SitesList {
    sitesGrid: any;
    zonesGrid: any;
    private selSite: ISite;
    private selZone: IZone;

    constructor(private httpClient: HttpClient, private notification: NotificationService, private router: Router) {        
    }

    get selectedSite() {
        return this.selSite;
    }

    set selectedSite(value: ISite) {
        this.selSite = value;
        this.selectedZone = null;
        this.zonesGrid.refresh();
    }

    get selectedZone() {
        return this.selZone;
    }

    set selectedZone(value: IZone) {
        this.selZone = value;
    }

    getSites(gridArgs) {
        var self = this;        
        return this.httpClient.get('api/site')
            .then(response => {
                var sites: ISite[] = response.content;
                var sitesCount = sites.length;
                if (sitesCount > 0) {
                    self.selectedSite = sites[0];
                    self.zonesGrid.refresh();
                }
                return {
                    data: sites,
                    count: sitesCount
                };
            });
            
    } 

    getZones(gridArgs) {
        var self = this;        
        return new Promise((resolve, reject) => {
            if (self.selectedSite == undefined) {
                return resolve({
                    data: [],
                    count: 0,
                });
            }

            var zones = self.selectedSite.zones;
            var zonesCount = zones.length;
            if (zonesCount > 0) {
                self.selectedZone = zones[0];
            }
            return resolve({
                data: zones,
                count: zonesCount,
            });
         }
        );
    }

    public editSite = () => {
        var self = this;
        if (self.selectedSite != undefined) {
            var fragment = `edit/${self.selectedSite.id}`;
            this.router.navigate(fragment);
        }
    }

    public editZone = () => {
        var self = this;
        if (self.selectedSite != undefined && self.selectedZone != undefined) {
            var fragment = `${self.selectedSite.id}/zones/edit/${self.selectedZone.id}`;
            this.router.navigate(fragment);
        }
    }
}