/**
 * Extension Types for the BootstrapV3DatetimePicker tsd files
 * Thanks to : https://github.com/DefinitelyTyped/DefinitelyTyped/tree/master/bootstrap.v3.datetimepicker 
 */

/// <reference path="./globals/jquery/index.d.ts"/>

declare module BootstrapV3DatetimePicker {
    interface DatetimepickerChangeEventObject extends DatetimepickerEventObject {
        oldDate: moment.Moment;
    }

    interface DatetimepickerEventObject extends JQueryEventObject {
        date: moment.Moment;
    }

    interface DatetimepickerIcons {
        time?: string;
        date?: string;
        up?: string;
        down?: string;
        previous?: string;
        next?: string;
        today?: string;
        clear?: string;
        close?: string;
    }

    interface DatetimepickerTooltips {
        today?: string,
        clear?: string,
        close?: string,
        selectMonth?: string,
        prevMonth?: string,
        nextMonth?: string,
        selectYear?: string,
        prevYear?: string,
        nextYear?: string,
        selectDecade?: string,
        prevDecade?: string,
        nextDecade?: string,
        prevCentury?: string,
        nextCentury?: string,
        pickHour?: string,
        incrementHour?: string,
        decrementHour?: string,
        pickMinute?: string,
        incrementMinute?: string,
        decrementMinute?: string,
        pickSecond?: string,
        incrementSecond?: string,
        decrementSecond?: string,
        togglePeriod?: string,
        selectTime?: string
    }

    interface DatetimepickerWidgetPositioning {
        horizontal?: string;
        vertical?: string;
    }

    interface DatetimepickerOptions {
        timeZone?: string;
        format?: string | boolean;
        dayViewHeaderFormat?: string;
        extraFormats?: boolean | Array<string>;
        stepping?: number;
        minDate?: moment.Moment | Date | string;
        maxDate?: moment.Moment | Date | string;
        useCurrent?: boolean;
        collapse?: boolean;
        locale?: moment.Moment | Date | string;
        defaultDate?: moment.Moment | Date | string;
        disabledDates?: Array<moment.Moment | Date | string>;
        enabledDates?: Array<moment.Moment | Date | string>;
        icons?: DatetimepickerIcons;
        tooltips?: DatetimepickerTooltips,
        useStrict?: boolean;
        sideBySide?: boolean;
        daysOfWeekDisabled?: Array<number>;
        calendarWeeks?: boolean;
        viewMode?: string;
        toolbarPlacement?: string;
        showTodayButton?: boolean;
        showClear?: boolean;
        showClose?: boolean;
        widgetPositioning?: DatetimepickerWidgetPositioning;
        widgetParent?: string;
        ignoreReadonly?: boolean;
        keepOpen?: boolean;
        focusOnShow?: boolean;
        inline?: boolean;
        keepInvalid?: boolean;
        datepickerInput?: string;
        keyBinds?: Object;
        debug?: boolean;
        allowInputToggle?: boolean;
        disabledTimeIntervals?: boolean;
        disabledHours?: boolean;
        enabledHours?: boolean;
        viewDate?: boolean;
    }

    interface Datetimepicker {
        date(date: moment.Moment | Date | string): void;
        date(): moment.Moment;
        minDate(date: moment.Moment | Date | string): void;
        minDate(): moment.Moment | boolean;
        maxDate(date: moment.Moment | Date | string): void;
        maxDate(): moment.Moment | boolean;
        show(): void;
        disable(): void;
        enable(): void;
    }

}


interface JQuery {

    datetimepicker(): JQuery;
    datetimepicker(options: BootstrapV3DatetimePicker.DatetimepickerOptions): JQuery;

    off(events: "dp.change", selector?: string, handler?: (eventobject: BootstrapV3DatetimePicker.DatetimepickerChangeEventObject) => any): JQuery;
    off(events: "dp.change", handler: (eventobject: BootstrapV3DatetimePicker.DatetimepickerChangeEventObject) => any): JQuery;

    on(events: "dp.change", selector: string, data: any, handler?: (eventobject: BootstrapV3DatetimePicker.DatetimepickerChangeEventObject) => any): JQuery;
    on(events: "dp.change", selector: string, handler: (eventobject: BootstrapV3DatetimePicker.DatetimepickerChangeEventObject) => any): JQuery;
    on(events: 'dp.change', handler: (eventObject: BootstrapV3DatetimePicker.DatetimepickerChangeEventObject) => any): JQuery;

    off(events: "dp.show", selector?: string, handler?: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;
    off(events: "dp.show", handler: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;

    on(events: "dp.show", selector: string, data: any, handler?: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;
    on(events: "dp.show", selector: string, handler: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;
    on(events: 'dp.show', handler: (eventObject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;

    off(events: "dp.hide", selector?: string, handler?: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;
    off(events: "dp.hide", handler: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;

    on(events: "dp.hide", selector: string, data: any, handler?: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;
    on(events: "dp.hide", selector: string, handler: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;
    on(events: 'dp.hide', handler: (eventObject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;

    off(events: "dp.error", selector?: string, handler?: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;
    off(events: "dp.error", handler: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;

    on(events: "dp.error", selector: string, data: any, handler?: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;
    on(events: "dp.error", selector: string, handler: (eventobject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;
    on(events: 'dp.error', handler: (eventObject: BootstrapV3DatetimePicker.DatetimepickerEventObject) => any): JQuery;

    data(key: 'DateTimePicker'): BootstrapV3DatetimePicker.Datetimepicker;
}