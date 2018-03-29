import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {ISiteSettings, SiteSettingsModel} from './../models/SiteSettingsModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class SiteSettings extends CaptureModel<SiteSettingsModel> {
    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'Site Settings';
    }

    public saveSettings = () => {
        let errors = this.controller.validate();        
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    protected getModel = () => {
        var self = this;
        this.httpClient.get(`/api/site/settings/${self.uri.id}`)
            .then(response => {
                var data: ISiteSettings = response.content;
                if (data != undefined) {
                    self.model = new SiteSettingsModel();
                    self.model.fill(data);
                    self.originalJson = JSON.stringify(self.model);
                    self.isDirty = false;
                }
            });
    }

    protected saveModel = () => {
        var self = this;
        this.httpClient.put(`/api/site/settings`, self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Site Settings saved');
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