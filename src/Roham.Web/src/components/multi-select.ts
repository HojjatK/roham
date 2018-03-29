import {inject, customElement, bindable, ObserverLocator} from 'aurelia-framework';
import 'bootstrap-multiselect'

@customElement('multi-select')
@inject(Element, ObserverLocator)
export class BootStrapSelectPicker {
    @bindable name: string;
    @bindable selectableValues;
    @bindable selectedValue;    

    options = {
        buttonClass: 'btn btn-select',
        nonSelectedText: '(none)',
        numberDisplayed: 1,
        buttonWidth: '100%'
    };

    constructor(private element, private observerLocator: ObserverLocator) {        
    }

    attached() {
        var self = this;
        var $: any = jQuery;
        var $elm = $(self.element).find('select');
        if ($elm.length > 0) {
            $elm.multiselect(self.options);
            $elm.on('change', function () {
                var values = [];
                for (var sel of $(this).find("option:selected")) {
                    values.push(sel.value);
                }
                self.selectedValue = values;
            });
            this.refresh($elm); 

            self.observerLocator
                .getArrayObserver(self.selectedValue)
                .subscribe(value => self.refreshMultiSelect());    
        }
    }

    detached() {
        var self = this;
        var $: any = jQuery;
        var $elm = $(this.element).find('select');
        if ($elm.length > 0) {
            $elm.multiselect('destroy')
        }
    }

    selectedValueChanged(newValue, oldValue) {
        this.refreshMultiSelect();
    }

    selectableValuesChanged(newValue, oldValue) {
        this.refreshMultiSelect();
    }

    private refreshMultiSelect = () => {
        var $: any = jQuery;
        var $elm = $(this.element).find('select');
        this.refresh($elm);
    }

    private refresh = ($elm) => {
        $elm.val(this.selectedValue);  
        $elm.multiselect('destroy');
        $elm.multiselect(this.options);
    }
}