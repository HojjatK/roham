import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IUser, UserModel} from './../models/UserModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class UpdateUser extends CaptureModel<UserModel> {
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
        this.viewTitle = 'Edit User';
        this.showDelete = true;
    }

    public saveUser = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public deleteUser = () => {
        var self = this;

        if (self.model != undefined && self.model.id > 0) {
            if (self.model.isSystemUser) {
                self.notification.modalInfo('Delete User', 'System user cannot be deleted.');
                return;
            }

            self.notification.modalConfirm('Delete User', 'Are you sure to delete "' + this.model.fullName + '" user?',
                () => {
                    self.deleteModel();
                });
        }
    }

    protected getModel = () => {
        var self = this;
        if (self.uri.id != undefined) {
            var url = `api/user/${self.uri.id}`;
            this.httpClient.get(url)
                .then(response => {
                    self.getAvailableRoles(() => self.getAvailableSites(() => {
                        var data: UserModel = response.content;
                        self.model = new UserModel();
                        self.model.fill(data);
                        self.originalJson = JSON.stringify(self.model);
                        self.selectedRoles = [];
                        for (var r of self.model.roleIdNames) {
                            self.selectedRoles.push(r.id.toString());
                        }

                        this.selectedSites = [];
                        for (var s of self.model.siteIdNames) {
                            self.selectedSites.push(s.id.toString());
                        }
                        self.isDirty = false;                        
                    }));
                });
        }
    }

    protected saveModel = () => {
        var self = this;
        self.model.roleIdNames = [];
        self.model.siteIdNames = [];
        for (var r of self.selectedRoles) {
            self.model.roleIdNames.push({ id: Number(r), name: '' });
        }
        for (var s of self.selectedSites) {
            self.model.siteIdNames.push({ id: Number(s), name: '' });
        }

        this.httpClient.put(`/api/user/${self.model.id}`, self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('User saved successfully');
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
        self.httpClient.delete(`api/user/${self.model.id}`)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('User deleted successfully');
                        self.originalJson = JSON.stringify(self.model);
                        self.isDirty = false;
                        self.goBack();
                    } else {
                        self.notification.modalError('Error', result.errorMessages.join('<br/>'));
                    }
                }
            });
    }

    private getAvailableRoles = (callback: { (): void } ) => {
        this.httpClient.get('/api/role/available')
            .then(response => {
                var data: any = response.content;

                this.availableRoles = [];
                for (var item of data) {
                    this.availableRoles.push({ key: item.id.toString(), value: item.name });
                }
                callback();
            });
    }

    private getAvailableSites = (callback: { (): void } ) => {
        this.httpClient.get('/api/site')
            .then(response => {
                var data: any = response.content;
                this.availableSites = [];
                for (var item of data) {
                    this.availableSites.push({ key: item.id.toString(), value: item.title });
                }
                callback();
            });
    }
}