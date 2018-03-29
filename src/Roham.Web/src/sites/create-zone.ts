import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {ISite, SiteModel} from './../models/SiteModel';
import {IZone, ZoneModel} from './../models/ZoneModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class CreateZone extends CaptureModel<ZoneModel> {    
    public zoneTypes: any = [
        { key: "WebContent", value: "Web Content" },
        { key: "Blog", value: "Blog" },
        { key: "Wikki", value: "Wikki" },
        { key: "News", value: "News " },
        { key: "Documents", value: "Documents" }
    ];    

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'New Zone';
    }

    public onTitleChange = (event: any) => {
        if (this.model.name == undefined || this.model.name == '') {
            this.model.name = this.slugify(this.model.title);
        }
    }

    public createZone = () => {
        let errors = this.controller.validate();        
        if (errors.length == 0 && this.model.siteId > 0) {
            this.saveModel();
        }
    }

    protected getModel = () => {
        var self = this;
        self.model = new ZoneModel();
        self.model.siteId = self.uri.siteId;
        self.model.zoneType = "Blog";
        self.model.isActive = true;
        self.model.isPublic = true;
        self.httpClient.get(`/api/site/${self.uri.siteId}`)
            .then(response => {
                if (response.isSuccess) {
                    var data: ISite = response.content;                    
                    self.model.siteTitle = data.title;
                    self.originalJson = JSON.stringify(self.model);
                    self.isDirty = false;
                }
            });
    }

    protected saveModel = () => {
        var self = this;
        this.httpClient.post(`/api/site/${self.model.siteId}/zone`, self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Zone created successfully');
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