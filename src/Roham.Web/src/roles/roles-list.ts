import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {Router} from 'aurelia-router'
import {NotificationService} from './../services/NotificationService';
import {IRole, IAppFunction, RoleModel, AppFunctionModel} from './../models/RoleModel';

@autoinject
export class RolesList {
    rolesGrid: any;
    funcsGrid: any;
    private selRole: IRole;    

    constructor(private httpClient: HttpClient, private notification: NotificationService, private router: Router) {
    }

    get selectedRole() {
        return this.selRole;
    }

    set selectedRole(value: IRole) {
        this.selRole = value;        
        this.funcsGrid.refresh();
    }

    getRoles(gridArgs) {
        var self = this;
        return this.httpClient.get('api/role/functions')
            .then(response => {
                var roles: IRole[] = response.content;
                var rolesCount = roles.length;
                if (rolesCount > 0) {
                    self.selectedRole = roles[0];
                    self.funcsGrid.refresh();             
                }
                return {
                    data: roles,
                    count: rolesCount
                };
            });

    } 

    public editRole = () => {
        var self = this;
        if (self.selectedRole != undefined) {
            var fragment = `edit/${self.selectedRole.id}`;
            this.router.navigate(fragment);
        }
    }

    public getFuncs = (gridArgs) => {
        var self = this;
        return new Promise((resolve, reject) => {
            if (self.selectedRole == undefined) {
                return resolve({
                    data: [],
                    count: 0,
                });
            }

            var funcs = self.selectedRole.functions;
            var funcsCount = funcs.length;            
            return resolve({
                data: funcs,
                count: funcsCount,
            });
        }
        );
    }
}