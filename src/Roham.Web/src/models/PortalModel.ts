import {required, length, numericality} from 'aurelia-validatejs';

export interface IPortal {    
    name: string;
    title: string;
    description: string;
    databaseInfo: string;
    settings: IPortalSettings;
}

export class PortalModel implements IPortal {    
    @required
    @length({ minimum: 2, maximum: 64 })
    name: string = '';

    @required
    @length({ minimum: 2, maximum: 64 })
    title: string = '';

    @length({ maximum: 255 })
    description: string = '';
    
    databaseInfo: string = '';

    settings: IPortalSettings;

    public fill = (source: IPortal) => {
        this.name = source.name;
        this.title = source.title;
        this.description = source.description;
        this.databaseInfo = source.databaseInfo;

        this.settings = new PortalSettingsModel();
        this.settings.fill(source.settings);       
    }
}

export interface IPortalSettings {    
    storageProvider: string;
    uploadPath: string;
    storageConnectionString: string;
    blobContainerName: string;

    adminTheme: string;
    availableThemes: any;
    availableStorageProviders: any;

    fill(source: IPortalSettings) : void;
}

export class PortalSettingsModel implements IPortalSettings {
    @length({ maximum: 150 })
    storageProvider: string;

    @length({ maximum: 1024 })
    uploadPath: string;

    @length({ maximum: 1024 })
    storageConnectionString: string;

    @length({ maximum: 150 })
    blobContainerName: string;

    @length({ maximum: 150 })
    adminTheme: string;

    availableThemes: any;
    availableStorageProviders: any;

    public fill = (source: IPortalSettings) => {
        this.storageProvider = source.storageProvider;
        this.uploadPath = source.uploadPath;
        this.storageConnectionString = source.storageConnectionString;
        this.blobContainerName = source.blobContainerName;
        this.adminTheme = source.adminTheme;
        this.availableThemes = source.availableThemes;
        this.availableStorageProviders = source.availableStorageProviders;
    }
}

export interface IPortalSmtp {
    smtpEnabled: boolean;
    smtpHost: string;
    smtpPort: number;
    smtpUsername: string;
    smtpPassword: string;
    smtpUseSsl: boolean;
    smtpFrom: string;
}

export class PortalSmtpModel implements IPortalSmtp {
    smtpEnabled: boolean;

    @length({ maximum: 150 })
    smtpHost: string;

    @numericality({ onlyInteger: true, lessThan: 65536, greaterThan: 0 }) 
    smtpPort: number;

    @length({ maximum: 64 })
    smtpUsername: string;

    @length({ maximum: 32 })
    smtpPassword: string;

    smtpUseSsl: boolean;

    @length({ maximum: 100 })
    smtpFrom: string;

    public fill = (source: IPortalSmtp) => {
        this.smtpEnabled = source.smtpEnabled;
        this.smtpHost = source.smtpHost;
        this.smtpPort = source.smtpPort;
        this.smtpUsername = source.smtpUsername;
        this.smtpPassword = source.smtpPassword;
        this.smtpUseSsl = source.smtpUseSsl;
        this.smtpFrom = source.smtpFrom;
    }
}

export interface IPortalCache {
    extCacheEnabled: boolean;
    cacheProvider: string;
    cacheHost: string;
    cachePort: number;
    cachePassword: string;
    cacheUseSsl: boolean;
    availableCacheProviders: any;
}

export class PortalCacheModel implements IPortalCache {
    extCacheEnabled: boolean;

    @required
    @length({ maximum: 150 })
    cacheProvider: string;

    @length({ maximum: 150 })
    cacheHost: string;

    @numericality({ onlyInteger: true, lessThan: 65536, greaterThan: 0 }) 
    cachePort: number;

    @length({ maximum: 32 })
    cachePassword: string;

    cacheUseSsl: boolean;
    availableCacheProviders: any;

    public fill = (source: IPortalCache) => {        
        this.extCacheEnabled = source.extCacheEnabled;
        this.cacheProvider = source.cacheProvider;
        this.cacheHost = source.cacheHost;
        this.cachePort = source.cachePort;
        this.cachePassword = source.cachePassword;
        this.cacheUseSsl = source.cacheUseSsl;        
        this.availableCacheProviders = source.availableCacheProviders;
    }
}

