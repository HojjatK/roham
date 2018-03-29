declare module 'aurelia-dialog' {
    export class DialogService {
        open(settings: any): Promise<any>;
    }
    export class DialogController {
        constructor(renderer, settings, resolve, reject);
        ok(result: any): Promise<DialogResult>;
        cancel(result: any): Promise<DialogResult>;
        error(message): Promise<DialogResult>;
        close(ok: boolean, result: any): Promise<DialogResult>;
        settings: { lock: boolean, centerHorizontalOnly: boolean };
    }

    export class DialogResult {
        wasCancelled: boolean;
        output: any;
        constructor(cancelled: boolean, result: any);
    }
}