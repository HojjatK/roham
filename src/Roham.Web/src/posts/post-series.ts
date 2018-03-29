import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {Router} from 'aurelia-router'
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IPostSerie, PostSerieModel} from './../models/PostModel';
import {ISite} from './../models/SiteModel';
import {IZone} from './../models/ZoneModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class PostSeries {
    private selSerie: IPostSerie;
    seriesGrid: any;

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService, private router: Router) {                
    }

    get selectedSerie() {
        return this.selSerie;
    }

    set selectedSerie(value: IPostSerie) {
        this.selSerie = value;
    }

    getSeries(gridArgs) {
        var self = this;
        return this.httpClient.get('api/post/serie')
            .then(response => {
                var series: IPostSerie[] = response.content;
                var seriesCount = series.length;
                if (seriesCount > 0) {
                    self.selectedSerie = series[0];
                }
                return {
                    data: series,
                    count: seriesCount
                };
            });
    }

    newSerie = () => {        
        var self = this;
        self.router.navigate('series/new');
    }
}