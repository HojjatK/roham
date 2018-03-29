import {inject, customAttribute, TaskQueue} from 'aurelia-framework';

@customAttribute('bootstrap-multiselect')
@inject(Element)
export class BootstrapMultiSelectAttrib {
    public value;

    constructor(private element) {
    }

    attached() { 
        var jq: any = jQuery;
        jq(this.element).multiselect();
    }

    detached() {
        var jq: any = jQuery;
        jq(this.element).multiselect('destroy');
    }
}