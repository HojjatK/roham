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
export class UpdateCategory extends CaptureModel<CategoryModel> {
    selectedParentId: string;
    availableParents: any;

    constructor(private controller: ValidationController, private httpClient: HttpClient, notification: NotificationService, navService: NavigationService) {
        super(notification, navService);
        this.viewTitle = 'Edit Category';
        this.showDelete = true;
    }

    public populateParents = (callback: { (): void }) => {
        var self = this;

        self.availableParents = [];
        self.httpClient.get(`/api/category`)
            .then(response => {
                if (response.isSuccess) {
                    var categories: ICategory[] = response.content;

                    var categoryParents = [];
                    categoryParents.push({ key: '', value: '(none)' });
                    if (categories.length > 0) {
                        for (var c of categories) {
                            if (c.id != self.model.id && c.siteId == self.model.siteId && !self.isModelDecendantOf(c, categories)) {
                                categoryParents.push({ key: c.id.toString(), value: c.name });
                            }
                        }
                    }
                    self.availableParents = categoryParents;
                    callback();
                }
            });
    }

    private isModelDecendantOf(category: ICategory, categories: ICategory[]) {
        var target = category;
        var id: number = target.parentId;
        while (id != undefined) {
            if (this.model.id == id) {
                return true;
            }
            target = this.findCategory(id, categories);
            id = target == undefined ? undefined : target.parentId;
        }
        return false;
    }

    private findCategory(id: number, categories: ICategory[]) {
        for (var c of categories) {
            if (id == c.id) {
                return c;
            }
        }
        return null;
    }

    public saveCategory = () => {
        let errors = this.controller.validate();
        if (errors.length == 0) {
            this.saveModel();
        }
    }

    public deleteCategory = () => {
        var self = this;

        if (self.model != undefined && self.model.id > 0) {
            self.notification.modalConfirm('Delete Category', 'Are you sure to delete "' + this.model.name + '" category?',
                () => {
                    self.deleteModel();
                });
        }
    }

    protected getModel = () => {
        var self = this;
        if (self.uri.id != undefined) {
            var url = `api/category/${self.uri.id}`;
            this.httpClient.get(url)
                .then(response => {
                    var data: CategoryModel = response.content;
                    self.model = new CategoryModel();
                    self.model.fill(data);                    

                    self.populateParents(() => {                                                
                        self.selectedParentId = data.parentId == undefined ? '' : '' + data.parentId;
                        self.originalJson = JSON.stringify(self.model);
                        self.isDirty = false;
                    });
                });
        }
    }

    protected saveModel = () => {
        var self = this;
        self.model.parentId = (self.selectedParentId == undefined || self.selectedParentId == '') ? null : +self.selectedParentId;
        this.httpClient.put(`/api/category/${self.model.id}`, self.model)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Category saved successfully');
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
        self.httpClient.delete(`api/category/${self.model.id}`)
            .then(response => {
                if (response.isSuccess) {
                    var result: IResult = response.content;
                    if (result.succeed) {
                        self.notification.success('Category deleted successfully');
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