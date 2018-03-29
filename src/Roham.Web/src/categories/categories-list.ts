import {autoinject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';
import {Router} from 'aurelia-router'
import {NotificationService} from './../services/NotificationService';
import {ISite} from './../models/SiteModel';
import {ICategory, CategoryModel} from './../models/CategoryModel';
import {NodeModel, ColumnModel} from './../components/treelist/node-model';

@autoinject
export class CategoriesList {
    availableSites: any;
    selectedSiteId: string;     
    nodes = [];
    columns: ColumnModel[];
    categories: ICategory[];
    selectedCategory: ICategory;

    constructor(private httpClient: HttpClient, private notification: NotificationService, private router: Router) {
        this.columns = [];
        this.columns.push({ name: 'title', type: "string", displayName: 'Name', class: 'col-4', nav: ''});
        this.columns.push({ name: 'isPublic', type: 'bool', displayName: 'Public', class: 'col-1', nav: ''});
        this.columns.push({ name: 'description', type: 'string', displayName: 'Description', class: 'col-4', nav:''});
        this.columns.push({ name: 'id', type: 'navigation', displayName: 'Action', class: 'col-2', nav: 'admin#/categories/edit'});        
    }

    attached() {
        var self = this;
        self.populateSites(() => self.onSiteChanged(self.selectedSiteId, null));
    }

    onCollapsed(e) {
        console.log(`node collapsed: ${e.detail.node.title}`);
    }

    onExpanded(e) {
        console.log(`node expanded: ${e.detail.node.title}`);
    }

    onFocus(e) {
        console.log(`node focused: ${e.detail.node.title}`);
    }

    onSelect(e) {
        console.log('[sample] events - ]', e);
        let titles = e.detail.nodes.map(node => node.title).join(', ');        
        console.log(`node selected: ${titles}`);

        let category: ICategory = e.detail.nodes[0].payload;
        this.selectedCategory = category;
    }

    onDblClicked(e) {
        let nodeModel = e.detail.node;
        if (nodeModel != undefined) {
            var model: ICategory = nodeModel.payload;
            if (model != undefined) {
                this.router.navigate('edit/' + model.id);
            }
        }
        
    }

    public populateSites = (callback: { (): void }) => {
        var self = this;
        self.availableSites = [];
        self.httpClient.get(`/api/site`)
            .then(response => {
                if (response.isSuccess) {
                    var sites: ISite[] = response.content;
                    if (sites.length > 0) {
                        for (var s of sites) {
                            self.availableSites.push({ key: s.id.toString(), value: s.title });
                        }
                        self.selectedSiteId = sites[0].id.toString();
                    }
                    callback();
                }
            });
    }

    public onSiteChanged = ($newValue, $oldValue) => {
        var self = this;
        self.selectedSiteId = $newValue;
        self.selectedCategory = null;
        if (self.selectedSiteId != undefined) {
            var siteId = +self.selectedSiteId;
            self.getModel(siteId, () => {
                var nn = [];
                for (var category of self.categories) {
                    var childItems: NodeModel[] = self.getChildTreeItems(category);                    
                    var nodeItem = new NodeModel(category.name, childItems, category);
                    nn.push(nodeItem);
                }
                self.nodes = nn;                
            });
        }
        else {
            self.nodes = [];
        }
    }

    getModel = (siteId: number, callback: { (): void }) => {
        var self = this;
        return self.httpClient.get(`api/category/tree/${siteId}`)
            .then(response => {
                self.categories = response.content;
                if (self.categories != undefined && self.categories.length > 0) {                    
                    self.selectedCategory = self.categories[0];
                }
                callback();       
            });
    }

    private getChildTreeItems = (category: ICategory): NodeModel[] => {
        var childItems: NodeModel[] = [];

        if (category.children != undefined && category.children.length > 0) {
            for (var childCategory of category.children) {
                var gradChildren = this.getChildTreeItems(childCategory);                
                childItems.push(new NodeModel(childCategory.name, gradChildren, childCategory));
            }
        }
        return childItems;
    }
}