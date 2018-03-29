import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {Router} from 'aurelia-router'
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {ISite, SiteModel} from './../models/SiteModel';
import {IZone} from './../models/ZoneModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class UpdateSite extends CaptureModel<SiteModel> {
    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService, private router: Router) {
        super(notification, navService);
        this.viewTitle = 'Edit Site';
        this.showDelete = true;
    }
    
    public saveSite = () => {
        let errors = this.controller.validate();        
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public deleteSite = () => {        
        var self = this;

        if (self.model != undefined && self.model.id > 0) {
            if (self.model.isDefault) {
                self.notification.modalInfo('Delete Site', 'Default site cannot be deleted.'); 
                return;
            }

            self.notification.modalConfirm('Delete Site', 'Are you sure to delete "' + this.model.title + '" site?',
                () => {
                    self.deleteModel();
                });
        }
    }

    public siteSettings = () => {
        var self = this;
        if (self.model != undefined) {
            var fragment = `settings/${self.model.id}`;
            self.router.navigate(fragment);
        }
    }

    public addZone = () => {
        var self = this;
        if (self.model != undefined) {
            var fragment = `${self.model.id}/zones/new`;
            this.router.navigate(fragment);
        }
    }

    protected getModel = () => {
        var self = this;
        if (self.uri.id != undefined) {
            var url = `api/site/${self.uri.id}`;
            this.httpClient.get(url)
                .then(response => {
                    var data: SiteModel = response.content;
                    self.model = new SiteModel();
                    self.model.fill(data);
                    self.originalJson = JSON.stringify(self.model);
                    self.isDirty = false;
                });
        }
    }

    protected saveModel = () => {
        var self = this;
        this.httpClient.put(`/api/site/${self.model.id}`, self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Site saved successfully');
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

    protected deleteModel = () => {
        var self = this;
        self.httpClient.delete(`api/site/ ${self.model.id}`)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Site deleted successfully');
                        self.originalJson = JSON.stringify(self.model);
                        self.isDirty = false;
                        self.goBack();
                    } else {
                        self.notification.modalError('Error', result.errorMessages.join('<br/>'));
                    }
                }
            });
    }
}