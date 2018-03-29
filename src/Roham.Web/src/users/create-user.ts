import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IUser, UserModel} from './../models/UserModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class CreateUser extends CaptureModel<UserModel> {
    public selectedRoles: Array<string>;
    public selectedSites: Array<string>;
    public availableRoles: any;
    public availableSites: any;
    public availableTitles: any = [
        { key: '(none)', value: '' },
        { key: 'Mr', value: 'Mr' },
        { key: 'Mrs', value: 'Mrs' }
    ];

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'New User';
    }

    public onEmailChange = (event: any) => {
        if (this.model.userName == undefined || this.model.userName == '') {
            this.model.userName = this.model.email;
        }
    }

    public createUser = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    protected getModel = () => {
        var self = this;
        self.model = new UserModel();
        self.model.isSystemUser = false;
        self.originalJson = JSON.stringify(self.model);
        self.isDirty = false;

        self.getAvailableRoles();
        self.getAvailableSites();
    }

    protected saveModel = () => {
        var self = this;
        self.model.roleIdNames = [];
        self.model.siteIdNames = [];
        for (var r of self.selectedRoles) {
            self.model.roleIdNames.push({ id: Number(r), name: ''});
        }
        for (var s of self.selectedSites) {
            self.model.siteIdNames.push({ id: Number(s), name: '' });
        }

        this.httpClient.post('/api/user', self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('User created successfully.');
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

    private getAvailableRoles = () => {
        this.httpClient.get('/api/role/available')
            .then(response => {
                var data: any = response.content;

                this.availableRoles = [];
                for (var item of data) {
                    this.availableRoles.push({ key: item.id.toString(), value: item.name });
                }
                this.selectedRoles = [];
            });
    }

    private getAvailableSites = () => {
        this.httpClient.get('/api/site')
            .then(response => {
                var data: any = response.content;                
                this.availableSites = [];
                for (var item of data) {
                    this.availableSites.push({ key: item.id.toString(), value: item.title });
                }
                this.selectedSites = [];
            });
    }
}
