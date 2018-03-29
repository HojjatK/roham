import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {Busy} from './../busy';
import {IPortal, PortalModel} from './../models/PortalModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class Portal extends CaptureModel<PortalModel> {
    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, private busy: Busy) {
        super(notification);
        this.viewTitle = 'Portal Settings';
    }

    public savePortal = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {            
            this.saveModel();
        }        
    }

    protected getModel = () => {
        var self = this;
        this.httpClient.get('/api/portal')
            .then(response => {
                var data: IPortal = response.content;
                if (data != undefined) {
                    self.model = new PortalModel();
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
            self.model.name = self.model.title;
            self.httpClient.put('/api/portal', self.model)
                .then(response => {
                    if (response.isSuccess) {
                        var result: IResult = response.content;
                        if (result != undefined && result.succeed) {
                            self.notification.success('Portal saved sucessfully');
                            self.getModel();                            
                        }
                        else {                            
                            self.notification.modalError('Error', result.errorMessages.join('<br/>'));
                        }
                    }
                    self.busy.off();
                });
        }
        catch(err) {
            self.busy.off();
        }
    }
}