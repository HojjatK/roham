import {autoinject} from 'aurelia-framework';
import {DialogController} from 'aurelia-dialog';

@autoinject
export class Prompt {
    controller: DialogController;
    title: string;
    message: string;
    icon: string;
    btnClass: string;
    okTitle: string = 'Ok';
    cancelTitle: string = 'Cancel';
    showCancel: boolean;

    constructor(controller: DialogController) {
        this.controller = controller;
        this.message = null;

        this.controller.settings.centerHorizontalOnly = true;
        this.controller.settings.lock = true;
    }

    activate(model: PromptModel) {
        this.title = model.title;
        this.message = model.message;
        switch (model.type) {
            case "info":
                this.icon = "fa-info-circle";
                this.btnClass = "btn-info";
                this.showCancel = false;
                this.okTitle = 'OK';
                this.cancelTitle = 'Cancel';
                break;
            case "warning":
                this.icon = "fa-exclamation-triangle";
                this.btnClass = "btn-warning";
                this.showCancel = true;
                this.okTitle = model.okText;
                this.cancelTitle = model.cancelText;
                break;
            case "error":
                this.icon = "fa-minus-circle";
                this.btnClass = "btn-danger";
                this.showCancel = false;
                this.okTitle = 'OK';
                this.cancelTitle = 'Cancel';
                break;
        }
    }
}

export type PromptType = "info" | "warning" | "error";

export class PromptModel {
    type: PromptType;
    title: string;
    message: string;
    okText: string = 'Ok';
    cancelText: string = 'Cancel';
}