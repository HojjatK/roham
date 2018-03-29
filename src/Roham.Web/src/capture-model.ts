import {NotificationService} from './services/NotificationService';
import {NavigationService} from './services/NavigationService';

export abstract class CaptureModel<TModel> {
    public numberMaxLen: number = 10;
    public passwordMaxLen: number = 20;
    public shortMameMaxLen: number = 32;
    public nameMaxLen: number = 64;
    public emailMaxLen: number = 100;
    public longNameMaxLen: number = 150;
    public descriptionMaxLen: number = 255;
    public longDescriptionMaxLen: number = 1024;
    public pathMaxLen: number = 1024;
    public connectionMaxLen: number = 1024;
    public htmlDescriptionMaxLen: number = 4096;

    viewTitle: string;
    showDelete: boolean = false;
    showUndo: boolean = true;
    isDirty: boolean = false;
    originalJson: string;
    model: TModel;
    protected uri: any;

    constructor(protected notification: NotificationService, protected navService?: NavigationService) {        
    }

    public activate = (uri?: any) => {
        this.uri = uri;
        this.getModel();
    }
    
    public canDeactivate = (): Promise<boolean> => {
        var self = this;
        return new Promise<boolean>((resolve, reject) => {
            var modelJso = JSON.stringify(self.model);
            if (modelJso != self.originalJson) {
                this.notification.modalConfirm2(`${self.viewTitle}`, 'There are unsaved changes, are you sure to navigate away?',
                    () => resolve(true), () => resolve(false), 'Yes', 'No');
            }
            else {
                resolve(true);
            }
        });
    }

    public goBack = () => {
        if (this.navService != undefined) {
            this.navService.goBack();
        }        
    }

    public undo = () => {
        this.getModel();
    }

    protected slugify = (text: string): string => {
        return text.toString().toLowerCase()
            .replace(/\s+/g, '-')           // Replace spaces with -
            .replace(/[^\w\-]+/g, '')       // Remove all non-word chars
            .replace(/\-\-+/g, '-')         // Replace multiple - with single -
            .replace(/^-+/, '')             // Trim - from start of text
            .replace(/-+$/, '');            // Trim - from end of text
    }

    protected getModel = () => {
    }

    protected saveModel = () => {
    }

    protected deleteModel = () => {
    }
}