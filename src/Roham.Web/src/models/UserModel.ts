import {required, email, length} from 'aurelia-validatejs';

export interface IIdNamePair {
    id: number,
    name: string
}

export class IdNamePair implements IIdNamePair {
    id: number;
    name: string;

    public fill = (source: IIdNamePair): void => {
        this.id = source.id;
        this.name = source.name;
    }
}

export interface IUser {
    uid: string,
    id: number,
    userName: string,
    email: string,
    emailConfirm: boolean,
    title: string,
    givenName: string,
    surname: string,
    fullName: string,
    phoneNumber: string,
    phoneNumberConfirmed: boolean,
    twoFactorEnabled: boolean,
    lockoutEnabled: boolean,
    lockoutEndDateUtc: Date,    
    accessFailedCount: number,
    isSystemUser: boolean,
    status: string,
    statusReason: string,
    passwordHashAlgorithm: string,
    siteIdNames: IIdNamePair[],
    roleIdNames: IIdNamePair[],
}

export class UserModel implements IUser {
    uid: string;
    id: number;

    @required
    @length({ minimum: 2, maximum: 150 })
    userName: string;

    @required
    @email
    @length({ minimum: 2, maximum: 150 })
    email: string;

    emailConfirm: boolean;

    @length({ maximum: 50 })
    title: string;

    @length({ maximum: 150 })
    givenName: string;

    @length({ maximum: 150 })
    surname: string;

    fullName: string;

    @length({ maximum: 30 })
    phoneNumber: string;

    phoneNumberConfirmed: boolean;

    twoFactorEnabled: boolean;

    lockoutEnabled: boolean;

    lockoutEndDateUtc: Date;

    accessFailedCount: number;

    isSystemUser: boolean;

    @length({ maximum: 50 })
    status: string;

    @length({ maximum: 255 })
    statusReason: string;

    @length({ maximum: 20 })
    passwordHashAlgorithm: string;

    siteIdNames: IIdNamePair[];

    roleIdNames: IIdNamePair[];

    public fill = (source: IUser): void => {
        this.uid = source.uid;
        this.id = source.id;
        this.userName = source.userName;
        this.email = source.email;
        this.emailConfirm = source.emailConfirm;
        this.title = source.title;
        this.givenName = source.givenName;
        this.surname = source.surname;
        this.fullName = source.fullName;
        this.phoneNumber = source.phoneNumber;
        this.phoneNumberConfirmed = source.phoneNumberConfirmed;
        this.twoFactorEnabled = source.twoFactorEnabled;
        this.lockoutEnabled = source.lockoutEnabled;
        this.lockoutEndDateUtc = source.lockoutEndDateUtc;
        this.accessFailedCount = source.accessFailedCount;
        this.isSystemUser = source.isSystemUser;
        this.status = source.status;
        this.statusReason = source.statusReason;
        this.passwordHashAlgorithm = source.passwordHashAlgorithm;

        this.siteIdNames = [];
        if (source.siteIdNames != undefined) {
            for (var idName of source.siteIdNames) {
                var newIdName = new IdNamePair();
                newIdName.fill(idName);
                this.siteIdNames.push(idName);
            }
        }

        this.roleIdNames = [];
        if (source.roleIdNames != undefined) {
            for (var idName of source.roleIdNames) {
                var newIdName = new IdNamePair();
                newIdName.fill(idName);
                this.roleIdNames.push(idName);
            }
        }
    }
}