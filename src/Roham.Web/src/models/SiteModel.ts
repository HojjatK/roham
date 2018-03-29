import {required, length} from 'aurelia-validatejs';
import {IZone, ZoneModel} from './ZoneModel';

export interface ISite {
    uid: string,
    id: number,
    name: string,
    title: string,
    description: string,
    isActive: boolean,
    isDefault: boolean,
    isPublic: boolean,
    siteOwner: string,
    zones: IZone[]
}

export class SiteModel implements ISite {
    uid: string;
    id: number;

    @required
    @length({ minimum: 2, maximum: 50 })
    name: string;

    @required
    @length({ minimum: 2, maximum: 50 })
    title: string;

    @length({ maximum: 255 })
    description: string;

    isActive: boolean;
    isDefault: boolean;
    isPublic: boolean;
    siteOwner: string;
    zones: IZone[]

    public fill = (source: ISite): void => {
        this.uid = source.uid;
        this.id = source.id;
        this.name = source.name;
        this.title = source.title;
        this.description = source.description;
        this.isActive = source.isActive;
        this.isDefault = source.isDefault;
        this.isPublic = source.isPublic;
        this.siteOwner = source.siteOwner;
        this.zones = [];
        if (source.zones != undefined) {
            for (var zone of source.zones) {
                var newZone = new ZoneModel();
                newZone.fill(zone);
                this.zones.push(newZone);
            }
        }
    }
}