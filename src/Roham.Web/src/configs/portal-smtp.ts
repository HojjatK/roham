import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {Busy} from './../busy';
import {IPortalSmtp, PortalSmtpModel} from './../models/PortalModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class PortalSmtp extends CaptureModel<PortalSmtpModel> {
    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, private busy: Busy) {
        super(notification);
        this.viewTitle = 'Smtp Settings';
    }

    public saveSmtpSettings = () => {
        let errors = this.controller.validate();        
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    protected getModel = () => {
        var self = this;
        this.httpClient.get('/api/portal/settings/smtp')
            .then(response => {
                var data: IPortalSmtp = response.content;
                if (data != undefined) {
                    self.model = new PortalSmtpModel();
                    self.model.fill(data);
                    self.originalJson = JSON.stringify(self.model);
                    self.isDirty = false; 
                }
            });
    }

    protected saveModel = () => {
        var self = this;
        try {
            self.busy.on();
            this.httpClient.put('/api/portal/settings/smtp', self.model)
                .then(response => {
                    if (response.isSuccess) {
                        var result: IResult = response.content;
                        if (result.succeed) {
                            self.notification.success('Smtp Settings saved successfully');
                            self.getModel();                            
                        } else {
                            self.notification.modalError('Error', result.errorMessages.join('<br/>'));
                        }                        
                    }
                    self.busy.off();
                });
        }
        catch (err) {
            self.busy.off();
        }
    }
}