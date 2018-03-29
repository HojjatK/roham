import 'toastr';
import {autoinject} from 'aurelia-framework';
import {DialogService} from 'aurelia-dialog';
import {Prompt, PromptModel} from './../modal/prompt';

export interface INotificationService {
    info(message: string);
    warning(message: string);
    error(message: string);
    success(message: string);

    modalInfo(title: string, message: string);
    modalError(title: string, message: string);
    modalConfirm(title: string, message: string, confirmAction: () => void, cancelAction?: () => void);
}

@autoinject
export class NotificationService implements INotificationService {    
    private _dialogService: DialogService;

    constructor(dialogService: DialogService) {        
        this._dialogService = dialogService;
    }

    info = (message: string) => {
        toastr.options.timeOut = 1000;
        toastr.options.positionClass = "toast-bottom-right";
        toastr.info(message, 'Information');
    }

    warning = (message: string) => {
        toastr.options.timeOut = 1000;
        toastr.options.positionClass = "toast-bottom-right";
        toastr.warning(message, 'Warning');
    }

    error = (message: string) => {
        toastr.options.timeOut = 2000;
        toastr.options.positionClass = "toast-bottom-right";
        toastr.options.closeButton = true;
        toastr.error(message, 'Error');
    }

    success = (message: string) => {
        toastr.options.timeOut = 500;
        toastr.options.positionClass = "toast-bottom-right";
        toastr.options.closeButton = true;
        toastr.success(message, 'Success');
    }

    modalInfo = (title: string, message: string): Promise<any> => {
        var promptModel = new PromptModel();
        promptModel.title = title;
        promptModel.message = message;
        promptModel.type = "info";

        return this._dialogService.open({ viewModel: Prompt, model: promptModel })
    }

    modalError = (title: string, message: string): Promise<any> => {
        var promptModel = new PromptModel();
        promptModel.title = title;
        promptModel.message = message;
        promptModel.type = "error";

        return this._dialogService.open({ viewModel: Prompt, model: promptModel});
    }

    modalConfirm = (title: string, message: string, confirmAction: () => void, cancelAction?: () => void): Promise<any> => {
        return this.modalConfirm2(title, message, confirmAction, null, 'Ok', 'Cancel');
    }

    modalConfirm2 = (title: string, message: string, confirmAction: () => void, cancelAction: () => void, okText: string, cancelText: string): Promise<any> => {
        var promptModel = new PromptModel();
        promptModel.title = title;
        promptModel.message = message;
        promptModel.type = "warning";
        promptModel.okText = okText;
        promptModel.cancelText = cancelText;

        return this._dialogService.open({ viewModel: Prompt, model: promptModel }).then(response => {
            if (!response.wasCancelled) {
                confirmAction();
            } else if (cancelAction != null && cancelAction != undefined) {
                cancelAction();
            }
        });
    }

}