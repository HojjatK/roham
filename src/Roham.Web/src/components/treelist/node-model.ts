import {computedFrom, observable} from 'aurelia-framework';
import {TreeList} from './tree-list'

export type ColumnType = "number" | "string" | "bool" | "date" | "navigation";
export class ColumnModel {
    name: string;
    type: ColumnType;
    displayName: string;
    class: string;
    nav: string = '';    
}

export class NodeModel {    
    tree: TreeList;
    children: NodeModel[];
    childrenGetter: { (): Promise<NodeModel[]> };

    visible = true;
    expanded = false;

    @observable() focused = false;
    @observable() selected = false;
    loading = false;
    template = null;
    templateModel = null;
    private parent = null;

    static createFromJSON(nodes: any[]) {
        let models = [];
        nodes.forEach(node => {
            let children = node.children;
            if (typeof children === 'function') {
                // create promise wrapper so children are of type NodeModel
                children = () => {
                    return new Promise((resolve, reject) => {
                        node.children().then(ch => {
                            resolve(NodeModel.createFromJSON(ch));
                        });
                    });
                };
            } else {
                children = node.children ? NodeModel.createFromJSON(node.children) : null;
            }
            let payload = node.payload;
            if (!payload) {
                payload = {};
                let keys = Object.keys(node);
                keys.forEach(key => {
                    switch (key) {
                        case 'children':
                        case 'title':
                            break;
                        default:
                            payload[key] = node[key];
                            break;
                    }
                });
            }
            models.push(new NodeModel(node.title, children, payload));
        });
        return models;
    }

    constructor(private title: string, children?: NodeModel[] | { (): Promise<NodeModel[]> }, public payload?: any) {
        if (typeof children === 'function') {
            this.childrenGetter = children as { (): Promise<NodeModel[]> };
        } else {
            this.children = (children || []) as NodeModel[];
        }
        if (this.hasChildren) {
            this.collapseNode(true);
            this.setParents(this, this.children);
        }
    }

    @computedFrom('children')
    get hasChildren() {
        let result = false;
        if (this.children) {
            result = this.children.length > 0;
        } else {
            result = true;
        }
        return result;
    }

    get level() {
        var lev: number = 0;
        var parent: NodeModel = this.parent;
        while (parent != undefined) {
            lev = lev + 1;
            parent = parent.parent;
        }
        return lev;
    }

    collapseNode(force = false) {
        this.collapseNodeRecursive(force, this);
        return Promise.resolve();
    }

    private collapseNodeRecursive(force = false, parent: NodeModel) {
        if (parent.children && (parent.expanded || force)) {
            parent.children.forEach(child => {
                child.visible = false;
                this.collapseNodeRecursive(force, child);
            });
            parent.expanded = false;
        }
    }

    expandNode(force = false) {
        if (!this.expanded || force) {
            let promise: Promise<any>;
            if (this.childrenGetter) {
                this.loading = true;
                promise = this.childrenGetter().then(children => {
                    children.forEach(child => {
                        if (this.template) {
                            child.template = this.template;
                        }
                        child.tree = this.tree;
                    });
                    this.children = children;
                });
            } else {
                promise = Promise.resolve();
            }
            return promise.then(() => {
                this.loading = false;
                this.children.forEach(child => {
                    child.visible = true;
                });
                this.expanded = true;
            });
        }
    }

    selectedChanged(newValue, oldValue) {
        if (this.tree != undefined && newValue !== oldValue) {
            if (newValue) {
                this.tree.selectNode(this);
            } else if (newValue === false) {
                this.tree.deselectNode(this);
            }
        }
    }

    toggleFocus() {
        this.focused = !this.focused;
    }

    focusedChanged(newValue, oldValue) {
        if (this.tree != undefined) {
            this.tree.focusNode(this);
        }
    }

    toggleSelected() {
        this.selected = !this.selected;
    }

    selectChildren(recursive = false) {
        let promise;
        if (this.expanded) {
            promise = Promise.resolve();
        } else {
            promise = this.expandNode();
        }
        return promise.then(() => {
            let childPromises = [];
            this.children.forEach(child => {
                child.selected = true;
                if (recursive) {                    
                    childPromises.push(child.selectChildren(recursive));
                }
            });
            return Promise.all(childPromises);
        });
    }

    deselectChildren(recursive = false) {
        let promise;
        if (this.expanded) {
            promise = Promise.resolve();
        } else {
            promise = this.expandNode();
        }
        return promise.then(() => {
            let childPromises = [];
            this.children.forEach(child => {
                child.selected = false;
                if (recursive) {
                    childPromises.push(child.deselectChildren(recursive));
                }
            });
            return Promise.all(childPromises);
        });
    }

    private setParents = (parent: NodeModel, children: NodeModel[]) => {
        if (children == undefined) {
            return;
        }

        for (var child of children) {
            child.parent = parent;
            if (child.hasChildren) {
                this.setParents(child, child.children);
            }
        }
    }
}