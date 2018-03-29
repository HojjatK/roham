import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {Router} from 'aurelia-router'
import {NotificationService} from './../services/NotificationService';
import {IUser, IIdNamePair, UserModel} from './../models/UserModel';

@autoinject
export class UsersList {
    private selUser: IUser;
    usersGrid: any;

    constructor(private httpClient: HttpClient, private notification: NotificationService, private router: Router) {
    }

    get selectedUser() {
        return this.selUser;
    }

    set selectedUser(value: IUser) {
        this.selUser = value;        
    }

    getUsers(gridArgs) {
        var self = this;
        return this.httpClient.get('api/user')
            .then(response => {
                var users: IUser[] = response.content;
                var usersCount = users.length;
                if (usersCount > 0) {
                    self.selectedUser = users[0];                    
                }
                return {
                    data: users,
                    count: usersCount
                };
            });

    }

    public editUser = () => {
        var self = this;
        if (self.selectedUser != undefined) {
            var fragment = `edit/${self.selectedUser.id}`;
            this.router.navigate(fragment);
        }
    }
}