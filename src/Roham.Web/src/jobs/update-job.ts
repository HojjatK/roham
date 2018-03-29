import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IJob, JobModel} from './../models/JobModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class UpdateJob extends CaptureModel<JobModel> {

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'Edit Job';
        this.showDelete = true;
    }

    public saveJob = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public deleteJob = () => {
        var self = this;

        if (self.model != undefined && self.model.id > 0) {
            if (self.model.isSystemJob) {
                self.notification.modalInfo('Delete Job', 'System job cannot be deleted.');
                return;
            }

            self.notification.modalConfirm('Delete Job', 'Are you sure to delete "' + this.model.name + '" job?',
                () => {
                    self.deleteModel();
                });
        }
    }

    protected getModel = () => {
        var self = this;
        if (self.uri.id != undefined) {
            var url = `api/job/${self.uri.id}`;
            this.httpClient.get(url)
                .then(response => {
                    var data: JobModel = response.content;
                    self.model = new JobModel();
                    self.model.fill(data);
                    self.originalJson = JSON.stringify(self.model);
                    self.isDirty = false;
                });
        }
    }

    protected saveModel = () => {
        var self = this;
        this.httpClient.put(`/api/job/${self.model.id}`, self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Job saved successfully');
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
        self.httpClient.delete(`api/job/${self.model.id}`)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Job deleted successfully');
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