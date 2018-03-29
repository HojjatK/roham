import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {ISite} from './../models/SiteModel';
import {IZone, ZoneModel} from './../models/ZoneModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class UpdateZone extends CaptureModel<ZoneModel> {
    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'Edit Zone';
        this.showDelete = true;
    }   

    public saveZone = () => {
        let errors = this.controller.validate();
        if (errors.length == 0 && this.model.siteId > 0) {
            this.saveModel();
        }
    }

    public deleteZone = () => {
        var self = this;
        if (self.model != undefined && self.model.siteId > 0 && self.model.id > 0) {
            self.notification.modalConfirm('Delete Zone', 'Are you sure to delete "' + this.model.title + '" zone?',
                () => {
                    self.deleteModel();              
                });
        }
    }

    protected getModel = () => {
        var self = this;
        if (self.uri.id != undefined) {
            var url = `api/site/${self.uri.siteId}/zone/${self.uri.id}`;
            this.httpClient.get(url)
                .then(response => {
                    var data: ZoneModel = response.content;
                    self.model = new ZoneModel();
                    self.model.fill(data);
                    self.originalJson = JSON.stringify(self.model);
                    self.isDirty = false;
                });
        }
    }

    protected saveModel = () => {
        var self = this;
        this.httpClient.put(`/api/site/${self.model.siteId}/zone/${self.model.id}`, self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Zone saved successfully');
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
        self.httpClient.delete(`api/site/${self.model.siteId}/zone/${self.model.id}`)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Zone deleted successfully');
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