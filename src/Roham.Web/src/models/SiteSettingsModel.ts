import {required, length} from 'aurelia-validatejs';

export interface ISiteSettings {
    siteId: number,
    siteTitle: string,
    availableThemes: any,
    theme: string,
    availablePageTemplates: any,
    pageTemplate: string,
    introduction: string,
    htmlHead: string,
    htmlFooter: string,
    mainLinks: string,
    akismetApiKey: string,
    searchAuthor: string,
    searchDescription: string,
    searchKeywords: string,
    spamWords: string,
    smtpFromEmailAddress: string
}

export class SiteSettingsModel implements ISiteSettings {
    siteId: number;
    siteTitle: string;

    availableThemes: any;
    theme: string;

    availablePageTemplates: any;
    pageTemplate: string;

    @length({ maximum: 1024 })
    introduction: string;

    @length({ maximum: 1024 })
    htmlHead: string;

    @length({ maximum: 1024 })
    htmlFooter: string;

    @length({ maximum: 1024 })
    mainLinks: string;

    @length({ maximum: 255 })
    akismetApiKey: string;

    @length({ maximum: 100 })
    searchAuthor: string;

    @length({ maximum: 255 })
    searchDescription: string;

    @length({ maximum: 1024 })
    searchKeywords: string;

    @length({ maximum: 1024 })
    spamWords: string;

    @length({ maximum: 100 })
    smtpFromEmailAddress: string;

    public fill(source: ISiteSettings) {
        this.siteId = source.siteId;
        this.siteTitle = source.siteTitle;
        this.availableThemes = source.availableThemes;
        this.theme = source.theme;
        this.availablePageTemplates = source.availablePageTemplates;
        this.pageTemplate = source.pageTemplate;
        this.introduction = source.introduction;
        this.htmlHead = source.htmlHead;
        this.htmlFooter = source.htmlFooter;
        this.mainLinks = source.mainLinks;
        this.akismetApiKey = source.akismetApiKey;
        this.searchAuthor = source.searchAuthor;
        this.searchDescription = source.searchDescription;
        this.searchKeywords = source.searchKeywords;
        this.spamWords = source.spamWords;
        this.smtpFromEmailAddress = source.smtpFromEmailAddress;
    }
}