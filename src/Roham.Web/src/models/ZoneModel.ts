import {required, length} from 'aurelia-validatejs';

export interface IZone {
    uid: string,
    id: number,
    siteId: number,
    siteTitle: string,
    siteName: string;
    name: string,
    title: string,
    description: string,
    zoneType: string,
    isActive: boolean,
    isPublic: boolean,
}

export class ZoneModel implements IZone {
    uid: string;
    id: number;

    @required
    siteId: number;

    siteTitle: string;
    siteName: string;

    @required
    @length({ minimum: 2, maximum: 50 })
    name: string;

    @required
    @length({ minimum: 2, maximum: 50 })
    title: string;

    @length({ maximum: 255 })
    description: string;

    @required
    zoneType: string;

    isActive: boolean;

    isPublic: boolean;

    public fill = (source: IZone) => {
        this.uid = source.uid;
        this.id = source.id;
        this.siteId = source.siteId;
        this.siteTitle = source.siteTitle;
        this.siteName = source.siteName;
        this.name = source.name;
        this.title = source.title;
        this.description = source.description;
        this.zoneType = source.zoneType;
        this.isActive = source.isActive;
        this.isPublic = source.isPublic;
    }
}