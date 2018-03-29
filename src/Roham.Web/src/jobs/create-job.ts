import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IJob, JobModel} from './../models/JobModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class CreateJob extends CaptureModel<JobModel> {
    public availableSites: Array<any>;
    // TODO: get this from server
    public availableTypes: any = [        
        { key: 'ImportEntries', value: 'Import Blog Entries' },
        { key: 'ExportEntries', value: 'Export Blog Entries' }
    ];

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'New Job';
    }

    public createJob = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    protected getModel = () => {
        var self = this;
        self.model = new JobModel();
        self.model.isSystemJob = false;
        self.originalJson = JSON.stringify(self.model);
        self.isDirty = false;
        
        self.getAvailableSites();
    }

    protected saveModel = () => {
        var self = this;

        this.httpClient.post('/api/job', self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Job created successfully.');
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

    private getAvailableSites = () => {
        var self = this;
        this.httpClient.get('/api/site')
            .then(response => {
                var data: any = response.content;
                var availableSites = [];
                for (var item of data) {
                    availableSites.push({ key: item.id.toString(), value: item.title });
                }
                self.availableSites = availableSites;
                if (self.availableSites.length > 0) {
                    self.model.siteId = self.availableSites[0].key;
                }
            });
    }
}