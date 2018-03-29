import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {ISite, SiteModel} from './../models/SiteModel';
import {IZone} from './../models/ZoneModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class CreateSite extends CaptureModel<SiteModel> {    
    availableUsers: any;

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'New Site';        
    }

    public onTitleChange = (event: any) => {
        if (this.model.name == undefined || this.model.name == '') {
            this.model.name = this.slugify(this.model.title);
        }
    }

    public createSite = () => {
        let errors = this.controller.validate();        
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public populateAdminUsers = (callback: { (): void } ) => {
        var self = this;

        self.availableUsers = [];
        self.httpClient.get(`/api/user/by-role/Administrator`)
            .then(response => {
                if (response.isSuccess) {
                    self.availableUsers = response.content;
                    if (self.availableUsers.length > 0) {
                        self.model.siteOwner = self.availableUsers[0].key;
                    }
                    callback();
                }
            });
    }

    protected getModel = () => {
        var self = this;
        self.model = new SiteModel();
        self.model.isActive = true;
        self.model.isPublic = true;
        self.populateAdminUsers(() => {
            self.originalJson = JSON.stringify(self.model);
            self.isDirty = false;
        });                
    }

    protected saveModel = () => {
        var self = this;
        this.httpClient.post('/api/site', self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Site created successfully.');
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