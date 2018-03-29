import {inject, customElement, customAttribute, bindable, ObserverLocator} from 'aurelia-framework';
import 'bootstrap-select'

@customAttribute('select-picker-option')
@inject(Element)
export class BootStrapSelectOptionPicker {
    private isLastOption: boolean = false;
    constructor(private element) {
    }

    attached() {
        if (this.isLastOption) {
            var $: any = jQuery;
            var $elm = $(this.element).closest('select');
            if ($elm.length > 0) {
                $elm.selectpicker('refresh');
            }        
            this.isLastOption = false;
        }
    }

    bind(bindingContext, overridingContext) {
        if (overridingContext.$last === true) {
            this.isLastOption = true;
        }
    }
}

@customElement('select-picker')
@inject(Element)
export class BootStrapSelectPicker {
    @bindable name: string;
    @bindable selectableValues;
    @bindable selectedValue;
    @bindable changed;    

    constructor(private element) {        
    }

    attached() {
        this.initPickerElement();
    }

    selectedValueChanged(newValue, oldValue) {                
        this.refreshPickerElement();
        if (this.changed) {
            this.changed({ $newValue: newValue, $oldValue: oldValue });
        }
    }

    selectableValuesChanged(newValue, oldValue) {
        this.refreshPickerElement();
    }

    private initPickerElement = () => {
        var self = this;
        var $: any = jQuery;
        var $elm = $(self.element).find('select');
        if ($elm.length > 0) {
            $elm.selectpicker();
            $elm.on('change', function ($event) {
                self.selectedValue = $(this).find("option:selected").val();
            });
            this.refreshPicker($elm);
        }
    }

    private refreshPickerElement = () => {
        var $: any = jQuery;
        var $elm = $(this.element).find('select');
        this.refreshPicker($elm);
    }

    private refreshPicker = ($elm) => {  
        $elm.val(this.selectedValue);
        $elm.selectpicker('refresh');
    }
}