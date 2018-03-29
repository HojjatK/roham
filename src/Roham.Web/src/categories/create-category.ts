import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {ValidationController} from 'aurelia-validation';
import {NotificationService} from './../services/NotificationService';
import {NavigationService} from './../services/NavigationService';
import {ICategory, CategoryModel} from './../models/CategoryModel';
import {ISite} from './../models/SiteModel';
import {IResult} from './../models/ResultModel';
import {CaptureModel} from './../capture-model';

@autoinject
export class CreateCategory extends CaptureModel<CategoryModel> {    
    categories: ICategory[];
    availableParents: any;
    availableSites: any;

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'New Category';
    }

    public onNameChange = (event: any) => {
    }

    public createCategory = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public populateSites = (callback: { (): void }) => {
        var self = this;
        self.availableSites = [];
        self.httpClient.get(`/api/site`)
            .then(response => {
                if (response.isSuccess) {
                    var sites: ISite[] = response.content;
                    var categorySites = [];
                    if (sites.length > 0) {
                        for (var s of sites) {
                            categorySites.push({ key: s.id.toString(), value: s.title });
                        }
                        self.availableSites = categorySites;

                        self.model.siteId = sites[0].id;
                        self.model.siteTitle = sites[0].title;
                    }                    
                    callback();
                }
            });
    }

    public populateParents = (callback: { (): void }) => {
        var self = this;

        self.availableParents = [];
        self.httpClient.get(`/api/category`)
            .then(response => {
                if (response.isSuccess) {
                    self.categories = response.content;

                    var categoryParents = [];
                    categoryParents.push({ key: '', value: '(none)'});
                    if (self.categories.length > 0) {
                        for (var c of self.categories) {
                            if (c.id != self.model.id && c.siteId == self.model.siteId) {
                                categoryParents.push({ key: c.id.toString(), value: c.name });
                            }
                        }
                    }
                    self.availableParents = categoryParents;                    
                    callback();
                }
            });
    }

    public onSiteChanged = ($newValue, $oldValue) => {
        var self = this;
        self.populateParents(() => {});
    }

    protected getModel = () => {
        var self = this;
        self.model = new CategoryModel();
        self.model.isPublic = true;
        self.populateSites(() => {
            self.populateParents(() => {
                self.originalJson = JSON.stringify(self.model);
                self.isDirty = false;
            });
        });
    }

    protected saveModel = () => {
        var self = this;
        this.httpClient.post('/api/category', self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Category created successfully.');
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