import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {Busy} from './../busy';
import {IPortalCache, PortalCacheModel, PortalModel} from './../models/PortalModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class PortalCache extends CaptureModel<PortalCacheModel> {    
    isRedis: boolean = false;

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, private busy: Busy) {
        super(notification);        
        this.viewTitle = 'Cache Settings';
    }    

    public saveCacheSettings = () => {
        let errors = this.controller.validate();        
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public onProviderChange = ($newValue, $oldValue) => {
        this.isRedis = ($newValue != 'Memory');
        this.model.cacheProvider = $newValue;
        this.isDirty = this.originalJson != JSON.stringify(this.model);
    }

    protected getModel = () => {
        var self = this;
        this.httpClient.get('/api/portal/settings/cache')
            .then(response => {
                var data: IPortalCache = response.content;
                if (data != undefined) {
                    self.model = new PortalCacheModel();
                    self.model.fill(data);
                    self.originalJson = JSON.stringify(self.model);
                    self.isRedis = (self.model.cacheProvider != 'Memory');
                    self.isDirty = false;
                }
            });
    }

    protected saveModel = () => {
        var self = this;
        try {
            self.busy.on();
            self.httpClient.put('/api/portal/settings/cache', self.model)
                .then(response => {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Cache Settings saved sucessfully');
                        self.getModel();                        
                    } else {                        
                        self.notification.modalError('Error', result.errorMessages.join('<br/>'));
                    }
                    self.busy.off();
                });
        }
        catch (err) {
            self.busy.off();
        }
    }

    public checkCache = () => {
        var self = this;
        try {
            self.busy.on();
            self.httpClient.post('/api/portal/settings/check-cache', self.model)
                .then(response => {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Caching service is working fine.');                        
                    } else {
                        self.notification.modalError('Error', result.errorMessages.join('<br/>'));
                    }
                    self.busy.off();
                });
        }
        catch (err) {
            self.busy.off();
        }
    }

    public resetCache = () => {
        var self = this;
        self.notification.modalConfirm('Confirmation', 'The whole application cache will be cleared, do you want to proceed?',
            () => {
                try {
                    self.busy.on();
                    self.httpClient.post('/api/portal/settings/reset-cache', self.model)
                        .then(response => {
                            var result: IResult = response.content;
                            if (result.succeed) {
                                self.notification.success('Application Cache cleared sucessfully');                                
                            } else {
                                self.notification.modalError('Error', result.errorMessages.join('<br/>'));
                            }
                            self.busy.off();
                        });
                }
                catch (err) {
                    self.busy.off();
                }
            },
            () => { });
    }
}