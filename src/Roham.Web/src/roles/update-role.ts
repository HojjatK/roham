import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {Router} from 'aurelia-router'
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IRole, IAppFunction, RoleModel} from './../models/RoleModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class UpdateRole extends CaptureModel<RoleModel> {
    funcsGrid: any;
    private selFunc: IAppFunction;

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService, private router: Router) {
        super(notification, navService);
        this.viewTitle = 'Edit Role';
        this.showDelete = true;
    }

    get selectedFunc() {
        return this.selFunc;
    }

    set selectedFunc(value: IAppFunction) {        
        this.selFunc = value;

        // find a better way to find out isallowed has been changed
        if (!this.isDirty && this.selFunc != undefined && this.selFunc.isAllowed != undefined &&
            value != undefined && value.isAllowed != undefined) {
            if (JSON.stringify(this.model) != this.originalJson) {
                this.isDirty = true;
            }
        }
    }

    public saveRole = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public deleteRole = () => {
        var self = this;

        if (self.model != undefined && self.model.id > 0) {
            if (self.model.isSystemRole) {
                self.notification.modalInfo('Delete Role', 'System role cannot be deleted.');
                return;
            }

            self.notification.modalConfirm('Delete Role', 'Are you sure to delete "' + this.model.name + '" role?',
                () => {
                    self.deleteModel();
                });
        }
    }

    protected getModel = () => {
        var self = this;
        if (self.uri.id != undefined) {
            var url = `api/role/${self.uri.id}/functions`;
            this.httpClient.get(url)
                .then(response => {
                    var data: RoleModel = response.content;
                    self.model = new RoleModel();
                    self.model.fill(data);
                    self.originalJson = JSON.stringify(self.model);
                    self.isDirty = false;
                    self.funcsGrid.refresh();
                });
        }
    }

    public getFuncs = (gridArgs) => {
        var self = this;
        return new Promise((resolve, reject) => {
            if (self.model == undefined) {
                return resolve({
                    data: [],
                    count: 0,
                });
            }

            var funcs = self.model.functions;
            var funcsCount = funcs.length;
            if (funcsCount > 0) {
                self.selectedFunc = funcs[0];
            }
            return resolve({
                data: funcs,
                count: funcsCount,
            });
        }
        );
    }

    protected saveModel = () => {
        var self = this;
        this.httpClient.put(`/api/role/${self.model.id}/functions`, self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Role saved successfully');
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
        self.httpClient.delete(`api/role/${self.model.id}`)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Role deleted successfully');
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
