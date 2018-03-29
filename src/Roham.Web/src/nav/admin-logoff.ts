import {customElement, InlineViewStrategy} from 'aurelia-framework';

@customElement('admin-logoff')
export class AdminLogoff {
    viewStrategy;

    constructor() {
        this.updateViewUrl();
    }

    updateViewUrl() {
        this.loadView().then(res => {
            this.viewStrategy = new InlineViewStrategy(res);
        });
    }

    private loadView = () => {
        return new Promise<string>((resolve, reject) => {
            let http = new XMLHttpRequest();

            http.addEventListener('load', function () {
                resolve(http.responseText);
            });

            http.addEventListener('error', function (e) {
                reject(e);
            });

            http.open('GET', '/AdminLogOffPartial');
            http.send();
        });
    }
}