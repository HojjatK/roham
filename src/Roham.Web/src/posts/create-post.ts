import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {IPost, PostModel, LinkModel} from './../models/PostModel';
import {ISite} from './../models/SiteModel';
import {IZone} from './../models/ZoneModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';
import {TinyMce} from './../feature-tinymce/tiny-mce';

@autoinject
export class CreatePost extends CaptureModel<PostModel> {    
    sites: any;
    availableSites: any;
    availableZones: any;

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'New Post';
    }

    public createPost = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public populateSites = (callback: { (): void }) => {
        var self = this;
        self.sites = [];
        self.availableSites = [];
        self.httpClient.get(`/api/site`)
            .then(response => {
                if (response.isSuccess) {
                    var sites: ISite[] = response.content;
                    if (sites.length > 0) {
                        for (var s of sites) {
                            self.availableSites.push({ key: s.id, value: s.title });
                        }
                        self.sites = sites;
                        self.model.siteId = sites[0].id;
                    }                    
                    callback();
                }
            });
    }

    public populateZones = (callback: { (): void }) => {
        var self = this;
        
        var selectedSite: ISite;
        for (var s of self.sites) {
            if (s.id == self.model.siteId) {
                selectedSite = s;
                break;
            }
        }
        self.availableZones = [];
        if (selectedSite != undefined && selectedSite.zones != undefined) {
            for (var z of selectedSite.zones) {
                self.availableZones.push({ key: z.id, value: z.title });
            }
        }

        callback();
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

    public onSiteChanged = ($newValue, $oldValue) => {
        var self = this;
        self.model.siteId = $newValue;        
        self.populateZones(() => { });
    }

    protected getModel = () => {
        var self = this;
        self.model = new PostModel();
        self.model.isPrivate = false;
        self.populateSites(() => {
            self.populateZones(() => {
                var tagsInputElm: any = jQuery('#tagsCommaSeparated');
                if (tagsInputElm.length > 0) {
                    tagsInputElm.tagsinput();
                }   

                self.originalJson = JSON.stringify(self.model);
                self.isDirty = false;
            });
        });
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

        this.httpClient.post('/api/post', self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Post created successfully.');
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
}