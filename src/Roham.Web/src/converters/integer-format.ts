export class IntegerFormatValueConverter {
    fromView(value) {
        return parseInt(value);
    }
    toView(value) {
        if (value == undefined) {
            return;
        }
        return value.toString();
    }
}