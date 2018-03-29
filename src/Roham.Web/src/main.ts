import 'bootstrap';
import {Aurelia} from 'aurelia-framework';

export function configure(aurelia: Aurelia) {
    aurelia.use
        .standardConfiguration()
        .developmentLogging()
        .plugin('aurelia-dialog')
        .plugin('aurelia-validation')
        .plugin('aurelia-validatejs')
        .plugin('aurelia-animator-css')
        .plugin('aurelia-event-aggregator')
        .plugin('aurelia-chart')
        .plugin('charlespockert/aurelia-bs-grid')        
        .plugin('aurelia-bootstrap-datepicker')
        .feature('bootstrap-validation');

  //Uncomment the line below to enable animation.
  //aurelia.use.plugin('aurelia-animator-css');

  //Anyone wanting to use HTMLImports to load views, will need to install the following plugin.
  //aurelia.use.plugin('aurelia-html-import-template-loader')

  aurelia.start().then(() => aurelia.setRoot());
}
