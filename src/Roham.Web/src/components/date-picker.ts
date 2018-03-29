import {inject, bindable, customElement, bindingMode} from "aurelia-framework";
import "jquery";
import moment from 'moment'
import "bootstrap-datepicker";
import "bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css!css";

@inject(Element)
@customElement("date-picker")
export class DatePicker {
    @bindable({ defaultBindingMode: bindingMode.twoWay })
    public options: any;//BootstrapV3DatetimePicker.DatetimepickerOptions;    

    @bindable({ defaultBindingMode: bindingMode.twoWay })
    public readonly: boolean;

    @bindable({ defaultBindingMode: bindingMode.twoWay })
    public value: any;

    @bindable
    public changeDate: Function;

    public element: HTMLElement;
    public dateTimePicker: any;

    constructor(element: HTMLElement) {
        this.element = element;
        this.options = {
            autoclose: true,           
            enableOnReadonly: false, 
            format: 'yyyy-mm-dd',
            language: 'en',
            todayBtn: true,
            todayHighlight: true,
        }; 
    }

    public attached(): void {
        if (this.options == undefined) {
            this.options = {
                autoclose: true,
                enableOnReadonly: false,
                format: 'yyyy-mm-dd',
                language: 'en',
                todayBtn: true,
                todayHighlight: true,
            }; 
        }
        var $input: any = jQuery(this.element).find(".input-group > input");
        this.dateTimePicker = $input.datepicker(this.options);

        if (this.value != undefined) {
            this.value = moment(this.value).format('YYYY-MM-DD');
        }

        this.dateTimePicker.on("changeDate", (e: any) => {
            this.value = moment(e.date).format('YYYY-MM-DD');
            if (this.changeDate != undefined) {
                this.changeDate();
            }
        });
    }

    public detached(): void {
        var $input: any = jQuery(this.element).find(".input-group > input");
        $input.datepicker('destroy');
    }
}