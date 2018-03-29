import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IRole, IAppFunction, RoleModel} from './../models/RoleModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class CreateRole extends CaptureModel<RoleModel> {
    availableRoleTypes: any;    

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'New Role';
    }

    public onNameChange = (event: any) => {
    }

    public createRole = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public populateRoleTypes = (callback: { (): void }) => {
        var self = this;

        self.availableRoleTypes = [];
        self.httpClient.get(`/api/role/type`)
            .then(response => {
                if (response.isSuccess) {
                    self.availableRoleTypes = response.content;
                    if (self.availableRoleTypes.length > 0) {
                        self.model.roleType = self.availableRoleTypes[0].key;
                    }
                    callback();
                }
            });
    }

    protected getModel = () => {
        var self = this;
        self.model = new RoleModel();
        self.model.isSystemRole = false;        
        self.populateRoleTypes(() => {
            self.originalJson = JSON.stringify(self.model);
            self.isDirty = false;            
        });        
    }

    protected saveModel = () => {
        var self = this;
        this.httpClient.post('/api/role', self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Role created successfully.');
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