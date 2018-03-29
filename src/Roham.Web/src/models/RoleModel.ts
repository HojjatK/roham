import {required, length} from 'aurelia-validatejs';

export interface IRole {
    uid: string,
    id: number,
    name: string,    
    description: string,
    isSystemRole: boolean,    
    roleType: string,
    functions: IAppFunction[]
}

export class RoleModel implements IRole {
    uid: string;
    id: number;

    @required
    @length({ minimum: 2, maximum: 50 })
    name: string;

    @length({ maximum: 255 })
    description: string;

    isSystemRole: boolean;   

    @required
    roleType: string;    

    functions: IAppFunction[];

    public fill = (source: IRole): void => {
        this.uid = source.uid;
        this.id = source.id;
        this.name = source.name;        
        this.description = source.description;
        this.isSystemRole = source.isSystemRole;
        this.roleType = source.roleType;        
        this.functions = [];
        if (source.functions != undefined) {
            for (var func of source.functions) {
                var newfunc = new AppFunctionModel();
                newfunc.fill(func);
                this.functions.push(newfunc);
            }
        }
    }
}

export interface IAppFunction {
    uid: string,
    id: number,
    name: string,
    displayName: string,    
    description: string,
    isAllowed: boolean,
    parent: IAppFunction
}

export class AppFunctionModel implements IAppFunction {
    uid: string;
    id: number;

    @required
    name: string;

    displayName: string;
    description: string;
    isAllowed: boolean;

    parent: IAppFunction;

    public fill = (source: IAppFunction): void => {
        this.uid = source.uid;
        this.id = source.id;
        this.name = source.name;
        this.displayName = source.displayName;
        this.description = source.description;
        this.isAllowed = source.isAllowed;
        this.parent = source.parent;
    }
}