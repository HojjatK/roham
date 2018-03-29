import {required, length} from 'aurelia-validatejs';

export interface ICategory {
    uid: string,
    id: number,
    siteId: number,
    siteTitle: string,
    name: string,
    isPublic: boolean,
    description: string,
    parentId: number,    
    parentName: string,
    children: ICategory[]
}

export class CategoryModel implements ICategory {
    uid: string;
    id: number;

    siteId: number;
    siteTitle: string;

    @required
    @length({ minimum: 2, maximum: 50 })
    name: string;

    isPublic: boolean;

    @length({ maximum: 255 })
    description: string;

    parentId: number;    
    parentName: string;
    
    children: ICategory[];

    public fill = (source: ICategory): void => {
        this.uid = source.uid;
        this.id = source.id;
        this.name = source.name;
        this.siteId = source.siteId;
        this.siteTitle = source.siteTitle;
        this.isPublic = source.isPublic;
        this.description = source.description;
        this.parentId = source.parentId;
        this.parentName = source.parentName;
        this.children = [];
        if (source.children != undefined) {
            for (var child of source.children) {
                var newChild = new CategoryModel();
                newChild.fill(child);
                this.children.push(child);
            }
        }
    }
}
