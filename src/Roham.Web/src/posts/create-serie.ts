import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IPostSerie, PostSerieModel} from './../models/PostModel';
import {ISite} from './../models/SiteModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class CreatePostSerie extends CaptureModel<PostSerieModel> {
    sites: any;
    availableSites: any;    

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'New Post Serie';
    }

    public createPostSerie = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public populateSites = (callback: { (): void }) => {
        var self = this;
        self.sites = [];
        self.availableSites = [];
        self.httpClient.get(`/api/site`)
            .then(response => {
                if (response.isSuccess) {
                    var sites: ISite[] = response.content;
                    if (sites.length > 0) {
                        for (var s of sites) {
                            self.availableSites.push({ key: s.id, value: s.title });
                        }
                        self.sites = sites;
                        self.model.siteId = sites[0].id;
                    }
                    callback();
                }
            });
    }

    public onTitleChanged = (event) => {
        if (this.model != undefined) {
            if (this.model.title == undefined) {
                this.model.name = undefined;
            } else {
                this.model.name = this.slugify(this.model.title);
            }
        }
    }

    public onSiteChanged = ($newValue, $oldValue) => {
        // empty
    }

    protected getModel = () => {
        var self = this;
        self.model = new PostSerieModel();
        self.model.isPrivate = false;
        self.populateSites(() => {
            self.originalJson = JSON.stringify(self.model);
            self.isDirty = false;
        });
    }

    protected saveModel = () => {
        var self = this;
        this.httpClient.post('/api/post/serie', self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Post serie created successfully.');
                        self.originalJson = JSON.stringify(self.model);
                        self.isDirty = false;
                        self.goBack();
                    }
                    else {
                        self.notification.modalError('Error', result.errorMessages.join('<br/>'));
                    }
                }
            });
    }
}