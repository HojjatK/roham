import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IPost, PostModel} from './../models/PostModel';
import {ISite} from './../models/SiteModel';
import {IZone} from './../models/ZoneModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';
import {TinyMce} from './../feature-tinymce/tiny-mce';

@autoinject
export class UpdatePost extends CaptureModel<PostModel> {
    availableSites: any;
    availableZones: any;

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'Edit Post';
        this.showDelete = true;
    }

    public onTitleChanged = (event) => {
        if (this.model != undefined) {
            if (this.model.title == undefined) {
                this.model.name = undefined;
            } else {
                this.model.name = this.slugify(this.model.title);
            }
        }
    }

    public onTemplateChanged = ($newValue, $oldValue) => {
        console.log('template changed');
    }

    public onDisplayPublishChanged = () => {
        // TODO: check display publish is not before create
        console.log('date changed');
    }

    public savePost = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public deletePost = () => {
        var self = this;

        if (self.model != undefined && self.model.id > 0) {
            self.notification.modalConfirm('Delete Post', 'Are you sure to delete "' + this.model.title + '" post?',
                () => {
                    self.deleteModel();
                });
        }
    }

    protected getModel = () => {
        var self = this;
        if (self.uri.id != undefined) {
            var url = `api/post/${self.uri.id}`;
            this.httpClient.get(url)
                .then(response => {
                    var data: PostModel = response.content;
                    self.model = new PostModel();
                    self.model.fill(data);                    

                    var tagsInputElm: any = jQuery('#tagsCommaSeparated');                   
                    if (tagsInputElm.length > 0) {
                        tagsInputElm.val(self.model.tagsCommaSeparated);
                        tagsInputElm.tagsinput();
                    } 

                    self.originalJson = JSON.stringify(self.model);
                    self.isDirty = false;

                   
                });
        }
    }

    protected saveModel = () => {
        var self = this;        
        var me: any = self;
        var links = me.tinyMce.getCssLinks();

        if (links != undefined) {
            self.model.links = [];
            for (var l of links) {
                self.model.links.push({ type: "text/css", ref: l.href });
            }
        }

        var tagsInputElm: any = jQuery('#tagsCommaSeparated');
        self.model.tagsCommaSeparated = tagsInputElm.val();

        this.httpClient.put(`/api/post/${self.model.id}`, self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Post saved successfully');
                        self.originalJson = JSON.stringify(self.model);
                        self.isDirty = false;
                        self.goBack();
                    }
                    else {
                        self.notification.modalError('Error', result.errorMessages.join('<br/>'));
                    }
                }
            });
    }

    protected deleteModel = () => {
        var self = this;
        self.httpClient.delete(`api/post/${self.model.id}`)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Post deleted successfully');
                        self.originalJson = JSON.stringify(self.model);
                        self.isDirty = false;
                        self.goBack();
                    } else {
                        self.notification.modalError('Error', result.errorMessages.join('<br/>'));
                    }
                }
            });
    }
}